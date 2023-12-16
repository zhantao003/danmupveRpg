using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

public static class ETHandlerReqUserOnlineList
{
    public static async ETVoid Request(int page)
    {
        G2C_GetOnlineUserList pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_GetOnlineUserList()
        {
            Page = page,
        }) as G2C_GetOnlineUserList;

        for(int i=0; i<pMsgRep.UserInfos.count; i++)
        {
            DUserListInfo userInfo = pMsgRep.UserInfos[i];
            EUserInfo pUser = new EUserInfo();
            pUser.nUserId = userInfo.PlayerId;
            pUser.szPlatformId = userInfo.PlatformId;
            pUser.szNickName = userInfo.NickName;
            pUser.szHeadIcon = userInfo.Head;
            pUser.nScore = userInfo.Score;

            //不为空游戏中
            pUser.szRoomId = userInfo.RoomId;

            if(EUserInfoMgr.Ins.GetOnlineUser(pUser.szPlatformId)!=null)
            {
                EUserInfoMgr.Ins.RemoveOnlineUser(pUser.szPlatformId);
            }

            EUserInfoMgr.Ins.AddOnlineUser(pUser);
        }
        UINetUserList userList = UIManager.Instance.GetUI(UIResType.ETNetUserList) as UINetUserList;
        userList.GetNextPagePlayer();
        await ETTask.CompletedTask;
    }
}
