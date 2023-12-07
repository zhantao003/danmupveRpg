using ETModel;
using UnityEngine;

public static class ETHandlerReqGetRoomList
{
    public static async ETVoid Request()
    {
        G2C_GetRoomList pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_GetRoomList()) as G2C_GetRoomList;

        if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("已经在房间中");
        }
        else
        {
            ERoomInfoMgr.Ins.ClearPublicRoom();

            for (int i = 0; i < pMsgRep.Rooms.count; i++)
            {
                DRoomSimpleInfo msgInfo = pMsgRep.Rooms[i];

                ERoomSimpleInfo pRoomInfo = new ERoomSimpleInfo();
                pRoomInfo.szRoomId = msgInfo.RoomId;
                pRoomInfo.nMaxPlayer = msgInfo.RoomConfig.MaxPlayer;
                ERoomInfoMgr.Ins.AddPublicRoom(pRoomInfo);
            }

            //刷新UI
            RefreshUI();
        }
    }

    static void RefreshUI()
    {
        UIETMain uiMain = GameObject.FindObjectOfType<UIETMain>();
        if (uiMain == null) return;

        UIETBoardLogin uiETLogin = uiMain.objBoardLogin.GetComponent<UIETBoardLogin>();
        uiETLogin.RefreshRoomList();
    }
}
