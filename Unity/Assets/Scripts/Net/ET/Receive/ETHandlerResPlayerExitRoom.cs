using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResPlayerExitRoom : AMHandler<Actor_PlayerExitRoom_M2C>
    {
        protected override async ETTask Run(Session session, Actor_PlayerExitRoom_M2C message)
        {
            if(message.UserID != EUserInfoMgr.Ins.pSelf.nUserId)
            {
                EUserInfoMgr.Ins.RemoveUser(message.UserID);
            }

            if (ERoomInfoMgr.Ins.pSelfRoom == null) return;

            ERoom.RoomSlot pRoomSlot = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerByID(message.UserID);
            if(pRoomSlot!=null)
            {
                RefreshUI(pRoomSlot.nSeatIdx);
            }

            ERoomInfoMgr.Ins.pSelfRoom.RemovePlayer(message.UserID);

            await ETTask.CompletedTask;
        }

        void RefreshUI(int seatIdx)
        {
            UIETMain uiMain = GameObject.FindObjectOfType<UIETMain>();
            if (uiMain == null) return;

            UIETBoardRoom uiETRoom = uiMain.objBoardRoom.GetComponent<UIETBoardRoom>();
            uiETRoom.ClearSlot(seatIdx);
        }
    }

}
