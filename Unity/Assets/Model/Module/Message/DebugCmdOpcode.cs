using ETModel;
namespace ETModel
{
	[Message(DebugCmdOpcode.C2G_CmdAddDiamond)]
	public partial class C2G_CmdAddDiamond : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdAddDiamond)]
	public partial class G2C_CmdAddDiamond : IResponse {}

	[Message(DebugCmdOpcode.C2G_CmdAddMoney)]
	public partial class C2G_CmdAddMoney : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdAddMoney)]
	public partial class G2C_CmdAddMoney : IResponse {}

	[Message(DebugCmdOpcode.C2G_CmdAddIcon)]
	public partial class C2G_CmdAddIcon : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdAddIcon)]
	public partial class G2C_CmdAddIcon : IResponse {}

	[Message(DebugCmdOpcode.C2G_CmdAddIconBG)]
	public partial class C2G_CmdAddIconBG : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdAddIconBG)]
	public partial class G2C_CmdAddIconBG : IResponse {}

	[Message(DebugCmdOpcode.C2G_CmdAddRole)]
	public partial class C2G_CmdAddRole : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdAddRole)]
	public partial class G2C_CmdAddRole : IResponse {}

// 解散房间
	[Message(DebugCmdOpcode.C2G_CmdDelRoom)]
	public partial class C2G_CmdDelRoom : IRequest {}

	[Message(DebugCmdOpcode.G2C_CmdDelRoom)]
	public partial class G2C_CmdDelRoom : IResponse {}

}
namespace ETModel
{
	public static partial class DebugCmdOpcode
	{
		 public const ushort C2G_CmdAddDiamond = 50001;
		 public const ushort G2C_CmdAddDiamond = 50002;
		 public const ushort C2G_CmdAddMoney = 50003;
		 public const ushort G2C_CmdAddMoney = 50004;
		 public const ushort C2G_CmdAddIcon = 50005;
		 public const ushort G2C_CmdAddIcon = 50006;
		 public const ushort C2G_CmdAddIconBG = 50007;
		 public const ushort G2C_CmdAddIconBG = 50008;
		 public const ushort C2G_CmdAddRole = 50009;
		 public const ushort G2C_CmdAddRole = 50010;
		 public const ushort C2G_CmdDelRoom = 50011;
		 public const ushort G2C_CmdDelRoom = 50012;
	}
}
