using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuLikeAttrite(CDanmuLikeConst.Like)]
public class DLikeCommon : CDanmuLikeAction
{
    public override void DoAction(CDanmuLike dm)
    {
        CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(dm.uid);
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (pPlayer == null)
            {
                //CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, 0, -1,
                //delegate ()
                //{
                //    CreateUnitLocal(CPlayerMgr.Ins.GetPlayer(dm.uid), dm.likeNum);
                //});
            }
            else
            {
                CreateUnitLocal(pPlayer, dm.likeNum);
            }
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if (CBattleMgr.Ins != null &&
                     CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                return;
            if (pPlayer == null)
            {
                CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, 0,
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
                   CreateUnitNet(baseInfo, dm.likeNum, true);
               });
            }
            else
            {
                CreateUnitNet(pPlayer, dm.likeNum, false);
            }
        }
    }
     
    void CreateUnitLocal(CPlayerBaseInfo player,long likecount)
    {
        //还未选阵营
        if (player.emCamp == EMUnitCamp.Max) return;

        if (CGameAntGlobalMgr.Ins.emGameType != CGameAntGlobalMgr.EMGameType.LocalPvP) return;

        if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming) return;

        if (CSceneMgr.Instance.m_objCurScene.emSceneType != CSceneFactory.EMSceneType.GameMap101) return;

        //CBattleMgr.Ins.AddLikesPlayerInfo(player);

        ///发送基地经验增加的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", (int)player.emCamp);
        msg.SetInt("exp", 1);
        CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);

        ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv1);

        long nlBasenum = CGameAntGlobalMgr.Ins.GetFreeCreatCount();// 2;

        CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
        pLSEvent.msgParams.SetString("uid", player.uid);
        pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
        pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
        pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
        pLSEvent.msgParams.SetString("nickname", player.userName);
        pLSEvent.msgParams.SetString("headIcon", player.userFace);
        pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
        pLSEvent.msgParams.SetLong("num", nlBasenum);

        CLockStepMgr.Ins.AddLSEvent(pLSEvent);

        if (player.emCamp == EMUnitCamp.Red)
        {
            switch (player.emPathType)
            {
                case EMStayPathType.Up:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bUpDead)
                            return;
                    }
                    break;
                case EMStayPathType.Center:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bCenterDead)
                            return;
                    }
                    break;
                case EMStayPathType.Down:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bDownDead)
                            return;
                    }
                    break;
            }
        }
        else if (player.emCamp == EMUnitCamp.Blue)
        {
            switch (player.emPathType)
            {
                case EMStayPathType.Up:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bUpDead)
                            return;
                    }
                    break;
                case EMStayPathType.Center:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bCenterDead)
                            return;
                    }
                    break;
                case EMStayPathType.Down:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bDownDead)
                            return;
                    }
                    break;
            }
        }

        player.nCurLikeCount++;
        if (player.nCurLikeCount >= player.nMaxLikeCount)
        {
            ST_UnitBattleInfo pTBLInfo2 = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv2);
            CLockStepEvent_CreateManySoldier pLSEvent2 = new CLockStepEvent_CreateManySoldier();
            pLSEvent2.msgParams.SetString("uid", player.uid);
            pLSEvent2.msgParams.SetInt("camp", (int)player.emCamp);
            pLSEvent2.msgParams.SetInt("path", (int)player.emPathType);
            pLSEvent2.msgParams.SetInt("tblId", pTBLInfo2.nID);
            pLSEvent2.msgParams.SetString("nickname", player.userName);
            pLSEvent2.msgParams.SetString("headIcon", player.userFace);
            pLSEvent2.msgParams.SetLong("vipLv", player.guardLevel);
            pLSEvent2.msgParams.SetLong("num", 2);

            CLockStepMgr.Ins.AddLSEvent(pLSEvent2);
            player.nCurLikeCount = 0;
        }
    }

    void CreateUnitNet(CPlayerBaseInfo player, long likecount, bool joinPlayer)
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
        long nlBasenum = CGameAntGlobalMgr.Ins.GetFreeCreatCount();//2;
        DLockStepFrameEvent pEventCreateSoldier = new DLockStepFrameEvent();
        pEventCreateSoldier.EventId = (int)EMLockStepEventType.CreateSoldier;
        pEventCreateSoldier.Camp = (int)player.emCamp;
        pEventCreateSoldier.Path = (int)player.emPathType;

        ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv1);
        pEventCreateSoldier.Tbid = pTBLInfo.nID;

        pEventCreateSoldier.NickName = player.userName;
        pEventCreateSoldier.HeadIcon = player.userFace;
        pEventCreateSoldier.Uid = player.uid;
        pEventCreateSoldier.Num = nlBasenum * likecount;

        listReqEvent.Add(pEventCreateSoldier);

        //判断是否添加高级兵
        bool bAddLv2Soldier = true;
        if (player.emCamp == EMUnitCamp.Red)
        {
            switch (player.emPathType)
            {
                case EMStayPathType.Up:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bUpDead)
                            bAddLv2Soldier = false;
                    }
                    break;
                case EMStayPathType.Center:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bCenterDead)
                            bAddLv2Soldier = false;
                    }
                    break;
                case EMStayPathType.Down:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.bDownDead)
                            bAddLv2Soldier = false;
                    }
                    break;
            }
        }
        else if (player.emCamp == EMUnitCamp.Blue)
        {
            switch (player.emPathType)
            {
                case EMStayPathType.Up:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bUpDead)
                            bAddLv2Soldier = false;
                    }
                    break;
                case EMStayPathType.Center:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bCenterDead)
                            bAddLv2Soldier = false;
                    }
                    break;
                case EMStayPathType.Down:
                    {
                        if (!CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.bDownDead)
                            bAddLv2Soldier = false;
                    }
                    break;
            }
        }

        if (bAddLv2Soldier)
        {
            //生成弓箭兵
            player.nCurLikeCount++;
            if (player.nCurLikeCount >= player.nMaxLikeCount)
            {
                ST_UnitBattleInfo pTBLInfo2 = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv2);

                DLockStepFrameEvent pEventCreateSoldierLv2 = new DLockStepFrameEvent();
                pEventCreateSoldierLv2.EventId = (int)EMLockStepEventType.CreateSoldier;
                pEventCreateSoldierLv2.Camp = (int)player.emCamp;
                pEventCreateSoldierLv2.Path = (int)player.emPathType;
                pEventCreateSoldierLv2.Tbid = pTBLInfo2.nID;
                pEventCreateSoldierLv2.NickName = player.userName;
                pEventCreateSoldierLv2.HeadIcon = player.userFace;
                pEventCreateSoldierLv2.Uid = player.uid;
                pEventCreateSoldierLv2.Num = 2;

                listReqEvent.Add(pEventCreateSoldierLv2);

                player.nCurLikeCount = 0;
            }
        }

        ETHandlerReqLockStepEvent.Request(listReqEvent.ToArray()).Coroutine();
    }
}
