using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqLockStepEvent
{
    public static async ETVoid Request(DLockStepFrameEvent[] reqValue)
    {
        Actor_LockStepEvent_C2M pReq = new Actor_LockStepEvent_C2M();
        pReq.FrameEvent.AddRange(reqValue);

        SessionComponent.Instance.Session.Send(pReq);

        await ETTask.CompletedTask;
    }
}
