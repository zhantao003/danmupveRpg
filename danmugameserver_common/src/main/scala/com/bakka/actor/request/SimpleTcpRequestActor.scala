package com.bakka.actor.request

import akka.actor.{Actor, ActorLogging, Props}
import akka.io.Tcp
import akka.util.ByteString
import com.bakka.entry.role.RoleManager
import com.bakka.entry.{Disconnected, PostEvent}
import com.bakka.handler.socket.event.EventIncoming
import com.bakka.protocol.{Cmd, Protocol}
import com.bakka.util.CommonUtils._

class SimpleTcpRequestActor extends Actor with ActorLogging {

  import Tcp._

  val eventActor = context.actorOf(Props[EventIncoming])

  def receive = {
    case Received(data) =>
      processRequest(data: ByteString)

    case PeerClosed =>
      eventActor ! Disconnected(sender())
      context stop self
  }

  def processRequest(data: ByteString): Unit = {
    val jValue = Protocol().receive(data)

    // Debug
    log.debug(s"Income message: $jValue")

    getJsonString(jValue, "cmd") match {
      case Cmd.EVENT =>
        eventActor ! PostEvent(sender(), jValue)
      case t: Any =>
        val role = RoleManager().getRole(sender())
        if (role.nonEmpty) {
          
        }

    }
  }
}
