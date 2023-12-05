package test.route

import akka.actor.ActorSystem
import akka.http.scaladsl.model.{ContentTypes, HttpEntity}
import akka.http.scaladsl.server.Directives.{path, _}
import akka.stream.ActorMaterializer
import com.bakka.actor.request.websocket.routes.CustomAPIRoutes
import com.bakka.util.CommonUtils.paramsGetString

import scala.concurrent.{ExecutionContext, Future}

class TestRoute extends CustomAPIRoutes {

  override def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer) = {
    routeUserLogin ~
      routeUserLogout
  }

  def routeUserLogin(implicit ec: ExecutionContext) = get {
    path("api" / "loginUser") {
      parameterMap { params =>
        val login = paramsGetString(params, "login", "")
        val password = paramsGetString(params, "password", "")
        complete {
          Future {
            HttpEntity(ContentTypes.`application/json`, s"""{"login":"$login","password":"$password"}""")
          }
        }
      }
    }
  }

  def routeUserLogout(implicit ec: ExecutionContext) = post {
    path("api" / "logoutUser") {
      formFieldMap { params =>
        val userTokenStr = paramsGetString(params, "userToken", "")
        complete {
          Future {
            HttpEntity(ContentTypes.`application/json`, "")
          }
        }
      }
    }
  }

}
