package cn.vrspy.lmgame.dao.JsonObj

import akka.http.scaladsl.marshallers.sprayjson.SprayJsonSupport
import spray.json.DefaultJsonProtocol

object DanmuMsg {
  case class DanmuMsg(msg_id: String, sec_openid: String,content:String,avatar_url:String,nickname:String,timestamp: Long)

  // collect your json format instances into a support trait:
  trait JsonSupport extends SprayJsonSupport with DefaultJsonProtocol {
    implicit val DanmuMsgFormat = jsonFormat6(DanmuMsg)
  }
}

object GiftMsg{
  case class GiftMsg(msg_id: String, sec_openid: String, sec_gift_id: String,gift_num: Long,gift_value: Long,  avatar_url: String, nickname: String, timestamp: Long)

  // collect your json format instances into a support trait:
  trait GiftJsonSupport extends SprayJsonSupport with DefaultJsonProtocol {
    implicit val GiftFormat = jsonFormat8(GiftMsg)
  }
}

object Like{
  case class Like(msg_id: String, sec_openid: String, like_num: String, avatar_url: String, nickname: String, timestamp: Long)

  // collect your json format instances into a support trait:
  trait LikeJsonSupport extends SprayJsonSupport with DefaultJsonProtocol {
    implicit val LikeFormat = jsonFormat6(Like)
  }
}
