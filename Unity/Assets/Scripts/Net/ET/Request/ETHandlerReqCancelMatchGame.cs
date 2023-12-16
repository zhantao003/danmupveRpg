using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

public static class ETHandlerCancelMatchGame
{
    public static async ETVoid Request()
    {
        G2C_CancelMatchGame pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_CancelMatchGame()
        {

        }) as G2C_CancelMatchGame;

        if (pMsgRep.Error == ErrorCode.C_MatchGameFailed)
        {
            Debug.LogError("匹配失败");
            return;
        }
        else if (pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogError("已经在房间里");
            return;
        }

        Debug.Log("停止匹配");
        RefreshUI();
    }

    static void RefreshUI()
    {
        UINetMatch match = UIManager.Instance.GetUI(UIResType.ETNetMatch) as UINetMatch;

        match.OnCancelMatch();
        match.MatchSetEnemyInfo(null);
    }
}
