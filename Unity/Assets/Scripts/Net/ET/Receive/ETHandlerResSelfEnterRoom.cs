using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResSelfEnterRoom : AMHandler<Actor_EnterRoom_M2C>
    {
        protected override async ETTask Run(Session session, Actor_EnterRoom_M2C message)
        {
            //创建房间对象
            ERoomInfoMgr.Ins.InitRoom(message.RoomInfo);

            Debug.Log($"进入房间{ERoomInfoMgr.Ins.pSelfRoom.szRoomID} 最大游玩人数：{ERoomInfoMgr.Ins.pSelfRoom.nMaxPlayer}");

            RefreshUI();

            await ETTask.CompletedTask;
        }

        static void RefreshUI()
        {
            UIETMain uiMain = GameObject.FindObjectOfType<UIETMain>();
            if (uiMain == null) return;
            uiMain.objBoardLogin.SetActive(false);
            uiMain.objBoardRoom.SetActive(true);

            UIETBoardRoom uiETRoom = uiMain.objBoardRoom.GetComponent<UIETBoardRoom>();
            for (int i = 0; i < ERoomInfoMgr.Ins.pSelfRoom.nMaxPlayer; i++)
            {
                ERoom.RoomSlot pSlot = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(i);
                if (pSlot == null) continue;

                uiETRoom.SetSlotInfo(pSlot);
            }

            uiETRoom.CheckGameStart();
        }
    }
}

