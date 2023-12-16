using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NativeWebSocket;
using Sirenix.OdinInspector;

namespace DouyuDanmu
{
    public class DouyuSDKMgr : MonoBehaviour
    {
        static DouyuSDKMgr ins = null;
        public static DouyuSDKMgr Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<DouyuSDKMgr>();
                }

                return ins;
            }
        }

        public DouyuDanmuConfig pConfig = null;
        public DouyuEventHandler pEventHandler = null;

        DouyuTokenInfo pGameInfo;    //回馈的token信息
        DouyuDMClient pClient = null;   //弹幕客户端

        public int nMaxConnectTimes = 10;

        public bool bDevMode = false;

        Action<int> dlgConnect = null;
        Action dlgReconnectSuc = null;

        [ReadOnly]
        public string szToken;  //鉴权用的Token
        [ReadOnly]
        public string szUid;    //主播uid
        [ReadOnly]
        public string szRoomID; //房间ID
        [ReadOnly]
        public string szNickName = "";
        [ReadOnly]
        public string szHeadIcon = "";

        private void Awake()
        {
            ins = this;
        }

        public async void StartGame(string roomId, Action<int> callSuc = null)
        {
            DouyuApi.szAppId = pConfig.appId;
            DouyuApi.szAppSecret = pConfig.appKey;
            DouyuApi.szVersion = pConfig.version;

            if(bDevMode)
            {
                if (roomId.Equals(""))
                {
                    roomId = pConfig.roomId;
                }
            }
            
            string res = await DouyuApi.GetToken(roomId);

            Debug.Log("场次信息：" + res);
            CLocalNetMsg msgRes = new CLocalNetMsg(res);
            pGameInfo = new DouyuTokenInfo(msgRes);
            if(pGameInfo.nCode != 200)
            {
                Debug.LogError("获取Token异常");
                callSuc?.Invoke(-1);
                return;
            }

            szToken = pGameInfo.szToken;
            szRoomID = roomId;
            szUid = roomId;
            szNickName = pGameInfo.szUserName;
            szHeadIcon = pGameInfo.szUserHeadIcon;

            //WSS连接
            dlgConnect = callSuc;
            pClient = new DouyuDMClient(pGameInfo);
            pClient.OnEventDanmu = pEventHandler.OnRevEventChat;
            pClient.OnEventGift = pEventHandler.OnRevEventGift;
            pClient.dlgConnectSuc += ClientConnectSuc;
            pClient.dlgConnectErr += ClientConnectFailed;
            pClient.Connect(DouyuApi.szAppSecret, TimeSpan.FromSeconds(1), nMaxConnectTimes);
        }

        /// <summary>
        /// 客户端连接成功
        /// </summary>
        async void ClientConnectSuc()
        {
            string startGameRes = await DouyuApi.StartGame(pGameInfo.szToken);
            Debug.Log("启动游戏：" + startGameRes);

            CLocalNetMsg msgStartGameRes = new CLocalNetMsg(startGameRes);
            int nCode = msgStartGameRes.GetInt("code");
            if (nCode != 200)
            {
                Debug.LogError("开始游戏失败，异常代码：" + nCode);
                dlgConnect?.Invoke(1);
                return;
            }

            Debug.Log("开始游戏成功");

            dlgConnect?.Invoke(0);

            dlgReconnectSuc?.Invoke();
            dlgReconnectSuc = null;
        }

        void ClientConnectFailed()
        {
            Debug.LogError("连接失败");
            dlgConnect?.Invoke(2);
        }

        public async void EndGame(Action call = null)
        {
            Debug.Log("关闭斗鱼弹幕连接");

            if (pGameInfo == null ||
               string.IsNullOrEmpty(pGameInfo.szToken))
            {
                Debug.LogWarning("不存在的游戏场次");
                return;
            }

            if (pClient != null) 
            {
                pClient.dlgConnectErr = null;
                if (pClient.ws != null)
                {
                    await pClient.ws.Close();
                }

                pClient.Dispose();
            }

            string endGameRes = await DouyuApi.EndGame(pGameInfo.szToken);
            CLocalNetMsg msgStartGameRes = new CLocalNetMsg(endGameRes);
            int nCode = msgStartGameRes.GetInt("code");
            if (nCode != 200)
            {
                Debug.LogError("结束游戏失败，异常代码：" + nCode);
                return;
            }

            Debug.Log("结束游戏成功");
        }

        public async void RepaireNet(Action callSuc = null)
        {
            if (pClient != null)
            {
                pClient.dlgConnectErr = null;
                if (pClient.ws != null)
                {
                    await pClient.ws.Close();
                }

                pClient.Dispose();
            }

            dlgConnect = null;
            dlgReconnectSuc = callSuc;
            pClient = new DouyuDMClient(pGameInfo);
            pClient.OnEventDanmu = pEventHandler.OnRevEventChat;
            pClient.OnEventGift = pEventHandler.OnRevEventGift;
            pClient.dlgConnectSuc += ClientConnectSuc;
            pClient.dlgConnectErr += ClientConnectFailed;
            pClient.Connect(DouyuApi.szAppSecret, TimeSpan.FromSeconds(1), nMaxConnectTimes);
        }

        public bool IsGaming()
        {
            return string.IsNullOrEmpty(szToken);
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (pClient is { ws: { State: WebSocketState.Open } })
            {
                pClient.ws.DispatchMessageQueue();
            }
#endif
        }

        private void OnDestroy()
        {
            EndGame();
        }
    }
}

