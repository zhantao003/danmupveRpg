using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResGameStart : AMHandler<Actor_GameStart_M2C>
    {
        protected override async ETTask Run(Session session, Actor_GameStart_M2C message)
        {
            if (ERoomInfoMgr.Ins.pSelfRoom == null) return;

            //处理游戏开始逻辑
            RefreshUI();
            
            //刷新下时间
            CLockStepData.g_uServerGameTime = 0;
            CLockStepData.g_uGameLogicFrame = 0;
            await ETTask.CompletedTask;
        }

        void RefreshUI()
        {
            UIManager.Instance.CloseUI(UIResType.Loading);
            CBattleMgr.Ins.StartChouJiangUI();

            CLockStepMgr.Ins.Init(CLockStepMgr.EMType.Net);
        }
    }
}

