using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CDanmuChatEventInfo
{
    public enum EMType
    {
        Direct,     //直接匹配文字
        FollowNum,  //后接数字
        Contain,    //包容
    }

    public CDanmuEventConst eventType;
    public EMType emType;
    [HideInInspector]
    public string szInfo;
}

public struct CDanmuGiftEventInfo
{
    public CDanmuGiftConst eventType;
}
