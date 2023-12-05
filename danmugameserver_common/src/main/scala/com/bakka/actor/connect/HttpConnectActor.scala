package com.bakka.actor.connect

import akka.actor.{Actor, ActorLogging}
import akka.http.scaladsl.Http
import akka.stream.ActorMaterializer
import akka.stream.scaladsl._
import com.bakka.actor.request.HttpInvocation

class HttpConnectActor(val host: String,
                       val port: Int) extends Actor with ActorLogging {

  implicit val system = context.system
  implicit val executionContext = system.dispatcher
  implicit val materializer = ActorMaterializer.create(context)

  val serverSource = Http().bind(host, port)
  val bindingFuture = serverSource.to(Sink.foreach { _.handleWithSyncHandler(HttpInvocation().processRequest) }).run()

  def stop(): Unit = {
    bindingFuture.flatMap(_.unbind())
  }

  override def receive: Receive = {
    case o: Any => println(o)
  }
}
