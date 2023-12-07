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
        Debug.Log("������¼��Ϣ��" + pMsg.GetData());

        //��¼���紫�ιؼ���Ϣ
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
