package com.bakka.config

import com.typesafe.config.{Config, ConfigFactory}

class ConfigManager private() {

  private var configRoot: Config = _

  load()

  // TODO: Auto load

  def load(): Unit = {
    configRoot = ConfigFactory.load("bakka.conf").getConfig("bakka")
  }

  def get(keys: String*): Config = {
    var cfg: Config = configRoot
    keys.foreach(key => cfg = cfg.getConfig(key))
    cfg
  }

}

object ConfigManager {

  private val _instance = new ConfigManager

  def apply(): ConfigManager = _instance

}
