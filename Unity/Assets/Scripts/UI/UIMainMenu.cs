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
                UIToast.Show("��������ȷ�������");
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
                UIToast.Show("��������ȷ�������");
            }
            else if (value == 7001)
            {
                UIToast.Show("������ȴ�У����Ժ�����");
            }
            else if (value == 7002)
            {
                UIToast.Show("�ظ���Ϸ������������");
            }
            else if (value == 7007)
            {
                UIToast.Show("����벻��ȷ");
            }
            else
            {
                UIToast.Show("���ӵ�Ļ�������쳣��" + value);
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
            //����code
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
    //            UIToast.Show("��ȡToken�쳣��" + value);
    //        }
    //        else if (value == 99)
    //        {
    //            UIToast.Show("����Ų���Ϊ�գ�" + value);
    //        }
    //        else if(value == 101)
    //        {
    //            UIToast.Show("���ӵ�Ļ�������쳣��" + value);
    //        }
    //        else
    //        {
    //            UIToast.Show("���ӷ������쳣��" + value);
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
    //            UIToast.Show("����Ų���Ϊ�գ�" + value);
    //        }
    //        else
    //        {
    //            UIToast.Show("���ӷ������쳣��" + value);
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
    //        UIToast.Show("���ӷ������쳣��" + value);

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    listCallSuc.Add(delegate ()
    //    {
    //        //��ʱ�����̲߳��õ���һЩ�ӿ�
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
    //        UIToast.Show("���ӷ������쳣��" + value);

    //        UIManager.Instance.CloseUI(UIResType.NetWait);
    //        return;
    //    }

    //    //����code
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
