namespace DouyuDanmu
{
// 公共信息
	[Message(DouyuMessageOpcode.PublicInfo)]
	public partial class PublicInfo {}

// 用户信息
	[Message(DouyuMessageOpcode.UserInfo)]
	public partial class UserInfo {}

// 礼物信息
	[Message(DouyuMessageOpcode.GiftInfo)]
	public partial class GiftInfo {}

// 道具信息
	[Message(DouyuMessageOpcode.PropInfo)]
	public partial class PropInfo {}

// 价值信息
	[Message(DouyuMessageOpcode.WorthInfo)]
	public partial class WorthInfo {}

// 心跳 协议:-2
	[Message(DouyuMessageOpcode.HeartBeat)]
	public partial class HeartBeat : IMessage {}

// token失效 协议:-1
	[Message(DouyuMessageOpcode.TokenInvalid)]
	public partial class TokenInvalid : IMessage {}

// 进入直播间 协议:1
	[Message(DouyuMessageOpcode.EnterRoom)]
	public partial class EnterRoom : IMessage {}

// 弹幕 协议:2
	[Message(DouyuMessageOpcode.Danmu)]
	public partial class Danmu : IMessage {}

// 礼物 协议:3
	[Message(DouyuMessageOpcode.Gift)]
	public partial class Gift : IMessage {}

// 道具 协议:4
	[Message(DouyuMessageOpcode.Prop)]
	public partial class Prop : IMessage {}

// 离开直播间 协议:6
	[Message(DouyuMessageOpcode.LeaveRoom)]
	public partial class LeaveRoom : IMessage {}

    public static partial class DouyuMessageOpcode
    {
        public const sbyte PublicInfo = 100;
        public const sbyte UserInfo = 101;
        public const sbyte GiftInfo = 102;
        public const sbyte PropInfo = 103;
        public const sbyte WorthInfo = 104;

        public const sbyte HeartBeat = -2;
        public const sbyte TokenInvalid = -1;
        public const sbyte EnterRoom = 1;
        public const sbyte Danmu = 2;
        public const sbyte Gift = 3;
        public const sbyte Prop = 4;
        public const sbyte LeaveRoom = 6;
    }
}
