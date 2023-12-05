package cn.vrspy.lmgame.dao.sdkdy

import com.bakka.entry.mongo.MongoOps.ec
import com.bakka.util.CommonUtils.{now, sendHttpPostRequest, sendHttpPostRequestWithToken}
import play.api.libs.json.Json

import scala.collection.mutable.ArrayBuffer
import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, Future}

object DYSdkLogic {
  //抖音SDK相关参数
  val sdk_AppID = "tta852deef376d66de10"
  val sdk_AppSecret = "5a0c9a836693dd0b105163d76a89ddd4e24d4364"
  val sdk_Sign = "u6vH69sS0DKUniJA"

  var sdk_token = ""
  var sdk_tokenOuttime:Long = 0 //超时时间(秒)
  var sdk_tokenGetTime:Long = 0 //获取时间(毫秒)

  //获取新的Token
  def newToken():Future[Int] = {
    //发起获取token的请求
    val url = "https://developer.toutiao.com/api/apps/v2/token"
    val arrBuffer = ArrayBuffer[(String,String)]()
    arrBuffer.+= (("appid",sdk_AppID),("secret",sdk_AppSecret),("grant_type","client_credential"))

    val content = sendHttpPostRequest(url,arrBuffer)
    println("token info:" + content)

    val jsonData = Json.parse(content)
    val errCode = (jsonData \ "err_no").as[Int]

    if(errCode == 0)
    {
      sdk_token = (jsonData \ "data" \ "access_token").as[String]
      sdk_tokenOuttime =(jsonData \ "data" \ "expires_in").as[Long]
      sdk_tokenGetTime = now

      Future(0)
    }
    else
    {
      Future(1)
    }
  }

  //Token是否有效
  def isTokenOk():Boolean = {
    if(sdk_token.isEmpty)
      false
    else{
      if(now - sdk_tokenGetTime > sdk_tokenOuttime * 1000 * 0.8)
        {
          false
        }
      else
        {
          true
        }
    }
  }

  //开启任务
  def startTask(roomId:String, msgType:String):Future[Int] = {
    println("房间号：" + roomId + "  消息："+ msgType)

    //判断token是否有效
    var bStartTaskOk = true
    if(!isTokenOk())
      {
        val code = Await.result(DYSdkLogic.newToken(),1.seconds)
        if(code != 0)
          {
            bStartTaskOk = false
          }
      }

    if(!bStartTaskOk)
      {
        Future(1)
      }
    else
      {
        val url = "https://webcast.bytedance.com/api/live_data/task/start"
        val arrBuffer = ArrayBuffer[(String,String)]()
        arrBuffer.+= (("roomid",roomId),("appid",sdk_AppID),("msg_type",msgType))

        val content = sendHttpPostRequestWithToken(url,sdk_token,arrBuffer)
        println("start task info:" + content)

        val jsonData = Json.parse(content)
        val errCode = (jsonData \ "err_no").as[Int]
        if(errCode == 0)
          {
            Future(0)
          }
        else
          {
            Future(2)
          }
      }
  }

  //关闭任务
  def endTask(roomId:String, msgType:String):Future[Int] =   {
    //判断token是否有效
    var bStartTaskOk = true
    if(!isTokenOk())
    {
      val code = Await.result(DYSdkLogic.newToken(),1.seconds)
      if(code != 0)
      {
        bStartTaskOk = false
      }
    }

    if(!bStartTaskOk)
    {
      Future(1)
    }
    else
    {
      val url = "https://webcast.bytedance.com/api/live_data/task/stop"
      val arrBuffer = ArrayBuffer[(String,String)]()
      arrBuffer.+= (("roomid",roomId),("appid",sdk_AppID),("msg_type",msgType))

      val content = sendHttpPostRequestWithToken(url,sdk_token,arrBuffer)
      println("end task info:" + content)

      val jsonData = Json.parse(content)
      val errCode = (jsonData \ "err_no").as[Int]
      if(errCode == 0)
      {
        Future(0)
      }
      else
      {
        Future(2)
      }
    }
  }
}
