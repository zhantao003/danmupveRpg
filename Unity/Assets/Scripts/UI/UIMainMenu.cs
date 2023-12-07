using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIBase
{
    public InputField uiInputCode;
    public InputField uiInputRoomId;

    public Toggle uiCodeTypeTog;

    List<DelegateNFuncCall> listCallSuc = new List<DelegateNFuncCall>();

    public override void OnOpen()
    {
        uiInputRoomId.gameObject.SetActive(false);

        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            uiInputCode.text = CGlobalInit.Ins.szArgCode;

            string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
            if (!CHelpTools.IsStringEmptyOrNone(szPreCode))
            {
                uiInputCode.text = szPreCode;
            }
        }
        else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
        {
            uiInputCode.text = "";
        }
        else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS)
        {
            string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
            if (!CHelpTools.IsStringEmptyOrNone(szPreCode))
            {
                uiInputCode.text = szPreCode;
            }
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.KuaiShou)
        {
            uiInputCode.text = "";
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.QQNow)
        {
            uiInputCode.text = "";
            uiInputRoomId.gameObject.SetActive(true);
            uiInputRoomId.text = "";

            string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
            if (!CHelpTools.IsStringEmptyOrNone(szPreCode))
            {
                uiInputRoomId.text = szPreCode;
            }
        }
    }

    protected override void OnUpdate(float dt)
    {
        if(listCallSuc.Count > 0)
        {
            listCallSuc[0].Invoke();
            listCallSuc.RemoveAt(0);
        }
    }

    public void OnClickLogin()
    {
        UIManager.Instance.OpenUI(UIResType.NetWait);

        string szCode = uiInputCode.text;
        if (CHelpTools.IsStringEmptyOrNone(szCode))
        {
            if (!CDanmuSDKCenter.Ins.bDevType)
            {
                UIManager.Instance.CloseUI(UIResType.NetWait);
                UIToast.Show("请输入正确的身份码");
                return;
            }
        }

        string szRoomId = "";
        if (uiInputRoomId != null)
        {
            szRoomId = uiInputRoomId.text;
        }

        if (CDanmuSDKCenter.Ins.IsGaming())
        {
            CDanmuSDKCenter.Ins.EndGame(true, delegate ()
            {
                OnLogin(szCode, szRoomId);
            });
        }
        else
        {
            OnLogin(szCode, szRoomId);
        }
    }

    void OnLogin(string szCode, string szRoomId)
    {
        Debug.Log("Login====" + szCode);

        if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_Bilibili);
        }
        //else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
        //{
        //    CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_DouyinOpen);
        //}
        //else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS)
        //{
        //    CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_DouyinYs);
        //}
        //else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.KuaiShou)
        //{
        //    CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_KuaishouOpen);
        //}
        //else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.QQNow)
        //{
        //    CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_QQNow);
        //}
    }

    void OnLoginOver_Bilibili(int value)
    {
        if (value != 0)
        {
            if (value == 4000)
            {
                UIToast.Show("请输入正确的身份码");
            }
            else if (value == 7001)
            {
                UIToast.Show("连接冷却中，请稍后再试");
            }
            else if (value == 7002)
            {
                UIToast.Show("重复游戏，请重启程序");
            }
            else if (value == 7007)
            {
                UIToast.Show("身份码不正确");
            }
            else
            {
                UIToast.Show("连接弹幕服务器异常：" + value);
            }

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        if (CDanmuBilibiliMgr.Ins.bLocalTest)
        {
            CloseSelf();

            UIManager.Instance.CloseUI(UIResType.NetWait);
            UIManager.Instance.CloseUI(UIResType.MainMenu);
            UIManager.Instance.OpenUI(UIResType.Loading);
            //CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);
        }
        else
        {
            //保存code
            string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
            if (szPreCode == null ||
               !szPreCode.Equals(uiInputCode.text))
            {
                CSystemInfoMgr.Inst.SetVCode(uiInputCode.text);
                CSystemInfoMgr.Inst.SaveFile();
            }

            CHttpParam pParamLoginReq = new CHttpParam
            (
                new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szUid),
                new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
                new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
                new CHttpParamSlot("nickName", System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UName)),
                new CHttpParamSlot("headIcon", System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UFace)),
                new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
            );

            CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, pParamLoginReq);
        }
    }

    //void OnLoginOver_DouyinOpen(int value)
    //{
    //    if(value!=0)
    //    {
    //        if(value == 999)
    //        {
    //            UIToast.Show("获取Token异常：" + value);
    //        }
    //        else if (value == 99)
    //        {
    //            UIToast.Show("房间号不能为空：" + value);
    //        }
    //        else if(value == 101)
    //        {
    //            UIToast.Show("连接弹幕服务器异常：" + value);
    //        }
    //        else
    //        {
    //            UIToast.Show("连接服务器异常：" + value);
    //        }

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    CHttpParam pParamLoginReq = new CHttpParam
    //        (
    //            new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szRoomId),
    //            new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
    //            new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
    //            new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
    //            new CHttpParamSlot("headIcon", ""),
    //            new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
    //        );

    //    CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, pParamLoginReq);
    //}

    //void OnLoginOver_DouyinYs(int value)
    //{
    //    if (value != 0)
    //    {
    //        if (value == 99)
    //        {
    //            UIToast.Show("房间号不能为空：" + value);
    //        }
    //        else
    //        {
    //            UIToast.Show("连接服务器异常：" + value);
    //        }

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    CHttpParam pParamLoginReq = new CHttpParam
    //    (
    //        new CHttpParamSlot("uid", DouyinYSClient.Ins.szRoomID),
    //        new CHttpParamSlot("roomId", DouyinYSClient.Ins.szRoomID),
    //        new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
    //        new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
    //        new CHttpParamSlot("headIcon", ""),
    //        new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
    //    );

    //    CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, pParamLoginReq);
    //}

    //void OnLoginOver_KuaishouOpen(int value)
    //{
    //    if (value != 0)
    //    {
    //        UIToast.Show("连接服务器异常：" + value);

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    listCallSuc.Add(delegate ()
    //    {
    //        //此时非主线程不让调用一些接口
    //        CHttpParam pParamLoginReq = new CHttpParam
    //           (
    //               new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szUid),
    //               new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
    //               new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
    //               new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
    //               new CHttpParamSlot("headIcon", ""),
    //               new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
    //           );

    //        CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, pParamLoginReq);
    //    });
    //}

    //void OnLoginOver_QQNow(int value)
    //{
    //    if (value != 0)
    //    {
    //        UIToast.Show("连接服务器异常：" + value);

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    //保存code
    //    string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
    //    if (CHelpTools.IsStringEmptyOrNone(szPreCode) ||
    //       !szPreCode.Equals(CDanmuSDKCenter.Ins.szRoomId))
    //    {
    //        CSystemInfoMgr.Inst.SetVCode(CDanmuSDKCenter.Ins.szRoomId);
    //        CSystemInfoMgr.Inst.SaveFile();
    //    }

    //    //CHttpMgr.Instance.SendHttpMsg(CHttpConst.GetVersion, new HHandlerGetVersion());
    //}
}
