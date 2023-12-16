using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqGameReady 
{
    public static async ETVoid Request()
    {
        SessionComponent.Instance.Session.Send(new C2G_GameReady()
        {
            Ready = true
        });

        await ETTask.CompletedTask;
    }
}
