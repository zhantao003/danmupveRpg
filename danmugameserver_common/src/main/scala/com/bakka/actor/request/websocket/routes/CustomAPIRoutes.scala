package com.bakka.actor.request.websocket.routes

import akka.actor.ActorSystem
import akka.http.scaladsl.model.HttpEntity
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer
import com.bakka.config.ConfigManager

import scala.concurrent.{ExecutionContext, Future}

class CustomAPIRoutes {

  def route(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer) = {
    pathEnd {
      complete(Future {
        HttpEntity("where are you?")
      })
    }
  }

}

object CustomAPIRoutes {

  val customRoutesClasspath = ConfigManager().get("class-path").getString("api-custon-route")
  val _customRoutes = Class.forName(customRoutesClasspath).newInstance().asInstanceOf[CustomAPIRoutes]

  def apply(): CustomAPIRoutes = _customRoutes

}
