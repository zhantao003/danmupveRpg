package cn.vrspy.lmgame.route.ws

import akka.cluster.pubsub.DistributedPubSubMediator.{Subscribe, Unsubscribe}
//import cn.vrspy.lmgame.dao.game_match.{GameMatchLogic, MatchRoomStatus}
//import cn.vrspy.lmgame.dao.topboard.TopBoardLogic
import cn.vrspy.lmgame.dao.user.UserLogic
import cn.vrspy.lmgame.route.CustomCmd
import com.bakka.actor.request.websocket.WebSocketWorkerActor
import com.bakka.protocol.Cmd
import play.api.libs.json.Json

import scala.concurrent.Future

class GameWSWorker extends WebSocketWorkerActor {

  import CustomCmd._
  //import GameMatchLogic._
  import com.bakka.util.CommonUtils._

  import scala.concurrent.ExecutionContext.Implicits.global

  var _roomID: String = _
  var _gaming = false

  override def onConnect(): Unit = {
//    for (matchRoom <- getMatchRoomByUserID(_uid)) yield {
//      if (matchRoom != null) {
//        _roomID = matchRoom.roomID
//        mediator ! Subscribe(s"Room-${_roomID}", self)
//
//        for {
////          user <- UserLogic.getUser(_uid)
//          topInfo <- TopBoardLogic.getUserTopInfo(_uid)
//          list <- getMatchRoomWithoutUserID(_roomID, _uid)
//        } yield {
////          val userList = appendUserInfoByMatchRooms(list)
////          val json = Json.obj(
////            "list" -> (Json.arr() /: userList) ((jsArr, item) => {
////              jsArr :+ Json.obj(
////                "avatar" -> item.avatar,
////                "name" -> item.name,
////                "status" -> item.status,
////                "score" -> item.score,
////                "randomFlag" -> item.randomFlag
////              )
////            }),
////            "self" -> Json.obj(
////              "userID" -> user.id,
////              "avatar" -> user.avatar,
////              "name" -> user.name,
////              "score" -> topInfo.score
////            )
////          )
//
////          response("", "", Response_connected, Json.stringify(json))
//        }
//
//      } else {
//        response(_uid, _roomId, Cmd.Response_reject, """{"code":1}""")
//        _disconnect()
//      }
//    }

  }

  override def onDisconnect(): Unit = {
    sendTo("Room", _roomID, (_uid, "", Broadcast_disconnect, "{}"))
    mediator ! Unsubscribe(s"Room-${_roomID}", self)
    //matchRoomCleanByRoon(_roomID)
  }

  override def onMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)] = {
    log.debug(s"onMessage: uid: $uid, roomId: $roomId, cmd: $cmd, content: $content, timestamp: $timestamp")
    cmd match {
      case Request_progressStage =>
        val json = Json.parse(content)
//        val status = if (getJsonBool(json, "finish")) MatchRoomStatus.Ready else MatchRoomStatus.Loading
//        matchRoomSetStatus(_uid, _roomID, status.id).map { success =>
//          if (success) {
//            sendTo("Room", _roomID, (uid, "", Broadcast_progressStage, content))
//            matchRoomHasUnready(_roomID).map(hasUnReady =>
//              if (!hasUnReady) startGame())
//          }
//          None
//        }

      case Request_custom =>
        sendTo("Room", _roomID, (uid, "", Broadcast_custom, content))

      case Request_gameover =>
        val isWin = getJsonBool(Json.parse(content), "isWin")
        gameOver(isWin)
    }
    None
  }

  def startGame(): Unit = {
    //matchRoomSetStatusAll(_roomID, MatchRoomStatus.Gaming.id)
    sendTo("Room", _roomID, ("", "", Broadcast_ready, "{}"))
    _gaming = true
  }

  def gameOver(isWin: Boolean): Unit = {
//    if (_gaming) {
//      _gaming = false
//      val status = if (isWin) MatchRoomStatus.GameOverWin else MatchRoomStatus.GameOverLose
//      matchRoomSetStatus(_uid, _roomID, status.id).map(flag => {
//        if (flag)
//          sendTo("Room", _roomID, (_uid, "", Broadcast_winlose, s"""{"isWin":$isWin}"""))
//        matchRoomGetWinner(_roomID).map(winner => {
//          if (winner.nonEmpty) {
//            sendTo("Room", _roomID, (winner.get, "", Broadcast_gameover, "{}"))
//          }
//        })
//
//      })
//    } else {
//      matchRoomClean(_uid)
//    }
  }

  /**
    * Request from other client
    */
  override def onClusterMessage(uid: String, roomId: String, cmd: String, content: String, timestamp: Long): Option[(String, String, String, String)] = {
    log.debug(s"onClusterMessage: uid: $uid, roomId: $roomId, cmd: $cmd, content: $content, timestamp: $timestamp")
    response(uid, roomId, cmd, content)
    if (cmd == Broadcast_gameover) {
      mediator ! Unsubscribe(s"Room-${_roomID}", self)
      //matchRoomClean(_uid)
    } else if (cmd == Broadcast_ready)
      _gaming = true
    None
  }
}
