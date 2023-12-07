using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CDanmuEventConst
{
    None = 0,
    JoinQueue,
    ExitGame,
    Chi,

    Common_IdleUnitDialog = 1000,

    Test = 2000,
}

//特定礼物事件
public enum CDanmuGiftConst
{
    None = 0,
    Common,

    Test = 2000,
}

public class CDanmuLikeConst
{
    public const string Like = "_点赞";
}
