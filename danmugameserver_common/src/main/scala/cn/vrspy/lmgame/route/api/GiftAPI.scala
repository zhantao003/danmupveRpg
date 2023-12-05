package cn.vrspy.lmgame.route.api

import akka.actor.ActorSystem
import akka.http.scaladsl.server
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import cn.vrspy.lmgame.dao.gift.GiftLogic
import cn.vrspy.lmgame.dao.user.UserLogic
import com.bakka.util.CommonUtils._
import play.api.libs.json.Json

import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, ExecutionContext, Future}

object GiftAPI {

  import Commons._

  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer): server.Route = {
    path("api" / "gift_sendFree") {
      post(processFormParams(routeSendFreeGift)) ~
        get(processGetParams(routeSendFreeGift))
    } //~
//      path("api"/"gameflg_gift_sendCash"){
//        post(processFormParams(routeGameFlgSendCashGift)) ~
//          get(processGetParams(routeGameFlgSendCashGift))
//      }
  }

  /**
    * 免费礼物
    */
  private def routeSendFreeGift(params: Map[String, String])(implicit executionContext: ExecutionContext): Future[String] = {
    printParams(params)
    val uid = paramsGetLong(params, "uid", 0)
    val roomId = paramsGetLong(params, "roomId", 0)
    val vtbUid = paramsGetLong(params, "vtbUid", 0)
    val gameId = paramsGetLong(params, "gameId", 0)
    val giftId = paramsGetLong(params, "giftId", 0)
    val giftName = paramsGetString(params, "giftName", "")
    val giftNum = paramsGetLong(params, "giftNum", 0)
    val battery = paramsGetLong(params, "battery", 0)
    for {
      res <- GiftLogic.sendFreeGift(uid, roomId, vtbUid, gameId, giftId, giftName, giftNum, battery)
    } yield {
      if(res != null) {
        Json.stringify(Json.obj(
          "status" -> "ok",
          "uid" -> 0,
          "gameCoin" -> 0,
          "msg" -> "记录免费礼物完成"
        ))
      }
      else {
        Json.stringify(Json.obj(
          "status" -> "error",
          "msg" -> "未知错误"
        ))
      }
    }
  }
//
//  /**
//    * 付费礼物
//    */
//  private def routeGameFlgSendCashGift(params: Map[String, String])(implicit executionContext: ExecutionContext) : Future[String] = {
//    printParams(params)
//    val uid = paramsGetLong(params, "uid", 0)
//    val roomId = paramsGetLong(params, "roomId", 0)
//    val vtbUid = paramsGetLong(params, "vtbUid", 0)
//    val gameId = paramsGetLong(params, "gameId", 0)
//    val giftType = paramsGetInt(params, "giftType", 0)
//    val giftId = paramsGetLong(params, "giftId", 0)
//    val giftName = paramsGetString(params, "giftName", "")
//    val giftNum = paramsGetLong(params, "giftNum", 0)
//    val battery = paramsGetLong(params, "battery", 0)
//    for {
//      res <- GiftLogic.sendCashGift(uid,roomId,vtbUid,gameId,giftType,giftId,giftName,giftNum,battery)
//    } yield {
//      if(res != null) {
//
//        //根据礼物增加玩家的金币
//        var addCoin:Long = -1;
//        if(giftName == "支援药水")
//        {
//          addCoin = 300 * giftNum;
//        }
//        else if(giftName == "捣蛋药水")
//        {
//          addCoin = 300* giftNum;
//        }
//        else if(giftName == "金币堆")
//        {
//          addCoin = 5000* giftNum;
//        }
//        else if(giftName == "导弹卡")
//        {
//          addCoin = 8800* giftNum;
//        }
//        else if(giftName == "守护主播")
//        {
//          addCoin = 0;
//        }
//        else if(giftName == "点赞")
//        {
//          addCoin = 300 * giftNum;
//        }
//        else if(giftName == "炫彩宝箱")
//        {
//          addCoin = 0;
//
//          //固定给主播两个碎片
//          val resVtb2 = Await.result(UserLogic.addAvatarFragmentToVtb(vtbUid,roomId,2*giftNum), 1.seconds)
//        }
//
//        if(addCoin >= 0)
//        {
//          val resUser = Await.result(UserLogic.addCoinAndBattery(uid,addCoin,battery * giftNum), 1.seconds)
//          val resVtb = Await.result(UserLogic.addVtbBattery(vtbUid,roomId,battery * giftNum), 1.seconds)
//
//          if(resUser != null)
//          {
//              Json.stringify(Json.obj(
//                "status" -> "ok",
//                "uid" -> resUser.uid,
//                "gameCoin" -> resUser.gameCoin,
//                "vtbUid" -> vtbUid,
//                "vtbAvatarFragments" -> resVtb.avatarFragments,
//                "msg" -> "记录付费礼物完成"
//              ))
//          }
//          else
//          {
//            Json.stringify(Json.obj(
//              "status" -> "error",
//              "msg" -> "用户未登录"
//            ))
//          }
//        }
//        else
//        {
//          Json.stringify(Json.obj(
//            "status" -> "ok",
//            "uid" -> 0,
//            "gameCoin" -> 0,
//            "msg" -> "记录付费礼物完成"
//          ))
//        }
//      }
//      else {
//        Json.stringify(Json.obj(
//          "status" -> "error",
//          "msg" -> "未知错误"
//        ))
//      }
//    }
//  }

}
