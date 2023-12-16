using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuGiftAttrite(CDanmuGiftConst.Soldier_Lv3)]
public class DGiftSoldier_Lv3 : CDanmuGiftAction
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
                    //����
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

    void CreateUnit(CPlayerBaseInfo player, long num, long price, bool joinPlayer)
    {
        //��δѡ��Ӫ
        if (player == null ||
            player.emCamp == EMUnitCamp.Max) return;

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101)
            {

                ///���ͻ��ؾ������ӵļ����¼�
                CLocalNetMsg msg = new CLocalNetMsg();
                msg.SetInt("camp", (int)player.emCamp);
                msg.SetInt("exp", (int)price);
                CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);

                ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(203);

                CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                pLSEvent.msgParams.SetString("uid", player.uid);
                pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                pLSEvent.msgParams.SetString("nickname", player.userName);
                pLSEvent.msgParams.SetString("headIcon", player.userFace);
                pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                pLSEvent.msgParams.SetLong("num", num * CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8"));

                CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                //���񶯻�
                if (player.emCamp == EMUnitCamp.Red)
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)num, 0));
                }
                else
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)num, 0));
                }
            }
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            //if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
            //    return;
            List<DLockStepFrameEvent> listReqEvent = new List<DLockStepFrameEvent>();

            //�Ƿ���Ӽ��������Ϣ
            if (joinPlayer)
            {
                DLockStepFrameEvent pEventJoinPlayer = new DLockStepFrameEvent();
                pEventJoinPlayer.EventId = (int)EMLockStepEventType.JoinPlayer;
                pEventJoinPlayer.Camp = (int)player.emCamp;
                pEventJoinPlayer.Path = (int)player.emPathType;
                pEventJoinPlayer.NickName = player.userName;
                pEventJoinPlayer.HeadIcon = player.userFace;
                pEventJoinPlayer.Uid = player.uid;

                //�����¼��ҵ���������
                pEventJoinPlayer.Num = player.nWorldRank;
                pEventJoinPlayer.TotalWin = player.nWinTimes;
                pEventJoinPlayer.TotalExp = player.nTotalExp;

                listReqEvent.Add(pEventJoinPlayer);
            }

            //�Ӿ������Ϣ
            DLockStepFrameEvent pEventAddExp = new DLockStepFrameEvent();
            pEventAddExp.EventId = (int)EMLockStepEventType.AddBaseExp;
            pEventAddExp.Camp = (int)player.emCamp;
            pEventAddExp.Num = price;

            listReqEvent.Add(pEventAddExp);

            //���С���¼�
            DLockStepFrameEvent pEventCreateSoldier = new DLockStepFrameEvent();
            pEventCreateSoldier.EventId = (int)EMLockStepEventType.CreateSoldier;
            pEventCreateSoldier.Camp = (int)player.emCamp;
            pEventCreateSoldier.Path = (int)player.emPathType;
            pEventCreateSoldier.Pay = 1;
            pEventCreateSoldier.UnitLev = (int)EMUnitLev.Lv3;

            ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(203);
            pEventCreateSoldier.Tbid = pTBLInfo.nID;

            pEventCreateSoldier.NickName = player.userName;
            pEventCreateSoldier.HeadIcon = player.userFace;
            pEventCreateSoldier.Uid = player.uid;
            pEventCreateSoldier.Num = num * CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8");

            listReqEvent.Add(pEventCreateSoldier);

            ETHandlerReqLockStepEvent.Request(listReqEvent.ToArray()).Coroutine();
        }
    }

}
