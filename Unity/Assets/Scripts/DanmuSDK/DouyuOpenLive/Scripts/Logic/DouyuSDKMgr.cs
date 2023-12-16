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

        DouyuTokenInfo pGameInfo;    //������token��Ϣ
        DouyuDMClient pClient = null;   //��Ļ�ͻ���

        public int nMaxConnectTimes = 10;

        public bool bDevMode = false;

        Action<int> dlgConnect = null;
        Action dlgReconnectSuc = null;

        [ReadOnly]
        public string szToken;  //��Ȩ�õ�Token
        [ReadOnly]
        public string szUid;    //����uid
        [ReadOnly]
        public string szRoomID; //����ID
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

            Debug.Log("������Ϣ��" + res);
            CLocalNetMsg msgRes = new CLocalNetMsg(res);
            pGameInfo = new DouyuTokenInfo(msgRes);
            if(pGameInfo.nCode != 200)
            {
                Debug.LogError("��ȡToken�쳣");
                callSuc?.Invoke(-1);
                return;
            }

            szToken = pGameInfo.szToken;
            szRoomID = roomId;
            szUid = roomId;
            szNickName = pGameInfo.szUserName;
            szHeadIcon = pGameInfo.szUserHeadIcon;

            //WSS����
            dlgConnect = callSuc;
            pClient = new DouyuDMClient(pGameInfo);
            pClient.OnEventDanmu = pEventHandler.OnRevEventChat;
            pClient.OnEventGift = pEventHandler.OnRevEventGift;
            pClient.dlgConnectSuc += ClientConnectSuc;
            pClient.dlgConnectErr += ClientConnectFailed;
            pClient.Connect(DouyuApi.szAppSecret, TimeSpan.FromSeconds(1), nMaxConnectTimes);
        }

        /// <summary>
        /// �ͻ������ӳɹ�
        /// </summary>
        async void ClientConnectSuc()
        {
            string startGameRes = await DouyuApi.StartGame(pGameInfo.szToken);
            Debug.Log("������Ϸ��" + startGameRes);

            CLocalNetMsg msgStartGameRes = new CLocalNetMsg(startGameRes);
            int nCode = msgStartGameRes.GetInt("code");
            if (nCode != 200)
            {
                Debug.LogError("��ʼ��Ϸʧ�ܣ��쳣���룺" + nCode);
                dlgConnect?.Invoke(1);
                return;
            }

            Debug.Log("��ʼ��Ϸ�ɹ�");

            dlgConnect?.Invoke(0);

            dlgReconnectSuc?.Invoke();
            dlgReconnectSuc = null;
        }

        void ClientConnectFailed()
        {
            Debug.LogError("����ʧ��");
            dlgConnect?.Invoke(2);
        }

        public async void EndGame(Action call = null)
        {
            Debug.Log("�رն��㵯Ļ����");

            if (pGameInfo == null ||
               string.IsNullOrEmpty(pGameInfo.szToken))
            {
                Debug.LogWarning("�����ڵ���Ϸ����");
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
                Debug.LogError("������Ϸʧ�ܣ��쳣���룺" + nCode);
                return;
            }

            Debug.Log("������Ϸ�ɹ�");
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

