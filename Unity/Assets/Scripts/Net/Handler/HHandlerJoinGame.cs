using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SharedLibrary;

public class HHandlerJoinGame : INetEventHandler
{
    DelegateNFuncCall call;
    public HHandlerJoinGame(DelegateNFuncCall call = null) {
        this.call = call;
    }

    public void OnMsgHandler(string resPonseJson)
    {
        GetPlayerInfoResponse response = new GetPlayerInfoResponse();
        if (CHelpTools.IsStringEmptyOrNone(resPonseJson))
        {
            //Debug.Log("Responese  Json String is Null");
            return;
        }
        response.FillDatas(resPonseJson);
        if (response == null ||
            response.PlayerInRoom == null ||
            response.PlayerInRoom.Player == null
            )
        {
            //Debug.Log("Responese  Info is Null");
            return;
        }
        CPlayerBaseInfo pPlayer = new CPlayerBaseInfo(response.PlayerInRoom.Player.UId, response.Name, response.FaceUrl, 0, "", true, response.PlayerIdentity,
                                                      CDanmuSDKCenter.Ins.szRoomId,
                                                      CPlayerBaseInfo.EMUserType.Guanzhong, response.PlayerInRoom.Exp,response.PlayerInRoom.WorldRank,response.PlayerInRoom.WinTimes);
        
        if (CGameAntGlobalMgr.Ins.leftPlayerUids.ContainsKey(response.PlayerInRoom.Player.UId))
        {
            pPlayer.emCamp = EMUnitCamp.Red;
            CGameAntGlobalMgr.Ins.leftPlayerUids.Remove(response.PlayerInRoom.Player.UId);
        }   
        else if(CGameAntGlobalMgr.Ins.rightPlayerUids.ContainsKey(response.PlayerInRoom.Player.UId))
        {
            pPlayer.emCamp = EMUnitCamp.Blue;
            CGameAntGlobalMgr.Ins.rightPlayerUids.Remove(response.PlayerInRoom.Player.UId);
        }   
        else
        {
            if(CBattleMgr.Ins.mapMgr.pRedBase.TotalBattleValue < CBattleMgr.Ins.mapMgr.pBlueBase.TotalBattleValue)
            {
                pPlayer.emCamp = EMUnitCamp.Red;
            }
            else
            {
                pPlayer.emCamp = EMUnitCamp.Blue;
            }
        }
        CPlayerMgr.Ins.AddPlayer(pPlayer);
        ///默认中路加入
        pPlayer.emPathType = EMStayPathType.Center;
        if (pPlayer.emCamp == EMUnitCamp.Red)
        {
            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左玩家加入, new IIPlayerJoin(pPlayer));
        }
        else if (pPlayer.emCamp == EMUnitCamp.Blue)
        {
            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右玩家加入, new IIPlayerJoin(pPlayer));
        }

        call?.Invoke();
    }

    public void OnErrorCode(string failReason)
    {

    }
}
