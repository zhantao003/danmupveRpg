using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepData
{
    //预定的每帧的时间长度
    public static Fix64 g_fixFrameLen = (Fix64)0.066f;

    //游戏的逻辑帧
    public static long g_uGameLogicFrame = 0;

    //超过1帧就更新逻辑，小于1帧差距等待服务器
    public static long g_uServerWaitFrame = 2;

    //等待服务器先行3帧
    public static long g_uServerStartFrame = 3;  

    //服务器当前的逻辑帧
    public static long g_uServerLogicFrame = 0;

    //服务器当前的游戏剩余时间
    public static long g_uServerGameTime = 0;

    public static SRandom pRand = new SRandom(100);
}
