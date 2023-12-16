using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMLockStepEventType
{
    JoinPlayer = 0,
    CreateSoldier = 1,
    AddBaseExp = 2,
}

public class CLockStepEvent
{
    public int nFrame;

    public CLocalNetMsg msgParams;

    public CLockStepEvent()
    {
        msgParams = new CLocalNetMsg();
    }

    public virtual void DoEvent()
    {

    }
}
