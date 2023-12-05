package com.bakka.actor.request.websocket.routes

import akka.actor.ActorSystem
import akka.http.scaladsl.server.Directives.{get, handleWebSocketMessages, path}
import akka.stream.ActorMaterializer
import com.bakka.actor.request.websocket.{WebSocketService, WebSocketWorkerActor}
import com.bakka.config.ConfigManager

import scala.concurrent.ExecutionContext

class CustomWSRoutes {

  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer) = {
    get {
      path("ws") {
        handleWebSocketMessages(new WebSocketService(classOf[WebSocketWorkerActor]).service)
      }
    }
  }

}

object CustomWSRoutes {

  val customRoutesClasspath = ConfigManager().get("class-path").getString("ws-custon-route")
  val _customRoutes = Class.forName(customRoutesClasspath).newInstance().asInstanceOf[CustomWSRoutes]

  def apply(): CustomWSRoutes = _customRoutes
}