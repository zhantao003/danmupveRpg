# Hakka

## 开发环境

* IDE Intellij IDEA
* JDK 1.8
* Scala 2.11.8

## APP WebSocket消息

>所有消息格式为{"userID": "xxx", "token": "xxx", "cmd", "xxx", "content": "xxx"}

### Request

cmd|What|content|response
-:|-|-|-
login|登录||无
end|客户端手动退出||断开连接

### Response

cmd|What|content|Descript
-:|-|-|-
reject|拒绝请求|{code: int}|Code 0->UserID/Token error
keepalive|保持连接不超时断开|{}|
randomMatch|随机匹配成功|{gameID:xxx}|UserID为对方玩家id

## Game WebSocket消息

>所有消息格式为{"userID": "xxx", "token": "xxx", "cmd", "xxx", "content": "xxx"}

### Request

cmd|What|content|response
-:|-|-|-
login|登录||无
end|客户端手动退出||断开连接
progressStage|更新资源加载进度|{progress:0-100, finish:true}|Broadcast
bc|客户端自定义消息||Broadcast
gameover|游戏结束|{isWin:true}|

### Response

cmd|What|content|Descript
-:|-|-|-
reject|拒绝请求|{code: int}|Code 0->UserID/Token error
|||Code 1->Game Room not found
keepalive|保持连接不超时断开|{}|
connected|已找到匹配对象|{list:[{userID:xxx,avatar:xxx,name:xxx}],self:{userID:xxx,avatar:xxx,name:xxx}}|content:所有匹配玩家信息，自身玩家信息(未实现)
gameover|游戏结束|{winner: userID}|

### Broadcast(房间每个人都收到相同的消息)

cmd|What|content|Descript
-:|-|-|-
progressStage|更新资源加载进度|{progress:0-100, finish:true}|
ready|可以开始游戏|{}|
bc|客户端自定义消息||Broadcast

## 目录结构

``` Page
+-- Bakka           //根目录
|   +-- .idea       //IDEA配置目录
|   +-- logs        日志目录
|   +-- project
|   +-- src
|   |   +-- main
|   |   |   +-- java
|   |   |   +-- scala
|   |   |   |   +-- cn.vrspy.lmgame
|   |   |   |   |   +-- dao
|   |   |   |   |   |   +-- DAO.scala           // 数据库访问对象
|   |   |   |   |   |   +-- DAOLogic.scala      // 数据库访问逻辑
|   |   |   |   |   +-- route
|   |   |   |   |   |   +-- api                     // 业务逻辑模块 (HTTP)
|   |   |   |   |   |   |   +-- Commons.scala       // 公共方法封装
|   |   |   |   |   |   |   +-- GameMatchAPI.scala  // 匹配业务逻辑
|   |   |   |   |   |   |   +-- UserAPI.scala       // 用户业务逻辑
|   |   |   |   |   |   +-- ws                      // 业务逻辑模块 (WebSocket)
|   |   |   |   |   |   |   +-- GameWSWorker.scala  // 游戏业务逻辑
|   |   |   |   |   |   |   +-- WSWorker.scala      // 平台业务逻辑
|   |   |   |   |   |   +-- APIRoute.scala      // 业务逻辑路由
|   |   |   |   |   |   +-- WSRoute.scala       // WebSocket逻辑路由
|   |   |   |   +-- com.bakka   // 主要框架
|   |   |   |   |   +-- actor           // 主要Actor
|   |   |   |   |   +-- config          // 配置文件管理器
|   |   |   |   |   +-- entry           // 扩展相关
|   |   |   |   |   +-- extension       // 扩展类
|   |   |   |   |   +-- handler         // 处理器
|   |   |   |   |   +-- main            // 服务器入口
|   |   |   |   |   +-- protocol        // 协议
|   |   |   |   |   +-- util            // 工具
|   |   |   |   +-- test        // 略
|   |   |   +-- resources
|   |   |   |   +-- application.conf        // Akka配置文件
|   |   |   |   +-- bakka.conf              // 服务器配置文件
|   |   |   |   +-- logback.xml             // 日志配置文件
|   |   +-- test
|   |   |   +-- http
|   |   |   |   +-- WSTest.html         // websocket逻辑测试
|   +-- target      // 编译输出文件
|   +-- build.sbt   // 项目编译设置
```