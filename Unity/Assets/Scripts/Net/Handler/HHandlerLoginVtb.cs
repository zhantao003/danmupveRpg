using DouyinDanmu;
using SharedLibrary;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CHttpEvent(CHttpConst.LoginVtuber)]
public class HHandlerLoginVtb : INetEventHandler
{
    public void OnErrorCode(string failReason)
    {

    }

    //public void OnMsgHandler(CLocalNetMsg pMsg)
    //{
    //    Debug.Log("主播登录信息：" + pMsg.GetData());

    //    //记录网络传参关键信息
    //    CHttpMgr.Instance.szUserId = CDanmuSDKCenter.Ins.szRoomId;
    //    CHttpMgr.Instance.szToken = pMsg.GetString("token");

    //    LoginOver();
    //}

    public void OnMsgHandler(string resPonseJson)
    {
        AuthenticationResponse resPonse = new AuthenticationResponse();
        resPonse.FillDatas(resPonseJson);
        //Debug.Log(resPonseJson);
        //Debug.Log(CJsonConver.SerializeObject(resPonse));
        CHttpMgr.Instance.szUserId = CDanmuSDKCenter.Ins.szRoomId;
        CHttpMgr.Instance.szToken = resPonse.Token;
        CHttpMgr.Instance.serverUTCBase = resPonse.ServerUTCTime;
        CHttpMgr.Instance.localTimeBase = DateTime.Now;
        CEncryptHelper.SHA256KEY = resPonse.Key;
        //Debug.Log("SHA256KEY" + resPonse.Key);
        LoginOver();
    }

    void LoginOver()
    {
        CHeartBeatInit.Ins.isLogin = true;
        UIManager.Instance.CloseUI(UIResType.NetWait);
        UIManager.Instance.OpenUI(UIResType.Loading);
        CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameModeSelect102);
    }
}
