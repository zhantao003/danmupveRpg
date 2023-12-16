using BarrageGrab.ProtoEntity;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class ETHandlerReqStartMatchGame 
{
    public static async ETVoid Request()
    {
        G2C_StartMatchGame pMsgRep = await SessionComponent.Instance.Session.Call(new C2G_StartMatchGame()
        {

        }) as G2C_StartMatchGame;

        if(pMsgRep.Error == ErrorCode.C_PlayerAlreadyInRoom)
        {
            Debug.LogError("已经在房间里");
            return;
        }

        Debug.Log("开始匹配");
        RefreshUI();
    }

    static void RefreshUI()
    {
        UINetMatch match = UIManager.Instance.GetUI(UIResType.ETNetMatch) as UINetMatch;

        match.OnStartMatch();
        //UIIdleNetMatching uiETNet = UIIdleMainMenu.Instance.uiNetMathcing;
        //if (uiETNet == null) return;

        //uiETNet.objIdleBoard.SetActive(false);
        //uiETNet.objMacthingBoard.SetActive(true);
        //uiETNet.fMatchTime = 0f;
    }
}
