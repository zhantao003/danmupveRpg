import akka.actor._
import akka.stream._
import akka.stream.scaladsl._
import akka._
import scala.concurrent._

object FlowSourceSink extends App {
  implicit val sys=ActorSystem("demo")
  implicit val mat=ActorMaterializer()
  implicit val ec=sys.dispatcher

  val s1: Source[Int,NotUsed] = Source(1 to 10)
  val sink: Sink[Any,Future[Done]] = Sink.foreach(println)
  val rg1: RunnableGraph[NotUsed] = s1.to(sink)
  val rg2: RunnableGraph[Future[Done]]  = s1.toMat(sink)(Keep.right)
  val res1: NotUsed = rg1.run()

  Thread.sleep(1000)

  val res2: Future[Done] = rg2.run()
  res2.andThen {
    case _ =>   //sys.terminate()
  }

  val seq = Seq[Int](1,2,3)
  def toIterator() = seq.iterator
  val flow1: Flow[Int,Int,NotUsed] = Flow[Int].map(_ + 2)
  val flow2: Flow[Int,Int,NotUsed] = Flow[Int].map(_ * 3)
  val s2 = Source.fromIterator(toIterator)
  val s3 = s1 ++ s2

  val s4: Source[Int,NotUsed] = s3.viaMat(flow1)(Keep.right)
  val s5: Source[Int,NotUsed] = s3.via(flow1).async.viaMat(flow2)(Keep.right)
  val s6: Source[Int,NotUsed] = s4.async.viaMat(flow2)(Keep.right)
  (s5.toMat(sink)(Keep.right).run()).andThen {case _ => } //sys.terminate()}

  s1.runForeach(println)
  val fres = s6.runFold(0)(_ + _)
  fres.onSuccess{case a => println(a)}
  fres.andThen{case _ => sys.terminate()}
}
