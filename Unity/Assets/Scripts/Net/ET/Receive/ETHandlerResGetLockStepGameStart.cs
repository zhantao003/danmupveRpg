using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResGetLockStepGameStart : AMHandler<Actor_GetLockStepGameStart_M2C>
    {
        protected override async ETTask Run(Session session, Actor_GetLockStepGameStart_M2C message)
        {
            if (ERoomInfoMgr.Ins.pSelfRoom == null) return;

            CBattleMgr.Ins.GameStart();

            ////刷新下时间
            //CLockStepData.g_uServerGameTime = 0;
            //CLockStepData.g_uGameLogicFrame = 0;
            await ETTask.CompletedTask;
        }

    }
}

