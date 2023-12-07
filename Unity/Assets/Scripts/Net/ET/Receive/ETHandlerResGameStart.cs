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

            RefreshUI();

            await ETTask.CompletedTask;
        }

        void RefreshUI()
        {
            UIETMain uiMain = GameObject.FindObjectOfType<UIETMain>();
            if (uiMain == null) return;

            uiMain.objBoardRoom.SetActive(false);

            //创建玩家单位
            Vector3 vPos = new Vector3();
            vPos.x = Random.Range(-5f, 5f);
            vPos.z = Random.Range(-5f, 5f);
            ERoom.RoomSlot pSelfslot = ERoomInfoMgr.Ins.pSelfRoom.GetSelfSlot();
            EPlayerMgr.Ins.CreatePlayer(vPos, Quaternion.Euler(0,Random.Range(0,360),0), pSelfslot.player, true);
        }
    }
}

