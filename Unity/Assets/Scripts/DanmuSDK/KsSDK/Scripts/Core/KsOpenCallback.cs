using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KsDanmu
{
    //�ص��ӿ�
    public class KsOpenCallback : KsInteractCallback
    {
        public KsOpenEventHandler pEventHandler;
        public KsOpenClient pClient;

        public System.Action<int> dlgConnectSuc;

        public System.Action<CDanmuChat> onEventDM;
        public System.Action<CDanmuGift> onEventGift;
        public System.Action<CDanmuLike> onEventLike;

        public void OnConnected(string data)
        {
            Debug.Log("���ֵ�Ļ���ӳɹ�");
            Debug.Log("��KSMsgData��" + data);

            CLocalNetMsg msgData = new CLocalNetMsg(data);
            string roomId = msgData.GetString("ksUid");
            string token = msgData.GetString("token");

            CLocalNetMsg msgUserInfo = msgData.GetNetMsg("user");
            string uid = msgUserInfo.GetString("id");
            string name = msgUserInfo.GetString("userName");
            string headIcon = msgUserInfo.GetString("headUrl");

            pClient.szRoomID = roomId;
            pClient.szToken = token;
            pClient.szUid = uid;
            pClient.szNickName = name;
            pClient.szHeadIcon = headIcon;

            pEventHandler.szRoomId = roomId;

            dlgConnectSuc?.Invoke(0);
        }

        public void OnDataReceived(int cmd, string data)
        {
            if (pEventHandler == null) return;

            try
            {
                CLocalNetMsg msgData = new CLocalNetMsg(data);

                switch (cmd)
                {
                    case 3://��Ļ
                        {
                            pEventHandler.OnRevEventChat(msgData);
                        }
                        break;
                    case 2://����
                        {
                            pEventHandler.OnRevEventGift(msgData);
                        }
                        break;
                    case 7://����
                        {
                            pEventHandler.OnRevEventLike(msgData);
                        }
                        break;
                    case 10://��ע
                        {
                            Debug.Log("�յ���ע:" + data);
                        }
                        break;
                }
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message);
                return;
            }
        }

        public void OnDisconnected()
        {
            Debug.LogWarning("���ֵ�Ļ���ӶϿ�");
        }

        public void OnError(int code, string msg)
        {
            Debug.LogError("KS��Ļ���Ӵ�����:" + code);
            Debug.LogError("KS��Ļ���Ӵ�����Ϣ:" + msg);
        }
    }
}

