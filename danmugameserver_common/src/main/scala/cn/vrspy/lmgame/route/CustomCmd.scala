package cn.vrspy.lmgame.route

object CustomCmd {

  val Request_login = "login"
  val Request_end = "end"

  val Request_progressStage = "progressStage"
  val Request_custom = "custom"
  val Request_gameover = "gameover"

  val Response_connected = "connected"
  val Response_gameover = "gameover_res"

  val Broadcast_ready = "ready"
  val Broadcast_custom = "custom"
  val Broadcast_progressStage = "progressStage"
  val Broadcast_winlose = "winlose"
  val Broadcast_gameover = "gameover"
  val Broadcast_disconnect = "disconnect"

}
