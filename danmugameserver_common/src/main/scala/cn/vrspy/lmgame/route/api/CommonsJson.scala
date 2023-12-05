package cn.vrspy.lmgame.route.api

import akka.http.scaladsl.marshallers.sprayjson.SprayJsonSupport
import akka.http.scaladsl.model.headers.HttpOriginRange
import akka.http.scaladsl.model._
import akka.http.scaladsl.server.directives.BasicDirectives.extract
import akka.http.scaladsl.server.{Directives, RequestContext, RouteResult}
import ch.megard.akka.http.cors.scaladsl.settings.CorsSettings
import cn.vrspy.lmgame.dao.JsonObj.DanmuMsg._
import cn.vrspy.lmgame.dao.JsonObj.GiftMsg._
import cn.vrspy.lmgame.dao.JsonObj.Like._

import scala.concurrent.{ExecutionContext, Future}


object CommonsJson extends Directives with JsonSupport with GiftJsonSupport with LikeJsonSupport{

  val settings = CorsSettings.defaultSettings.copy(
    allowedOrigins = HttpOriginRange.* // * refers to all
  )

  def responseByEntry(custonEntity: ResponseEntity): HttpResponse =
    HttpResponse(entity = custonEntity)

  def printParams(p: Map[String, String]): Unit = p.foreach(item => println(s"${item._1} = ${item._2}"))

  def processPostDanmuJson(th: (DanmuMsg,String) => Future[String])(implicit ex: ExecutionContext):RequestContext => Future[RouteResult] = {
    headerValueByName("x-roomid") {
      roomId =>
      entity(as[DanmuMsg]) {
        p => {
          val res = th(p, roomId)
          complete {
            res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
            //        res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
          }
        }
      }
    }
  }

  def processPostGiftJson(th: (GiftMsg, String) => Future[String])(implicit ex: ExecutionContext): RequestContext => Future[RouteResult] = {
    headerValueByName("x-roomid") {
      roomId =>
        entity(as[GiftMsg]) {
          p => {
            val res = th(p, roomId)
            complete {
              res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
              //        res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
            }
          }
        }
    }
  }

  def processPostLikeJson(th: (Like, String) => Future[String])(implicit ex: ExecutionContext): RequestContext => Future[RouteResult] = {
    headerValueByName("x-roomid") {
      roomId =>
        entity(as[Like]) {
          p => {
            val res = th(p, roomId)
            complete {
              res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
              //        res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
            }
          }
        }
    }
  }


}
