package com.bakka.actor.request

import akka.http.scaladsl.model._
import akka.stream.ActorMaterializer
import com.bakka.handler.http.BaseHttpHandler
import test.www.TestWWW

import scala.collection.mutable

class HttpInvocation private(val invocationMap: mutable.HashMap[String, BaseHttpHandler])
                            (implicit materializer: ActorMaterializer){

  def processRequest: HttpRequest => HttpResponse = {
    case req: HttpRequest if invocationMap.contains(req.uri.path.toString()) =>
      invocationMap(req.uri.path.toString()).processRequest(req)

    case r: HttpRequest =>
      r.discardEntityBytes()
      HttpResponse(404, List(headers.Connection("close")), "No invocation!")
  }
}

object HttpInvocation {

  private var invocationMap = mutable.HashMap[String, BaseHttpHandler]()

  def add(baseHttpHandler: BaseHttpHandler): Unit = {
    invocationMap += baseHttpHandler.schema -> baseHttpHandler
  }

  add(new TestWWW)

  def apply()(implicit materializer: ActorMaterializer): HttpInvocation = new HttpInvocation(invocationMap)

}
