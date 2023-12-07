using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CDanmuGift
{
    public string uid;
    public string roomId;
    public string nickName;
    public string headIcon;

    public string giftId = "";
    public string giftName = "";
    public string giftIcon = "";
    public long giftNum;    //��������
    public long price;      //�����ֵ
    public bool paid;       //�Ƿ񸶷ѵ�

    public string fanName = "";
    public long vipLv;
    public long fanLv;
    public bool fanEquip;
    public long timeStamp;
}
