using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace QQDanmu
{
    public class DMsgSimulater : MonoBehaviour
    {
        public string szRoomId = "";
        public string szUrl = "http://127.0.0.1:12347";
        public string szWssUrl = "ws://127.0.0.1:12347/ws";

        QQOpenWebsocket pClient = null;

        public void SendDanmu()
        {
            string szParam = $"[{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                               $"\"sec_openid\":\"11111\"," +
                               $"\"content\":\"testContent\"," +
                               $"\"avatar_url\":\"testHeadIcon\"," +
                               $"\"nickname\":\"testUser\"," +
                               $"\"timestamp\":111111111}}," +
                               $"{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                               $"\"sec_openid\":\"22222\"," +
                               $"\"content\":\"testContent2\"," +
                               $"\"avatar_url\":\"testHeadIcon2\"," +
                               $"\"nickname\":\"testUser2\"," +
                               $"\"timestamp\":222222222}}]";

            StartCoroutine(RequestWebUTF8(szUrl + "/api/danmu", "POST", szParam, null, null, delegate (string value)
            {
                Debug.Log("回馈：" + value);
            }));
        }

        public void SendGift()
        {
            string szParam = $"[{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                               $"\"sec_openid\":\"111111\"," +
                               $"\"sec_gift_id\":\"testGift\"," +
                               $"\"gift_num\":100," +
                               $"\"gift_value\":5000," +
                               $"\"avatar_url\":\"testHeadIcon\"," +
                               $"\"nickname\":\"testUser\"," +
                               $"\"timestamp\":11111111}}," +
                               $"{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                               $"\"sec_openid\":\"22222\"," +
                               $"\"sec_gift_id\":\"testGift2\"," +
                               $"\"gift_num\":50," +
                               $"\"gift_value\":2000," +
                               $"\"avatar_url\":\"testHeadIcon2\"," +
                               $"\"nickname\":\"testUser2\"," +
                               $"\"timestamp\":22222222}}]";

            StartCoroutine(RequestWebUTF8(szUrl + "/api/gift", "POST", szParam, null, null, delegate (string value)
            {
                Debug.Log("回馈：" + value);
            }));
        }

        public void SendLike()
        {
            string szParam = $"[{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                             $"\"sec_openid\":\"111111\"," +
                             $"\"like_num\":\"1\"," +
                             $"\"avatar_url\":\"testHeadIcon\"," +
                             $"\"nickname\":\"testUser\"," +
                             $"\"timestamp\":11111111}}," +
                             $"{{\"msg_id\":\"{System.Guid.NewGuid().ToString()}\"," +
                             $"\"sec_openid\":\"222222\"," +
                             $"\"like_num\":\"1\"," +
                             $"\"avatar_url\":\"testHeadIcon2\"," +
                             $"\"nickname\":\"testUser2\"," +
                             $"\"timestamp\":22222222}}]";

            StartCoroutine(RequestWebUTF8(szUrl + "/api/dianzan", "POST", szParam, null, null, delegate (string value)
            {
                Debug.Log("回馈：" + value);
            }));
        }

        private void OnGUI()
        {
            GUILayout.Label("当前服务器URL：" + szUrl);
            GUILayout.Label("当前WSS URL：" + szWssUrl);

            if (GUILayout.Button("Send Danmu"))
            {
                SendDanmu();
            }

            if (GUILayout.Button("Send Gift"))
            {
                SendGift();
            }

            if (GUILayout.Button("Send Like"))
            {
                SendLike();
            }

            //if (GUILayout.Button("TestJSON"))
            //{
            //    CLocalNetArrayMsg arrInfos = new CLocalNetArrayMsg();
            //    for(int i=0; i<3; i++)
            //    {
            //        CLocalNetMsg msgSlot = new CLocalNetMsg();
            //        msgSlot.SetString("id", "test" + i);
            //        msgSlot.SetString("content", "testContent_" + i);
            //        arrInfos.AddMsg(msgSlot);
            //    }

            //    Debug.Log(arrInfos.GetData());
            //}

            //if (GUILayout.Button("TestJSON2"))
            //{
            //    string szData = "[{\"uid\":\"11111\",\"roomId\":\"666666\",\"danmu\":\"testContent\",\"nickName\":\"testUser\",\"avatar\":\"testHeadIcon\"},{\"uid\":\"22222\",\"roomId\":\"666666\",\"danmu\":\"testContent2\",\"nickName\":\"testUser2\",\"avatar\":\"testHeadIcon2\"}]";
            //    CLocalNetArrayMsg arrInfos = new CLocalNetArrayMsg(szData);
            //    for(int i=0; i<arrInfos.GetSize(); i++)
            //    {
            //        Debug.Log("Msg_" + i + ":" + arrInfos.GetNetMsg(i).GetData());
            //    }
            //    Debug.Log(arrInfos.GetData());
            //}

            if (GUILayout.Button("连接WSS"))
            {
                pClient = new QQOpenWebsocket(szWssUrl);
                pClient.dlgConnectSuc += this.OnConnectSuc;
                pClient.dlgConnectErr += this.OnConnectErr;
                pClient.OnMsgChat = OnRevDanmu;
                pClient.OnMsgGift = OnRevGift;
                pClient.OnMsgLike = OnRevLike;
                pClient.Connect("", TimeSpan.FromSeconds(1), 10);
            }
        }

        void OnConnectSuc()
        {
            Debug.Log("连接成功_发起登录");

            string szParam = $"{{\"uid\":\"{szRoomId}\"," +
                             $"\"roomId\":\"{szRoomId}\"," +
                             $"\"cmd\":\"login\"," +
                             $"\"content\":\"n\"}}";

            pClient.SendMsg(szParam);
        }

        void OnConnectErr()
        {
            EndGame();
        }

        void OnRevDanmu(CLocalNetMsg msgContent)
        {
            Debug.Log("收到弹幕：" + msgContent.GetData());
        }

        void OnRevGift(CLocalNetMsg msgContent)
        {
            Debug.Log("收到礼物：" + msgContent.GetData());
        }

        void OnRevLike(CLocalNetMsg msgContent)
        {
            Debug.Log("收到点赞：" + msgContent.GetData());
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

            webRequest.SetRequestHeader("x-nonce-str", "123456");
            webRequest.SetRequestHeader("x-timestamp", "123456789");
            webRequest.SetRequestHeader("x-signature", "application/json");
            webRequest.SetRequestHeader("x-roomid", szRoomId);
            webRequest.SetRequestHeader("x-msg-type", "live_comment");
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

        public async void EndGame(System.Action call = null)
        {
            //Debug.Log("关闭弹幕连接");

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

        private void OnDestroy()
        {
            EndGame();
        }
    }
}

