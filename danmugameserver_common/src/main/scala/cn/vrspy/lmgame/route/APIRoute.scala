package cn.vrspy.lmgame.route

import akka.actor.ActorSystem
import akka.http.scaladsl.model.headers.HttpOriginRange
import akka.http.scaladsl.server.Directives._
import akka.http.scaladsl.server.Route
import akka.stream.ActorMaterializer
import ch.megard.akka.http.cors.scaladsl.settings.CorsSettings
import cn.vrspy.lmgame.route.api._
import com.bakka.actor.request.websocket.routes.CustomAPIRoutes
import ch.megard.akka.http.cors.scaladsl.CorsDirectives.cors

import scala.concurrent.ExecutionContext

class APIRoute extends CustomAPIRoutes {

  override def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer): Route = {
      GiftAPI.route~
        SDKApi.route~
        UserAPI.route~
        DebugTest.route
      //DebugCmd.route
  }

}
