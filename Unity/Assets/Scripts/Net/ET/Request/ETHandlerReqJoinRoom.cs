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
            Debug.LogWarning(roomId + " �Ѿ���ʼ��Ϸ");
            return;
        }
        else if(pMsgRep.Error == ErrorCode.C_RoomIsFull)
        {
            Debug.LogWarning(roomId + " �������");
            return;
        }
        else if (pMsgRep.Error == ErrorCode.C_NoRoom)
        {
            Debug.LogWarning(roomId + " �Ѳ�����");
            return;
        }
        else if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("�Ѽ��뷿�䣬�����ظ�����");
            return;
        }

        Debug.Log("���뷿��ɹ�");
    }
}
