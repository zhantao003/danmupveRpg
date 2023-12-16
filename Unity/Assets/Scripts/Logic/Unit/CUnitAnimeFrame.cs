using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public enum EMUnitAnimeState
{
    Idle = 0,
    Move,
    Atk,
    Dead,
    Skill,
    Skill2,

    Max,
}

public enum EMUnitAnimeDir
{
    UpL = 0,
    UpR,
    DownL,
    DownR,
    Left,
    Right,
    Up,
    Down,
   
    Max,
}

[Serializable]
public class CUnitAnimeStateSlot
{
    public EMUnitAnimeState emState;
    public Dictionary<EMUnitAnimeDir, CUnitAnimeDirSlot> dicDirSlots = new Dictionary<EMUnitAnimeDir, CUnitAnimeDirSlot>();

    public CUnitAnimeDirSlot GetDirSlot(EMUnitAnimeDir dir)
    {
        CUnitAnimeDirSlot pRes = null;
        if(dicDirSlots.TryGetValue(dir, out pRes))
        {

        }

        return pRes;
    }
}

[Serializable]
public class CUnitAnimeDirSlot
{
    public Sprite[] arrFrames;
    public bool bLoop;
    public bool bRevert;
    public float fFrameTime = 0.15f;
}
