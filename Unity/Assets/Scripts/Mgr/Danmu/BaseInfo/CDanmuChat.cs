using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CDanmuChat
{
    public string uid;
    public string roomId;
    public string nickName;
    public string headIcon;
    public string content;

    public string fanName;
    public long vipLv;
    public long fanLv;
    public bool fanEquip;
    public long timeStamp;
}
