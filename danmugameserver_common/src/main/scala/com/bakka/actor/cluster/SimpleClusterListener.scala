package com.bakka.actor.cluster

import akka.actor.{Actor, ActorLogging}
import akka.cluster.ClusterEvent.{MemberEvent, MemberRemoved, MemberUp, UnreachableMember}
import akka.cluster.{Cluster, ClusterEvent}

class SimpleClusterListener extends Actor with ActorLogging {
  val cluster = Cluster(context.system)

  override def preStart() = {
    cluster.subscribe(self, ClusterEvent.initialStateAsEvents, classOf[MemberEvent], classOf[UnreachableMember])
  }

  override def postStop()= {
    cluster.unsubscribe(self)
  }

  override def receive: Receive = {
    case mUp: MemberUp =>
      log.info("Member is Up: {}", mUp.member)

    case mUnreachable: UnreachableMember =>
      log.info("Member detected as unreachable: {}，", mUnreachable.member)
    case mRemoved: MemberRemoved =>
      log.info("Member is Removed: {}", mRemoved.member)
    case mEvent: MemberEvent =>
    case msg: Any => unhandled(msg)
  }
}
