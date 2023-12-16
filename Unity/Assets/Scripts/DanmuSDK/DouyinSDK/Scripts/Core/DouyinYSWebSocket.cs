using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace DouyinDanmu
{
    public class DouyinYSWebSocket : WebsocketClient
    {
        [ReadOnly]
        public long nRoomID;

        public delegate void OnConnectSucEvent();
        public OnConnectSucEvent dlgConnectSuc;
        public delegate void OnConnectErrorEvent();
        public OnConnectErrorEvent dlgConnectErr;

        public delegate void OnReciveMsg(Msg value);
        public delegate void OnReciveGift(GiftMsg value);
        public delegate void OnReciveLike(LikeMsg value);
        public OnReciveMsg OnMsgChat;
        public OnReciveGift OnMsgGift;
        public OnReciveLike OnMsgLike;

        public DouyinYSWebSocket(string url)
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

            //string szFinalUrl = $"ws://{szWssUrl}" +
            //    "/push?" +
            //    $"timestamp={szTimeStamp}" +
            //    $"&token={szToken}" +
            //    $"&sign={szSignContent}";
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
            BarrageMsgPack pack = JsonConvert.DeserializeObject<BarrageMsgPack>(packet.szJsonData);
            switch (pack.Type)
            {
                case BarrageMsgType.消息:
                    {
                        Msg chatMsg = JsonConvert.DeserializeObject<Msg>(pack.Data);
                        OnMsgChat?.Invoke(chatMsg);
                    }
                    break;
                case BarrageMsgType.送礼:
                    {
                        GiftMsg giftMsg = JsonConvert.DeserializeObject<GiftMsg>(pack.Data);
                        OnMsgGift?.Invoke(giftMsg);
                    }
                    break;
                case BarrageMsgType.点赞:
                    {
                        LikeMsg likeMsg = JsonConvert.DeserializeObject<LikeMsg>(pack.Data);
                        OnMsgLike?.Invoke(likeMsg);
                    }
                    break;
            }
        }

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


