using NativeWebSocket;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace QQDanmu
{
    public class QQOpenClient : MonoBehaviour
    {
        static QQOpenClient ins = null;
        public static QQOpenClient Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<QQOpenClient>();
                }

                return ins;
            }
        }

        #region ����

        //�����ַ
        public string URL_SERVER = "http://127.0.0.1:12347";
        public string WSS_SERVER = "wss://127.0.0.1:12347";

        public const string METHOD_POST = "POST";

        public const string QQ_GETTOKEN = "/api/checkQQToken";
        public const string QQ_STARTTASK = "/api/startQQTask";
        public const string QQ_ENDTASK = "/api/endQQTask";

        #endregion

        public QQDanmuConfig pStaticConfig; //ƽ̨�������
        public QQOpenEventHandler pEvent;    //ʱ�������

        [ReadOnly]
        public string szToken;  //��Ȩ�õ�Token
        public string szCode;   //�����
        public string szRoomID; //����ID

        public bool bListenTask;    //��������

        QQOpenWebsocket pClient = null;

        public bool bDebug;
        System.Action<int> connectCallSuc = null;

        public void StartConnect(string code, string roomId, System.Action<int> callSuc = null)
        {
            connectCallSuc = callSuc;

            if (string.IsNullOrEmpty(roomId))
            {
                Debug.LogError("�����벻��Ϊ��");
                connectCallSuc?.Invoke(99);
                return;
            }

            GetToken(roomId, delegate (bool value)
            {
                if (!value)
                {
                    Debug.LogError("��ȡToken�쳣");
                    connectCallSuc?.Invoke(999);

                    return;
                }

                szCode = code;
                szRoomID = roomId;

                if(bListenTask)
                {
                    StartTask(code, QQ_STARTTASK, delegate (bool taskRes)
                    {
                        if(taskRes)
                        {
                            Debug.Log($"���������ɹ�");
                            OnListenSuc();
                        }  
                    });
                }
                else
                {
                    OnListenSuc();
                }
            });
        }

        //��������
        void OnListenSuc()
        {
            //����WSS
            Debug.Log("��������׼����");
            pClient = new QQOpenWebsocket(WSS_SERVER);
            pClient.dlgConnectSuc = this.OnWSSConnectSuc;
            pClient.dlgConnectErr = this.OnWSSConnectFailed;
            pClient.OnMsgChat = pEvent.OnRevEventChat;
            pClient.OnMsgGift = pEvent.OnRevEventGift;
            pClient.OnMsgLike = pEvent.OnRevEventLike;
            pClient.Connect("", TimeSpan.FromSeconds(2), 10);
        }

        void OnWSSConnectSuc()
        {
            Debug.Log("���ӳɹ�");

            //�����¼
            string szParam = $"{{\"uid\":\"{szRoomID}\"," +
                             $"\"roomId\":\"{szRoomID}\"," +
                             $"\"cmd\":\"login\"," +
                             $"\"content\":\"n\"}}";

            pClient.SendMsg(szParam);

            connectCallSuc?.Invoke(0);
        }

        void OnWSSConnectFailed()
        {
            Debug.Log("����ʧ��");
            //��ֹ����
            CloseConnect();
        }

        public async void CloseConnect(System.Action call = null)
        {
            Debug.Log("�ر�QQ��Ļ����");

            if (!string.IsNullOrEmpty(szCode))
            {
                EndTask(szCode, "");
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

            if (!string.IsNullOrEmpty(szCode))
            {
                szCode = "";
            }

            if (!string.IsNullOrEmpty(szRoomID))
            {
                szRoomID = "";
            }

            if (!string.IsNullOrEmpty(szToken))
            {
                szToken = "";
            }

            call?.Invoke();
        }

        public bool IsGaming()
        {
            return string.IsNullOrEmpty(szRoomID);
        }

        #region ��������

        //��ȡToken
        void GetToken(string roomId, System.Action<bool> callSuc = null)
        {
            if (pStaticConfig == null)
            {
                Debug.LogWarning("None Douyin DanmuConfig");
                return;
            }

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + QQ_GETTOKEN, METHOD_POST, dataForm, null, null, delegate (string value)
            {
                CLocalNetMsg msgContent = new CLocalNetMsg(value);
                string code = msgContent.GetString("status");
                if (!code.Equals("ok"))
                {
                    Debug.LogError("��ȡToken�쳣");
                    callSuc?.Invoke(false);

                    return;
                }

                szToken = msgContent.GetString("token");

                callSuc?.Invoke(true);
            }));
        }

        /// <summary>
        /// ��������
        /// </summary>
        void StartTask(string roomId, string msgType, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            //string szParam = $"{{\"roomId\":\"{roomId}\",\"msgType\":\"{msgType}\"}}";

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);
            //dataForm.AddField("msgType", "");

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szToken);

            StartCoroutine(RequestWebUTF8WWWForm(URL_SERVER + QQ_STARTTASK, METHOD_POST, dataForm, dicHeaders, null,
                delegate (string value)
                {
                    CLocalNetMsg msgContent = new CLocalNetMsg(value);
                    string szStatus = msgContent.GetString("status");
                    if (!szStatus.Equals("ok"))
                    {
                        Debug.LogError($"������������ʧ��!");
                        callSuc?.Invoke(false);

                        return;
                    }

                    callSuc?.Invoke(true);
                }));
        }

        void EndTask(string roomId, string msgType, System.Action<bool> callSuc = null)
        {
            if (string.IsNullOrEmpty(szToken))
            {
                Debug.LogWarning("None Token");
                return;
            }

            WWWForm dataForm = new WWWForm();
            dataForm.AddField("roomId", roomId);

            Dictionary<string, string> dicHeaders = new Dictionary<string, string>();
            dicHeaders.Add("access-token", szToken);

            RequestWebUTF8WWWFormDirect(URL_SERVER + QQ_ENDTASK, METHOD_POST, dataForm, dicHeaders);
        }

        #endregion

        #region Http����ӿ�

        void RequestWebUTF8WWWFormDirect(string url, string method, WWWForm param, Dictionary<string, string> dicHeader)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(URL_SERVER + QQ_ENDTASK, param);
            webRequest.method = method;
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 5000;

            if (dicHeader != null)
            {
                foreach (string key in dicHeader.Keys)
                {
                    webRequest.SetRequestHeader(key, dicHeader[key]);
                }
            }
            webRequest.SetRequestHeader("Accept", "application/json");
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            Debug.Log("Req Url:" + url + "\r\nReq Params:" + param);
            webRequest.SendWebRequest();
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
                Debug.LogError("��Error Info��\r\n" + e.Message);
            }
        }

        #endregion

        void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (pClient is { ws: { State: WebSocketState.Open } })
            {
                pClient.ws.DispatchMessageQueue();
            }
#endif

            ////���Token��ʱ
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

