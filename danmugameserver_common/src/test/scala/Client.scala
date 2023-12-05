import java.net.InetSocketAddress

import akka.actor.{Actor, ActorRef, ActorSystem, Props}
import akka.io._
import akka.util.ByteString
import com.typesafe.config.ConfigFactory

object Client {
  def props(remote: InetSocketAddress, replies: ActorRef) =
    Props(new Client(remote, replies))

  def main(args: Array[String]): Unit = {
    val config = ConfigFactory.load.getConfig("tcp-server")
    implicit val client = ActorSystem("WebSocketClient")
    client.actorOf(props(new InetSocketAddress(config.getString("host"), config.getInt("port")), client.actorOf(Props(new ResponseHandler))))
  }

}

class ResponseHandler extends Actor {

  import Tcp._

  override def receive: Receive = {
    case data: (String, ActorRef) =>
      data._2 ! Write(ByteString("Hello"))
  }
}

class Client(remote: InetSocketAddress, listener: ActorRef) extends Actor {

  import Tcp._
  import context.system

  IO(Tcp) ! Connect(remote)

  def receive = {
    case CommandFailed(_: Connect) =>
      listener ! "connect failed"
      context stop self

    case c @ Connected(_, _) =>
      val connection = sender()
      listener ! (c, connection)
      connection ! Register(self)
      context become {
        case data: ByteString =>
          connection ! Write(data)
        case CommandFailed(w: Write) =>
          // O/S buffer was full
          listener ! ("write failed", connection)
        case Received(data) =>
          listener ! (data, connection)
        case "close" =>
          connection ! Close
        case _: ConnectionClosed =>
          listener ! ("connection closed", connection)
          context stop self
      }
  }
}