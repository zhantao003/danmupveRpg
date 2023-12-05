package com.bakka.actor.request.websocket

import akka.NotUsed
import akka.actor.{ActorRef, ActorSystem, Props}
import akka.http.scaladsl.model.ws.TextMessage.Strict
import akka.http.scaladsl.model.ws.{Message, TextMessage}
import akka.stream._
import akka.stream.scaladsl.{Broadcast, Flow, GraphDSL, Merge, Sink, Source}
import cn.vrspy.lmgame.dao.user.UserLogic
import cn.vrspy.lmgame.route.CustomCmd.{Broadcast_disconnect, Request_end}
import com.bakka.entry._
import com.bakka.entry.mongo.MongoLogic
import com.bakka.protocol.Cmd
import com.bakka.util.CommonUtils.getJsonString
import org.slf4j.LoggerFactory
import play.api.libs.json.Json

import scala.concurrent.{ExecutionContext, Future}
import scala.concurrent.duration._

class WebSocketService(worker: Class[_ <: WebSocketWorkerActor])(implicit ec: ExecutionContext, actorSystem: ActorSystem, materializer: ActorMaterializer) {
  implicit val chatMessageWrites = Json.writes[Message2Client]

  private val log = LoggerFactory.getLogger(classOf[WebSocketService])

  val webSocketService = actorSystem.actorOf(Props(worker))
  val source: Source[WSMessageDown, ActorRef] = Source.actorRef[WSMessageDown](bufferSize = 1024, OverflowStrategy.dropHead)

  val service = Flow.fromGraph(GraphDSL.create(source) { implicit builder =>
    source =>
      import GraphDSL.Implicits._

      val flowFromWs: FlowShape[Message, WSMessageUp] = builder.add(
        Flow[Message].collect {
          case tm: TextMessage =>
            tm.textStream.runFold("")(_ + _).map { jsonStr =>
              var userID = ""
              var roomId = ""
              var msgType = ""
              var content = ""
              try {
                val json = Json.parse(jsonStr)
                userID = getJsonString(json, "uid")
                roomId = getJsonString(json, "roomId")
                msgType = getJsonString(json, "cmd")
                content = getJsonString(json, "content")
              } catch {
                case e: Throwable =>
                  log.error(s"Incoming message parse error: $jsonStr")
              }

              Future(WSTextUp(userID, roomId, msgType, content))
//              UserLogic.getVtbAccount(userID, roomId).map(user => WSTextUp(userID, roomId, msgType, content))
            }
        }.buffer(1024, OverflowStrategy.dropHead).mapAsync(6)(t => t.flatMap(i => i))
      )

      val broadcastWs: UniformFanOutShape[WSMessageUp, WSMessageUp] = builder.add(Broadcast[WSMessageUp](2))

      val filterFailure: FlowShape[WSMessageUp, WSMessageUp] = builder.add(Flow[WSMessageUp].filter(_.uid == ""))
      val flowReject: FlowShape[WSMessageUp, WSTextDown] = builder.add(
        Flow[WSMessageUp].map(_ => WSTextDown("", "", Cmd.Response_reject, """{"code":0}"""))
      )

      val filterSuccess: FlowShape[WSMessageUp, WSMessageUp] = builder.add(Flow[WSMessageUp].filter(_.uid != ""))

      val flowAccept: FlowShape[WSMessageUp, WSMessageDown] = builder.add(
        Flow[WSMessageUp].collect {
          case WSTextUp(uid, token, msgType, content) =>
            Future(
              WSTextDown(uid, token, msgType, content)
            )
        }.buffer(1024, OverflowStrategy.dropHead).mapAsync(6)(t => t)
      )

      val mergeAccept: UniformFanInShape[WSMessageDown, WSMessageDown] = builder.add(Merge[WSMessageDown](2))

      val connectedWs: Flow[ActorRef, UserOnline, NotUsed] = Flow[ActorRef].map { actor =>
        UserOnline(actor)
      }

      val chatActorSink: Sink[WSMessageDown, NotUsed] = Sink.actorRef[WSMessageDown](webSocketService, UserOffline)

      val flowAcceptBack: FlowShape[WSMessageDown, WSMessageDown] = builder.add(
        Flow[WSMessageDown].keepAlive(50.seconds, () => WSTextDown("", "", Cmd.Response_keepAlive, "{}"))
      )

      val mergeBackWs: UniformFanInShape[WSMessageDown, WSMessageDown] = builder.add(Merge[WSMessageDown](2))

      val flowBackWs: FlowShape[WSMessageDown, Strict] = builder.add(
        Flow[WSMessageDown].collect {
          case WSTextDown(uid, roomId, msgType, content, dateline) =>
            val msg2c = Message2Client(uid, roomId, msgType, content, dateline)
            TextMessage(Json.stringify(Json.toJson(msg2c)))
        }
      )
      flowFromWs ~> broadcastWs
      broadcastWs ~> filterFailure ~> flowReject
      broadcastWs ~> filterSuccess ~> flowAccept ~> mergeAccept.in(0)
      builder.materializedValue ~> connectedWs ~> mergeAccept.in(1)
      mergeAccept ~> chatActorSink // --> to chatSessionActor

      /* from chatSessionActor --> */
      source ~> flowAcceptBack ~> mergeBackWs.in(0)
      flowReject ~> mergeBackWs.in(1)
      mergeBackWs ~> flowBackWs

      FlowShape(flowFromWs.in, flowBackWs.out)
  })
}
