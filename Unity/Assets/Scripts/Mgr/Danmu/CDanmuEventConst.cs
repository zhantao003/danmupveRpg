using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CDanmuEventConst
{
    None = 0,
    JoinQueue,
    ExitGame,
    Chi,
    JoinLeft,
    JoinRight,
    UpPath,
    CenterPath,
    DownPath,

    CreateSoldier, // 666出兵

    Common_IdleUnitDialog = 1000,

    Test = 2000,
}

//特定礼物事件
public enum CDanmuGiftConst
{
    None = 0,
    Common,

    Soldier_Archer,
    Soldier_BoxLv1,
    Soldier_BoxLv2,
    Hero,
    Soldier_Lv1,
    Soldier_Lv2,
    Soldier_Lv3,


    Test = 2000,
}

public class CDanmuLikeConst
{
    public const string Like = "_点赞";
}
