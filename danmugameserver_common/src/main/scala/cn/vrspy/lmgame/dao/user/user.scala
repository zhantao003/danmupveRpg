package cn.vrspy.lmgame.dao

import com.bakka.entry.mongo._
import reactivemongo.bson.BSONObjectID

package object user {

  case class userVtuber(var _id: BSONObjectID,
                     var userid: String,
                     var uid: String,
                     var roomId: String,
                     var code:String,
                     var nickName: String,
                     var headIcon: String,
                     var userType: Int,
                     var channel: String,     //渠道
                     var totalBattery: Long,  //主播收到的电池
                     var createdTime: Long,
                     var lastActiveTime:Long
                    ) extends BaseMongoObj

  case class userViewer(var _id: BSONObjectID,
                        var userid: String,
                        var uid: String,
                        var roomId: String,
                        var nickName: String,
                        var headIcon: String,
                        var userType: Int,
                        var channel: String,     //渠道
                        var fansMedalLevel: Long, //粉丝等级
                        var fansMedalName: String,
                        var fansMedalWearingStatus: Boolean,
                        var guardLevel: Long,   //直播间Vip等级
                        var totalBattery: Long, //玩家送出得电池
                        var lastSignInTime:Long,
                        var createdTime: Long
                      ) extends BaseMongoObj
}
