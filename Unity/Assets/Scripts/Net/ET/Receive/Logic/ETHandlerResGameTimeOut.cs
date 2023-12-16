using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResGameTimeOut : AMHandler<Actor_LockStepGameTimeOut_M2C>
    {
        protected override async ETTask Run(Session session, Actor_LockStepGameTimeOut_M2C message)
        {
            //TODO:游戏超时处理

            CBattleMgr.Ins.GameTimeOut();

            await ETTask.CompletedTask;
        }
    }
}

