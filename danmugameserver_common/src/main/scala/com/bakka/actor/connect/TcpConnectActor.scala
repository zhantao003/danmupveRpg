package com.bakka.actor.connect

import java.net.InetSocketAddress

import akka.actor.{Actor, ActorLogging, Props}
import akka.io.Tcp._
import akka.io.{IO, Tcp}
import com.bakka.actor.request.SimpleTcpRequestActor

class TcpConnectActor(val host: String,
                      val port: Int) extends Actor with ActorLogging {

  import context.system

  IO(Tcp) ! Bind(self, new InetSocketAddress(host, port))

  def receive: PartialFunction[Any, Unit] = {
    case b @ Bound(localAddress) =>
      context.parent ! b
      log.info(s"TCP server is listening $localAddress...")

    case CommandFailed(_: Bind) => context stop self

    case Connected(remote, _) =>
      val handler = context.actorOf(Props[SimpleTcpRequestActor])
      val connection = sender()
      connection ! Register(handler)

  }
}
