/// <summary>
/// 内部消息处理Handler
/// </summary>
public interface INetEventHandler {
    void OnMsgHandler(CLocalNetMsg pMsg);

    void OnErrorCode(CLocalNetMsg pMsg);
}
