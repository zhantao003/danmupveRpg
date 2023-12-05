package cn.vrspy.lmgame.route.api

import akka.actor.ActorSystem
import akka.http.scaladsl.server
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import cn.vrspy.lmgame.dao.sdkdy.DYSdkLogic
import cn.vrspy.lmgame.dao.sdkdy.DYSdkLogic.{sdk_token, sdk_tokenOuttime}
import com.bakka.util.CommonUtils._
import play.api.libs.json.Format.GenericFormat
import play.api.libs.json.{JsValue, Json}

import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, ExecutionContext, Future}

object SDKApi {

  import Commons._

  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer): server.Route = {
    path("api" / "checkToken") {
      post(processFormParams(checkToken)) ~
        get(processGetParams(checkToken))
    } ~
      path("api" / "startDanmuTask") {
        post(processFormParams(startTask)) ~
          get(processGetParams(startTask))
      }~
      path("api" / "endDanmuTask") {
        post(processFormParams(endTask)) ~
          get(processGetParams(endTask))
      }
  }

  /**
    * 启动壳程序时调用接口
    */
  private def checkToken(params: Map[String, String])(implicit executionContext: ExecutionContext): Future[String] = {
    printParams(params)
    val roomId = paramsGetString(params, "roomId", "")

    //判断是否要重新获取token
    var bGetNewToken = false
    if(!DYSdkLogic.isTokenOk())
      {
        bGetNewToken = true;
      }

    if(bGetNewToken)
      {
        println("请求新的token")
        val code = Await.result(DYSdkLogic.newToken(),1.seconds)

        if(code == 0)
          {
            Future(Json.stringify(Json.obj(
              "status" -> "ok",
              "roomId" ->roomId,
              "token" -> sdk_token,
              "expireTime" -> sdk_tokenOuttime
            )))
          }
        else
          {
            Future(Json.stringify(Json.obj(
              "status" -> "err",
              "msg" ->"向平台方请求Token失败"
            )))
          }
      }
    else
      {
        println("缓存Token:" + sdk_token)
        Future(Json.stringify(Json.obj(
          "status" -> "ok",
          "roomId" ->roomId,
          "token" -> sdk_token,
          "expireTime" -> sdk_tokenOuttime
        )))
      }
  }

  /**
    * 开启推送任务
    */
  private def startTask(params: Map[String, String])(implicit executionContext: ExecutionContext): Future[String] = {
    printParams(params)
    val roomId = paramsGetString(params, "roomId", "")
    val msgType = paramsGetString(params, "msgType", "")

    for{
      res <- DYSdkLogic.startTask(roomId,msgType)
    }yield{
      if(res == 0)
        {
          Json.stringify(Json.obj(
            "status" -> "ok",
            "roomId" ->roomId,
            "msg" ->"启动任务成功"
          ))
        }
      else
        {
          Json.stringify(Json.obj(
            "status" -> "err",
            "roomId" ->roomId,
            "msg" ->"启动任务失败"
          ))
        }
    }
  }

  /**
    * 开启推送任务
    */
  private def endTask(params: Map[String, String])(implicit executionContext: ExecutionContext): Future[String] = {
    printParams(params)
    val roomId = paramsGetString(params, "roomId", "")
    val msgType = paramsGetString(params, "msgType", "")

    for{
      res <- DYSdkLogic.endTask(roomId,msgType)
    }yield{
      if(res == 0)
      {
        Json.stringify(Json.obj(
          "status" -> "ok",
          "roomId" ->roomId,
          "msg" ->"关闭任务成功"
        ))
      }
      else
      {
        Json.stringify(Json.obj(
          "status" -> "err",
          "roomId" ->roomId,
          "msg" ->"关闭任务失败"
        ))
      }
    }
  }
}
