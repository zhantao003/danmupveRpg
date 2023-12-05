package cn.vrspy.lmgame.dao.gift

import com.bakka.entry.mongo.MongoOps._
import reactivemongo.api.collections.bson.BSONCollection
import reactivemongo.bson.{Macros, document}

import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, Future}

object GiftLogic {

  import com.bakka.util.CommonUtils._

  private[dao] val colGift = "giftList"
  val giftCollection = bakkaDB.map(_.collection[BSONCollection](colGift))
  implicit val colGiftHandler = Macros.handler[Gift]

  def sendCashGift(uid:Long,roomId:Long,vtbUid:Long,gameId:Long,giftType:Int,giftId:Long,giftName:String,giftNum:Long,battery:Long): Future[Gift] =
  {
    for{
      res <- insertCollection[Gift](giftCollection, Gift(null, uid, roomId, vtbUid,gameId,giftType,giftId,giftName,giftNum,battery,true,now))
    }
      yield res._1
  }

  def sendFreeGift(uid:Long,roomId:Long,vtbUid:Long,gameId:Long,giftId:Long,giftName:String,giftNum:Long,battery:Long):Future[Gift] =
  {
    for{
      res <- insertCollection[Gift](giftCollection, Gift(null, uid, roomId, vtbUid,gameId,0,giftId,giftName,giftNum,battery,false,now))
    }
    yield res._1
  }
}
