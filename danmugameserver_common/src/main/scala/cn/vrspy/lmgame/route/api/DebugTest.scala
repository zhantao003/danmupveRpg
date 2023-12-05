package cn.vrspy.lmgame.route.api

import akka.actor.ActorSystem
import akka.cluster.pubsub.DistributedPubSub
import akka.cluster.pubsub.DistributedPubSubMediator.Publish
import akka.http.scaladsl.server
import akka.stream.ActorMaterializer
import cn.vrspy.lmgame.dao.JsonObj.DanmuMsg.DanmuMsg
import cn.vrspy.lmgame.dao.JsonObj.Like.Like
import cn.vrspy.lmgame.dao.JsonObj.GiftMsg.GiftMsg
import cn.vrspy.lmgame.dao.user.{UserLogic, userViewer, userVtuber}
import cn.vrspy.lmgame.route.ws.WSWorker
import com.bakka.entry.ClusterText
import play.api.libs.json.Format.GenericFormat
import play.api.libs.json.Json

import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, ExecutionContext, Future}

object DebugTest {
  import CommonsJson._

  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer): server.Route = {
    path("api" / "danmu") {
      post(processPostDanmuJson(routeDanmuRelink)) ~
        get(processPostDanmuJson(routeDanmuRelink))
    }~
    path("api" / "gift") {
      post(processPostGiftJson(routeGiftRelink)) ~
        get(processPostGiftJson(routeGiftRelink))
    }~
    path("api" / "dianzan") {
      post(processPostLikeJson(routeLikeRelink)) ~
        get(processPostLikeJson(routeLikeRelink))
    }
  }

  private def routeDanmuRelink(params:DanmuMsg,roomID:String)(implicit executionContext: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer):Future[String] = {
    val uid = params.sec_openid
    val roomId = roomID
    val danmu = params.content
    val nickName = params.nickname
    val avatar = params.avatar_url

    val mediator = DistributedPubSub(system).mediator

    val jsonStr =
      s"""{"uid":"$uid","roomId":"$roomId","danmu":"$danmu","nickName":"$nickName",
         |"avatar":"$avatar"}""".stripMargin
    mediator ! Publish(s"${classOf[WSWorker].getSimpleName}-$roomId",
      ClusterText(uid, roomId, "danmu", jsonStr, System.currentTimeMillis()))

    val jsonObj =
          //TODO 返回值
      Json.obj(
        "status" -> "ok"
      )
      Future(Json.stringify(jsonObj))
  }

  private def routeGiftRelink(params:GiftMsg,roomID:String)(implicit executionContext: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer):Future[String] = {
    val uid = params.sec_openid
    val roomId = roomID
    val giftId = params.sec_gift_id
    val giftNum = params.gift_num
    val giftValue = params.gift_value
    val nickName = params.nickname
    val avatar = params.avatar_url

    val mediator = DistributedPubSub(system).mediator

    val jsonStr =
      s"""{"uid":"$uid","roomId":"$roomId","giftId":"$giftId","giftNum":"$giftNum","giftValue":"$giftValue",
         |"nickName":"$nickName","avatar":"$avatar"}""".stripMargin
    mediator ! Publish(s"${classOf[WSWorker].getSimpleName}-$roomId",
      ClusterText(uid, roomId, "gift", jsonStr, System.currentTimeMillis()))

    val jsonObj =
    //TODO 返回值
      Json.obj(
        "status" -> "ok"
      )
    Future(Json.stringify(jsonObj))
  }

  private def routeLikeRelink(params:Like,roomID:String)(implicit executionContext: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer):Future[String] = {
    val uid = params.sec_openid
    val roomId = roomID
    val likeNum = params.like_num
    val nickName = params.nickname
    val avatar = params.avatar_url

    val mediator = DistributedPubSub(system).mediator

    val jsonStr =
      s"""{"uid":"$uid","roomId":"$roomId","likeNum":"$likeNum","nickName":"$nickName","avatar":"$avatar"}""".stripMargin
    mediator ! Publish(s"${classOf[WSWorker].getSimpleName}-$roomId",
      ClusterText(uid, roomId, "like", jsonStr, System.currentTimeMillis()))

    val jsonObj =
    //TODO 返回值
      Json.obj(
        "status" -> "ok"
      )
    Future(Json.stringify(jsonObj))
  }

}