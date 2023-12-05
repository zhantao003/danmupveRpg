package com.bakka.protocol

import java.nio.charset.Charset

import akka.actor.ActorRef
import akka.io.Tcp.Write
import akka.util.ByteString
import com.bakka.config.ConfigManager
import play.api.libs.json.{JsValue, Json}

class SimpleProtocol {

  def receive(data: ByteString): JsValue = {
    val content = data.decodeString(Charset.forName("utf8")).trim
    receive(content)
  }

  def receive(str: String): JsValue = {
    Json.parse(str)
  }

  def send(json: JsValue, connRef: ActorRef): Unit = {
    connRef ! Write(ByteString(Json.stringify(json), Charset.forName("utf8")))
  }

}

object Protocol {

  private val _instance: SimpleProtocol = Class.forName(
    ConfigManager().get("class-path").getString("protocol")).newInstance().asInstanceOf[SimpleProtocol]

  def apply(): SimpleProtocol = _instance

}
