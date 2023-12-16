using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HHandlerGetGameResult : INetEventHandler
{
    public void OnMsgHandler(string resPonseJson)
    {
        GameResultResponse resPonse = new GameResultResponse();
        resPonse.FillDatas(resPonseJson);

        //Debug.Log(resPonseJson);

     
        UIGameEndResult uiResult = UIManager.Instance.GetUI(UIResType.GameEndResult) as UIGameEndResult;
        uiResult.GameEndResponse(resPonse);

    }
    public void OnErrorCode(string failReason)
    {
        throw new System.NotImplementedException();
    }


    //public void OnMsgHandler(CLocalNetMsg pMsg)
    //{
    //    string uid = pMsg.GetString("uid");
    //    long gameCoin = pMsg.GetLong("gameCoin");
    //    long avatarCount = pMsg.GetLong("avatarCount");

    //    CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(uid);
    //    if (pPlayer == null)
    //    {
    //        return;
    //    }

    //    pPlayer.nGameCoins = gameCoin;

    //    //继续获取装备的Avatar信息
    //    CHttpParam pParamReq = new CHttpParam
    //    (
    //        new CHttpParamSlot("uid", uid),
    //        new CHttpParamSlot("roomId", CDanmuBilibiliMgr.Ins.lRoomID.ToString())
    //    );

    //    //CHttpMgr.Instance.SendHttpMsg(CHttpConst.GetEquipAvatar, new HHandlerGetEquipAvatarList(this.OnGetInfoSuc), pParamReq);
    //}



    //void OnGetInfoSuc(CPlayerBaseInfo player)
    //{
    //    callLoginOver?.Invoke(player);
    //}
}
