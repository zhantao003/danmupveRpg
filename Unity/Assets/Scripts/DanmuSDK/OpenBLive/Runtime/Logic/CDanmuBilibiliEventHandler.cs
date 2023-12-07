using OpenBLive.Runtime.Data;
using System.Text;
using UnityEngine;

public class CDanmuBilibiliEventHandler : MonoBehaviour
{
    public System.Action<CDanmuChat> onEventDM;
    public System.Action<CDanmuGift> onEventGift;
    public System.Action<CDanmuVipInfo> onEventVip;

    public void WebSocketBLiveClientOnSuperChat(SuperChat superChat)
    {
        StringBuilder sb = new StringBuilder("�յ�SC!");
        sb.AppendLine();
        sb.Append("�����û���");
        sb.AppendLine(superChat.userName);
        sb.Append("�������ݣ�");
        sb.AppendLine(superChat.message);
        sb.Append("��");
        sb.Append(superChat.rmb);
        sb.Append("Ԫ");
        Debug.Log(sb);
    }

    public void WebSocketBLiveClientOnGuardBuy(Guard guard)
    {
        StringBuilder sb = new StringBuilder("�յ��󺽺�!");
        sb.AppendLine();
        sb.Append("�����û���");
        sb.AppendLine(guard.userInfo.userName);
        sb.Append("������");
        sb.Append(guard.guardUnit);
        Debug.Log(sb);

        CDanmuVipInfo vipInfo = new CDanmuVipInfo();
        vipInfo.uid = guard.userInfo.uid.ToString();
        vipInfo.roomId = guard.roomID.ToString();
        vipInfo.nickName = guard.userInfo.userName;
        vipInfo.headIcon = guard.userInfo.userFace;

        vipInfo.vipLv = guard.guardLevel;
        vipInfo.vipNum = guard.guardNum;

        vipInfo.fanLv = guard.fansMedalLevel;
        vipInfo.fanName = guard.fansMedalName;
        vipInfo.fanEquip = guard.fansMedalWearingStatus;
        vipInfo.timeStamp = CTimeMgr.NowMillonsSec();

        onEventVip?.Invoke(vipInfo);
    }

    public void WebSocketBLiveClientOnGift(SendGift sendGift)
    {
        StringBuilder sb = new StringBuilder("�յ�����!");
        sb.AppendLine();
        sb.Append("�����û���");
        sb.AppendLine(sendGift.userName);
        sb.Append("������");
        sb.Append(sendGift.giftNum);
        sb.Append("��");
        sb.Append($"��{sendGift.giftName}��");
        sb.AppendLine();
        sb.Append("�����ֵ��");
        sb.Append(sendGift.price.ToString());
        sb.AppendLine();
        sb.Append("��ؼ�ֵ��");
        int nBattery = (int)(sendGift.price * 0.1);
        sb.Append(nBattery.ToString());
        sb.AppendLine();
        sb.Append("����ID��");
        sb.Append(sendGift.giftId.ToString());
        Debug.Log(sb);

        CDanmuGift giftInfo = new CDanmuGift();
        giftInfo.uid = sendGift.uid.ToString();
        giftInfo.roomId = sendGift.roomId.ToString();
        giftInfo.nickName = sendGift.userName;
        giftInfo.headIcon = sendGift.userFace;

        giftInfo.giftId = sendGift.giftId.ToString();
        giftInfo.giftName = sendGift.giftName;
        giftInfo.giftIcon = "";
        giftInfo.giftNum = sendGift.giftNum;
        //ֱ�����ܼ�ֵ
        giftInfo.price = Mathf.RoundToInt(sendGift.price * 0.01F * sendGift.giftNum);
        giftInfo.paid = sendGift.paid;

        giftInfo.vipLv = sendGift.guardLevel;
        giftInfo.fanLv = sendGift.fansMedalLevel;
        giftInfo.fanName = sendGift.fansMedalName;
        giftInfo.fanEquip = sendGift.fansMedalWearingStatus;
        giftInfo.timeStamp = sendGift.timestamp;

        onEventGift?.Invoke(giftInfo);
    }

    public void WebSocketBLiveClientOnDanmaku(Dm dm)
    {
        StringBuilder sb = new StringBuilder("�յ���Ļ!");
        sb.AppendLine();
        sb.Append("UID��");
        sb.AppendLine(dm.uid.ToString());
        sb.Append("�û���");
        sb.AppendLine(dm.userName);
        sb.Append("��Ļ���ݣ�");
        sb.Append(dm.msg);
        Debug.Log(sb);

        CDanmuChat chatInfo = new CDanmuChat();
        chatInfo.uid = dm.uid.ToString();
        chatInfo.roomId = dm.roomId.ToString();
        chatInfo.nickName = dm.userName;
        chatInfo.headIcon = dm.userFace;
        chatInfo.content = dm.msg;

        chatInfo.vipLv = dm.guardLevel;
        chatInfo.fanLv = dm.fansMedalLevel;
        chatInfo.fanName = dm.fansMedalName;
        chatInfo.fanEquip = dm.fansMedalWearingStatus;
        chatInfo.timeStamp = dm.timestamp;

        onEventDM?.Invoke(chatInfo);
    }
}
