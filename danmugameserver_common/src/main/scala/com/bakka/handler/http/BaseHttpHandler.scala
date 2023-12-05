package com.bakka.handler.http

import akka.http.scaladsl.model._

abstract class BaseHttpHandler {

  val schema: String

  def process(request: HttpRequest): String

  final def processRequest(request: HttpRequest): HttpResponse = {
    HttpResponse(200, entity = process(request))
  }

}
