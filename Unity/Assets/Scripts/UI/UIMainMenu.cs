using DouyinDanmu;
using ILRuntime.Runtime;
using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainMenu : UIBase
{
    public InputField uiInputCode;
    public InputField uiInputRoomId;

    public Toggle uiCodeTypeTog;

    public GameObject objNormalLogo;
    public GameObject objDouYinLogo;

    List<DelegateNFuncCall> listCallSuc = new List<DelegateNFuncCall>();

    public static int nAutoLoginCount = 1;

    protected override void OnStart()
    {
        if (nAutoLoginCount > 0)
        {
            nAutoLoginCount--;
            return;
        }
        
        //发送传参Token
        if (!CHelpTools.IsStringEmptyOrNone(CGlobalInit.Ins.szArgToken))
        {
            UIManager.Instance.OpenUI(UIResType.NetWait);

            //自动登录
            string szCode = SystemInfo.deviceUniqueIdentifier;

            if (CDanmuSDKCenter.Ins.IsGaming())
            {
                CDanmuSDKCenter.Ins.EndGame(true, delegate ()
                {
                    OnAutoLogin(szCode, CGlobalInit.Ins.szArgToken);
                });
            }
            else
            {
                OnAutoLogin(szCode, CGlobalInit.Ins.szArgToken);
            }
        }
    }

    public override void OnOpen()
    {
        //float m_fFrameLen = (float)CLockStepData.g_fixFrameLen;
        //Debug.Log("FrameTime:" + m_fFrameLen);

        uiInputRoomId.gameObject.SetActive(false);

        objDouYinLogo.SetActive(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen ||
            CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS);
        objNormalLogo.SetActive(CDanmuSDKCenter.Ins.emPlatform != CDanmuSDKCenter.EMPlatform.DouyinOpen &&
           CDanmuSDKCenter.Ins.emPlatform != CDanmuSDKCenter.EMPlatform.DouyinYS);

        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            uiInputCode.text = CGlobalInit.Ins.szArgCode;

            string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
            if (!CHelpTools.IsStringEmptyOrNone(szPreCode))
            {
                uiInputCode.text = szPreCode;
            }
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
        {
            uiInputCode.text = "";
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS)
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
        else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            uiInputCode.text = "";
        }
    }

    protected override void OnUpdate(float dt)
    {
        if (listCallSuc.Count > 0)
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
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "inputcode"));
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
        //CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);

        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            Debug.LogWarning("Login To Platform Code：" + szCode);
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_Bilibili);
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_DouyinOpen);
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_DouyinYs);
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.KuaiShou)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_KuaishouOpen);
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.QQNow)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szRoomId, OnLoginOver_QQNow);
        }
        else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            CDanmuSDKCenter.Ins.Login(szCode, szCode, OnLoginOver_YYOpen);
        }
    }

    void OnAutoLogin(string szCode, string token)
    {
        if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
        {
            CDanmuSDKCenter.Ins.AutoLogin(szCode, token, delegate (int value)
            {
                uiInputCode.text = CDanmuSDKCenter.Ins.szUid;

                OnLoginOver_DouyinOpen(value);
            });
        }
        else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            CDanmuSDKCenter.Ins.Login(szCode, token, delegate (int value) {
                uiInputCode.text = token;

                OnLoginOver_YYOpen(value);
            });
        }
    }

    void OnLoginOver_Bilibili(int value)
    {
        if (value != 0)
        {
            if (value == 4000)
            {
                //CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "matchsuc")
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "inputcode"));
            }
            else if (value == 7001)
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "connectwait"));
            }
            else if (value == 7002)
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "regame"));
            }
            else if (value == 7007)
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "errorcode"));
            }
            else
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "connecterror") + value);
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
            Debug.Log(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UFace);

            AuthenticationRequest request = new AuthenticationRequest(CDanmuSDKCenter.Ins.szRoomId, System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UName),
                System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UFace), "0.0.0", CDanmuSDKCenter.Ins.szUid);
            //CHttpParam pParamLoginReq = new CHttpParam
            //(
            //    new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szUid),
            //    new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
            //    new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
            //    new CHttpParamSlot("nickName", System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UName)),
            //    new CHttpParamSlot("headIcon", System.Net.WebUtility.UrlEncode(CDanmuBilibiliMgr.Ins.gameAnchorInfo.UFace)),
            //    new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
            //);

            CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, request.GetJsonMsg().GetData());
        }
    }

    void OnLoginOver_DouyinOpen(int value)
    {
        if (value != 0)
        {
            //if (value == 999)
            //{
            //    UIToast.Show("获取Token异常：" + value);
            //}
            //else if (value == 99)
            //{
            //    UIToast.Show("房间号不能为空：" + value);
            //}
            //else if (value == 101)
            //{
            //    UIToast.Show("连接弹幕服务器异常：" + value);
            //}
            //else
            //{
            //    UIToast.Show("连接服务器异常：" + value);
            //}

            UIManager.Instance.OpenUI(UIResType.NetError);

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        AuthenticationRequest request = new AuthenticationRequest(
            CDanmuSDKCenter.Ins.szRoomId,
            CDanmuSDKCenter.Ins.szRoomId,
             "",
              "0.0.0",
              CDanmuSDKCenter.Ins.szRoomId);

        //CHttpParam pParamLoginReq = new CHttpParam
        //    (
        //        new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szRoomId),
        //        new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
        //        new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
        //        new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
        //        new CHttpParamSlot("headIcon", ""),
        //        new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
        //    );

        CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, request.GetJsonMsg().GetData());
    }

    void OnLoginOver_DouyinYs(int value)
    {
        if (value != 0)
        {
            if (value == 99)
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "codenull") + value);
            }
            else
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "connecterror2") + value);
            }

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        AuthenticationRequest request = new AuthenticationRequest(
            CDanmuSDKCenter.Ins.szRoomId,
          CDanmuSDKCenter.Ins.szRoomId,
           "",
             "0.0.0",
            DouyinYSClient.Ins.szRoomID);

        //CHttpParam pParamLoginReq = new CHttpParam
        //(
        //    new CHttpParamSlot("uid", DouyinYSClient.Ins.szRoomID),
        //    new CHttpParamSlot("roomId", DouyinYSClient.Ins.szRoomID),
        //    new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
        //    new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
        //    new CHttpParamSlot("headIcon", ""),
        //    new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
        //);

        CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, request.GetJsonMsg().GetData());
    }

    void OnLoginOver_KuaishouOpen(int value)
    {
        if (value != 0)
        {
            UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "connecterror2") + value);

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        listCallSuc.Add(delegate ()
        {
            AuthenticationRequest request = new AuthenticationRequest(
      CDanmuSDKCenter.Ins.szRoomId,
    CDanmuSDKCenter.Ins.szRoomId,
     "",
       "0.0.0",
     CDanmuSDKCenter.Ins.szUid);

            //此时非主线程不让调用一些接口
            //CHttpParam pParamLoginReq = new CHttpParam
            //   (
            //       new CHttpParamSlot("uid", CDanmuSDKCenter.Ins.szUid),
            //       new CHttpParamSlot("roomId", CDanmuSDKCenter.Ins.szRoomId),
            //       new CHttpParamSlot("channel", CDanmuSDKCenter.Ins.emPlatform.ToString().ToLower()),
            //       new CHttpParamSlot("nickName", CDanmuSDKCenter.Ins.szRoomId),
            //       new CHttpParamSlot("headIcon", ""),
            //       new CHttpParamSlot("userType", ((int)CPlayerBaseInfo.EMUserType.Zhubo).ToString())
            //   );

            CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, request.GetJsonMsg().GetData());
        });
    }

    void OnLoginOver_QQNow(int value)
    {
        if (value != 0)
        {
            UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "connecterror2") + value);

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        //保存code
        string szPreCode = CSystemInfoMgr.Inst.GetString(CSystemInfoConst.VUTBERCODE);
        if (CHelpTools.IsStringEmptyOrNone(szPreCode) ||
           !szPreCode.Equals(CDanmuSDKCenter.Ins.szRoomId))
        {
            CSystemInfoMgr.Inst.SetVCode(CDanmuSDKCenter.Ins.szRoomId);
            CSystemInfoMgr.Inst.SaveFile();
        }

        //CHttpMgr.Instance.SendHttpMsg(CHttpConst.GetVersion, new HHandlerGetVersion());
    }

    void OnLoginOver_YYOpen(int value)
    {
        if (value != 0)
        {
            UIManager.Instance.OpenUI(UIResType.NetError);

            UIManager.Instance.CloseUI(UIResType.NetWait);
            return;
        }

        AuthenticationRequest request = new AuthenticationRequest(
            CDanmuSDKCenter.Ins.szRoomId,
            CDanmuSDKCenter.Ins.szRoomId,
             "",
              "0.0.0",
              CDanmuSDKCenter.Ins.szRoomId);


        CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginVtuber, request.GetJsonMsg().GetData());
    }
}
