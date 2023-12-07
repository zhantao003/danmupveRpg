using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HHandlerGetGameInfo : INetEventHandler
{
    public OnDeleagteLoginByTypeSuc callLoginOver;

    public HHandlerGetGameInfo(OnDeleagteLoginByTypeSuc call = null)
    {
        callLoginOver = call;
    }

    public void OnErrorCode(CLocalNetMsg pMsg)
    {
        
    }

    public void OnMsgHandler(CLocalNetMsg pMsg)
    {
        string uid = pMsg.GetString("uid");
        long gameCoin = pMsg.GetLong("gameCoin");
        long avatarCount = pMsg.GetLong("avatarCount");

        CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(uid);
        if(pPlayer == null)
        {
            return;
        }

        pPlayer.nGameCoins = gameCoin;

        //继续获取装备的Avatar信息
        CHttpParam pParamReq = new CHttpParam
        (
            new CHttpParamSlot("uid", uid),
            new CHttpParamSlot("roomId", CDanmuBilibiliMgr.Ins.lRoomID.ToString())
        );

        //CHttpMgr.Instance.SendHttpMsg(CHttpConst.GetEquipAvatar, new HHandlerGetEquipAvatarList(this.OnGetInfoSuc), pParamReq);
    }

    void OnGetInfoSuc(CPlayerBaseInfo player)
    {
        callLoginOver?.Invoke(player);
    }
}
