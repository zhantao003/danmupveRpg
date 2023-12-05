package cn.vrspy.lmgame.route

package object ws {

  case class GameUserInfo(var name: String,
                          var avatar: String,
                          var score: Int,
                          var status: Int,
                          var randomFlag: Boolean)

}
