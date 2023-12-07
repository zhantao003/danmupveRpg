using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//时间管理类
public class CTimeMgr {
    private static readonly long epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks;

    public static float fTimeScale = 1F;

    public static float TimeScale
    {
        get { return fTimeScale; }
        set
        {
            fTimeScale = value;
            //Time.timeScale = value;
        }
    }

    public static float DeltaTime
    {
        get { return Time.unscaledDeltaTime * fTimeScale; }
    }

    public static float DeltaTimeUnScale
    {
        get { return Time.unscaledDeltaTime; }
    }

    public static float FixedDeltaTime
    {
        get { return Time.fixedDeltaTime * fTimeScale; }
    }

    public static float FixedTimeUnScale
    {
        get { return Time.fixedUnscaledDeltaTime; }
    }

    public static long NowMillonsSec()
    {
        return (DateTime.UtcNow.Ticks - epoch) / 10000;
    }
}
