using NativeWebSocket;
using System;
using System.Threading.Tasks;
using UnityEngine;

namespace DouyuDanmu
{
    public class DouyuDMClient : WebsocketClient
    {
        public delegate void OnConnectSucEvent();
        public OnConnectSucEvent dlgConnectSuc;
        public delegate void OnConnectErrorEvent();
        public OnConnectErrorEvent dlgConnectErr;

        public DouyuDMClient(DouyuTokenInfo info)
        {
            if (info == null) return;

            szWssUrl = info.szWsUrl;
            szToken = info.szToken;
        }

        public override async void Connect(string appkey, TimeSpan timeSpan, int count)
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
            string szSignContent = $"timestamp={szTimeStamp}&token={szToken}&{appkey}";
            szSignContent = SignTools.Md5(szSignContent);

            string szFinalUrl = $"ws://{szWssUrl}" + 
                "/push?" + 
                $"timestamp={szTimeStamp}" +
                $"&token={szToken}" + 
                $"&sign={szSignContent}";
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

        public override void Send(byte[] packet)
        {
            if (ws.State == WebSocketState.Open)
            {
                ws.Send(packet);
            }
        }

        public override void Send(Package packet) => Send(packet.ToBytes);
        public override Task SendAsync(byte[] packet) => Task.Run(() => Send(packet));
        protected override Task SendAsync(Package packet) => SendAsync(packet.ToBytes);

        public override void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}

