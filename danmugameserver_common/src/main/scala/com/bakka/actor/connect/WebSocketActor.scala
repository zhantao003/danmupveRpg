package com.bakka.actor.connect

import akka.actor.{Actor, ActorLogging}
import akka.http.scaladsl.Http
import akka.stream.ActorMaterializer
import com.bakka.actor.request.websocket.routes.Route

class WebSocketActor(val host: String,
                     val port: Int) extends Actor with ActorLogging {

  implicit val system = context.system
  implicit val executionContext = system.dispatcher
  implicit val materializer = ActorMaterializer()

  val bindingFuture = Http().bindAndHandle(Route.logRoute, host, port)

  log.info(s"Listening websocket on $host:$port...")

  def stop(): Unit = {
    bindingFuture.flatMap(_.unbind())
  }

  override def receive: Receive = {
    case o: Any => println(o)
  }

  private sealed trait UserEvent

  private case class Leave(roomId: Long) extends UserEvent

}
