package cn.vrspy.lmgame.dao.online

import java.io.IOException
import java.net.{DatagramPacket, DatagramSocket, InetAddress}
import java.util.Date

import com.bakka.entry.mongo.MongoOps.bakkaDB
import reactivemongo.api.collections.bson.BSONCollection
import reactivemongo.bson.{Macros, document}
import com.bakka.entry.mongo.MongoOps._
import com.bakka.util.CommonUtils.now

object OnlineLogic {

  private[dao] val colOnlineInfo = "onlineInfo"
  val onlineInfoCollection = bakkaDB.map(_.collection[BSONCollection](colOnlineInfo))
  implicit val colOnlineInfoHandler = Macros.handler[OnlineInfo]

  def recordInfo(onlineChange:Int,route:String) = {
    for {
      onlineInfo <- findCollectionOne[OnlineInfo](onlineInfoCollection, document("route" -> route))
    } yield {
      for {
        info <- if (onlineInfo == null) createInfo(route) else findAndUpdateCollection[OnlineInfo](onlineInfoCollection, document("route" -> route), document("$set" -> document("online" -> (onlineInfo.online + onlineChange))))
      } yield info
    }
  }

    def createInfo(route:String) ={
      val onlineInfo = OnlineInfo(null,1,route,now)
      for{
        res <- insertCollection(onlineInfoCollection, onlineInfo)
      }yield res._1
    }

  def pushOLInfo() = {
    for {
      list <- findCollection[OnlineInfo](onlineInfoCollection, document())
    } yield {
      list.foreach( item =>
      {
          val plat = if(item.route == "android") "3|32010001|2006603" else if(item.route == "ios") "1|32010001|1000401" else if(item.route == "oppo") "3|32010001|100603"
          import java.text.SimpleDateFormat
          val format0 = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss")
          val time = format0.format(new Date().getTime)
          val msg = "log_cu|" + plat + "|" + time + "|0|" + item.online
          println(msg)
          try {
            val socket = new DatagramSocket(0)
            try {
              socket.setSoTimeout(10000)
              val host = InetAddress.getByName("140.143.193.143")
              val data = msg.getBytes("UTF-8")
              //指定包要发送的目的地
              val request = new DatagramPacket(data, data.length, host, 4141)
              //为接受的数据包创建空间
              //val response = new DatagramPacket(new Array[Byte](1024), 1024)
              socket.send(request)
              //socket.receive(response)
              //val result = new String(response.getData, 0, response.getLength, "UTF-8")
              //System.out.println(result)
            } catch {
              case e: IOException =>
                e.printStackTrace()
            } finally if (socket != null) socket.close()
          } catch {
            case e: IOException =>
              e.printStackTrace()
          }
          finally{

          }
      })
      None
    }
  }

}
