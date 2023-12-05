name := "Hakka"

version := "0.1"

scalaVersion := "2.11.8"

scalacOptions := Seq("-unchecked", "-deprecation", "-encoding", "utf8")


libraryDependencies ++= {
  val akkaVersion = "2.5.11"
  val akkaHttpVersion = "10.1.0"
  val reactivemongoV = "0.15.0"
  Seq(
    "com.typesafe.akka" %% "akka-actor" % akkaVersion,
    "com.typesafe.akka" %% "akka-slf4j" % akkaVersion,
    "com.typesafe.akka" %% "akka-stream" % akkaVersion,
    "com.typesafe.akka" %% "akka-cluster" % akkaVersion,
    "com.typesafe.akka" %% "akka-cluster-tools" % akkaVersion,
    "com.typesafe.akka" %% "akka-http" % akkaHttpVersion,
    "com.typesafe.akka" %% "akka-testkit" % akkaVersion,
    "ch.megard" %% "akka-http-cors" % "0.2.2",
    "org.scalatest" %% "scalatest" % "3.0.1" % "test",
    "com.typesafe.akka" %% "akka-http-testkit" % akkaHttpVersion % "test",
    "com.typesafe.play" %% "play-json" % "2.5.15",
    "org.reactivemongo" %% "reactivemongo" % reactivemongoV,
    "org.scalaj" %% "scalaj-http" % "2.3.0"
//    "org.reactivemongo" %% "reactivemongo-play-json" % reactivemongoV
  )
}

libraryDependencies += "ch.qos.logback" % "logback-classic" % "1.2.3"
libraryDependencies += "net.minidev" % "json-smart" % "2.4.8"
libraryDependencies += "com.typesafe.akka" %% "akka-http-spray-json" % "10.1.0"

