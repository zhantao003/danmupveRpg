using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace DouyuDanmu
{
    public class DouyuEventHandler : MonoBehaviour
    {
        [ReadOnly]
        public string szRoomId;

        public System.Action<CDanmuChat> onEventDM;
        public System.Action<CDanmuGift> onEventGift;
        public System.Action<CDanmuLike> onEventLike;

        public void OnRevEventChat(Danmu dm)
        {
            StringBuilder sb = new StringBuilder("收到弹幕!");
            sb.AppendLine();
            sb.Append("UID：");
            sb.AppendLine(dm.UserInfo.UserId.ToString());
            sb.Append("用户：");
            sb.AppendLine(dm.UserInfo.Nick);
            sb.Append("弹幕内容：");
            sb.Append(dm.Content);
            Debug.Log(sb);
            
            CDanmuChat chatInfo = new CDanmuChat();
            chatInfo.uid = dm.UserInfo.UserId.ToString();
            chatInfo.roomId = szRoomId;
            chatInfo.nickName = dm.UserInfo.Nick;
            chatInfo.headIcon = dm.UserInfo.Avatar;
            chatInfo.content = dm.Content;

            chatInfo.vipLv = 0;
            chatInfo.fanLv = 0;
            chatInfo.fanName = "";
            chatInfo.fanEquip = false;
            chatInfo.timeStamp = dm.PublicInfo.MilliTime;

            onEventDM?.Invoke(chatInfo);
        }

        public void OnRevEventGift(Gift sendGift)
        {
            StringBuilder sb = new StringBuilder("收到礼物!");
            sb.AppendLine();
            sb.Append("来自用户：");
            sb.AppendLine(sendGift.UserInfo.Nick);
            sb.Append("赠送了");
            sb.Append(sendGift.WorthInfo.Amount);
            sb.Append("个");
            sb.Append($"【{sendGift.GiftInfo.Name}】");
            sb.AppendLine();
            sb.Append("礼物总价值：");
            sb.Append(sendGift.WorthInfo.TotalPrice.ToString());
            sb.Append("元");
            sb.AppendLine();
            sb.Append("礼物ID：");
            sb.Append(sendGift.GiftInfo.GiftId);
            Debug.Log(sb);

            CDanmuGift pInfo = new CDanmuGift();
            pInfo.uid = sendGift.UserInfo.UserId;
            pInfo.roomId = szRoomId;
            pInfo.nickName = sendGift.UserInfo.Nick;
            pInfo.headIcon = sendGift.UserInfo.Avatar;
            pInfo.giftId = sendGift.GiftInfo.GiftId;
            pInfo.giftNum = sendGift.WorthInfo.Amount;
            pInfo.giftName = sendGift.GiftInfo.Name;
            pInfo.price = Mathf.RoundToInt((float)sendGift.WorthInfo.TotalPrice * 0.1f);
            pInfo.timeStamp = sendGift.PublicInfo.MilliTime;

            onEventGift?.Invoke(pInfo);
        }
    }
}

