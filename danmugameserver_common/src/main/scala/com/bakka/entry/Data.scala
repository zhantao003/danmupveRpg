package com.bakka.entry

import java.net.InetSocketAddress

import akka.actor.ActorRef
import play.api.libs.json.JsValue

sealed trait Message
/************************************************/
// Network
case class NewConnection(connRef: ActorRef, remoteIp: InetSocketAddress) extends Message
case class Disconnected(connRef: ActorRef) extends  Message

// Role
case class CreateRole(connRef: ActorRef, remote: InetSocketAddress) extends Message
case class RoleOffline(connRef: ActorRef) extends Message

// Tcp-Message
case class PostEvent(connRef: ActorRef, msg: JsValue) extends Message
case class SendString(str: String) extends Message


//akka stream message type
sealed trait WSMessageUp {val uid: String}
case class WSTextUp(uid: String, roomId: String, cmd: String, content: String) extends WSMessageUp

//akka stream message type
sealed trait WSMessageDown
case class WSTextDown(uid: String, roomId: String, cmd: String, content: String, timestamp: Long = System.currentTimeMillis()) extends WSMessageDown
case class ClusterText(uid: String, roomId: String, msgType: String, content: String, timestamp: Long = System.currentTimeMillis()) extends WSMessageDown

//akka stream message type
case class UserOnline(actor: ActorRef) extends WSMessageDown
case object UserOffline extends WSMessageDown

//client message type
case class Message2Client(uid: String, roomId: String, cmd: String, content: String, timestamp: Long = System.currentTimeMillis()) extends WSMessageDown


// server-side event

/************************************************/