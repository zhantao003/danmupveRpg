using NativeWebSocket;
using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DouyuDanmu
{
    public abstract class WebsocketClient : IDisposable
    {
        public WebSocket ws;

        protected string szWssUrl = "";
        protected string szToken = "";

        protected Timer m_Timer;

        #region 

        public delegate void ReceiveDanmakuEvent(Danmu dm);
        public delegate void ReceiveGiftEvent(Gift dm);

        public ReceiveDanmakuEvent OnEventDanmu;
        public ReceiveGiftEvent OnEventGift;

        #endregion

        protected virtual void OnOpen()
        {
            //SendAsync(Package.HeartBeat(szToken));

            m_Timer?.Dispose();
            m_Timer = new Timer((e) => (
                (WebsocketClient)e)?.SendAsync(Package.HeartBeat(szToken)), this, 0, 30 * 1000);
        }

        public abstract void Connect(string secret, TimeSpan timeSpan, int count);

        public abstract void Disconnect();

        protected void ProcessPacket(byte[] bytes) =>
          ProcessPacketAsync(new Package(bytes));

        private void ProcessPacketAsync(Package packet)
        {
            PackageHeader header = packet.Header;
            Debug.Log("收到消息字节长度：" + packet.Length + "\r\n消息头：" + header.Header + "  操作协议：" + header.ProtoType + "  消息体长度：" + header.PackageLength);
            //string szMsgContentUTF8 = Encoding.UTF8.GetString(packet.PacketBody);
            //Debug.Log("消息体长度：" + packet.PacketBody.Length);

            switch (header.ProtoType)
            {
                case DouyuMessageOpcode.HeartBeat:
                    {
                        HeartBeat pMsg = (HeartBeat)ProtobufHelper.FromBytes(typeof(HeartBeat), packet.PacketBody, 0, (int)packet.Length);
                        if(pMsg!=null)
                        {
                            Debug.Log("【收到心跳包】\r\nToken：" + pMsg.Token + "\r\n时间戳：" + pMsg.MilliTime + "\r\n间隔：" + pMsg.Interval);
                        }
                        else
                        {
                            Debug.Log("【收到心跳包】\r\n信息异常");
                        }
                    }
                    break;
                case DouyuMessageOpcode.TokenInvalid:
                    {
                        Debug.LogWarning("【Token失效】");
                    }
                    break;
                case DouyuMessageOpcode.EnterRoom:
                    {
                        EnterRoom pMsg = (EnterRoom)ProtobufHelper.FromBytes(typeof(EnterRoom), packet.PacketBody, 0, (int)packet.Length);
                        if(pMsg!=null)
                        {
                            Debug.Log($"【玩家进入房间】\r\nUID：{pMsg.UserInfo.UserId}\r\n用户名：{pMsg.UserInfo.Nick}\r\n头像：{pMsg.UserInfo.Avatar}\r\nOpenId：{pMsg.UserInfo.AuthOpenId}");
                        }
                    }
                    break;
                case DouyuMessageOpcode.Danmu:
                    {
                        Danmu pMsg = (Danmu)ProtobufHelper.FromBytes(typeof(Danmu), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"【弹幕消息】\r\nUID：{pMsg.UserInfo.UserId}\r\n" +
                                $"用户名：{pMsg.UserInfo.Nick}\r\n" +
                                $"头像：{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId：{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"弹幕内容：{pMsg.Content}");

                            OnEventDanmu?.Invoke(pMsg);
                        }
                    }
                    break;
                case DouyuMessageOpcode.Gift:
                    {
                        Gift pMsg = (Gift)ProtobufHelper.FromBytes(typeof(Gift), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"【礼物消息】\r\nUID：{pMsg.UserInfo.UserId}\r\n" +
                                $"用户名：{pMsg.UserInfo.Nick}\r\n" +
                                $"头像：{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId：{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"礼物ID：{pMsg.GiftInfo.GiftId}\r\n" +
                                $"礼物名字：{pMsg.GiftInfo.Name}\r\n" +
                                $"礼物图标：{pMsg.GiftInfo.Icon}\r\n" +
                                $"礼物价值：{pMsg.WorthInfo.TotalPrice}元\r\n" +
                                $"礼物数量：{pMsg.WorthInfo.Amount}个");

                            OnEventGift?.Invoke(pMsg);
                        }
                    }
                    break;
                case DouyuMessageOpcode.Prop:
                    {
                        Prop pMsg = (Prop)ProtobufHelper.FromBytes(typeof(Prop), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"【道具消息】\r\nUID：{pMsg.UserInfo.UserId}\r\n" +
                                $"用户名：{pMsg.UserInfo.Nick}\r\n" +
                                $"头像：{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId：{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"道具ID：{pMsg.PropInfo.PropId}\r\n" +
                                $"道具名字：{pMsg.PropInfo.Name}\r\n" +
                                $"道具图标：{pMsg.PropInfo.Icon}\r\n" +
                                $"道具价值：{pMsg.WorthInfo.TotalPrice}元\r\n" +
                                $"道具数量：{pMsg.WorthInfo.Amount}个");
                        }
                    }
                    break;
                case DouyuMessageOpcode.LeaveRoom:
                    {
                        LeaveRoom pMsg = (LeaveRoom)ProtobufHelper.FromBytes(typeof(LeaveRoom), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"【玩家离开房间】\r\nUID：{pMsg.UserInfo.UserId}\r\n用户名：{pMsg.UserInfo.Nick}\r\n头像：{pMsg.UserInfo.Avatar}\r\nOpenId：{pMsg.UserInfo.AuthOpenId}");
                        }
                    }
                    break;
            }
        }

        public abstract void Send(Package packet);
        protected abstract Task SendAsync(Package packet);
        public abstract void Send(byte[] packet);
        public abstract Task SendAsync(byte[] packet);

        public byte[] ProtobufObjToBytes(object msg)
        {
            byte[] array = new byte[2];

            try
            {
                //MemoryStream stream = new MemoryStream();
                //stream.Seek(0, SeekOrigin.Begin);
                //stream.SetLength(Packet.MessageIndex);
                //this.Network.MessagePacker.SerializeTo(message, stream);
                //stream.Seek(0, SeekOrigin.Begin);
            }
            catch(Exception e)
            {
                return null;
            }

            return array;
        }

        public abstract void Dispose();
    }
}

