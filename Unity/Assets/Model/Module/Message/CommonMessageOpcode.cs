using ETModel;
namespace ETModel
{
	[Message(CommonMessageOpcode.C2R_Regist)]
	public partial class C2R_Regist : IRequest {}

	[Message(CommonMessageOpcode.R2C_Regist)]
	public partial class R2C_Regist : IResponse {}

	[Message(CommonMessageOpcode.C2R_Login)]
	public partial class C2R_Login : IRequest {}

	[Message(CommonMessageOpcode.R2C_Login)]
	public partial class R2C_Login : IResponse {}

	[Message(CommonMessageOpcode.C2G_LoginGate)]
	public partial class C2G_LoginGate : IRequest {}

	[Message(CommonMessageOpcode.G2C_LoginGate)]
	public partial class G2C_LoginGate : IResponse {}

//登出
	[Message(CommonMessageOpcode.C2G_LogOut)]
	public partial class C2G_LogOut : IMessage {}

//被踢下线
	[Message(CommonMessageOpcode.G2C_KickOut)]
	public partial class G2C_KickOut : IMessage {}

//获取用户信息
	[Message(CommonMessageOpcode.C2G_GetUserInfo)]
	public partial class C2G_GetUserInfo : IRequest {}

//返回用户信息
	[Message(CommonMessageOpcode.G2C_GetUserInfo)]
	public partial class G2C_GetUserInfo : IResponse {}

//创建房间
	[Message(CommonMessageOpcode.C2G_CreateRoom)]
	public partial class C2G_CreateRoom : IRequest {}

//返回创建房间信息
	[Message(CommonMessageOpcode.G2C_CreateRoom)]
	public partial class G2C_CreateRoom : IResponse {}

//退出房间
	[Message(CommonMessageOpcode.C2G_ExitRoom)]
	public partial class C2G_ExitRoom : IMessage {}

//加入房间
	[Message(CommonMessageOpcode.C2G_JoinRoom)]
	public partial class C2G_JoinRoom : IRequest {}

//加入房间的反馈
	[Message(CommonMessageOpcode.G2C_JoinRoom)]
	public partial class G2C_JoinRoom : IResponse {}

//获取房间列表
	[Message(CommonMessageOpcode.C2G_GetRoomList)]
	public partial class C2G_GetRoomList : IRequest {}

//获取房间列表反馈
	[Message(CommonMessageOpcode.G2C_GetRoomList)]
	public partial class G2C_GetRoomList : IResponse {}

//进入房间(广播)
	[Message(CommonMessageOpcode.Actor_EnterRoom_M2C)]
	public partial class Actor_EnterRoom_M2C : IActorMessage {}

//退出房间(广播)
	[Message(CommonMessageOpcode.Actor_PlayerEnterRoom_M2C)]
	public partial class Actor_PlayerEnterRoom_M2C : IActorMessage {}

//退出房间(广播)
	[Message(CommonMessageOpcode.Actor_PlayerExitRoom_M2C)]
	public partial class Actor_PlayerExitRoom_M2C : IActorMessage {}

//准备
	[Message(CommonMessageOpcode.C2G_GameReady)]
	public partial class C2G_GameReady : IMessage {}

//退出房间(广播)
	[Message(CommonMessageOpcode.Actor_PlayerReady_M2C)]
	public partial class Actor_PlayerReady_M2C : IActorMessage {}

//游戏开始广播
	[Message(CommonMessageOpcode.Actor_GameStart_M2C)]
	public partial class Actor_GameStart_M2C : IActorMessage {}

//游戏开始广播
	[Message(CommonMessageOpcode.Actor_GameReconnect_M2C)]
	public partial class Actor_GameReconnect_M2C : IActorMessage {}

//查询已有游戏
	[Message(CommonMessageOpcode.C2G_IsInGame)]
	public partial class C2G_IsInGame : IRequest {}

	[Message(CommonMessageOpcode.G2C_IsInGame)]
	public partial class G2C_IsInGame : IResponse {}

//重连进入游戏
	[Message(CommonMessageOpcode.C2G_ReconnectGame)]
	public partial class C2G_ReconnectGame : IMessage {}

}
namespace ETModel
{
	public static partial class CommonMessageOpcode
	{
		 public const ushort C2R_Regist = 5001;
		 public const ushort R2C_Regist = 5002;
		 public const ushort C2R_Login = 5003;
		 public const ushort R2C_Login = 5004;
		 public const ushort C2G_LoginGate = 5005;
		 public const ushort G2C_LoginGate = 5006;
		 public const ushort C2G_LogOut = 5007;
		 public const ushort G2C_KickOut = 5008;
		 public const ushort C2G_GetUserInfo = 5009;
		 public const ushort G2C_GetUserInfo = 5010;
		 public const ushort C2G_CreateRoom = 5011;
		 public const ushort G2C_CreateRoom = 5012;
		 public const ushort C2G_ExitRoom = 5013;
		 public const ushort C2G_JoinRoom = 5014;
		 public const ushort G2C_JoinRoom = 5015;
		 public const ushort C2G_GetRoomList = 5016;
		 public const ushort G2C_GetRoomList = 5017;
		 public const ushort Actor_EnterRoom_M2C = 5018;
		 public const ushort Actor_PlayerEnterRoom_M2C = 5019;
		 public const ushort Actor_PlayerExitRoom_M2C = 5020;
		 public const ushort C2G_GameReady = 5021;
		 public const ushort Actor_PlayerReady_M2C = 5022;
		 public const ushort Actor_GameStart_M2C = 5023;
		 public const ushort Actor_GameReconnect_M2C = 5024;
		 public const ushort C2G_IsInGame = 5025;
		 public const ushort G2C_IsInGame = 5026;
		 public const ushort C2G_ReconnectGame = 5027;
	}
}
