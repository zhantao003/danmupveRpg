package com.bakka.util

import java.security.MessageDigest
import java.text.SimpleDateFormat
import java.util.UUID
import akka.http.scaladsl.model.DateTime
import com.typesafe.config.ConfigFactory
import play.api.libs.json._
import scalaj.http._

import java.util.concurrent.TimeUnit
import scala.collection.mutable.ArrayBuffer

/**
  * Created by cookeem on 16/9/25.
  */
object CommonUtils {

  val config = ConfigFactory.load("bakka.conf")

  val configMongo = config.getConfig("mongodb")
  val configMongoDbname = configMongo.getString("dbname")
  var configMongoUri = configMongo.getString("uri")

  case class CustomException(message: String = "", cause: Throwable = null) extends Exception(message, cause)

  def consoleLog(logType: String, msg: String) = {
    val timeStr = DateTime.now.formatted("yyyy-MM-dd HH:mm:ss")
    println(s"[$logType] $timeStr: $msg")
  }

  def now = System.currentTimeMillis()

  def aMonth:Long = 1000 * 3600 * 24 * 30

  //超时时间
  def overTime:Long = 100 * 1000 //3600 * 12 * 1000

  def newUUID = UUID.randomUUID().toString

  //每日免费鱼币获取上限
  def dailyGetFishCoinMax_normal:Long = 1280000
  def dailyGetFishCoinMax_tidu:Long = 2560000
  def dailyGetFishCoinMax_zongdu:Long = 5120000

  //每日免费经验获取上限
  def dailyGetFishExpMax:Long = 50000

  def isSameDay(time1:Long,time2:Long):Boolean ={
    TimeUnit.MILLISECONDS.toDays(time1) == TimeUnit.MILLISECONDS.toDays(time2)
  }

  def isOverTime(time:Long):Boolean ={
    Math.abs(time-now) > overTime
  }

  def md5(bytes: Array[Byte]) = {
    MessageDigest.getInstance("MD5").digest(bytes).map("%02x".format(_)).mkString
  }

  def getJsonString(json: JsValue, field: String, default: String = ""): String = {
    val ret = (json \ field).getOrElse(JsString(default)).as[String]
    ret
  }

  def getJsonInt(json: JsValue, field: String, default: Int = 0): Int = {
    val ret = (json \ field).getOrElse(JsNumber(default)).as[Int]
    ret
  }

  def getJsonLong(json: JsValue, field: String, default: Long = 0L): Long = {
    val ret = (json \ field).getOrElse(JsNumber(default)).as[Long]
    ret
  }

  def getJsonDouble(json: JsValue, field: String, default: Double = 0D): Double = {
    val ret = (json \ field).getOrElse(JsNumber(default)).as[Double]
    ret
  }

  def getJsonSeq(json: JsValue, field: String, default: Seq[JsValue] = Seq[JsValue]()): Seq[JsValue] = {
    val ret = (json \ field).getOrElse(JsArray(default)).as[Seq[JsValue]]
    ret
  }

  def getJsonBool(json: JsValue, field: String, default: Boolean = false): Boolean =
    (json \ field).getOrElse(JsBoolean(default)).as[Boolean]

  //从参数Map中获取Int
  def paramsGetInt(params: Map[String, String], key: String, default: Int): Int = {
    var ret = default
    if (params.contains(key)) {
      try {
        ret = params(key).toInt
      } catch {
        case e: Throwable =>
      }
    }
    ret
  }

  def paramsGetLong(params: Map[String, String], key: String, default: Long): Long = {
    var ret = default
    if (params.contains(key)) {
      try {
        ret = params(key).toLong
      } catch {
        case e: Throwable =>
      }
    }
    ret
  }

  //从参数Map中获取String
  def paramsGetString(params: Map[String, String], key: String, default: String): String = {
    var ret = default
    if (params.contains(key)) {
      ret = params(key)
    }
    ret
  }

  def sha1(str: String) = MessageDigest.getInstance("SHA-1").digest(str.getBytes).map("%02x".format(_)).mkString

  def md5(str: String) = MessageDigest.getInstance("MD5").digest(str.getBytes).map("%02x".format(_)).mkString

  def isEmail(email: String): Boolean = {
    """(?=[^\s]+)(?=(\w+)@([\w\.]+))""".r.findFirstIn(email).isDefined
  }

  def timeToStr(time: Long = System.currentTimeMillis()) = {
    val sdf = new SimpleDateFormat("MM-dd HH:mm:ss")
    sdf.format(time)
  }

  def classToMap(c: AnyRef): Map[String, String] = {
    c.getClass.getDeclaredFields.map{ f =>
      f.setAccessible(true)
      f.getName -> f.get(c).toString
    }.toMap
  }

  def trimUtf8(str: String, len: Int) = {
    var i = 0
    var strNew = ""
    str.foreach { ch =>
      if (i < len) {
        strNew = strNew + ch
      }
      var charLen = ch.toString.getBytes.length
      if (charLen > 2) {
        charLen = 2
      }
      i = i + charLen
    }
    strNew
  }

  @throws(classOf[java.io.IOException])
  @throws(classOf[java.net.SocketTimeoutException])
  def httpGet(url: String,
          connectTimeout: Int = 5000,
          readTimeout: Int = 5000,
          requestMethod: String = "GET"): String = {
    import java.net.{URL, HttpURLConnection}
    val connection = (new URL(url)).openConnection.asInstanceOf[HttpURLConnection]
    connection.setConnectTimeout(connectTimeout)
    connection.setReadTimeout(readTimeout)
    connection.setRequestMethod(requestMethod)
    val inputStream = connection.getInputStream
    val content = scala.io.Source.fromInputStream(inputStream,"utf-8").mkString

    content

  }

  def sendGet(url: String): String = {
    try {
      val content = httpGet(url)
      return content

    } catch {
      case ioe: java.io.IOException => // handle this
      case ste: java.net.SocketTimeoutException => // handle this
    }
    ""
  }

  def arrayBufferToJson(params:ArrayBuffer[(String,String)]): String ={

    var jsonString = "{"
    var count: Int = 0
    for(param <- params){
      jsonString+="\""+param._1+"\":\""+param._2+"\""+ ( if(count!=params.length-1) "," else "")
      count+=1
    }
    jsonString+="}"

    jsonString

  }

  def sendHttpPostRequest(url: String,params: ArrayBuffer[(String,String)]): String = {

    println(" Send Http Post Request (Start) ")

    try {
      val postData : String = arrayBufferToJson(params)
      println("Parameters: "+postData)
      val httpResponse: HttpResponse[String] = Http(url)
        .header("X-Requested-By","sdc")
        .header("Content-Type", "application/json;charset=UTF-8")
        .header("X-Stream" , "true")
        .header("Accept", "application/json")
        .postData(postData.getBytes)
//        .postForm(params)
        .asString


      val response = if (httpResponse.code == 200) httpResponse.body
      else{
        println("Bad HTTP response: code = "+httpResponse.code )
        "ERROR"
      }

      println(" Send Http Post Request (End) ")

      response

    } catch {
      case e: Exception => println("Error in sending Post request: " + e.getMessage)
         "ERROR"
    }
  }

  def sendHttpPostRequestWithToken(url: String, token:String, params: ArrayBuffer[(String,String)]): String = {

    println(" Send Http Post Request (Start) ")

    try {
      val postData : String = arrayBufferToJson(params)
      println("Parameters: "+postData)
      val httpResponse: HttpResponse[String] = Http(url)
        .header("X-Requested-By","sdc")
        .header("access-token",token)
        .header("Content-Type", "application/json;charset=UTF-8")
        .header("X-Stream" , "true")
        .header("Accept", "application/json")
        .postData(postData.getBytes)
        //        .postForm(params)
        .asString


      val response = if (httpResponse.code == 200) httpResponse.body
      else{
        println("Bad HTTP response: code = "+httpResponse.code )
        "ERROR"
      }

      println(" Send Http Post Request (End) ")

      response

    } catch {
      case e: Exception => println("Error in sending Post request: " + e.getMessage)
        "ERROR"
    }
  }

  var seqRequestNonce:Seq[String] = Seq()
  def checkNonce(nonce:String):Boolean = {
    if(seqRequestNonce.contains(nonce)) {
      false
    }
    else {
      seqRequestNonce = seqRequestNonce :+ nonce
      true
    }
  }
}
