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
            //TODO:��Ϸ��ʱ����

            CBattleMgr.Ins.GameTimeOut();

            await ETTask.CompletedTask;
        }
    }
}

