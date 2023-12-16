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
            Debug.Log("�յ���Ϣ�ֽڳ��ȣ�" + packet.Length + "\r\n��Ϣͷ��" + header.Header + "  ����Э�飺" + header.ProtoType + "  ��Ϣ�峤�ȣ�" + header.PackageLength);
            //string szMsgContentUTF8 = Encoding.UTF8.GetString(packet.PacketBody);
            //Debug.Log("��Ϣ�峤�ȣ�" + packet.PacketBody.Length);

            switch (header.ProtoType)
            {
                case DouyuMessageOpcode.HeartBeat:
                    {
                        HeartBeat pMsg = (HeartBeat)ProtobufHelper.FromBytes(typeof(HeartBeat), packet.PacketBody, 0, (int)packet.Length);
                        if(pMsg!=null)
                        {
                            Debug.Log("���յ���������\r\nToken��" + pMsg.Token + "\r\nʱ�����" + pMsg.MilliTime + "\r\n�����" + pMsg.Interval);
                        }
                        else
                        {
                            Debug.Log("���յ���������\r\n��Ϣ�쳣");
                        }
                    }
                    break;
                case DouyuMessageOpcode.TokenInvalid:
                    {
                        Debug.LogWarning("��TokenʧЧ��");
                    }
                    break;
                case DouyuMessageOpcode.EnterRoom:
                    {
                        EnterRoom pMsg = (EnterRoom)ProtobufHelper.FromBytes(typeof(EnterRoom), packet.PacketBody, 0, (int)packet.Length);
                        if(pMsg!=null)
                        {
                            Debug.Log($"����ҽ��뷿�䡿\r\nUID��{pMsg.UserInfo.UserId}\r\n�û�����{pMsg.UserInfo.Nick}\r\nͷ��{pMsg.UserInfo.Avatar}\r\nOpenId��{pMsg.UserInfo.AuthOpenId}");
                        }
                    }
                    break;
                case DouyuMessageOpcode.Danmu:
                    {
                        Danmu pMsg = (Danmu)ProtobufHelper.FromBytes(typeof(Danmu), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"����Ļ��Ϣ��\r\nUID��{pMsg.UserInfo.UserId}\r\n" +
                                $"�û�����{pMsg.UserInfo.Nick}\r\n" +
                                $"ͷ��{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId��{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"��Ļ���ݣ�{pMsg.Content}");

                            OnEventDanmu?.Invoke(pMsg);
                        }
                    }
                    break;
                case DouyuMessageOpcode.Gift:
                    {
                        Gift pMsg = (Gift)ProtobufHelper.FromBytes(typeof(Gift), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"��������Ϣ��\r\nUID��{pMsg.UserInfo.UserId}\r\n" +
                                $"�û�����{pMsg.UserInfo.Nick}\r\n" +
                                $"ͷ��{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId��{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"����ID��{pMsg.GiftInfo.GiftId}\r\n" +
                                $"�������֣�{pMsg.GiftInfo.Name}\r\n" +
                                $"����ͼ�꣺{pMsg.GiftInfo.Icon}\r\n" +
                                $"�����ֵ��{pMsg.WorthInfo.TotalPrice}Ԫ\r\n" +
                                $"����������{pMsg.WorthInfo.Amount}��");

                            OnEventGift?.Invoke(pMsg);
                        }
                    }
                    break;
                case DouyuMessageOpcode.Prop:
                    {
                        Prop pMsg = (Prop)ProtobufHelper.FromBytes(typeof(Prop), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"��������Ϣ��\r\nUID��{pMsg.UserInfo.UserId}\r\n" +
                                $"�û�����{pMsg.UserInfo.Nick}\r\n" +
                                $"ͷ��{pMsg.UserInfo.Avatar}\r\n" +
                                $"OpenId��{pMsg.UserInfo.AuthOpenId}\r\n" +
                                $"����ID��{pMsg.PropInfo.PropId}\r\n" +
                                $"�������֣�{pMsg.PropInfo.Name}\r\n" +
                                $"����ͼ�꣺{pMsg.PropInfo.Icon}\r\n" +
                                $"���߼�ֵ��{pMsg.WorthInfo.TotalPrice}Ԫ\r\n" +
                                $"����������{pMsg.WorthInfo.Amount}��");
                        }
                    }
                    break;
                case DouyuMessageOpcode.LeaveRoom:
                    {
                        LeaveRoom pMsg = (LeaveRoom)ProtobufHelper.FromBytes(typeof(LeaveRoom), packet.PacketBody, 0, (int)packet.Length);
                        if (pMsg != null)
                        {
                            Debug.Log($"������뿪���䡿\r\nUID��{pMsg.UserInfo.UserId}\r\n�û�����{pMsg.UserInfo.Nick}\r\nͷ��{pMsg.UserInfo.Avatar}\r\nOpenId��{pMsg.UserInfo.AuthOpenId}");
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

