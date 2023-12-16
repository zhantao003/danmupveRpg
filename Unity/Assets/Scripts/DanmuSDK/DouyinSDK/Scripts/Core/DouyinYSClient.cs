using NativeWebSocket;
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
    public class DouyinYSClient : MonoBehaviour
    {
        static DouyinYSClient ins = null;
        public static DouyinYSClient Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<DouyinYSClient>();
                }

                return ins;
            }
        }

        public string szWssUrl;

        public string szRoomID; //房间ID

        public DouyinDanmuConfig pStaticConfig;
        public DouyinYSEventHandler pEvent;

        DouyinYSWebSocket pClient = null;

        public bool bDebug;

        public void StartConnect(string code, System.Action<int> callSuc = null)
        {
            if (string.IsNullOrEmpty(code))
            {
                if (bDebug)
                {
                    code = pStaticConfig.szDevID;
                }
            }

            if (string.IsNullOrEmpty(code))
            {
                Debug.LogError("房间号不能为空");
                callSuc?.Invoke(99);
                return;
            }

            szRoomID = code;

            pClient = new DouyinYSWebSocket(szWssUrl);
            pClient.dlgConnectSuc = this.OnWSSConnectSuc;
            pClient.dlgConnectErr = this.OnWSSConnectFailed;
            pClient.OnMsgChat = pEvent.OnRevEventChat;
            pClient.OnMsgGift = pEvent.OnRevEventGift;
            pClient.OnMsgLike = pEvent.OnRevEventLike;
            pClient.Connect("", TimeSpan.FromSeconds(2), 10);

            callSuc?.Invoke(0);
        }
        
        void OnWSSConnectSuc()
        {
            Debug.Log("连接成功");
        }

        void OnWSSConnectFailed()
        {
            Debug.Log("连接失败");
            //终止任务
            CloseConnect();
        }

        public async void CloseConnect(System.Action call = null)
        {
            Debug.Log("关闭Douyin YS弹幕连接");

            if (pClient != null)
            {
                pClient.dlgConnectErr = null;

                if (pClient.ws != null)
                {
                    await pClient.ws.Close();
                }

                pClient.Dispose();
            }

            call?.Invoke();
        }
        
        IEnumerator RequestWebUTF8(string url, string method, string param, Dictionary<string,string> dicHeader, string cookie, System.Action<string> handler = null)
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
            catch(System.Exception e)
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
            }
        }

        void Update()
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
            CloseConnect();
        }
    }
}

