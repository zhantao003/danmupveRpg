package com.bakka.actor.request.websocket

import akka.http.scaladsl.model.ws.{Message, TextMessage}
import akka.stream.FlowShape
import akka.stream.scaladsl.{Flow, Source}
import com.bakka.entry.WSTextUp
import com.bakka.entry.mongo.MongoLogic
import com.bakka.util.CommonUtils.getJsonString
import play.api.libs.json.Json

object SimpleWebSocketService {

  val service: Flow[Message, Message, Any] =
    Flow[Message].mapConcat {
      case tm: TextMessage =>
        TextMessage(Source.single("Hello ") ++ tm.textStream ++ Source.single("!")) :: Nil
    }

}
