using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.Common_IdleUnitDialog)]
public class DCmdCommon : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        if (dm.content.Equals("666"))
            return;
        CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(dm.uid);
        //if (pPlayer == null)
        //{
        //    //CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, dm.vipLv, -1,
        //    //delegate ()
        //    //{
        //    //    CreateUnit(CPlayerMgr.Ins.GetPlayer(dm.uid));
        //    //});
        //}
        //else
        //{
        //    CreateUnit(pPlayer);
        //}

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (pPlayer == null)
            {
                return;
            }
            else
            {
                CreateUnit(pPlayer);
            }
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if (CBattleMgr.Ins != null &&
                    CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                return;
            if (pPlayer == null)
            {
                CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, dm.vipLv,
                                                 EUserInfoMgr.Ins.emSelfCamp == EMUnitCamp.Red ? 0 : 1,
                delegate ()
                {
                    CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(dm.uid);
                    if (baseInfo != null &&
                        baseInfo.emCamp == EMUnitCamp.Max)
                    {
                        baseInfo.emCamp = EUserInfoMgr.Ins.emSelfCamp;
                    }

                    //发兵
                    CreateUnitNet(baseInfo, true);
                });
            }
            else
            {
                CreateUnitNet(pPlayer, false);
            }
        }
    }

    void CreateUnit(CPlayerBaseInfo player)
    {
        //还未选阵营
        if (player == null ||
            player.emCamp == EMUnitCamp.Max) return;

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101)
            {
                if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                    return;
                ///发送基地经验增加的监听事件
                CLocalNetMsg msg = new CLocalNetMsg();
                msg.SetInt("camp", (int)player.emCamp);
                msg.SetInt("exp", 1);
                CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);

                ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv1);

                CLockStepEvent_CreateSoldier pLSEvent = new CLockStepEvent_CreateSoldier();
                pLSEvent.msgParams.SetString("uid", player.uid);
                pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                pLSEvent.msgParams.SetString("nickname", player.userName);
                pLSEvent.msgParams.SetString("headIcon", player.userFace);
                pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);

                CLockStepMgr.Ins.AddLSEvent(pLSEvent);
            }
        }
    }

    void CreateUnitNet(CPlayerBaseInfo player, bool joinPlayer)
    {
        //if (CSceneMgr.Instance.m_objCurScene.emSceneType != CSceneFactory.EMSceneType.GameMap101Net)
        //    return;

        //if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
        //    return;

        List<DLockStepFrameEvent> listReqEvent = new List<DLockStepFrameEvent>();

        //是否添加加入玩家消息
        if (joinPlayer)
        {
            DLockStepFrameEvent pEventJoinPlayer = new DLockStepFrameEvent();
            pEventJoinPlayer.EventId = (int)EMLockStepEventType.JoinPlayer;
            pEventJoinPlayer.Camp = (int)player.emCamp;
            pEventJoinPlayer.Path = (int)player.emPathType;
            pEventJoinPlayer.NickName = player.userName;
            pEventJoinPlayer.HeadIcon = player.userFace;
            pEventJoinPlayer.Uid = player.uid;

            //这个记录玩家的世界排名
            pEventJoinPlayer.Num = player.nWorldRank;
            pEventJoinPlayer.TotalWin = player.nWinTimes;
            pEventJoinPlayer.TotalExp = player.nTotalExp;

            listReqEvent.Add(pEventJoinPlayer);
        }

        //加经验的消息
        DLockStepFrameEvent pEventAddExp = new DLockStepFrameEvent();
        pEventAddExp.EventId = (int)EMLockStepEventType.AddBaseExp;
        pEventAddExp.Camp = (int)player.emCamp;
        pEventAddExp.Num = 1;

        listReqEvent.Add(pEventAddExp);

        //添加小兵事件
        long nlBasenum = 1;//2;
        DLockStepFrameEvent pEventCreateSoldier = new DLockStepFrameEvent();
        pEventCreateSoldier.EventId = (int)EMLockStepEventType.CreateSoldier;
        pEventCreateSoldier.Camp = (int)player.emCamp;
        pEventCreateSoldier.Path = (int)player.emPathType;

        ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv1);
        pEventCreateSoldier.Tbid = pTBLInfo.nID;

        pEventCreateSoldier.NickName = player.userName;
        pEventCreateSoldier.HeadIcon = player.userFace;
        pEventCreateSoldier.Uid = player.uid;
        pEventCreateSoldier.Num = nlBasenum;

        listReqEvent.Add(pEventCreateSoldier);

        ETHandlerReqLockStepEvent.Request(listReqEvent.ToArray()).Coroutine();
    }
}

