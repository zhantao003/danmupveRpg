using BarrageGrab.ProtoEntity;
using NativeWebSocket;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DouyinDanmu
{
    public class DouyinOpenClient : MonoBehaviour
    {
        static DouyinOpenClient ins = null;
        public static DouyinOpenClient Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<DouyinOpenClient>();
                }

                return ins;
            }
        }

        #region 常量

        //请求地址
        public string URL_SERVER = "http://127.0.0.1:12347";
        public string WSS_SERVER = "wss://127.0.0.1:12347";

        public const string METHOD_POST = "POST";

        public const string DY_GETTOKEN = "/api/ttcheckToken";
        public const string DY_GETROOMINFO = "/api/ttgetroomInfo";
        public const string DY_STARTTASK = "/api/ttstartDanmuTask";
        public const string DY_ENDTASK = "/api/ttendDanmuTask";
        public const string DY_GIFTSHOW = "/api/ttshowGiftList";

        public const string DY_TASK_COMMENT = "live_comment";   //弹幕
        public const string DY_TASK_GIFT = "live_gift";         //礼物
        public const string DY_TASK_LIKE = "live_like";         //点赞

        #endregion

        public DouyinDanmuConfig pStaticConfig; //平台相关配置
        public DouyinOpenEventHandler pEvent;    //时间监听器

        [ReadOnly]
        public string szAccessToken;  //鉴权用的Token
        public string szUid;        //房间ID
        public string szNickname;   //房间ID
        public string szRoomID;     //房间ID

        public bool bListen_LiveComment;    //监听弹幕
        public bool bListen_LiveGift;       //监听礼物
        public bool bListen_LiveLike;       //监听点赞
        public Dictionary<string, bool> dicListenCount = new Dictionary<string, bool>();

        public string[] arrGiftList;    //需要置顶的礼物

        DouyinOpenWebsocket pClient = null;

        public bool bDebug;
        System.Action<int> connectCallSuc = null;
        System.Action dlgReconnectSuc = null;

        public void StartConnect(string roomId, System.Action<int> callSuc = null)
        {
            connectCallSuc = callSuc;

            if (string.IsNullOrEmpty(roomId))
            {
                if (bDebug)
                {
                    roomId = pStaticConfig.szDevID;
                }
            }

            if (string.IsNullOrEmpty(roomId))
            {
                Debug.LogError("房间号不能为空");
                connectCallSuc?.Invoke(99);
                return;
            }

            GetToken(roomId, delegate (bool value)
            {
                if (!value)
                {
                    Debug.LogError("获取Token异常");
                    connectCallSuc?.Invoke(999);

                    return;
                }

                szUid = roomId;
                szRoomID = SystemInfo.deviceUniqueIdentifier;
                szNickname = "";

                OnAccessTokenSuc(roomId);
            });
        }

        public void AutoConnect(string token, System.Action<int> callSuc = null)
        {
            connectCallSuc = callSuc;

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("传参Token不能为空");
                connectCallSuc?.Invoke(99);
                return;
            }

            GetToken("", delegate (bool value)
            {
                if (!value)
                {
                    Debug.LogError("获取Token异常");
                    connectCallSuc?.Invoke(999);

                    return;
                }

                GetRoomInfo(token, delegate (bool value)
                {
                    if (!value)
                    {
                        Debug.LogError("获取房间信息异常");
                        connectCallSuc?.Invoke(999);

                        return;
                    }

                    OnAccessTokenSuc(szRoomID);
                });
            });
        }

        //获取AccessToken成功
        void OnAccessTokenSuc(string roomId)
        {
            szRoomID = roomId;
            dicListenCount.Clear();

            //请求置顶礼物
            GiftShowReq(szRoomID);

            if (bListen_LiveComment)
            {
                StartTask(roomId, DY_TASK_COMMENT, delegate (bool taskRes)
                {
                    if (!taskRes)
                    {
                        AddListenTask(DY_TASK_COMMENT, false);
                        return;
                    }

                    Debug.Log($"启动{DY_TASK_COMMENT}监听成功");
                    AddListenTask(DY_TASK_COMMENT, true);
                });
            }

            if (bListen_LiveGift)
            {
                StartTask(roomId, DY_TASK_GIFT, delegate (bool taskRes)
                {
                    if (!taskRes)
                    {
                        AddListenTask(DY_TASK_GIFT, false);
                        return;
                    }

                    Debug.Log($"启动{DY_TASK_GIFT}监听成功");
                    AddListenTask(DY_TASK_GIFT, true);
                });
            }

            if (bListen_LiveLike)
            {
                StartTask(roomId, DY_TASK_LIKE, delegate (bool taskRes)
                {
                    if (!taskRes)
                    {
                        AddListenTask(DY_TASK_LIKE, false);
                        return;
                    }

                    Debug.Log($"启动{DY_TASK_LIKE}监听成功");
                    AddListenTask(DY_TASK_LIKE, true);
                });
            }

            if (!bListen_LiveComment &&
               !bListen_LiveGift &&
               !bListen_LiveLike)
            {
                OnListenSuc();
            }
        }

        void AddListenTask(string msgType, bool res)
        {
            dicListenCount.Add(msgType, res);
            OnListenSuc();
        }

        //监听回馈
        void OnListenSuc()
        {
            int nCount = (bListen_LiveComment ? 1 : 0) +
                         (bListen_LiveGift ? 1 : 0) +
                         (bListen_LiveLike ? 1 : 0);

            bool bConnectWssAble = true;
            foreach (string key in dicListenCount.Keys)
            {
                nCount--;
                //有一个没通过都当作异常处理
                if (!dicListenCount[key])
                {
                    bConnectWssAble = false;
                }
            }

            if (nCount > 0)
            {
                return;
            }

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
            pClient = new DouyinOpenWebsocket(WSS_SERVER);
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
            Debug.Log("关闭Douyin Open弹幕连接");

            if (!string.IsNullOrEmpty(szRoomID))
            {
                if (bListen_LiveComment)
                {
                    EndTask(szRoomID, DY_TASK_COMMENT);
                }

                if (bListen_LiveGift)
                {
                    EndTask(szRoomID, DY_TASK_GIFT);
                }

                if (bListen_LiveLike)
                {
                    EndTask(szRoomID, DY_TASK_LIKE);
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
        void GetToken(string roomId, System.Action<bool> callSuc = null)
        {
            if (pStaticConfig == null)
            {
                Debug.LogWarning("None Douyin DanmuConfig");
                return;
            }

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_GETTOKEN, METHOD_POST, dataForm, null, null, delegate (string value)
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

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_GETROOMINFO, METHOD_POST, dataForm, null, null, delegate (string value)
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

                Debug.LogWarning("获取主播信息：" + szUid + "  房间号：" + szRoomID + "   昵称：" + szNickname);

                callSuc?.Invoke(true);
            }));
        }

        /// <summary>
        /// 开启任务
        /// </summary>
        void StartTask(string roomId, string msgType, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szAccessToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            //string szParam = $"{{\"roomId\":\"{roomId}\",\"msgType\":\"{msgType}\"}}";

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);
            dataForm.AddField("msgType", msgType);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_STARTTASK, METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError($"启动{msgType}任务失败!");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        void EndTask(string roomId, string msgType, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szAccessToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);
            dataForm.AddField("msgType", msgType);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_ENDTASK, METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError("启动任务失败!");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        void GiftShowReq(string roomId)
        {
            string szGiftList = "[";
            for (int i = 0; i < arrGiftList.Length; i++)
            {
                szGiftList += "\"" + arrGiftList[i].Trim() + "\"" + ((i < (arrGiftList.Length - 1)) ? "," : "");
            }
            szGiftList += "]";

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);
            dataForm.AddField("giftList", szGiftList);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szAccessToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + DY_GIFTSHOW, METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError($"置顶任务失败!");

                        return;
                    }

                    Debug.Log("置顶礼物成功");
                }));
        }

        #endregion

        #region Http请求接口

        IEnumerator RequestWebUTF8(string url, string method, string param, Dictionary<string, string> dicHeader, string cookie, System.Action<string> handler = null)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url);
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

            var bytes = Encoding.UTF8.GetBytes(param);
            webRequest.uploadHandler = new UploadHandlerRaw(bytes);

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
            pClient = new DouyinOpenWebsocket(WSS_SERVER);
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

