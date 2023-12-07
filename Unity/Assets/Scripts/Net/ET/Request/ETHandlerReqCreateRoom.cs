using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqCreateRoom
{
    public static async ETVoid Request(int mapID, int maxPlayer)
    {
        Debug.Log("��������... ...");

        DRoomConfig pRoomConfig = new DRoomConfig();
        pRoomConfig.MapId = mapID;
        pRoomConfig.MaxPlayer = maxPlayer;

        G2C_CreateRoom pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_CreateRoom()
        {
            RoomConfig = pRoomConfig
        }) as G2C_CreateRoom;

        if (pMsgRep.Error == ErrorCode.C_CreateRoomFail)
        {
            Debug.LogWarning("��������ʧ��");
            //UIMsgBox.Show(CTBLLanguageInfo.Inst.GetContentGame("createroomF"), UIMsgBox.BoxType.OK);
        }
        else if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("�Ѿ��ڷ�����");
            //UIMsgBox.Show(CTBLLanguageInfo.Inst.GetContentGame("inroom"), UIMsgBox.BoxType.OK);
        }
        else
        {
            Debug.Log("��������ɹ�");
        }
    }
}
