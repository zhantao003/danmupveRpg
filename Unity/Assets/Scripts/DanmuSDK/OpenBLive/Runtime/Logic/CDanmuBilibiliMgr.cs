using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NativeWebSocket;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using OpenBLive.Runtime.Utilities;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class CDanmuBilibiliMgr : MonoBehaviour
{
    public static CDanmuBilibiliMgr Ins = null;

    // Start is called before the first frame update
    private WebSocketBLiveClient m_WebSocketBLiveClient;
    public WebSocketBLiveClient WebSocketBLiveClient
    {
        get
        {
            return m_WebSocketBLiveClient;
        }
    }

    private InteractivePlayHeartBeat m_PlayHeartBeat;

    public CDanmuBiliBiliConfig pConfig;
    public CDanmuBilibiliEventHandler pEvent;

    public int nMaxConnectTimes = 10;

    AppStartInfo gameIdResObj;

    [HideInInspector]
    public AppStartAnchorInfo gameAnchorInfo;

    [ReadOnly]
    public string lUserId;
    [ReadOnly]
    public string lRoomID;
    [ReadOnly]
    string szGameID;
    [ReadOnly]
    string szCode;

    //本地测试模式
    public bool bLocalTest = false;
    //开发者模式
    public bool bDevMode = false;

    //断线重连
    bool bNeedReconnect = false;
    float pReconnectTicker = new float();
    System.Action dlgReconnectSuc;

    public string BCode
    {
        get
        {
            return szCode;
        }
    }

    private void Awake()
    {
        if(Ins !=null)
        {
            Destroy(gameObject);
            return;
        }

        Ins = this;
        DontDestroyOnLoad(gameObject);

        //StartGame();
    }

    public async void StartGame(string code, System.Action<int> callSuc = null)
    {
        if (bLocalTest)
        {
            AppStartInfo wsInfo = default;
            m_WebSocketBLiveClient = new WebSocketBLiveClient(wsInfo);
            m_WebSocketBLiveClient.OnDanmaku += pEvent.WebSocketBLiveClientOnDanmaku;
            m_WebSocketBLiveClient.OnGift += pEvent.WebSocketBLiveClientOnGift;
            m_WebSocketBLiveClient.OnGuardBuy += pEvent.WebSocketBLiveClientOnGuardBuy;
            m_WebSocketBLiveClient.OnSuperChat += pEvent.WebSocketBLiveClientOnSuperChat;

            //设置mock的wss对象
            MockDataUtility.webSocketBLiveClient = m_WebSocketBLiveClient;

            szCode = code;
            lRoomID = pConfig.roomId;

            callSuc?.Invoke(0);
        }
        else
        {
            //测试环境的域名现在不可用
            BApi.isTestEnv = false;
            //测试的密钥
            SignUtility.accessKeySecret = pConfig.accessKeySecret;
            //测试的ID
            SignUtility.accessKeyId = pConfig.accessKeyId;

            //获取直播场次信息
            if (bDevMode &&
                string.IsNullOrEmpty(code))
            {
                code = pConfig.code;
            }

            szCode = code;
            bNeedReconnect = false;
            pReconnectTicker = 0;

            string ret = await BApi.StartInteractivePlay(code, pConfig.appId);
            Debug.Log("场次信息：" + ret);

            //打印到控制台日志
            gameIdResObj = JsonConvert.DeserializeObject<AppStartInfo>(ret);
            if (gameIdResObj == null)
            {
                callSuc?.Invoke(-1);
                return;
            }

            if (gameIdResObj.Code != 0)
            {
                Debug.LogError(gameIdResObj.Message);

                callSuc?.Invoke(gameIdResObj.Code);
                return;
            }

            szGameID = gameIdResObj?.Data?.GameInfo?.GameId;
            lUserId = gameIdResObj.Data.AnchorInfo.Uid;
            lRoomID = gameIdResObj.Data.AnchorInfo.RoomId.ToString();

            gameAnchorInfo = gameIdResObj.Data.AnchorInfo;

            Debug.Log($"【直播间场次信息】 gameID：{gameIdResObj.Data.GameInfo.GameId}  房间id：{lRoomID}");
            Debug.Log($"【直播间主播信息】 uid：{gameIdResObj.Data.AnchorInfo.Uid}  name：{gameIdResObj.Data.AnchorInfo.UName}  face：{gameIdResObj.Data.AnchorInfo.UFace}");

            //开启长连
            m_WebSocketBLiveClient = new WebSocketBLiveClient(gameIdResObj);
            m_WebSocketBLiveClient.OnDanmaku += pEvent.WebSocketBLiveClientOnDanmaku;
            m_WebSocketBLiveClient.OnGift += pEvent.WebSocketBLiveClientOnGift;
            m_WebSocketBLiveClient.OnGuardBuy += pEvent.WebSocketBLiveClientOnGuardBuy;
            m_WebSocketBLiveClient.OnSuperChat += pEvent.WebSocketBLiveClientOnSuperChat;
            m_WebSocketBLiveClient.dlgConnectErr += this.OnWebsocketConnectErr;
            m_WebSocketBLiveClient.Connect(TimeSpan.FromSeconds(1), nMaxConnectTimes);

            //if (bDevMode)
            //    MockDataUtility.webSocketBLiveClient = m_WebSocketBLiveClient;

            try
            {
                //开始心跳包
                //获取到场次id后，开启心跳轮询，默认30秒一次
                m_PlayHeartBeat = new InteractivePlayHeartBeat(szGameID, 20000);
                m_PlayHeartBeat.HeartBeatError += OnHeartBeatError;
                m_PlayHeartBeat.HeartBeatSucceed += OnHeartBeatSuc;
                m_PlayHeartBeat.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("Init Game Failed：" + e.Message);
                callSuc?.Invoke(-1);
                return;
            }

            callSuc?.Invoke(0);
        }
    }

    public async void EndGame(bool clearGameId, System.Action call = null)
    {
        Debug.Log("关闭B站弹幕连接");

        //停止心跳包
        if (m_PlayHeartBeat != null)
        {
            m_PlayHeartBeat.Stop();
            m_PlayHeartBeat.Dispose();
            m_PlayHeartBeat = null;
        }

        if (!string.IsNullOrEmpty(szGameID))
        {
            string closeRes = await BApi.EndInteractivePlay(pConfig.appId, szGameID);
            Debug.Log("Game Close:" + closeRes);
            szGameID = "";
        }

        if (m_WebSocketBLiveClient != null)
        {
            m_WebSocketBLiveClient.dlgConnectErr = null;

            if(m_WebSocketBLiveClient.ws!=null)
            {
                await m_WebSocketBLiveClient.ws.Close();           
            }

            m_WebSocketBLiveClient.Dispose();
        }

        if(true)
        {
            szGameID = "";
        }

        call?.Invoke();
    }

    /// <summary>
    /// 是否在游戏中
    /// </summary>
    /// <returns></returns>
    public bool IsGaming()
    {
        return !string.IsNullOrEmpty(szGameID);
    }

    void OnHeartBeatError(string json)
    {
        Debug.LogError("Heart Beat Error：" + json);
        //if (Application.isPlaying)
        //{
        //UIToast.Show("网络异常，心跳已中断");
        //}

        //过6.4秒开始断线重连
        StartAllReconnect();
    }

    void OnHeartBeatSuc()
    {
        Debug.Log("Heart Beat Suc：" + DateTime.Now);
    }

    /// <summary>
    /// 开始断线重连
    /// </summary>
    public void StartAllReconnect(System.Action call = null)
    {
        if (Application.isPlaying)
        {
            EndGame(false);

            bNeedReconnect = true;
            pReconnectTicker = 7.2F;

            dlgReconnectSuc = call;
        }
    }

    /// <summary>
    /// 修复网络
    /// </summary>
    public void RepairAllConnect(System.Action call = null)
    {
        if (!string.IsNullOrEmpty(szCode))
        {
            dlgReconnectSuc = call;
            Reconnect(szCode);
        }
    }

    void OnWebsocketConnectErr()
    {
        EndGame(true);

        StartCoroutine(DoWssReconnect());
    }

    IEnumerator DoWssReconnect()
    {
        yield return new WaitForSeconds(4F);

        Reconnect(szCode);
    }

    async void StartWebsocektConnect()
    {
        if (m_WebSocketBLiveClient != null)
        {
            //清空委托
            m_WebSocketBLiveClient.dlgConnectErr = null;

            if(m_WebSocketBLiveClient.ws!=null)
            {
                await m_WebSocketBLiveClient.ws.Close();
            }
            
            m_WebSocketBLiveClient.Dispose();
        }

        m_WebSocketBLiveClient = new WebSocketBLiveClient(gameIdResObj);
        m_WebSocketBLiveClient.OnDanmaku += pEvent.WebSocketBLiveClientOnDanmaku;
        m_WebSocketBLiveClient.OnGift += pEvent.WebSocketBLiveClientOnGift;
        m_WebSocketBLiveClient.OnGuardBuy += pEvent.WebSocketBLiveClientOnGuardBuy;
        m_WebSocketBLiveClient.OnSuperChat += pEvent.WebSocketBLiveClientOnSuperChat;
        m_WebSocketBLiveClient.dlgConnectErr += this.OnWebsocketConnectErr;
        m_WebSocketBLiveClient.Connect(TimeSpan.FromSeconds(1), nMaxConnectTimes);
    }

    /// <summary>
    /// 断线重连
    /// </summary>
    async void Reconnect(string code)
    {
        Debug.Log("执行断线重连");

        string ret = await BApi.StartInteractivePlay(code, pConfig.appId);
        Debug.Log("场次信息：" + ret);

        //打印到控制台日志
        gameIdResObj = JsonConvert.DeserializeObject<AppStartInfo>(ret);
        if (gameIdResObj == null)
        {
            Debug.LogError("网络已断开");

            //UIToast.Show("网络已断开,请等待重连");

            StartAllReconnect(dlgReconnectSuc);

            return;
        }

        if (gameIdResObj.Code != 0)
        {
            Debug.LogError("断线重连失败：" + gameIdResObj.Message);

            //UIToast.Show("断线重连B站失败");

            StartAllReconnect(dlgReconnectSuc);

            return;
        }

        szGameID = gameIdResObj?.Data?.GameInfo?.GameId;
        lUserId = gameIdResObj.Data.AnchorInfo.Uid;
        lRoomID = gameIdResObj.Data.AnchorInfo.RoomId.ToString();

        Debug.Log($"【重连直播间场次信息】 gameID：{gameIdResObj.Data.GameInfo.GameId}  房间id：{lRoomID}");
        Debug.Log($"【重连直播间主播信息】 uid：{gameIdResObj.Data.AnchorInfo.Uid}  name：{gameIdResObj.Data.AnchorInfo.UName}  face：{gameIdResObj.Data.AnchorInfo.UFace}");

        //断开旧连接
        StartWebsocektConnect();

        if (bDevMode)
            MockDataUtility.webSocketBLiveClient = m_WebSocketBLiveClient;

        try
        {
            //开始心跳包
            //获取到场次id后，开启心跳轮询，默认30秒一次
            m_PlayHeartBeat = new InteractivePlayHeartBeat(szGameID, 25000);
            m_PlayHeartBeat.HeartBeatError += OnHeartBeatError;
            m_PlayHeartBeat.HeartBeatSucceed += OnHeartBeatSuc;
            m_PlayHeartBeat.Start();

            dlgReconnectSuc?.Invoke();
            dlgReconnectSuc = null;
        }
        catch (Exception e)
        {
            Debug.LogError("Init Game Failed：" + e.Message);
            //UIToast.Show("断线重连心跳启动异常");
            StartAllReconnect(dlgReconnectSuc);

            return;
        }
    }

    private void Update()
    {
        if (bNeedReconnect && pReconnectTicker > 0f)
        {
            pReconnectTicker -= Time.unscaledDeltaTime;
            if (pReconnectTicker <= 0F)
            {
                bNeedReconnect = false;
                pReconnectTicker = 0;

                if (!string.IsNullOrEmpty(szCode))
                {
                    Reconnect(szCode);
                }
            }
        }

#if !UNITY_WEBGL || UNITY_EDITOR
        if (m_WebSocketBLiveClient is { ws: { State: WebSocketState.Open } })
        {
            m_WebSocketBLiveClient.ws.DispatchMessageQueue();
        }
#endif
    }

    private void OnDestroy()
    {
        EndGame(true);
    }
}
