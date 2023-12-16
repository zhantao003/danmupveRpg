using SharedLibrary;
/// <summary>
/// 内部消息处理Handler
/// </summary>
public interface INetEventHandler {
    void OnMsgHandler(string resPonseJson);

    void OnErrorCode(string failReason);
}
