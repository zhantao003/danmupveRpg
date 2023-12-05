package cn.vrspy.lmgame.route.api

import akka.http.scaladsl.model.headers.{HttpOriginRange, RawHeader}
import akka.http.scaladsl.model._
import ch.megard.akka.http.cors.scaladsl.settings.CorsSettings
import com.bakka.util.AES
import play.libs.Json

import scala.collection.immutable
import scala.concurrent.{ExecutionContext, Future}

object Commons {

  import akka.http.scaladsl.server.Directives._

  val settings = CorsSettings.defaultSettings.copy(
    allowedOrigins = HttpOriginRange.* // * refers to all
  )

  def validateAndProcessFormParams(th: Map[String, String] => Future[String])(implicit ex: ExecutionContext) = {
    val params = formFieldMap

    params { p =>
      complete {
        import com.bakka.util.CommonUtils._
        import com.bakka.entry.mongo.MongoLogic.validateToken

        val userID = paramsGetString(p, "roomId", "")
        val token = paramsGetString(p, "token", "")
        for (isTokenOk <- validateToken(userID, token)) yield
          if (isTokenOk) {
            val res = th(p)
            res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
          } else
            Future(responseByEntry(HttpEntity(ContentTypes.`application/json`, "{}")))
      }
    }
  }

  def validateEngryetAndProcessFormParams(th: Map[String, String] => Future[String])(implicit ex: ExecutionContext) = {
    val params = formFieldMap

    params { p =>
      complete {
        import com.bakka.util.CommonUtils._
        import com.bakka.entry.mongo.MongoLogic.validateToken
        import play.api.libs.json.Format.GenericFormat
        import play.api.libs.json.{Json}

        val data = paramsGetString(p, "data", "")
        val jsonData = Json.parse(AES.AESDecode(data.replaceAll(" ","+")))

        val userID = (jsonData \ "roomId").as[String]
        val token = (jsonData \ "token").as[String]
        for (isTokenOk <- validateToken(userID, token)) yield
          if (isTokenOk) {
            val res = th(p)
            res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
          } else
            Future(responseByEntry(HttpEntity(ContentTypes.`application/json`, "{}")))
      }
    }
  }

  def processFormParams(th: Map[String, String] => Future[String])(implicit ex: ExecutionContext) = {
    val params = formFieldMap

    params { p =>
      val res = th(p)
      complete {
        res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
      }
    }
  }

  def validateAndProcessGetParams(th: Map[String, String] => Future[String])(implicit ex: ExecutionContext) = {
    val params = parameterMap

    params { p =>
      complete {
        import com.bakka.util.CommonUtils._
        import com.bakka.entry.mongo.MongoLogic.validateToken

        val userID = paramsGetString(p, "roomId", "")
        val token = paramsGetString(p, "token", "")
        for (isTokenOk <- validateToken(userID, token)) yield
          if (isTokenOk) {
            val res = th(p)
            res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
          } else
            Future(responseByEntry(HttpEntity(ContentTypes.`application/json`, "{}")))
      }
    }
  }

  def processGetParams(th: Map[String, String] => Future[String])(implicit ex: ExecutionContext) = {
    val params = parameterMap

    params { p =>
      val res = th(p)
      complete {
        res.map(json => responseByEntry(HttpEntity(ContentTypes.`application/json`, json)))
      }
    }
  }

  def responseByEntry(custonEntity: ResponseEntity): HttpResponse =
    HttpResponse(entity = custonEntity)

  def printParams(p: Map[String, String]): Unit = p.foreach(item => println(s"${item._1} = ${item._2}"))

  def processPostJson(th:Json => Future[String])(implicit ex: ExecutionContext) = {

  }

}
