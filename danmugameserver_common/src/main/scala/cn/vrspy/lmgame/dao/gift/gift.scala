package cn.vrspy.lmgame.dao

import com.bakka.entry.mongo._
import reactivemongo.bson.BSONObjectID

package object gift {
  case class Gift(var _id: BSONObjectID,
                        var uid: Long,
                        var roomId: Long,
                        var vtuberUid: Long,
                        var gameId: Long,
                        var giftType: Long,
                        var giftId: Long,
                        var giftName: String,
                        var giftNum: Long,
                        var battery: Long,
                        var isCash: Boolean,
                        var timeStamp: Long
                       ) extends BaseMongoObj

}
