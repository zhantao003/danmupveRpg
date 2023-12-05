package com.bakka.entry.zone

import com.bakka.config.ConfigManager
import com.typesafe.config.Config

import scala.collection.mutable

class ZoneManager private[zone]() {

  private[zone] var zoneNameMap = mutable.HashMap[String, Zone]()
  private[zone] var zoneIDMap = mutable.HashMap[Int, Zone]()

  private[zone] def init(): Unit = {
    val zoneCfg = ConfigManager().get("zones")
    val zones = zoneCfg.getConfigList("zone")
    for (i <- 0 until zones.size()) {
      val config = zones.get(i)

      addZone(config)
    }
  }

  def addZone(cfg: Config): Zone = {
    val zone = new Zone(cfg)

    zoneNameMap += (zone.name -> zone)
    zoneIDMap += (zone.id -> zone)

    zone
  }

  def getZone(name: String): Option[Zone] = zoneNameMap get name

  init()

}

object ZoneManager {

  private val _instance = new ZoneManager()

  def apply(): ZoneManager = _instance

}
