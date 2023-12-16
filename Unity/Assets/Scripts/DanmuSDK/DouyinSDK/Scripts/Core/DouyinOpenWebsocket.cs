using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyinDanmu
{
    public class DouyinOpenWebsocket : WebsocketClient
    {
        public delegate void OnConnectSucEvent();
        public OnConnectSucEvent dlgConnectSuc;
        public delegate void OnConnectErrorEvent();
        public OnConnectErrorEvent dlgConnectErr;

        public delegate void OnReciveMsg(CLocalNetMsg value);
        public OnReciveMsg OnMsgChat;
        public OnReciveMsg OnMsgGift;
        public OnReciveMsg OnMsgLike;

        public DouyinOpenWebsocket(string url)
        {
            szWssUrl = url;
        }

        public override async void Connect(string secret, TimeSpan timeSpan, int count)
        {
            if (string.IsNullOrEmpty(szWssUrl))
            {
                throw new Exception("wsslink is invalid");
            }

            //尝试释放已连接的ws
            if (ws != null && ws.State != WebSocketState.Closed)
            {
                await ws.Close();
            }

            //签到内容
            string szTimeStamp = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString("f0");
            string szSignContent = $"timestamp={szTimeStamp}&token={szToken}&{secret}";
            szSignContent = SignTools.Md5(szSignContent);

            string szFinalUrl = szWssUrl;
            Debug.Log("WSS连接地址：" + szFinalUrl);
            ws = new WebSocket(szFinalUrl);

            ws.OnOpen += delegate ()
            {
                OnOpen();
                dlgConnectSuc?.Invoke();
            };
            ws.OnMessage += data =>
            {
                ProcessPacket(data);
            };
            ws.OnError += msg => { Debug.LogError("WebSocket Error Message: " + msg); };
            ws.OnClose += code => { Debug.Log("WebSocket Close: " + code); dlgConnectErr?.Invoke(); };

            await ws.Connect(timeSpan, count);
        }

        protected override void ProcessPacketAsync(Package packet)
        {
            Debug.Log("长链收到消息:" + packet.szJsonData);
            string szFinalContent = packet.szJsonData;//.Replace("\\", "");

            try
            {
                CLocalNetMsg msgContent = new CLocalNetMsg(szFinalContent);
                string cmd = msgContent.GetString("cmd");
                
                if (cmd.Equals("danmu"))
                {
                    string szChatContent = msgContent.GetString("content");
                    CLocalNetArrayMsg arrMsgInfos = new CLocalNetArrayMsg(szChatContent);
                    if (arrMsgInfos != null)
                    {
                        Debug.Log("弹幕消息数：" + arrMsgInfos.GetSize());
                        for(int i=0; i< arrMsgInfos.GetSize(); i++)
                        {
                            OnMsgChat?.Invoke(arrMsgInfos.GetNetMsg(i));
                        }
                    }
                    else
                    {
                        Debug.LogWarning("解析失败");
                    }
                }
                else if(cmd.Equals("gift"))
                {
                    string szChatContent = msgContent.GetString("content");
                    CLocalNetArrayMsg arrMsgInfos = new CLocalNetArrayMsg(szChatContent);
                    if (arrMsgInfos != null)
                    {
                        Debug.Log("礼物数：" + arrMsgInfos.GetSize());
                        for (int i = 0; i < arrMsgInfos.GetSize(); i++)
                        {
                            OnMsgGift?.Invoke(arrMsgInfos.GetNetMsg(i));
                        }
                    }
                    else
                    {
                        Debug.LogWarning("解析失败");
                    }
                }
                else if(cmd.Equals("like"))
                {
                    string szChatContent = msgContent.GetString("content");
                    CLocalNetArrayMsg arrMsgInfos = new CLocalNetArrayMsg(szChatContent);
                    if (arrMsgInfos != null)
                    {
                        Debug.Log("点赞消息数：" + arrMsgInfos.GetSize());
                        for (int i = 0; i < arrMsgInfos.GetSize(); i++)
                        {
                            OnMsgLike?.Invoke(arrMsgInfos.GetNetMsg(i));
                        }
                    }
                    else
                    {
                        Debug.LogWarning("解析失败");
                    }
                }
            }
            catch(Exception e)
            {
                Debug.LogWarning("【Error Info】" + e.Message);
            }
           
        }

        // Start is called before the first frame update
        public override void Disconnect()
        {
#if NET5_0_OR_GREATER
            clientWebSocket?.Stop(WebSocketCloseStatus.Empty, string.Empty);
            clientWebSocket?.Dispose();
#elif UNITY_2020_3_OR_NEWER
            m_Timer?.Dispose();

            ws?.Close();
            ws = null;
#endif
        }

        public override void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}

