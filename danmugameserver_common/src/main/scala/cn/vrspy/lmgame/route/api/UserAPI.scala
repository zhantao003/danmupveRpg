package cn.vrspy.lmgame.route.api

import akka.actor.ActorSystem
import akka.http.scaladsl.server
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import cn.vrspy.lmgame.dao.user.{UserLogic, userViewer, userVtuber}
import com.bakka.util.AES
import com.bakka.util.CommonUtils._
import play.api.libs.json.Format.GenericFormat
import play.api.libs.json.{JsValue, Json}

import java.net.URLDecoder
import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, ExecutionContext, Future}

object UserAPI {

  import Commons._


  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer): server.Route = {
      path("api2" / "login_vtb") {
        post(processFormParams(routeVTBLogin))
      } ~
      path("api2"/"login_viewer"){
        post(validateAndProcessFormParams(routeViewerLogin))
      } ~
      path("api2"/"vtb_getActive"){
        post(processFormParams(routeGetVtbActive))
      }
  }

  /**
    * VTB登录
    */
  private def routeVTBLogin(params: Map[String, String])(implicit executionContext: ExecutionContext) = {
    printParams(params)
    val uid = paramsGetString(params, "uid", "")
    val roomId = paramsGetString(params, "roomId", "")
    val code = paramsGetString(params, "code", "")
    val channel = paramsGetString(params, "channel", "")
    val nickName = paramsGetString(params, "nickName", "")
    val headIcon = paramsGetString(params, "headIcon", "")
    val userType = paramsGetInt(params, "userType", 0)
    for {
      (account, token) <- UserLogic.vtbLogin(uid, roomId, code, URLDecoder.decode(nickName,"UTF8"), URLDecoder.decode(headIcon,"UTF8"),userType,channel)
    } yield {
      if(account != null && token != null) {
        Json.stringify(Json.obj(
          "status" -> "ok",
          "userid" -> account.userid,
          "uid" -> account.uid,
          "roomId" -> account.roomId,
          "token" -> token
        ))
      }
      else {
        Json.stringify(Json.obj(
          "status" -> "error",
          "msg" -> "主播和房间ID不匹配"
        ))
      }
    }
  }

  /**
    * 天狗登录
    */
  private def routeViewerLogin(params: Map[String, String])(implicit executionContext: ExecutionContext) = {
    printParams(params)
    val uid = paramsGetString(params, "uid", "")
    val roomId = paramsGetString(params, "roomId", "")
    val channel = paramsGetString(params, "channel", "")
    val nickName = paramsGetString(params, "nickName", "")
    val headIcon = paramsGetString(params, "headIcon", "")
    val userType = paramsGetInt(params, "userType", 1)
    val fansMedalLevel = paramsGetLong(params, "fansMedalLevel", 0)
    val fansMedalName = paramsGetString(params, "fansMedalName", "")
    val fansMedalWearingStatus = paramsGetString(params, "fansMedalWearingStatus", "false") == "true"
    val guardLevel = paramsGetLong(params, "guardLevel", 0)

    for {
      (account, token) <- UserLogic.fanLogin(uid,roomId, URLDecoder.decode(nickName,"UTF8") ,URLDecoder.decode(headIcon,"UTF8"),userType,channel,fansMedalLevel,fansMedalName,fansMedalWearingStatus,guardLevel)
    } yield {
      if(account != null && token != null) {
        Json.stringify(Json.obj(
          "status" -> "ok",
          "userid" -> account.userid,
          "uid" -> account.uid,
          "headIcon" -> account.headIcon,
          "token" -> token,
          "createdTime" -> account.createdTime,
          "fansMedalLevel"-> account.fansMedalLevel,
          "fansMedalName"-> account.fansMedalName,
          "fansMedalWearingStatus"-> account.fansMedalWearingStatus,
          "guardLevel"-> account.guardLevel,
          "totalBattery" -> account.totalBattery
        ))
      }
      else {
        Json.stringify(Json.obj(
          "status" -> "error",
          "msg" -> "这还能有什么error?"
        ))
      }
    }
  }

  private def routeGetVtbActive(params:Map[String,String])(implicit executionContext: ExecutionContext):Future[String] = {
    printParams(params)
    val count = paramsGetInt(params, "count", 0)
    val page = paramsGetInt(params, "page", 0)
    for{
      list <- UserLogic.getVtbActive(count, page)
    }
    yield{
      if(list != null) {
        Json.stringify(Json.obj(
          "status" -> "ok",
          "list" -> list.map( n => {
            Json.obj(
              "uid" -> n.uid,
              "roomId" -> n.roomId,
              "lastActiveTime" -> n.lastActiveTime
            )}
          )
        ))
      }
      else {
        Json.stringify(Json.obj(
          "status" -> "error",
          "msg" -> "CD中")
        )
      }
    }
  }
}
