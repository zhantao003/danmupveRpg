using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CHttpEvent(CHttpConst.LoginVtuber)]
public class HHandlerLoginVtb : INetEventHandler
{
    public void OnErrorCode(CLocalNetMsg pMsg)
    {
        
    }

    public void OnMsgHandler(CLocalNetMsg pMsg)
    {
        Debug.Log("主播登录信息：" + pMsg.GetData());

        //记录网络传参关键信息
        CHttpMgr.Instance.szUserId = CDanmuSDKCenter.Ins.szRoomId;
        CHttpMgr.Instance.szToken = pMsg.GetString("token");

        LoginOver();
    }

    void LoginOver()
    {
        UIManager.Instance.CloseUI(UIResType.NetWait);
        UIManager.Instance.OpenUI(UIResType.Loading);
        CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);
    }
}
