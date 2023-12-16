using NativeWebSocket;
using System;
using System.Threading;
using UnityEngine;

namespace DouyinDanmu
{
    public abstract class WebsocketClient : IDisposable
    {
        public WebSocket ws;

        protected string szWssUrl = "";
        protected string szToken = "";

        protected Timer m_Timer;

        protected virtual void OnOpen()
        {

        }

        public abstract void Connect(string secret, TimeSpan timeSpan, int count);

        public abstract void Disconnect();

        protected virtual void ProcessPacket(byte[] bytes) =>
          ProcessPacketAsync(new Package(bytes));

        protected virtual void ProcessPacketAsync(Package packet)
        {
            Debug.Log("接受到消息：" + packet.szJsonData);
        }

        public virtual void SendMsg(string msg)
        {
            ws.SendText(msg);
        }

        public abstract void Dispose();
    }
}

