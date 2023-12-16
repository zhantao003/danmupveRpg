namespace ETModel
{
	public static class ErrorCode
	{
		public const int ERR_Success = 0;
		
		// 1-11004 是SocketError请看SocketError定义
		//-----------------------------------
		// 100000 以上，避免跟SocketError冲突
		public const int ERR_MyErrorCode = 100000;
		public const int ERR_NotFoundActor = 100002;
		public const int ERR_ActorNoMailBoxComponent = 100003;
		public const int ERR_ActorRemove = 100004;
		public const int ERR_PacketParserError = 100005;
		public const int ERR_ConnectGateKeyError = 100006;
		public const int ERR_KcpCantConnect = 102005;
		public const int ERR_KcpChannelTimeout = 102006;
		public const int ERR_KcpRemoteDisconnect = 102007;
		public const int ERR_PeerDisconnect = 102008;
		public const int ERR_SocketCantSend = 102009;
		public const int ERR_SocketError = 102010;
		public const int ERR_KcpWaitSendSizeTooLarge = 102011;

		public const int ERR_WebsocketPeerReset = 103001;
		public const int ERR_WebsocketMessageTooBig = 103002;
		public const int ERR_WebsocketError = 103003;
		public const int ERR_WebsocketConnectError = 103004;
		public const int ERR_WebsocketSendError = 103005;
		public const int ERR_WebsocketRecvError = 103006;
		
		public const int ERR_RpcFail = 102001;
		public const int ERR_ReloadFail = 102003;
		
		public const int ERR_ActorLocationNotFound = 102004;
		
		//-----------------------------------
		// 小于这个Rpc会抛异常，大于这个异常的error需要自己判断处理，也就是说需要处理的错误应该要大于该值
		public const int ERR_Exception = 200000;

        public const int C_AccountOrPasswordError = 2000001;    //账号密码错误
        public const int C_AccountAlreadyRegisted = 2000002;    //账号已被注册
        public const int C_UserNotOnline = 2000003;             //用户不在线
        public const int C_UserDataEmpty = 2000003;             //用户没有数据
        public const int C_CreateRoomFail = 2000004;            //创建房间失败
        public const int C_PlayerAlreadyInRoom = 2000005;       //已经在房间里
        public const int C_NoRoom = 2000007;                    //房间不存在
        public const int C_RoomIsGaming = 2000008;              //房间在游戏种
        public const int C_RoomIsFull = 2000009;                //房间满员
        public const int C_AccountRegistFailed = 2000015;       //注册失败
        public const int C_MatchGameFailed = 2000016;           //匹配游戏失败

        //-----------------------------------
        public static bool IsRpcNeedThrowException(int error)
		{
			if (error == 0)
			{
				return false;
			}

			if (error > ERR_Exception)
			{
				return false;
			}

			return true;
		}
	}
}