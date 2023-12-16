using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepEvent_AddBaseExp : CLockStepEvent
{
    public override void DoEvent()
    {
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", msgParams.GetInt("camp"));
        msg.SetInt("exp", msgParams.GetInt("exp"));

        CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
    }
}
