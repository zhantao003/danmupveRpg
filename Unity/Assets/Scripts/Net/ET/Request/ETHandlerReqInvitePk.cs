using ETModel;
using UnityEngine;

public static class ETHandlerReqInvitePk
{
    public static async ETVoid Request(long userId)
    {
        G2C_InvitePk pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_InvitePk()
        {
            UserId = userId,
        }) as G2C_InvitePk;

        if(pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogWarning("玩家已经在房间中");
            //UIIdleMainMenu.Instance.toast.Show("玩家已经在房间中");
            return;
        }
        else if(pMsgRep.Error == ErrorCode.C_UserNotOnline)
        {
            Debug.LogWarning("玩家不在线");
            //UIIdleMainMenu.Instance.toast.Show("玩家已经在房间中");
            return;
        }
        else
        {
            //成功邀请
            //UIIdleMainMenu.Instance.toast.Show("已发送邀请");
        }
    }
}
