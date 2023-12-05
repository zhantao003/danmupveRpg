package cn.vrspy.lmgame.route.ws

import com.bakka.actor.request.websocket.WebSocketWorkerActor

class WSWorker extends WebSocketWorkerActor {

  override def onMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)] = {
    log.info(s"onMessage: uid: $uid, roomId: $roomId, cmd: $cmd, content: $content, timestamp: $timestamp")
    Some((uid, roomId, cmd, content))
  }

  /**
    * Request from other client
    */
  override def onClusterMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)] = {
    log.info(s"onClusterMessage: uid: $uid, roomId: $roomId, cmd: $cmd, content: $content, timestamp: $timestamp")
    response(uid, roomId, cmd, content)
    None
  }
}
