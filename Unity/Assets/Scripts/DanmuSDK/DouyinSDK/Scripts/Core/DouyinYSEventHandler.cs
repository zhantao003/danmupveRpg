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
            Debug.Log($"==��Ļ��Ϣ==\r\n" +
                    $"�û�ID��{msgContent.User.Id}\r\n" +
                    $"�û��ǳƣ�{msgContent.User.Nickname}\r\n" +
                    $"�û����䣺{msgContent.User.Avatar}\r\n" +
                    $"�Ա�{msgContent.User.GenderToString()}\r\n" +
                    $"��Ļ���ݣ�{msgContent.Content}");

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
            Debug.Log($"****������Ϣ****\r\n" +
                        $"�û�ID��{giftMsg.User.Id}\r\n" +
                        $"�û��ǳƣ�{giftMsg.User.Nickname}\r\n" +
                        $"�Ա�{giftMsg.User.GenderToString()}\r\n" +
                        $"����ID��{giftMsg.GiftId}\r\n" +
                        $"�������֣�{giftMsg.GiftName}\r\n" +
                        $"����������{giftMsg.GiftCount}\r\n" +
                        $"���Ҽ�ֵ��{giftMsg.GiamondCount}");

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
            Debug.Log($"****������Ϣ****\r\n" +
                    $"�û�ID��{msgLike.User.Id}\r\n" +
                    $"�û��ǳƣ�{msgLike.User.Nickname}\r\n" +
                    $"�Ա�{msgLike.User.GenderToString()}\r\n" +
                    $"��������{msgLike.Count}");

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
