using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KsDanmu
{
    //回调接口
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
            Debug.Log("快手弹幕连接成功");
            Debug.Log("【KSMsgData】" + data);

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
                    case 3://弹幕
                        {
                            pEventHandler.OnRevEventChat(msgData);
                        }
                        break;
                    case 2://送礼
                        {
                            pEventHandler.OnRevEventGift(msgData);
                        }
                        break;
                    case 7://点赞
                        {
                            pEventHandler.OnRevEventLike(msgData);
                        }
                        break;
                    case 10://关注
                        {
                            Debug.Log("收到关注:" + data);
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
            Debug.LogWarning("快手弹幕连接断开");
        }

        public void OnError(int code, string msg)
        {
            Debug.LogError("KS弹幕连接错误码:" + code);
            Debug.LogError("KS弹幕连接错误信息:" + msg);
        }
    }
}

