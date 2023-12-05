package com.bakka.handler.main

import akka.actor.TypedActor.context

import java.util.{Timer, TimerTask}
import akka.actor.{ActorSystem, Props}
import cn.vrspy.lmgame.dao.user.UserLogic
import com.bakka.actor.cluster.SimpleClusterListener
import com.bakka.actor.connect.{HttpConnectActor, TcpConnectActor, WebSocketActor}
import com.bakka.config.ConfigManager
import com.bakka.entry.zone.ZoneManager
import com.bakka.util.CommonUtils._

import java.util.concurrent.{ExecutorService, Executors, ScheduledExecutorService, TimeUnit}

object ServerMain {

  def main(args: Array[String]): Unit = {
    init()
    implicit val server = ActorSystem("BakkaServer")

    //initCluster()

    bindWebSocket()
    //GachaLogic.loadGachaConfig()
    //GameFishLogic.loadGachaBoxConfig();

    rankingListTask()

    checkCD()

    //清理nonce
    cleanNonce()
//    task()
  }

  def initCluster()(implicit server: ActorSystem): Unit = {
    server.actorOf(Props(new SimpleClusterListener), "simpleClusterListener")
  }

  def bindWebSocket()(implicit server: ActorSystem): Unit = {
    val config = ConfigManager().get("ws-server")
    server.actorOf(Props(new WebSocketActor(config.getString("host"), config.getInt("port"))))
  }

  def checkCD():Unit = {
    val timer = new Timer()
    timer.schedule(new TimerTask {
      override def run(): Unit = {
        if(UserLogic.isCD)
          {
            val cdTimer = new Timer()
            cdTimer.schedule(new TimerTask {
              override def run(): Unit = {
                UserLogic.isCD = false
                cdTimer.cancel()
              }
            },0,500)
          }
      }
    },0,500)
  }

  def init(): Unit = {
    ConfigManager()
  }

  def task() = {
    val timer = new Timer()
    timer.schedule(new TimerTask {
      override def run(): Unit = {
        //OnlineLogic.pushOLInfo()
        //GameFishLogic.addUserFishFes(154999,1,1000,true)
      }
    },2000,50)
  }

  def cleanNonce() = {
    val timer = new Timer()
    timer.schedule(new TimerTask {
      override def run(): Unit = {
        seqRequestNonce = Seq()
      }
    },0,overTime)
  }

  def rankingListTask():Unit = {
    val runnable: Runnable = new Runnable {
      override def run(): Unit = {
        //RankingListLogic.generateCallCoinRL()
        //RankingListLogic.generateTotalBatteryRL()
        //RankingListLogic.generateFishCoinRL()
        //RankingListLogic.generateFishMapLevelRL()
      }
    }
      val service: ScheduledExecutorService = Executors.newSingleThreadScheduledExecutor()
      //service.scheduleAtFixedRate(runnable,10,10 * 60, TimeUnit.SECONDS)
      service.scheduleAtFixedRate(runnable,3,10*60, TimeUnit.SECONDS)
  }

}
