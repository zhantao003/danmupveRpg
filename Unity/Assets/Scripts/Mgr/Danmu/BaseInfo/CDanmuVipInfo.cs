using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CDanmuVipInfo
{
    public string uid;
    public string roomId;
    public string nickName;
    public string headIcon;

    public long vipLv;  //Vip等级
    public long vipNum; //Vip赠送数量

    public string fanName;
    public long fanLv;
    public bool fanEquip;
    public long timeStamp;
}
