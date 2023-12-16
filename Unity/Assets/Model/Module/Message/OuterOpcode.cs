using ETModel;
namespace ETModel
{
	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.C2G_HeartBeat)]
	public partial class C2G_HeartBeat : IRequest {}

	[Message(OuterOpcode.G2C_HeartBeat)]
	public partial class G2C_HeartBeat : IResponse {}

	[Message(OuterOpcode.C2M_Reload)]
	public partial class C2M_Reload : IRequest {}

	[Message(OuterOpcode.M2C_Reload)]
	public partial class M2C_Reload : IResponse {}

	[Message(OuterOpcode.C2G_StartMatchGame)]
	public partial class C2G_StartMatchGame : IRequest {}

	[Message(OuterOpcode.G2C_StartMatchGame)]
	public partial class G2C_StartMatchGame : IResponse {}

	[Message(OuterOpcode.Actor_CreateTetsPlayerUnit_C2M)]
	public partial class Actor_CreateTetsPlayerUnit_C2M : IActorMessage {}

	[Message(OuterOpcode.C2G_CancelMatchGame)]
	public partial class C2G_CancelMatchGame : IRequest {}

	[Message(OuterOpcode.G2C_CancelMatchGame)]
	public partial class G2C_CancelMatchGame : IResponse {}

	[Message(OuterOpcode.C2G_GetOnlineUserList)]
	public partial class C2G_GetOnlineUserList : IRequest {}

	[Message(OuterOpcode.G2C_GetOnlineUserList)]
	public partial class G2C_GetOnlineUserList : IResponse {}

	[Message(OuterOpcode.C2G_InvitePk)]
	public partial class C2G_InvitePk : IRequest {}

	[Message(OuterOpcode.G2C_InvitePk)]
	public partial class G2C_InvitePk : IResponse {}

	[Message(OuterOpcode.Actor_InvitePk_G2C)]
	public partial class Actor_InvitePk_G2C : IMessage {}

	[Message(OuterOpcode.Actor_AcceptInvitePk_C2G)]
	public partial class Actor_AcceptInvitePk_C2G : IMessage {}

//同步游戏逻辑帧
	[Message(OuterOpcode.Actor_LockStepFrame_M2C)]
	public partial class Actor_LockStepFrame_M2C : IActorMessage {}

//同步游戏帧事件
	[Message(OuterOpcode.Actor_LockStepEvent_C2M)]
	public partial class Actor_LockStepEvent_C2M : IActorMessage {}

//同步游戏超时
	[Message(OuterOpcode.Actor_LockStepGameTimeOut_M2C)]
	public partial class Actor_LockStepGameTimeOut_M2C : IActorMessage {}

//同步游戏结束
	[Message(OuterOpcode.Actor_LockStepGameEnd_C2M)]
	public partial class Actor_LockStepGameEnd_C2M : IActorMessage {}

//同步游戏开始
	[Message(OuterOpcode.Actor_LockStepGameStart_C2M)]
	public partial class Actor_LockStepGameStart_C2M : IActorMessage {}

//获取同步游戏开始的消息
	[Message(OuterOpcode.Actor_GetLockStepGameStart_M2C)]
	public partial class Actor_GetLockStepGameStart_M2C : IActorMessage {}

}
namespace ETModel
{
	public static partial class OuterOpcode
	{
		 public const ushort C2R_Ping = 101;
		 public const ushort R2C_Ping = 102;
		 public const ushort C2G_HeartBeat = 103;
		 public const ushort G2C_HeartBeat = 104;
		 public const ushort C2M_Reload = 105;
		 public const ushort M2C_Reload = 106;
		 public const ushort C2G_StartMatchGame = 107;
		 public const ushort G2C_StartMatchGame = 108;
		 public const ushort Actor_CreateTetsPlayerUnit_C2M = 109;
		 public const ushort C2G_CancelMatchGame = 110;
		 public const ushort G2C_CancelMatchGame = 111;
		 public const ushort C2G_GetOnlineUserList = 112;
		 public const ushort G2C_GetOnlineUserList = 113;
		 public const ushort C2G_InvitePk = 114;
		 public const ushort G2C_InvitePk = 115;
		 public const ushort Actor_InvitePk_G2C = 116;
		 public const ushort Actor_AcceptInvitePk_C2G = 117;
		 public const ushort Actor_LockStepFrame_M2C = 118;
		 public const ushort Actor_LockStepEvent_C2M = 119;
		 public const ushort Actor_LockStepGameTimeOut_M2C = 120;
		 public const ushort Actor_LockStepGameEnd_C2M = 121;
		 public const ushort Actor_LockStepGameStart_C2M = 122;
		 public const ushort Actor_GetLockStepGameStart_M2C = 123;
	}
}
