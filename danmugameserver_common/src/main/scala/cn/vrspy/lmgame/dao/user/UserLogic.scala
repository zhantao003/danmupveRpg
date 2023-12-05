package cn.vrspy.lmgame.dao.user

import com.bakka.entry.mongo.MongoLogic.refreshToken
import com.bakka.entry.mongo.MongoOps._
import reactivemongo.api.collections.bson.BSONCollection
import reactivemongo.bson.{Macros, document}

import scala.annotation.tailrec
import scala.concurrent.duration.DurationInt
import scala.concurrent.{Await, Future}

object UserLogic {

  import com.bakka.util.CommonUtils._

  private[dao] val colVtb = "userVtuber"
  private[dao] val colVtbFan = "userViewer"
  val vtbCollection = bakkaDB.map(_.collection[BSONCollection](colVtb))
  val vtbFanCollection = bakkaDB.map(_.collection[BSONCollection](colVtbFan))
  implicit val colVtbHandler = Macros.handler[userVtuber]
  implicit val colVtbFanHandler = Macros.handler[userViewer]

  def Index = Map(
    colVtb -> Array(
      ("uid", 1, true, 0),
      ("roomId", 1, true, 0)
    ),
    colVtbFan -> Array(
      ("uid", 1, true, 0)
    )
  )

//  def CompoundIndex = Map(
//    colUserRareFishInfo -> Array(
//      ("uid-rareFishId",1,true,0)
//    )
//  )
//
//  def TripleCompoundIndex = Map(
//    colRoomRareFishInfo -> Array(
//      ("uid-roomId-rareFishId",1,true,0)
//    )
//  )

  Index.foreach(idx => createIndex(idx._1, idx._2))
  //CompoundIndex.foreach(idx => createCompoundIndex(idx._1,idx._2))
  //TripleCompoundIndex.foreach(idx => createTripleCompoundIndex(idx._1,idx._2))


  def vtbLogin(uid: String, roomId: String, code: String, nickName: String, headIcon: String, userType: Int, channel: String): Future[(userVtuber, String)] = {
    for {
      res <- getVtbAccount(uid, roomId)
      account <-  if(res == null) createVtbAccount(uid,roomId, code, nickName, headIcon, userType, channel) else isVtbCodeCorrect(uid,roomId,code)
      token <- if(account != null) refreshToken(account.roomId) else Future.successful(null)
    } yield
      {
        var newNickName = nickName
        var newHeadIcon = headIcon
        if(account !=null)
          {
            if(newNickName.isEmpty() ||
               newHeadIcon.isEmpty())
              {
                updateCollection(vtbCollection, document("uid" -> uid, "roomId" ->roomId), document("$set" -> document("userType" -> userType)))
              }
            else
              {
                updateCollection(vtbCollection, document("uid" -> uid, "roomId" ->roomId), document("$set" -> document("nickName" -> nickName,"headIcon" -> headIcon,"userType" -> userType)))
              }
          }

        (account, token)
      }
  }

  def getVtbAccount(uid:String,roomId:String):Future[userVtuber] = {
    for{
      account <- findCollectionOne[userVtuber](vtbCollection, document("uid" -> uid,"roomId" ->roomId))
    }
    yield
      {
        account
      }
  }

  //验证主播身份码和房间号对不对得上
  def isVtbCodeCorrect(uid:String,roomId:String,code:String):Future[userVtuber] = {
    for{
      account <- findCollectionOne[userVtuber](vtbCollection, document("uid" -> uid,"roomId" ->roomId,"code" ->code))
    }
    yield
    {
      account
    }
  }

  def getVtbByID(uid:String):Future[userVtuber] = {
    for{
      account <- findCollectionOne[userVtuber](vtbCollection, document("uid" -> uid))
    } yield account
  }

  def getVtbByRoomId(roomId:Long):Future[userVtuber] = {
    for{
      account <- findCollectionOne[userVtuber](vtbCollection, document("roomId" -> roomId))
    } yield account
  }

  def createVtbAccount(uid:String, roomId:String, code:String, nickName: String, headIcon:String,userType:Int, channel:String): Future[userVtuber] = {
    val userID = newUUID
    for {
      res <- insertCollection[userVtuber](vtbCollection, userVtuber(null, userID, uid, roomId, code, nickName,headIcon,userType,channel,0,now,now))
    } yield res._1
  }

  def fanLoginSync(uid: String, roomId:String, nickName: String, headIcon: String, userType:Int, channel:String, fansMedalLevel:Long, fansMedalName: String, fansMedalWearingStatus: Boolean, guardLevel: Long): (userViewer, String) = {
        val res = getFanAccountSync(uid)

        val account = Await.result(if(res == null && !uid.isEmpty()) createFanAccount(uid, roomId, nickName, headIcon, userType,channel,fansMedalLevel,fansMedalName,fansMedalWearingStatus,guardLevel)
                                   else findAndUpdateCollection[userViewer](vtbFanCollection, document("uid" -> uid), document(
                                    "$set"-> document(
                                      "nickName" -> nickName,
                                      "headIcon" -> headIcon,
                                      "userType" -> userType,
                                      "fansMedalLevel" -> fansMedalLevel,
                                      "fansMedalName" -> fansMedalName,
                                      "fansMedalWearingStatus" -> fansMedalWearingStatus,
                                      "guardLevel" -> guardLevel)),fetchNewObj = true),1.seconds)
        val token = Await.result(if(account != null) refreshToken(account.userid) else Future.successful(null),1.seconds)

        (account, token)
  }
  
  def fanLogin(uid: String, roomId:String, nickName: String, headIcon: String, userType: Int,channel:String, fansMedalLevel:Long, fansMedalName: String, fansMedalWearingStatus: Boolean, guardLevel: Long): Future[(userViewer, String)] = {
      for {
        res <- getFanAccount(uid)
        account <- if (res == null && !uid.isEmpty()) createFanAccount(uid, roomId, nickName, headIcon, userType,channel, fansMedalLevel, fansMedalName, fansMedalWearingStatus, guardLevel)
        else
          {
            var newNickName = nickName
            var newHeadIcon = headIcon
            if(newNickName.isEmpty() ||
               newHeadIcon.isEmpty())
              {
                findAndUpdateCollection[userViewer](vtbFanCollection, document("uid" -> uid), document(
                  "$set" -> document(
                    "userType" -> userType,
                    "fansMedalLevel" -> fansMedalLevel,
                    "fansMedalName" -> fansMedalName,
                    "fansMedalWearingStatus" -> fansMedalWearingStatus,
                    "guardLevel" -> guardLevel)), fetchNewObj = true)
              }
            else
              {
                findAndUpdateCollection[userViewer](vtbFanCollection, document("uid" -> uid), document(
                  "$set" -> document(
                    "nickName" -> nickName,
                    "headIcon" -> headIcon,
                    "userType" -> userType,
                    "fansMedalLevel" -> fansMedalLevel,
                    "fansMedalName" -> fansMedalName,
                    "fansMedalWearingStatus" -> fansMedalWearingStatus,
                    "guardLevel" -> guardLevel)), fetchNewObj = true)
              }

          }

        token <- if (account != null) refreshToken(account.userid) else Future.successful(null)
      } yield {
      (account, token)
    }
  }

  //同步获取用户信息
  def getFanAccountSync(uid:String):userViewer = {
      val account = Await.result(findCollectionOne[userViewer](vtbFanCollection, document("uid" -> uid)),1.seconds)
      account
  }

  def getFanAccount(uid:String):Future[userViewer] = {
    for{
      account <- findCollectionOne[userViewer](vtbFanCollection, document("uid" -> uid))
    } yield account
  }

  def createFanAccount(uid:String, roomId:String, nickName:String, headIcon:String,userType:Int,channel:String, fansMedalLevel:Long, fansMedalName:String, fansMedalWearingStatus:Boolean, guardLevel:Long): Future[userViewer] = {
    val userID = newUUID
    for {
      res <- insertCollection[userViewer](vtbFanCollection,
        userViewer(null, userID, uid, roomId, nickName,headIcon,userType,channel,fansMedalLevel,fansMedalName,fansMedalWearingStatus,guardLevel,0,now,now))
    } yield res._1
  }

  def addVtbBattery(uid:String,roomId:String,battery:Long): Future[userVtuber] = {
    for{
      res <- findAndUpdateCollection[userVtuber](vtbCollection,document("uid" -> uid,"roomId" ->roomId),
        document("$inc" -> document("totalBattery" -> battery)),fetchNewObj = true)
    }yield
      res
  }

  def addViewerBattery(uid:String, battery:Long): Future[userVtuber] = {
    for{
      res <- findAndUpdateCollection[userVtuber](vtbFanCollection,document("uid" -> uid),
        document("$inc" -> document("totalBattery" -> battery)),fetchNewObj = true)
    }yield
      res
  }

  def updateSignTime(uid:String):Future[Int] ={
    for {
      res <- updateCollection(vtbFanCollection, document("uid" -> uid),
        document("$set" -> document("lastSignInTime" -> now)))
    }
      yield res.n
  }

  def getVtbListSortByCol(sortCol:String,count:Int):Future[List[userVtuber]] = {
    for(
      list <- findCollection[userVtuber](vtbCollection,document(sortCol -> document("$gt" -> 0)),count,sort = document(sortCol -> -1))
    )
      yield list
  }

  def getViewerListSortByCol(sortCol:String,count:Int):Future[List[userViewer]] = {
    for(
      list <- findCollection[userViewer](vtbFanCollection,document(sortCol -> document("$gt" -> 0)),count,sort = document(sortCol -> -1))
    )
    yield list
  }

//  def randomUser():Future[User] ={
//    val count = Await.result(countCollection(userCollection, document()),1.seconds)
//    val rand = (Math.random() * count).intValue()
//    println(rand)
//    for{
//      user <- findCollectionOneSkip[User](userCollection,document(),rand)
//    }yield user
//  }

  var isCD = false

  def getVtbActive(count:Int,page:Int):Future[List[userVtuber]] = {
    println(isCD)
    if(!isCD) {
      isCD = true
      for {
        list <- findCollection[userVtuber](vtbCollection, document("lastActiveTime" -> document("$gte" -> (now - 10 * 60 * 1000))), count, page)
      }
      yield list
    }
    else Future(null)
  }
}
