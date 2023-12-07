using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ETHandlerReqLogin
{
    public static async ETVoid Login(string userId, string nickName, string headIcon, int loginType)
    {
        //Debug.Log("服务器IP地址：" + );
        //创建Realm session

        string szGateUrl = CNetConfigMgr.Ins.GetETServerIP() + ":" + CNetConfigMgr.Ins.GetETServerPort();
        Debug.Log("URL:" + szGateUrl);
        Session sessionRealm = ETGame.Scene.GetComponent<NetOuterComponent>().Create(szGateUrl);
        sessionRealm.callErrorCall += ETNetSessionHelper.Inst.OnSessionCallError;

        Debug.Log("发起登录请求");

        //发送请求登录的消息
        R2C_Login msgLoginRealm = await sessionRealm.Call(new C2R_Login()
        {
            PlatformID = userId,
            LoginType = loginType
        }) as R2C_Login;

        sessionRealm.Dispose();

        if (msgLoginRealm.Error == ErrorCode.C_AccountOrPasswordError)
        {
            Debug.LogError("密码或者账号错误");
            return;
        }

        Debug.Log($"账号验证成功，Gate地址：{msgLoginRealm.Address}");
        Session sessionGate = ETGame.Scene.GetComponent<NetOuterComponent>().Create(msgLoginRealm.Address);
        if (SessionComponent.Instance == null)
        {
            //创建唯一Session
            ETGame.Scene.AddComponent<ETModel.SessionComponent>().Session = sessionGate;
        }
        else
        {
            SessionComponent.Instance.Session = sessionGate;
        }

        SessionComponent.Instance.Session.callErrorCall += ETNetSessionHelper.Inst.OnSessionCallError;
        //登录到网关服务器
        G2C_LoginGate msgLoginGate = await SessionComponent.Instance.Session.Call(new C2G_LoginGate()
        {
            Key = msgLoginRealm.Key,
            PlatformID = userId,
            NickName = nickName,
            HeadIcon = headIcon
        }) as G2C_LoginGate;

        //连接网关超时
        if (msgLoginGate.Error == ErrorCode.ERR_ConnectGateKeyError)
        {
            Debug.LogError("连接到网关服务器超时");
            ETNetSessionHelper.Inst.EventClearSession();
            return;
        }

        //用户失败
        if (msgLoginGate.UserInfo == null)
        {
            Debug.LogError("获取用户信息失败");
            ETNetSessionHelper.Inst.EventClearSession();
            return;
        }

        //登录成功后注册断开事件
        SessionComponent.Instance.Session.callDispose += ETNetSessionHelper.Inst.OnSessionDispose;

        //记录用户基本信息
        Debug.Log($"用户[{msgLoginGate.UserInfo.PlayerId}]登录成功");
        EUserInfo user = new EUserInfo();
        user.nUserId = msgLoginGate.UserInfo.PlayerId;
        user.szPlatformId = msgLoginGate.UserInfo.PlatformId;
        user.szNickName = msgLoginGate.UserInfo.NickName;
        user.szHeadIcon = msgLoginGate.UserInfo.Head;
        EUserInfoMgr.Ins.AddUser(user);
        EUserInfoMgr.Ins.pSelf = user;

        //调登录成功事件
        ETGame.EventSystem.Run(EventIdType.LoginFinish);
    }
}
