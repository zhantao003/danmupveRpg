namespace ETModel
{
	public static class EventIdType
	{
		public const string RecvHotfixMessage = "RecvHotfixMessage";
		public const string BehaviorTreeRunTreeEvent = "BehaviorTreeRunTreeEvent";
		public const string BehaviorTreeOpenEditor = "BehaviorTreeOpenEditor";
		public const string BehaviorTreeClickNode = "BehaviorTreeClickNode";
		public const string BehaviorTreeAfterChangeNodeType = "BehaviorTreeAfterChangeNodeType";
		public const string BehaviorTreeCreateNode = "BehaviorTreeCreateNode";
		public const string BehaviorTreePropertyDesignerNewCreateClick = "BehaviorTreePropertyDesignerNewCreateClick";
		public const string BehaviorTreeMouseInNode = "BehaviorTreeMouseInNode";
		public const string BehaviorTreeConnectState = "BehaviorTreeConnectState";
		public const string BehaviorTreeReplaceClick = "BehaviorTreeReplaceClick";
		public const string BehaviorTreeRightDesignerDrag = "BehaviorTreeRightDesignerDrag";
		public const string SessionRecvMessage = "SessionRecvMessage";
		public const string NumbericChange = "NumbericChange";
		public const string MessageDeserializeFinish = "MessageDeserializeFinish";
		public const string SceneChange = "SceneChange";
		public const string FrameUpdate = "FrameUpdate";
		public const string LoadingBegin = "LoadingBegin";
		public const string LoadingFinish = "LoadingFinish";
		public const string MaxModelEvent = "MaxModelEvent";

        //网络事件
        public const string SessionDisconnect = "SessionDisconnect";
        public const string SessionCallError = "SessionCallError";
        public const string LoginOut = "LoginOut";  //登出事件

        //自定义事件
        public const string ETFrameWorkInitFinish = "ETFrameWorkInitFinish";    //ET框架初始化完成   
		public const string LoginFinish = "LoginFinish";
	}
}