using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResCreateTestPlayerUnit : AMHandler<Actor_CreateTetsPlayerUnit_C2M>
    {
        protected override async ETTask Run(Session session, Actor_CreateTetsPlayerUnit_C2M message)
        {
            if (ERoomInfoMgr.Ins.pSelfRoom == null) return;

            ERoom.RoomSlot pRoomSlot = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerByID(message.UserId);
            if (pRoomSlot == null) return;

            Vector3 pos = new Vector3();
            pos.x = message.Pos.X;
            pos.z = message.Pos.Z;

            Quaternion rotation = new Quaternion(message.Rotation.X, message.Rotation.Y, message.Rotation.Z, message.Rotation.W);

            EPlayerMgr.Ins.CreatePlayer(pos, rotation, pRoomSlot.player, false);

            await ETTask.CompletedTask;
        }
    }
}

