using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KsDanmu
{
    public class KsOpenEventHandler : MonoBehaviour
    {
        [ReadOnly]
        public string szRoomId;

        public System.Action<CDanmuChat> onEventDM;
        public System.Action<CDanmuGift> onEventGift;
        public System.Action<CDanmuLike> onEventLike;

        public List<CDanmuChat> listDM = new List<CDanmuChat>();
        public List<CDanmuGift> listGift = new List<CDanmuGift>();
        public List<CDanmuLike> listLike = new List<CDanmuLike>();

        private void Update()
        {
            if(listDM.Count > 0)
            {
                CDanmuChat chat = listDM[0];
                listDM.RemoveAt(0);

                onEventDM?.Invoke(chat);
            }

            if(listGift.Count > 0)
            {
                CDanmuGift gift = listGift[0];
                listGift.RemoveAt(0);

                onEventGift?.Invoke(gift);
            }

            if(listLike.Count > 0)
            {
                CDanmuLike like = listLike[0];
                listLike.RemoveAt(0);

                onEventLike?.Invoke(like);
            }
        }

        public void OnRevEventChat(CLocalNetMsg msgContent)
        {
            Debug.Log("收到弹幕:" + msgContent.GetData());
            //用户信息
            CLocalNetMsg msgUserInfo = msgContent.GetNetMsg("user");
            string uid = msgUserInfo.GetString("id");
            string nickName = msgUserInfo.GetString("userName");
            string headIcon = msgUserInfo.GetString("headUrl");

            string chatInfo = msgContent.GetString("content");
            long timeStamp = CTimeMgr.NowMillonsSec();

            CDanmuChat pInfo = new CDanmuChat();
            pInfo.uid = uid;
            pInfo.roomId = szRoomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.content = chatInfo;
            pInfo.timeStamp = timeStamp;

            //onEventDM?.Invoke(pInfo);
            listDM.Add(pInfo);
        }

        public void OnRevEventGift(CLocalNetMsg msgGift)
        {
            Debug.Log("收到礼物:" + msgGift.GetData());
            CLocalNetMsg msgUserInfo = msgGift.GetNetMsg("user");
            string uid = msgUserInfo.GetString("id");
            string nickName = msgUserInfo.GetString("userName");
            string headIcon = msgUserInfo.GetString("headUrl");

            string giftId = msgGift.GetString("giftId");
            long giftNum = msgGift.GetLong("count");
            string giftName = msgGift.GetString("giftName");
            long giftPrice = msgGift.GetLong("giftTotalDou");
            long timeStamp = CTimeMgr.NowMillonsSec();

            CDanmuGift pInfo = new CDanmuGift();
            pInfo.uid = uid;
            pInfo.roomId = szRoomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.giftId = giftId;
            pInfo.giftNum = giftNum;
            pInfo.giftName = giftName;
            pInfo.price = giftPrice;
            pInfo.timeStamp = CTimeMgr.NowMillonsSec();

            //onEventGift?.Invoke(pInfo);
            listGift.Add(pInfo);
        }

        public void OnRevEventLike(CLocalNetMsg msgLike)
        {
            Debug.Log("收到点赞:" + msgLike.GetData());
            CLocalNetMsg msgUserInfo = msgLike.GetNetMsg("user");
            string uid = msgUserInfo.GetString("id");
            string nickName = msgUserInfo.GetString("userName");
            string headIcon = msgUserInfo.GetString("headUrl");

            long likeNum = msgLike.GetLong("count");

            CDanmuLike pInfo = new CDanmuLike();
            pInfo.uid = uid;
            pInfo.roomId = szRoomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.likeNum = likeNum;

            //onEventLike?.Invoke(pInfo);
            listLike.Add(pInfo);
        }
    }
}

