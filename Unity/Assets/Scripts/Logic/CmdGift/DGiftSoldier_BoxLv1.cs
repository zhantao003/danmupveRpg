using ETModel;
using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuGiftAttrite(CDanmuGiftConst.Soldier_BoxLv1)]
public class DGiftSoldier_BoxLv1 : CDanmuGiftAction
{
    public override void DoAction(CDanmuGift dm)
    {
        long num = dm.giftNum;
        long price = dm.price;

        CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(dm.uid);
        if (pPlayer == null)
        {
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
            {
                CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, dm.vipLv, -1,
                delegate ()
                {
                    CreateUnit(CPlayerMgr.Ins.GetPlayer(dm.uid), num, price, true);
                });
            }
            else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            {
                if (CBattleMgr.Ins != null &&
                    CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                    return;
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
                    CreateUnit(CPlayerMgr.Ins.GetPlayer(dm.uid), num, price, true);
                });
            }
        }
        else
        {
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            {
                if (CBattleMgr.Ins != null &&
                    CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                    return;
            }
            CreateUnit(pPlayer, num, price, false);
        }
    }

    void CreateUnit(CPlayerBaseInfo player, long num, long price,bool joinPlayer)
    {
        //还未选阵营
        if (player == null ||
            player.emCamp == EMUnitCamp.Max) return;

        long creatNum = num;
        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101)
            {  
             
                ///发送基地经验增加的监听事件
                CLocalNetMsg msg = new CLocalNetMsg();
                msg.SetInt("camp", (int)player.emCamp);
                msg.SetInt("exp", (int)price);
                CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);

                ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv3);
              
                switch (pTBLInfo.nID)
                {
                    case 201:
                        {
                            creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider6");
                        }
                        break;
                    case 202:
                        {
                            creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider7");
                        }
                        break;
                    case 203:
                        {
                            creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8");
                        }
                        break;
                    case 204:
                        {
                            creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider5");
                        }
                        break;
                }
                CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                pLSEvent.msgParams.SetString("uid", player.uid);
                pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                pLSEvent.msgParams.SetString("nickname", player.userName);
                pLSEvent.msgParams.SetString("headIcon", player.userFace);
                pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                pLSEvent.msgParams.SetLong("num", creatNum);

                CLockStepMgr.Ins.AddLSEvent(pLSEvent);

                //送礼动画
                if (player.emCamp == EMUnitCamp.Red)
                {
                    string name = "";
                    //if (player.userName.Length < 8)
                    //    name = player.userName;
                    //else
                    //    name = player.userName.Substring(0, 7);
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送盲盒, new IIPlayerSendBlindBox(player, pTBLInfo.nID - 201, string.Format("<color={0}>{1}</color> 召唤 ", CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), name) + pTBLInfo.szName + " x " + num  ,delegate ()
                    {

                    }));
                }
                else
                {
                    string name = "";
                    //if (player.userName.Length < 8)
                    //    name = player.userName;
                    //else
                    //    name = player.userName.Substring(0, 7);
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送盲盒, new IIPlayerSendBlindBox(player, pTBLInfo.nID - 201, string.Format("<color={0}>{1}</color> 召唤 ", CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), name) + pTBLInfo.szName + " x " + num,delegate ()
                    {

                    }));
                }
            }
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
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
            pEventAddExp.Num = price;

            listReqEvent.Add(pEventAddExp);

            //添加小兵事件
            DLockStepFrameEvent pEventCreateSoldier = new DLockStepFrameEvent();
            pEventCreateSoldier.EventId = (int)EMLockStepEventType.CreateSoldier;
            pEventCreateSoldier.Camp = (int)player.emCamp;
            pEventCreateSoldier.Path = (int)player.emPathType;
            pEventCreateSoldier.Pay = 1;
            pEventCreateSoldier.UnitLev = (int)EMUnitLev.Lv3;

            ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv3);

            switch (pTBLInfo.nID)
            {
                case 201:
                    {
                        creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider6");
                    }
                    break;
                case 202:
                    {
                        creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider7");
                    }
                    break;
                case 203:
                    {
                        creatNum *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8");
                    }
                    break;
                //case 204:
                //    {
                //        num *= CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider5");
                //    }
                //    break;
            }
            pEventCreateSoldier.Tbid = pTBLInfo.nID;

            pEventCreateSoldier.NickName = player.userName;
            pEventCreateSoldier.HeadIcon = player.userFace;
            pEventCreateSoldier.Uid = player.uid;
            pEventCreateSoldier.Num = creatNum;

            listReqEvent.Add(pEventCreateSoldier);

            ETHandlerReqLockStepEvent.Request(listReqEvent.ToArray()).Coroutine();
        }
    }

}
