package com.bakka.entry.zone

import com.bakka.extension.BaseExtension
import com.bakka.entry.role.Role
import com.typesafe.config.Config
import play.api.libs.json.JsValue

class Zone(val id: Int,
           val name: String,
           private var extension: BaseExtension) {

  def this(cfg: Config) {
    this(cfg.getInt("id"),
      cfg.getString("name"),
      Class.forName(cfg.getString("ext_class")).newInstance().asInstanceOf[BaseExtension])
  }

  def init(): Unit = {
    println("")
  }

  def destory(): Unit ={

  }

  def handleRequest(role: Role, msg: JsValue): Unit = {

  }

  def send(cmd: String, msg: JsValue, role: Role): Unit = {

  }

  def send(cmd: String, msg: JsValue, roles: List[Role]): Unit = {

  }

}
