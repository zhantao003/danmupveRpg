using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyinDanmu
{
    public class DouyinOpenEventHandler : MonoBehaviour
    {
        public System.Action<CDanmuChat> onEventDM;
        public System.Action<CDanmuGift> onEventGift;
        public System.Action<CDanmuLike> onEventLike;

        public void OnRevEventChat(CLocalNetMsg msgContent)
        {
            Debug.Log("收到弹幕:" + msgContent.GetData());
            string uid = msgContent.GetString("uid");
            string roomId = msgContent.GetString("roomId");
            string nickName = msgContent.GetString("nickName");
            string headIcon = msgContent.GetString("avatar");
            string chatInfo = msgContent.GetString("danmu");
            long timeStamp = CTimeMgr.NowMillonsSec();

            CDanmuChat pInfo = new CDanmuChat();
            pInfo.uid = uid;
            pInfo.roomId = roomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.content = chatInfo;
            pInfo.timeStamp = timeStamp;

            onEventDM?.Invoke(pInfo);
        }

        public void OnRevEventGift(CLocalNetMsg msgGift)
        {
            Debug.Log("收到礼物:" + msgGift.GetData());
            string uid = msgGift.GetString("uid");
            string roomId = msgGift.GetString("roomId");
            string nickName = msgGift.GetString("nickName");
            string headIcon = msgGift.GetString("avatar");
            string giftId = msgGift.GetString("giftId");
            long giftNum = msgGift.GetLong("giftNum");
            long giftPrice = msgGift.GetLong("giftValue");
            long timeStamp = CTimeMgr.NowMillonsSec();

            CDanmuGift pInfo = new CDanmuGift();
            pInfo.uid = uid;
            pInfo.roomId = roomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.giftName = giftId;
            pInfo.giftId = giftId;
            pInfo.giftNum = giftNum;
            pInfo.price = Mathf.RoundToInt(giftPrice * 0.1F); ;
            pInfo.timeStamp = CTimeMgr.NowMillonsSec();

            onEventGift?.Invoke(pInfo);
        }

        public void OnRevEventLike(CLocalNetMsg msgLike)
        {
            Debug.Log("收到点赞:" + msgLike.GetData());
            string uid = msgLike.GetString("uid");
            string roomId = msgLike.GetString("roomId");
            string nickName = msgLike.GetString("nickName");
            string headIcon = msgLike.GetString("avatar");
            long likeNum = msgLike.GetLong("likeNum");

            CDanmuLike pInfo = new CDanmuLike();
            pInfo.uid = uid;
            pInfo.roomId = roomId;
            pInfo.nickName = nickName;
            pInfo.headIcon = headIcon;
            pInfo.likeNum = likeNum;

            onEventLike?.Invoke(pInfo);
        }
    }
}

