package com.bakka.entry.role

import java.net.InetSocketAddress
import java.util.UUID

import akka.actor.{Actor, ActorLogging, ActorRef}
import com.bakka.entry.{CreateRole, RoleOffline}
import com.bakka.entry.zone.Zone

import scala.collection.mutable

class RoleManager private extends Actor with ActorLogging {
  log.info("RoleManager instanced!!")

  private[role] var roleCache = mutable.HashMap[ActorRef, Role]()

  private[role] def createRole(connRef: ActorRef, remote: InetSocketAddress): Unit = {
    val roleID = UUID.randomUUID().toString

    val zone: Zone = null

    val role = new Role(roleID, connRef, remote)
    role.setZone(zone)

    roleCache += (connRef -> role)
    log.debug(s"New role $role, online: ${roleCache.size}")
  }

  private[role] def removeRole(connRef: ActorRef): Unit = {
    if (roleCache.contains(connRef)) {
      val role = roleCache(connRef)
      roleCache -= connRef
      log.debug(s"Remove role $role, online: ${roleCache.size}")
    }
  }

  def getRole(connRef: ActorRef): Option[Role] = roleCache.get(connRef)

  override def receive: Receive = {
    case CreateRole(actorRef, remote) =>
      createRole(actorRef, remote)
    case RoleOffline(actorRef) =>
      removeRole(actorRef)
  }
}

object RoleManager {

  private val _instance = new RoleManager

  def apply(): RoleManager = _instance

}
