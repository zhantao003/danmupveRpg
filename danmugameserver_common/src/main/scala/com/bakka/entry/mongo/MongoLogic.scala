package com.bakka.entry.mongo

import reactivemongo.api.collections.bson.BSONCollection
import reactivemongo.bson.{Macros, document}

import scala.concurrent.{Await, Future}

object MongoLogic {

  import MongoOps._
  import com.bakka.util.CommonUtils._

  private val colIdentityValues = "_identity_values"
  private val identityValuesCollection = bakkaDB.map(_.collection[BSONCollection](colIdentityValues))
  private implicit val colIdentityValuesHandler = Macros.handler[IdentityValues]

  def getIdentityValue(key: String): Future[Int] = {
    findAndUpdateCollection[IdentityValues](identityValuesCollection,
      document("key" -> "Test"),
      document("$inc" -> document("value" -> 1)), fetchNewObj = true, upsert = true).map(_.value)
  }

  private[this] val colUserToken = "_user_token"
  val userTokenCollection = bakkaDB.map(_.collection[BSONCollection](colUserToken))
  implicit val colUserTokenHandler = Macros.handler[UserToken]

  def refreshToken(userID: String): Future[String] = {
    val token = newUUID
    for {
      res <- updateCollection(userTokenCollection, document("userID" -> userID),
        document("userID" -> userID, "token" -> token, "lastUpdate" -> now), upsert = true).map(_.n)
    } yield token
  }

  def validateToken(uid: String, token: String): Future[Boolean] = {
    for (userToken <- findCollectionOne[UserToken](userTokenCollection, document("userID" -> uid, "token" -> token)))
      yield if (userToken == null) false else true
  }

  private def createCustomIndex(): Future[String] = {
    val indexSettings = Array(
      ("key", 1, true, 0)
    )
    createIndex(colIdentityValues, indexSettings)
  }

  private def createUserTokenIndex(): Future[String] = {
    val indexSettings = Array(
      ("userID", 1, true, 0)
    )
    createIndex(colUserToken, indexSettings)
  }

  createCustomIndex()
  createUserTokenIndex()

}
