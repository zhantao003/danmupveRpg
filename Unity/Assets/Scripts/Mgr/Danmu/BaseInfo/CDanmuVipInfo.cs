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

    public long vipLv;  //Vip�ȼ�
    public long vipNum; //Vip��������

    public string fanName;
    public long fanLv;
    public bool fanEquip;
    public long timeStamp;
}
