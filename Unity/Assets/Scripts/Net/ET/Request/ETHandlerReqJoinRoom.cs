using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqJoinRoom 
{
    public static async ETVoid Request(string roomId)
    {
        G2C_JoinRoom pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_JoinRoom() { 
            RoomID = roomId
        }) as G2C_JoinRoom;

        if(pMsgRep.Error == ErrorCode.C_RoomIsGaming)
        {
            Debug.LogWarning(roomId + " 已经开始游戏");
            return;
        }
        else if(pMsgRep.Error == ErrorCode.C_RoomIsFull)
        {
            Debug.LogWarning(roomId + " 玩家已满");
            return;
        }
        else if (pMsgRep.Error == ErrorCode.C_NoRoom)
        {
            Debug.LogWarning(roomId + " 已不存在");
            return;
        }
        else if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("已加入房间，不能重复加入");
            return;
        }

        Debug.Log("加入房间成功");
    }
}
