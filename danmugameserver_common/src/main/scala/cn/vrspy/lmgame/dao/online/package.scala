package cn.vrspy.lmgame.dao

import com.bakka.entry.mongo.BaseMongoObj
import reactivemongo.bson.BSONObjectID

package object online {
  case class OnlineInfo(var _id: BSONObjectID,
                          var online: Int,
                          var route:String,
                          var lastCommit:Long) extends BaseMongoObj
}
