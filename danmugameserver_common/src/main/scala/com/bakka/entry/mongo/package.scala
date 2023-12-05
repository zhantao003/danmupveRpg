package com.bakka.entry

import reactivemongo.bson.BSONObjectID

package object mongo {
  //mongoDB schema
  trait BaseMongoObj{var _id: BSONObjectID}
  case class UpdateResult(n: Int, errmsg: String, upserted: scala.Seq[reactivemongo.api.commands.Upserted] = Seq())

  case class IdentityValues(var _id: BSONObjectID, key: String, value: Int) extends BaseMongoObj
  case class Test(var _id: BSONObjectID, nID: Int, name: String) extends BaseMongoObj

  case class UserToken(var _id: BSONObjectID,
                       var userID: String,
                       var token: String,
                       var lastUpdate: Long) extends BaseMongoObj

}
