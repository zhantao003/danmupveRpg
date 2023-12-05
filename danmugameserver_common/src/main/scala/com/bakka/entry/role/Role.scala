package com.bakka.entry.role

import java.net.InetSocketAddress

import akka.actor.ActorRef
import akka.io.Tcp.{NoAck, Write}
import akka.util.ByteString
import com.bakka.entry.zone.Zone
import play.api.libs.json.JsValue

class Role private[role](val id: String,
                         private val connectionRef: ActorRef,
                         val remoteAddress: InetSocketAddress) {

  private var zone: Zone = _

  var lastRequest = System.currentTimeMillis()
  var isConnected = true

  private[role] def setZone(z: Zone): Unit = {
    zone = z
  }

  def getZone: Zone = zone

  def send(msg: JsValue): Unit = {
    connectionRef ! Write(ByteString(msg.toString), NoAck)
  }

  override def toString: String = {
    s"{id: $id, remote: $remoteAddress}"
  }
}
