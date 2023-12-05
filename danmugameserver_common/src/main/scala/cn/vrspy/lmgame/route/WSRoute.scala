package cn.vrspy.lmgame.route

import akka.actor.ActorSystem
import akka.http.scaladsl.server.Directives.{get, handleWebSocketMessages, path, _}
import akka.stream.ActorMaterializer
import cn.vrspy.lmgame.route.ws.{GameWSWorker, WSWorker}
import com.bakka.actor.request.websocket.WebSocketService
import com.bakka.actor.request.websocket.routes.CustomWSRoutes

import scala.concurrent.ExecutionContext

class WSRoute extends CustomWSRoutes {
  override def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer) = {
    get {
      path("ws") {
        handleWebSocketMessages(new WebSocketService(classOf[WSWorker]).service)
      } ~
        path("game-ws") {
          handleWebSocketMessages(new WebSocketService(classOf[GameWSWorker]).service)
        }
    }
  }
}
