using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqCreateRoom
{
    public static async ETVoid Request(int mapID, int maxPlayer)
    {
        Debug.Log("创建房间... ...");

        DRoomConfig pRoomConfig = new DRoomConfig();
        pRoomConfig.MapId = mapID;
        pRoomConfig.MaxPlayer = maxPlayer;

        G2C_CreateRoom pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_CreateRoom()
        {
            RoomConfig = pRoomConfig
        }) as G2C_CreateRoom;

        if (pMsgRep.Error == ErrorCode.C_CreateRoomFail)
        {
            Debug.LogWarning("创建房间失败");
            //UIMsgBox.Show(CTBLLanguageInfo.Inst.GetContentGame("createroomF"), UIMsgBox.BoxType.OK);
        }
        else if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("已经在房间中");
            //UIMsgBox.Show(CTBLLanguageInfo.Inst.GetContentGame("inroom"), UIMsgBox.BoxType.OK);
        }
        else
        {
            Debug.Log("创建房间成功");
        }
    }
}
