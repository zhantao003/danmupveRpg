using BarrageGrab.ProtoEntity;
using NativeWebSocket;
using ProtoBuf.WellKnownTypes;
using Sirenix.OdinInspector;
//using Sirenix.Utilities.Editor.Expressions;
//using Sirenix.Utilities.Editor.Expressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;

namespace YYDanmu
{
    public class YYOpenClient : MonoBehaviour
    {
        static YYOpenClient ins = null;
        public static YYOpenClient Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<YYOpenClient>();
                }

                return ins;
            }
        }

        #region 常量

        //请求地址
        public string URL_SERVER = "http://127.0.0.1:12347";
        public string WSS_SERVER = "wss://127.0.0.1:12347";

        public const string METHOD_POST = "POST";

        public const string DY_GETTOKEN = "/api/yycheckToken";
        public const string DY_GETROOMINFO = "/api/yygetroomInfo";
        public const string DY_BINDROOM = "/api/yybindRoom";
        public const string DY_STARTTASK = "/api/yystartDanmuTask";
        public const string DY_ENDTASK = "/api/yyendDanmuTask";

        public const string DY_TASK_COMMENT = "live_comment";   //弹幕
        public const string DY_TASK_GIFT = "live_gift";         //礼物
        public const string DY_TASK_LIKE = "live_like";         //点赞

        #endregion

        public YYDanmuConfig pStaticConfig; //平台相关配置
        public YYOpenEventHandler pEvent;    //时间监听器

        [ReadOnly]
        public string szAccessToken;  //鉴权用的Token
        public string szUid;        //房间ID
        public string szNickname;   //房间ID
        public string szHeadIcon;   //房间ID
        public string szRoomID;     //房间ID

        public string szSerialCode; //传参码

        public bool bListenTask;    //监听弹幕
        public Dictionary<string, bool> dicListenCount = new Dictionary<string, bool>();

        YYOpenWebsocket pClient = null;

        public bool bDebug;
        public bool bIsTest;
        System.Action<int> connectCallSuc = null;
        System.Action dlgReconnectSuc = null;

        public void StartConnect(string token, System.Action<int> callSuc = null)
        {
            connectCallSuc = callSuc;
            szSerialCode = token;

            if (string.IsNullOrEmpty(szSerialCode))
            {
                if (bDebug)
                {
                    szSerialCode = pStaticConfig.szDevID;
                }
            }

            if (string.IsNullOrEmpty(szSerialCode))
            {
                Debug.LogError("传参码不能为空");
                connectCallSuc?.Invoke(99);
                return;
            }

            GetToken(delegate (bool value)
            {
                if (!value)
                {
                    Debug.LogError("获取Token异常");
                    connectCallSuc?.Invoke(999);
                    return;
                }

                GetRoomInfoByClient(szAccessToken, pStaticConfig.szAppId, szSerialCode, delegate (bool value)
                {
                    if (!value)
                    {
                        Debug.LogError(szAccessToken + "====获取房间信息异常===" + pStaticConfig.szAppId);
                        connectCallSuc?.Invoke(999);

                        return;
                    }

                    OnAccessTokenSuc(szSerialCode);
                });
            });
        }

        //获取AccessToken成功
        void OnAccessTokenSuc(string roomId)
        {
            szRoomID = roomId;
            dicListenCount.Clear();

            ////请求置顶礼物
            //GiftShowReq(szRoomID);

            if (bListenTask)
            {
                StartDanmuTask(roomId, delegate (bool taskRes)
                {
                    if (!taskRes)
                    {
                        Debug.LogError("YY启动监听失败");
                        return;
                    }

                    Debug.Log($"YY启动监听成功");
                    OnListenSuc();
                });
            }

           
            if (!bListenTask)
            {
                OnListenSuc();
            }
        }

        //监听回馈
        void OnListenSuc()
        {
            bool bConnectWssAble = true;
            
            //有一个任务开启失败都不允许连接
            if (!bConnectWssAble)
            {
                connectCallSuc?.Invoke(101);
                CloseConnect();
                Debug.LogError("开启平台推送任务失败,停止连接");
                return;
            }

            //连接WSS
            Debug.Log("连接任务准备！");
            pClient = new YYOpenWebsocket(WSS_SERVER);
            pClient.dlgConnectSuc = this.OnWSSConnectSuc;
            pClient.dlgConnectErr = this.OnWSSConnectFailed;
            pClient.OnMsgChat = pEvent.OnRevEventChat;
            pClient.OnMsgGift = pEvent.OnRevEventGift;
            pClient.OnMsgLike = pEvent.OnRevEventLike;
            pClient.Connect("", TimeSpan.FromSeconds(2), 10);
        }

        void OnWSSConnectSuc()
        {
            Debug.Log("连接成功");

            //发起登录
            string szParam = $"{{\"uid\":\"{szRoomID}\"," +
                             $"\"roomId\":\"{szRoomID}\"," +
                             $"\"cmd\":\"login\"," +
                             $"\"content\":\"n\"}}";

            pClient.SendMsg(szParam);

            connectCallSuc?.Invoke(0);

            //断线重连请求只执行一次
            dlgReconnectSuc?.Invoke();
            dlgReconnectSuc = null;
        }

        void OnWSSConnectFailed()
        {
            Debug.Log("连接失败");
            //终止任务
            CloseConnect();
        }

        public async void CloseConnect(System.Action call = null)
        {
            Debug.Log("关闭YY Open弹幕连接");

            if (!string.IsNullOrEmpty(szSerialCode))
            {
                if (bListenTask)
                {
                    EndDanmuTask(szSerialCode);
                }
            }

            if (pClient != null)
            {
                pClient.dlgConnectErr = null;

                if (pClient.ws != null)
                {
                    await pClient.ws.Close();
                }

                pClient.Dispose();
                pClient = null;
            }

            if (!string.IsNullOrEmpty(szRoomID))
            {
                szRoomID = "";
            }

            if (!string.IsNullOrEmpty(szAccessToken))
            {
                szAccessToken = "";
            }

            call?.Invoke();
        }

        public bool IsGaming()
        {
            return string.IsNullOrEmpty(szRoomID);
        }

        #region 动作指令

        //获取Token
        void GetToken(System.Action<bool> callSuc = null)
        {
            if (pStaticConfig == null)
            {
                Debug.LogWarning("None Douyin DanmuConfig");
                return;
            }

            WWWForm dataForm = new WWWForm();

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_GETTOKEN + (bIsTest?"Test":""), METHOD_POST, dataForm, null, null, delegate (string value)
            {
                CLocalNetMsg msgContent = new CLocalNetMsg(value);
                string code = msgContent.GetString("status");
                if (!code.Equals("ok"))
                {
                    Debug.LogError("获取Token异常");
                    callSuc?.Invoke(false);

                    return;
                }

                szAccessToken = msgContent.GetString("token");

                callSuc?.Invoke(true);
            }));
        }

        /// <summary>
        /// 获取主播房间信息
        /// </summary>
        void GetRoomInfo(string token, System.Action<bool> callSuc = null)
        {
            WWWForm dataForm = new WWWForm();
            dataForm.AddField("token", token);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_GETROOMINFO + (bIsTest ? "Test" : ""), METHOD_POST, dataForm, null, null, delegate (string value)
            {
                CLocalNetMsg msgContent = new CLocalNetMsg(value);
                string code = msgContent.GetString("status");
                if (!code.Equals("ok"))
                {
                    Debug.LogError("获取Token异常");
                    callSuc?.Invoke(false);

                    return;
                }

                szRoomID = msgContent.GetString("roomId");
                szUid = msgContent.GetString("uid");
                szNickname = msgContent.GetString("nickName");
                szHeadIcon = msgContent.GetString("headIcon");

                Debug.LogWarning("获取主播信息：" + szUid + "  房间号：" + szRoomID + "   昵称：" + szNickname + "   头像：" + szHeadIcon);

                callSuc?.Invoke(true);
            }));
        }

        void GetRoomInfoByClient(string token, string appId, string serial, System.Action<bool> callSuc = null)
        {
            Debug.Log("请求查询房间信息： Token:" + token + "  Appid:" + appId + "   Serial:" + serial);

            CHttpParam pParams = new CHttpParam();
            pParams.AddSlot(new CHttpParamSlot("appid", appId));
            pParams.AddSlot(new CHttpParamSlot("serial", serial));

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", token);

            string szReqUrl = bIsTest ?
                "https://w-open-test.yy.com/api/data/room/query" :
                "https://w-open.yy.com/api/data/room/query";

            StartCoroutine(RequestWebUTF8Get($"{szReqUrl}?serial={serial.Trim()}&appid={appId}", 
                "",
                "", 
                dicHeaders, "",
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    int code = msgContent.GetInt("code");
                    if (code != 0)
                    {
                        Debug.LogError("获取房间信息异常");
                        callSuc?.Invoke(false);

                        return;
                    }

                    CLocalNetMsg msgData = msgContent.GetNetMsg("data");
                    if(msgData!=null)
                    {
                        szRoomID = msgData.GetString("room_id");
                        szUid = msgData.GetString("anchor_id");
                        szNickname = msgData.GetString("nickname");
                        szHeadIcon = msgData.GetString("avatar_url");

                        Debug.LogWarning("获取主播信息：" + szUid + "  房间号：" + szRoomID + "   昵称：" + szNickname + "   头像：" + szHeadIcon);

                        BindRoom(szUid, serial, szNickname,delegate(bool resBind) {
                            Debug.Log(resBind ? "绑定房间信息成功" : "绑定房间信息失败");
                        });

                        callSuc?.Invoke(true);
                    }
                    else
                    {
                        callSuc?.Invoke(false);
                    }
                }));
        }

        void BindRoom(string uid, string serial, string nickName, System.Action<bool> callSuc = null)
        {
            WWWForm dataForm = new WWWForm();
            dataForm.AddField("uid", uid);
            dataForm.AddField("serial", serial);
            dataForm.AddField("nickName", nickName);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_BINDROOM + (bIsTest ? "Test" : ""), METHOD_POST, dataForm, null, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError($"绑定房间信息失败");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        /// <summary>
        /// 开启弹幕任务
        /// </summary>
        void StartDanmuTask(string roomId, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szAccessToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            //string szParam = $"{{\"roomId\":\"{roomId}\",\"msgType\":\"{msgType}\"}}";

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("token", roomId);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_STARTTASK + (bIsTest ? "Test" : ""), METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError($"YY启动监听失败!");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        /// <summary>
        /// 结束弹幕任务
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callSuc"></param>
        void EndDanmuTask(string roomId, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szAccessToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("token", roomId);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_ENDTASK + (bIsTest ? "Test" : ""), METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError("停止任务失败!");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        /// <summary>
        /// 启动游戏轮次
        /// </summary>
        public void StartRound(System.Action<bool> callSuc = null)
        {
            long nTimeStamp = CTimeMgr.NowMillonsSec();
            string szJsonContent = string.Empty;

            string szRound = "main" + (10000 + CGameAntGlobalMgr.Ins.nRoundCount).ToString();

            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
            {
                szJsonContent = $"{{\"timestamp\": {nTimeStamp}, \"round\": {szRound}, \"serial\":\"{szSerialCode}\",\"appid\":\"{pStaticConfig.szAppId}\",\"duration\":30, \"is_pk\": 0,\"team\":\"n\"}}";
            }
            else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            {
                szJsonContent = $"{{\"timestamp\": {nTimeStamp}, \"round\": {szRound}, \"serial\":\"{szSerialCode}\",\"appid\":\"{pStaticConfig.szAppId}\",\"duration\":30, \"is_pk\": 1,\"team\":\"n\"}}";
            }
            CGameAntGlobalMgr.Ins.nRoundCount++;
            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            Debug.Log($"启动游戏轮次传参  appid:{pStaticConfig.szAppId}  serial:{szSerialCode}  token:{szAccessToken}   timestamp:{nTimeStamp}   round:{szRound}");

            string szReqUrl = bIsTest ?
                "https://w-open-test.yy.com/api/data/interactive/start" :
                "https://w-open.yy.com/api/data/interactive/start";

            StartCoroutine(RequestWebUTF8WWWJson(szReqUrl,
                METHOD_POST,
                szJsonContent, dicHeaders, null, delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    int code = msgContent.GetInt("status");
                    if (code != 0)
                    {
                        Debug.LogError("启动游戏轮次失败===" + code);
                        callSuc?.Invoke(false);

                        return;
                    }

                    Debug.LogWarning("启动游戏轮次成功！");

                    callSuc?.Invoke(true);
                }));
        }

        /// <summary>
        /// 结束游戏轮次
        /// </summary>
        public void EndRound(System.Action<bool> callSuc = null)
        {
            //WWWForm dataForm = new WWWForm();
            //dataForm.AddField("appid", pStaticConfig.szAppId);
            //dataForm.AddField("serial", szSerialCode);
            //dataForm.AddField("round", "maingame");

            string szJsonContent = $"{{\"serial\":\"{szSerialCode}\",\"appid\":\"{pStaticConfig.szAppId}\",\"round\": \"main001\"}}";

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            string szReqUrl = bIsTest ?
                "https://w-open-test.yy.com/api/data/interactive/stop" :
                "https://w-open.yy.com/api/data/interactive/stop";

            StartCoroutine(RequestWebUTF8WWWJson(szReqUrl,
                METHOD_POST,
                szJsonContent, dicHeaders, null, delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    int code = msgContent.GetInt("status");
                    if (code != 0)
                    {
                        Debug.LogError("结束游戏轮次失败");
                        callSuc?.Invoke(false);

                        return;
                    }

                    Debug.LogWarning("结束游戏轮次成功！");

                    callSuc?.Invoke(true);
                }));
        }

        #endregion

        #region Http请求接口

        IEnumerator RequestWebUTF8Get(string url, string method, string param, Dictionary<string, string> dicHeader, string cookie, System.Action<string> handler = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            //webRequest.method = method;

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;

            if (dicHeader != null)
            {
                foreach (string key in dicHeader.Keys)
                {
                    webRequest.SetRequestHeader(key, dicHeader[key]);
                }
            }

            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            if (cookie != null)
            {
                webRequest.SetRequestHeader("Cookie", cookie);
            }

            Debug.Log("Req Url:" + url + "\r\nReq Params:" + param);

            if(!string.IsNullOrEmpty(param))
            {
                var bytes = Encoding.UTF8.GetBytes(param);
                webRequest.uploadHandler = new UploadHandlerRaw(bytes);
            }

            yield return webRequest.SendWebRequest();
            string text = webRequest.downloadHandler.text;

            webRequest.Dispose();

            try
            {
                Debug.Log("Req Content:" + text);
                handler?.Invoke(text);
            }
            catch (System.Exception e)
            {
                Debug.LogError("【Error Info】\r\n" + e.Message);
            }
        }

        IEnumerator RequestWebUTF8WWWForm(string url, string method, WWWForm param, Dictionary<string, string> dicHeader, string cookie, System.Action<string> handler = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(url, param);
            webRequest.method = method;

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;

            if (dicHeader != null)
            {
                foreach (string key in dicHeader.Keys)
                {
                    webRequest.SetRequestHeader(key, dicHeader[key]);
                }
            }

            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            if (cookie != null)
            {
                webRequest.SetRequestHeader("Cookie", cookie);
            }

            Debug.Log("Req Url:" + url + "\r\nReq Params:" + param);

            //var bytes = Encoding.UTF8.GetBytes(param);
            //webRequest.uploadHandler = new UploadHandlerRaw(bytes);

            yield return webRequest.SendWebRequest();
            string text = webRequest.downloadHandler.text;

            webRequest.Dispose();

            try
            {
                Debug.Log("Req Content:" + text);
                handler?.Invoke(text);
            }
            catch (System.Exception e)
            {
                Debug.LogError("【Error Info】\r\n" + e.Message);

                handler?.Invoke("{status:\"error\"}");
            }
        }

        IEnumerator RequestWebUTF8Json(string url, string method, string param, Dictionary<string, string> dicHeader, string cookie, System.Action<string> handler = null)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(url, param);
            webRequest.method = method;

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;

            if (dicHeader != null)
            {
                foreach (string key in dicHeader.Keys)
                {
                    webRequest.SetRequestHeader(key, dicHeader[key]);
                }
            }

            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/json");

            if (cookie != null)
            {
                webRequest.SetRequestHeader("Cookie", cookie);
            }

            Debug.Log("Req Url:" + url + "\r\nReq Params:" + param);

            //var bytes = Encoding.UTF8.GetBytes(param);
            //webRequest.uploadHandler = new UploadHandlerRaw(bytes);

            yield return webRequest.SendWebRequest();
            string text = webRequest.downloadHandler.text;

            webRequest.Dispose();

            try
            {
                Debug.Log("Req Content:" + text);
                handler?.Invoke(text);
            }
            catch (System.Exception e)
            {
                Debug.LogError("【Error Info】\r\n" + e.Message);

                handler?.Invoke("{status:\"error\"}");
            }
        }

        IEnumerator RequestWebUTF8WWWJson(string url, string method, string param, Dictionary<string, string> dicHeader, string cookie, System.Action<string> handler = null)
        {
            UnityWebRequest www = new UnityWebRequest(url, method);
            www.timeout = 8;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.SetRequestHeader("access-token", szAccessToken);

            var bodyRaw = Encoding.UTF8.GetBytes(param);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);

            www.downloadHandler = new DownloadHandlerBuffer();
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError ||
                www.result == UnityWebRequest.Result.DataProcessingError)
            {
                Debug.LogError("Request Failed:\n" + (url.IndexOf('?') == -1 ? url + "?" : url + "&") + param + "\nServer Error:\n" + www.error);

                yield break;
            }

            string response = www.downloadHandler.text;
            try
            {
                Debug.Log("Req Content:" + response);
                handler?.Invoke(response);
            }
            catch (System.Exception e)
            {
                Debug.LogError("【Error Info】\r\n" + e.Message);

                handler?.Invoke("{status:\"error\"}");
            }

            yield break;
        }

        #endregion

        public async void RepairNet(System.Action call = null)
        {
            dlgReconnectSuc = call;

            //先断开当前连接
            if (pClient != null)
            {
                pClient.dlgConnectErr = null;

                if (pClient.ws != null)
                {
                    await pClient.ws.Close();
                }

                pClient.Dispose();
                pClient = null;
            }

            Debug.Log("连接任务准备！");
            pClient = new YYOpenWebsocket(WSS_SERVER);
            pClient.dlgConnectSuc = this.OnWSSConnectSuc;
            pClient.dlgConnectErr = this.OnWSSConnectFailed;
            pClient.OnMsgChat = pEvent.OnRevEventChat;
            pClient.OnMsgGift = pEvent.OnRevEventGift;
            pClient.OnMsgLike = pEvent.OnRevEventLike;
            pClient.Connect("", TimeSpan.FromSeconds(2), 10);
        }

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (pClient is { ws: { State: WebSocketState.Open } })
            {
                pClient.ws.DispatchMessageQueue();
            }
#endif

            ////检查Token超时
            //if(nTokenExpireTime > 0)
            //{
            //    nTokenExpireTime -= Time.unscaledDeltaTime;
            //    if(nTokenExpireTime <= 0F)
            //    {
            //        GetToken();
            //    }
            //}
        }

        private void OnDestroy()
        {
            CloseConnect();
        }
    }
}

