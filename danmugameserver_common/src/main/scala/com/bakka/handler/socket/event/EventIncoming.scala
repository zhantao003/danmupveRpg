package com.bakka.handler.socket.event

import akka.actor.{Actor, ActorLogging, ActorRef, Props}
import com.bakka.entry.{Disconnected, PostEvent, RoleOffline}
import com.bakka.entry.role.RoleManager
import com.bakka.protocol.tcp.Event
import play.api.libs.json.JsValue
import com.bakka.util.CommonUtils._

class EventIncoming extends Actor with ActorLogging {

  override def receive: Receive = {
    case PostEvent(connRef, msg) => onEvent(connRef, msg)
    case Disconnected(connRef) => onDisconnected(connRef)
  }

  protected def onEvent(connRef: ActorRef, msg: JsValue): Unit = {
    getJsonString(msg, "type") match {
      case Event.Login => login(connRef, msg)
      case t: String =>
        log.error(s"No event handler with $t")
    }
  }

  protected def onDisconnected(connRef: ActorRef): Unit = {
    context.actorOf(Props(RoleManager())) ! RoleOffline(connRef)
  }

  protected def login(connRef: ActorRef, msg: JsValue): Unit = {

  }

}
