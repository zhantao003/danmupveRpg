using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HHandlerJoinGame : INetEventHandler
{
    public string szName;
    public string szIcon;

    public OnDeleagteLoginByTypeSuc callLoginOver;
    public string szNextHttpReq;
    public CHttpParam pNextParamReq;

    public HHandlerJoinGame(string name, string icon, string nextReq = "", CHttpParam nextParams = null, OnDeleagteLoginByTypeSuc call = null)
    {
        szName = name;
        szIcon = icon;

        callLoginOver = call;
        szNextHttpReq = nextReq;
        pNextParamReq = nextParams;
    }

    public void OnErrorCode(CLocalNetMsg pMsg)
    {
        
    }

    public void OnMsgHandler(CLocalNetMsg pMsg)
    {
        string uid = pMsg.GetString("uid");
        if (string.IsNullOrEmpty(uid)) return;

        string szToken = pMsg.GetString("token");
        //string szToken = pMsg.GetString("token");
        long fansMedalLv = pMsg.GetLong("fansMedalLevel");
        string fansMedalName = pMsg.GetString("fansMedalName");
        bool fansMedalWear = pMsg.GetBool("fansMedalWearingStatus");
        long guardLv = pMsg.GetLong("guardLevel");

        CPlayerBaseInfo pPlayerInfo = CPlayerMgr.Ins.GetPlayer(uid);
        if (pPlayerInfo == null)
        {
            pPlayerInfo = new CPlayerBaseInfo(uid, szName, szIcon,
                                              fansMedalLv, fansMedalName, fansMedalWear,
                                              guardLv, CDanmuSDKCenter.Ins.szRoomId, CPlayerBaseInfo.EMUserType.Guanzhong);

            CPlayerMgr.Ins.AddPlayer(pPlayerInfo);
        }
        else
        {
            pPlayerInfo.userName = szName;
            pPlayerInfo.userFace = szIcon;
            pPlayerInfo.fansMedalLevel = fansMedalLv;
            pPlayerInfo.fansMedalName = fansMedalName;
            pPlayerInfo.fansMedalWearingStatus = fansMedalWear;
            pPlayerInfo.guardLevel = guardLv;
            pPlayerInfo.roomId = CDanmuBilibiliMgr.Ins.lRoomID;
        }

        CHttpParam pParamReq = new CHttpParam
        (
            new CHttpParamSlot("uid", uid),
            new CHttpParamSlot("roomId", CDanmuBilibiliMgr.Ins.lRoomID.ToString())
        );

        CHttpMgr.Instance.SendHttpMsg(CHttpConst.GetGameInfo, new HHandlerGetGameInfo(this.OnLoginOver), pParamReq);
    }

    void OnLoginOver(CPlayerBaseInfo player)
    {
        callLoginOver?.Invoke(player);

        if (!CHelpTools.IsStringEmptyOrNone(szNextHttpReq))
        {
            CHttpMgr.Instance.SendHttpMsg(szNextHttpReq, pNextParamReq);
        }
    }
}
