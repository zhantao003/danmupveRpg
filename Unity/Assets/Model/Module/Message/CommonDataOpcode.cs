using ETModel;
namespace ETModel
{
	[Message(CommonDataOpcode.DUserInfo)]
	public partial class DUserInfo {}

// 缩略信息
	[Message(CommonDataOpcode.DUserSimpleInfo)]
	public partial class DUserSimpleInfo {}

// 缩略信息
	[Message(CommonDataOpcode.DUserListInfo)]
	public partial class DUserListInfo {}

	[Message(CommonDataOpcode.DPlayerInfo)]
	public partial class DPlayerInfo {}

	[Message(CommonDataOpcode.DRoomConfig)]
	public partial class DRoomConfig {}

	[Message(CommonDataOpcode.DRoomSeatInfo)]
	public partial class DRoomSeatInfo {}

	[Message(CommonDataOpcode.DRoomInfo)]
	public partial class DRoomInfo {}

	[Message(CommonDataOpcode.DRoomSimpleInfo)]
	public partial class DRoomSimpleInfo {}

	[Message(CommonDataOpcode.DPackIconInfo)]
	public partial class DPackIconInfo {}

	[Message(CommonDataOpcode.DGameUnit)]
	public partial class DGameUnit {}

	[Message(CommonDataOpcode.DFindPlayer)]
	public partial class DFindPlayer {}

	[Message(CommonDataOpcode.DVec3)]
	public partial class DVec3 {}

	[Message(CommonDataOpcode.DVec4)]
	public partial class DVec4 {}

	[Message(CommonDataOpcode.DLockStepFrameEvent)]
	public partial class DLockStepFrameEvent {}

}
namespace ETModel
{
	public static partial class CommonDataOpcode
	{
		 public const ushort DUserInfo = 10001;
		 public const ushort DUserSimpleInfo = 10002;
		 public const ushort DUserListInfo = 10003;
		 public const ushort DPlayerInfo = 10004;
		 public const ushort DRoomConfig = 10005;
		 public const ushort DRoomSeatInfo = 10006;
		 public const ushort DRoomInfo = 10007;
		 public const ushort DRoomSimpleInfo = 10008;
		 public const ushort DPackIconInfo = 10009;
		 public const ushort DGameUnit = 10010;
		 public const ushort DFindPlayer = 10011;
		 public const ushort DVec3 = 10012;
		 public const ushort DVec4 = 10013;
		 public const ushort DLockStepFrameEvent = 10014;
	}
}
