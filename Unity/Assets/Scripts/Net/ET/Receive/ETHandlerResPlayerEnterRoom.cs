using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResPlayerEnterRoom : AMHandler<Actor_PlayerEnterRoom_M2C>
    {
        protected override async ETTask Run(Session session, Actor_PlayerEnterRoom_M2C message)
        {
            if (ERoomInfoMgr.Ins.pSelfRoom == null) return;

            DRoomSeatInfo pSeatInfo = message.SeatInfo;
            ERoomInfoMgr.Ins.AddRoomPlayer(ref ERoomInfoMgr.Ins.pSelfRoom, pSeatInfo);

            RefreshUI(pSeatInfo.SeatIdx);

            await ETTask.CompletedTask;
        }

        static void RefreshUI(int seatIdx)
        {
            UIETMain uiMain = GameObject.FindObjectOfType<UIETMain>();
            if (uiMain == null) return;

            UIETBoardRoom uiETRoom = uiMain.objBoardRoom.GetComponent<UIETBoardRoom>();

            ERoom.RoomSlot pSlot = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(seatIdx);
            if (pSlot == null) return;

            uiETRoom.SetSlotInfo(pSlot);
            uiETRoom.CheckGameStart();
        }
    }
}

