using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CPlayerNetHelper
{
    public static void Login(string uid, string nickname, string headIcon, long vipLv, DelegateNFuncCall call = null)
    {
        GetPlayerInfoRequest request = new GetPlayerInfoRequest(uid, nickname, headIcon, vipLv);
        CHttpMgr.Instance.SendHttpMsg(CHttpConst.LoginViewer, request.GetJsonMsg().GetData(), new HHandlerJoinGame(call), true);
    }

    public static void PlayerSpending(string upId, string uid, string cName, string faceUrl, long spend)
    {
        PlayerSpendingRequest request = new PlayerSpendingRequest(upId, uid, cName, faceUrl, spend);
        CHttpMgr.Instance.SendHttpMsg(CHttpConst.PlayerSpending, request.GetJsonMsg().GetData(), true, 10, true);
    }

    public static void PlayerGetPoints(string uid,long gain) {
        PlayerGetPointsRequest request = new PlayerGetPointsRequest(uid, gain);
        CHttpMgr.Instance.SendHttpMsg(CHttpConst.PlayerGetPoints, request.GetJsonMsg().GetData(), true, 10, true);
    }

    /// <summary>
    /// 统计礼物数据
    /// </summary>
    public static void RecordVtuberGift(string gameName, string version, string platform, string roomId, string nickName, long price)
    {
        CHttpParam pParams = new CHttpParam();
        pParams.AddSlot(new CHttpParamSlot("game", gameName));
        pParams.AddSlot(new CHttpParamSlot("version", version));
        pParams.AddSlot(new CHttpParamSlot("platform", platform));
        pParams.AddSlot(new CHttpParamSlot("room", roomId));
        pParams.AddSlot(new CHttpParamSlot("price", price.ToString()));
        pParams.AddSlot(new CHttpParamSlot("nickname", nickName));

        CHttpMgr.Instance.SendHttpMsgWWWForms("http://api.jh.biggersun.com", 0, CHttpRecordConst.AddGift, pParams, 10);
    }
}
