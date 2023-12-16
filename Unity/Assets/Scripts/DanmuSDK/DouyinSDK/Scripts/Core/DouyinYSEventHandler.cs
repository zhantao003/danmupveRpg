using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyinDanmu
{
    public class DouyinYSEventHandler : MonoBehaviour
    {
        public System.Action<CDanmuChat> onEventDM;
        public System.Action<CDanmuGift> onEventGift;
        public System.Action<CDanmuLike> onEventLike;

        public void OnRevEventChat(Msg msgContent)
        {
            Debug.Log($"==弹幕消息==\r\n" +
                    $"用户ID：{msgContent.User.Id}\r\n" +
                    $"用户昵称：{msgContent.User.Nickname}\r\n" +
                    $"用户拖箱：{msgContent.User.Avatar}\r\n" +
                    $"性别：{msgContent.User.GenderToString()}\r\n" +
                    $"弹幕内容：{msgContent.Content}");

            CDanmuChat pInfo = new CDanmuChat();
            pInfo.uid = msgContent.User.Id.ToString();
            pInfo.roomId = msgContent.RoomId.ToString();
            pInfo.nickName = msgContent.User.Nickname;
            pInfo.headIcon = msgContent.User.Avatar;
            pInfo.content = msgContent.Content;
            pInfo.timeStamp = CTimeMgr.NowMillonsSec();

            onEventDM?.Invoke(pInfo);
        }

        public void OnRevEventGift(GiftMsg giftMsg)
        {
            Debug.Log($"****礼物消息****\r\n" +
                        $"用户ID：{giftMsg.User.Id}\r\n" +
                        $"用户昵称：{giftMsg.User.Nickname}\r\n" +
                        $"性别：{giftMsg.User.GenderToString()}\r\n" +
                        $"礼物ID：{giftMsg.GiftId}\r\n" +
                        $"礼物名字：{giftMsg.GiftName}\r\n" +
                        $"礼物数量：{giftMsg.GiftCount}\r\n" +
                        $"抖币价值：{giftMsg.GiamondCount}");

            CDanmuGift pInfo = new CDanmuGift();
            pInfo.uid = giftMsg.User.Id.ToString();
            pInfo.roomId = giftMsg.RoomId.ToString();
            pInfo.nickName = giftMsg.User.Nickname;
            pInfo.headIcon = giftMsg.User.Avatar;
            pInfo.giftId = giftMsg.GiftId.ToString();
            pInfo.giftName = giftMsg.GiftName;
            pInfo.giftNum = giftMsg.GiftCount;
            pInfo.price = giftMsg.GiamondCount;
            pInfo.timeStamp = CTimeMgr.NowMillonsSec();

            onEventGift?.Invoke(pInfo);
        }

        public void OnRevEventLike(LikeMsg msgLike)
        {
            Debug.Log($"****点赞消息****\r\n" +
                    $"用户ID：{msgLike.User.Id}\r\n" +
                    $"用户昵称：{msgLike.User.Nickname}\r\n" +
                    $"性别：{msgLike.User.GenderToString()}\r\n" +
                    $"点赞数：{msgLike.Count}");

            CDanmuLike pInfo = new CDanmuLike();
            pInfo.uid = msgLike.User.Id.ToString();
            pInfo.roomId = msgLike.RoomId.ToString();
            pInfo.nickName = msgLike.User.Nickname;
            pInfo.headIcon = msgLike.User.Avatar;
            pInfo.likeNum = msgLike.Count;

            onEventLike?.Invoke(pInfo);
        }
    }

}
