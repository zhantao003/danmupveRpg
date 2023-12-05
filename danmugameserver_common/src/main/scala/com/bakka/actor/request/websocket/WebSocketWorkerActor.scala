package com.bakka.actor.request.websocket

import akka.actor.{Actor, ActorLogging, ActorRef}
import akka.cluster.pubsub.DistributedPubSub
import cn.vrspy.lmgame.dao.online.OnlineLogic
import com.bakka.entry.{ClusterText, UserOffline, UserOnline, WSTextDown}

abstract class WebSocketWorkerActor extends Actor with ActorLogging {

  val system = context.system

  import akka.cluster.pubsub.DistributedPubSubMediator._
  import cn.vrspy.lmgame.route.CustomCmd._

  var _online = false
  var _channel: ActorRef = ActorRef.noSender

  var _uid: String = ""
  var _roomId: String = ""
  var _route: String = ""

  protected val mediator = DistributedPubSub(context.system).mediator

  final def _init(uid: String, roomId: String,route:String): Unit = {
    if (_uid.isEmpty && _roomId.isEmpty) {
      _uid = uid
      _roomId = roomId
      mediator ! Subscribe(s"${getClass.getSimpleName}-${_roomId}", self)

      if(route != "")
      {
        _route = route
        OnlineLogic.recordInfo(1,route)
      }

      _online = true
      onConnect()
      _channel ! WSTextDown(uid, roomId, "login", "ok")
    }
    println("connected!")
  }

  final def _disconnect(): Unit = {
    _online = false
    if (_uid.nonEmpty && _roomId.nonEmpty)
      mediator ! Unsubscribe(s"${getClass.getSimpleName}-${_uid}", self)
    if(_route != "")
      OnlineLogic.recordInfo(-1,_route)
    onDisconnect()
    println("disconnected!")
    context stop _channel
  }

  override def receive: Receive = {
    case UserOnline(connRef) => _channel = connRef
    case UserOffline => _disconnect()
    case WSTextDown(uid, roomId, Request_login, route, _) => _init(uid, roomId,route)
    case WSTextDown(_, _, Request_end, _, _) => _disconnect()
    case WSTextDown(uid, roomId, cmd, content, timestamp) if _online =>
      val res = onMessage(uid, roomId, cmd, content, timestamp)
      if (res.nonEmpty)
        response(res.get)

    case SubscribeAck(Subscribe(userID, None, `self`)) if userID != "" =>
      log.debug(s"SubscribeAck: $userID")

    case UnsubscribeAck(Unsubscribe(userID, None, `self`)) =>
      _channel = ActorRef.noSender
      log.debug(s"UnsubscribeAck: $userID")

    case ClusterText(uid, roomId, cmd, content, timestamp) if _online =>
      val res = onClusterMessage(uid, roomId, cmd, content, timestamp)
      if (res.nonEmpty)
        sender() ! WSTextDown(res.get._1, res.get._2, res.get._3, res.get._4, System.currentTimeMillis())

    case msg: Any => println(msg)
  }

  def onConnect(): Unit = {}

  def onDisconnect(): Unit = {}

  /**
    * Request from client
    */
  def onMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)]

  /**
    * Request from other client
    */
  def onClusterMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)]

  final def response(uid: String, roomId: String, cmd: String, content: String): Unit = {
    require(_online)
    require(_channel != ActorRef.noSender)

    val selfUID = _uid
    val timestamp = System.currentTimeMillis()
    _channel ! WSTextDown(uid, roomId, cmd, content, timestamp)
    log.debug(s"""Response to $selfUID - {"uid": $uid, "token": $roomId, "cmd: $cmd, "content": $content, "timestamp": $timestamp}""")
  }

  final def response(msg: (String, String, String, String)): Unit = {
    response(msg._1, msg._2, msg._3, msg._4)
  }

  final def sendTo(worker: String, to: String, msg: (String, String, String, String)): Unit = {
    val timestamp = System.currentTimeMillis()
    val toWorker = s"$worker-$to"
    mediator ! Publish(toWorker, ClusterText(msg._1, msg._2, msg._3, msg._4, timestamp))
    log.debug(s"""Publish to $toWorker - {"uid": ${msg._1}, "token": ${msg._2}, "cmd": ${msg._3}, "content": ${msg._4}, "timestamp": $timestamp}""")
  }

}
