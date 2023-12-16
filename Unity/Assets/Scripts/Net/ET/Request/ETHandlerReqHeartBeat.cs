using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqHeartBeat
{
    public static async ETVoid Request()
    {
        G2C_HeartBeat pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_HeartBeat()
        {

        }) as G2C_HeartBeat;
    }
}
