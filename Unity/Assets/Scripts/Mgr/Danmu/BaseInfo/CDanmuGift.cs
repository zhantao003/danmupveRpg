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
    public long giftNum;    //礼物数量
    public long price;      //礼物价值
    public bool paid;       //是否付费的

    public string fanName = "";
    public long vipLv;
    public long fanLv;
    public bool fanEquip;
    public long timeStamp;
}
