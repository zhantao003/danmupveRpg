package com.bakka.actor.request.websocket.routes

import akka.actor.{ActorRef, ActorSystem}
import akka.http.scaladsl.server.Directives._
import akka.stream.ActorMaterializer

import scala.concurrent.ExecutionContext

/**
  * Created by cookeem on 16/11/3.
  */
object RouteOps {

  def routeLogic(implicit ec: ExecutionContext, system: ActorSystem, materializer: ActorMaterializer, notificationActor: ActorRef) = {
    CustomWSRoutes().route ~ CustomAPIRoutes().route
  }

}
