using SharedLibrary;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResLockStepFrame : AMHandler<Actor_LockStepFrame_M2C>
    {
        protected override async ETTask Run(Session session, Actor_LockStepFrame_M2C message)
        {
            //Debug.LogError("Get Cur Trame ====" + message.CurFrame + "=== GameTime====" + message.GameTime);
            //Debug.Log("�߼�֡��" + message.CurFrame);
            CLockStepData.g_uServerLogicFrame = message.CurFrame;
            CLockStepData.g_uServerGameTime = message.GameTime;

            //����¼�
            if(message.FrameEvents != null)
            {
                for(int i=0; i<message.FrameEvents.Count; i++)
                {
                    CheckEvent(message.CurFrame, message.FrameEvents[i]);
                }
            }

            if(!CLockStepMgr.Ins.bActive)
            {
                if(CLockStepData.g_uServerLogicFrame - CLockStepData.g_uGameLogicFrame >= CLockStepData.g_uServerStartFrame)
                {
                    CLockStepMgr.Ins.bActive = true;

                    //CLockStepMgr.Ins.pPhysicMgr.Init();
                }
            }

            await ETTask.CompletedTask;
        }

        /// <summary>
        /// �����¼�
        /// </summary>
        void CheckEvent(long frame, DLockStepFrameEvent frameEvent)
        {
            //���ؼӾ���
            //Debug.Log("�¼�֡��" + frame + "  �¼���" + frameEvent.EventId);
            if(frameEvent.EventId == (int)EMLockStepEventType.AddBaseExp)
            {
                CLockStepEvent_AddBaseExp pLSEvent = new CLockStepEvent_AddBaseExp();
                pLSEvent.msgParams.SetInt("camp", frameEvent.Camp);
                pLSEvent.msgParams.SetLong("exp", frameEvent.Num);

                CLockStepMgr.Ins.AddLSEvent(frame, pLSEvent);
            }
            else if(frameEvent.EventId == (int)EMLockStepEventType.CreateSoldier)   //����С���¼�
            {
                CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                pLSEvent.msgParams.SetString("uid", frameEvent.Uid);
                pLSEvent.msgParams.SetInt("camp", frameEvent.Camp);
                pLSEvent.msgParams.SetInt("path", frameEvent.Path);
                pLSEvent.msgParams.SetInt("tblId", frameEvent.Tbid);
                pLSEvent.msgParams.SetString("nickname", frameEvent.NickName);
                pLSEvent.msgParams.SetString("headIcon", frameEvent.HeadIcon);
                pLSEvent.msgParams.SetLong("vipLv", 0);
                pLSEvent.msgParams.SetLong("num", frameEvent.Num);

                CLockStepMgr.Ins.AddLSEvent(frame, pLSEvent);
                if(frameEvent.Pay > 0)
                {
                    CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(frameEvent.Uid);
                    if (player == null)
                    {
                        player = new CPlayerBaseInfo(frameEvent.Uid, frameEvent.NickName, frameEvent.HeadIcon, 0, "", false,
                                                                          0, "", CPlayerBaseInfo.EMUserType.Guanzhong, frameEvent.TotalExp, frameEvent.Num, frameEvent.TotalWin);
                        player.emCamp = (EMUnitCamp)frameEvent.Camp;
                        player.emPathType = (EMStayPathType)frameEvent.Path;

                        CPlayerMgr.Ins.AddPlayer(player);
                    }
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(frameEvent.Tbid);
                    //���񶯻�
                    if (frameEvent.UnitLev == (int)EMUnitLev.Lv2)
                    {
                        if (player.emCamp == EMUnitCamp.Red)
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                        else
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                    }
                    else if(frameEvent.UnitLev == (int)EMUnitLev.Lv3)
                    {
                        if (player.emCamp == EMUnitCamp.Red)
                        {
                            string name = "";
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.����ä��, new IIPlayerSendBlindBox(player, pTBLInfo.nID - 201, string.Format("<color={0}>{1}</color> �ٻ� ", CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), name) + pTBLInfo.szName + " x " + frameEvent.Num, delegate ()
                            {

                            }));
                            //UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv1, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                        else
                        {
                            string name = "";
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.����ä��, new IIPlayerSendBlindBox(player, pTBLInfo.nID - 201, string.Format("<color={0}>{1}</color> �ٻ� ", CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), name) + pTBLInfo.szName + " x " + frameEvent.Num, delegate ()
                            {

                            }));
                            //UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv1, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                        //if (frameEvent.Tbid == 201)
                        //{
                        //    if (player.emCamp == EMUnitCamp.Red)
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //    else
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //}
                        //else if (frameEvent.Tbid == 202)
                        //{
                        //    if (player.emCamp == EMUnitCamp.Red)
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //    else
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //}
                        //else if (frameEvent.Tbid == 203)
                        //{
                        //    if (player.emCamp == EMUnitCamp.Red)
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //    else
                        //    {
                        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���͸߼���, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        //    }
                        //}
                    }
                    else if(frameEvent.UnitLev == (int)EMUnitLev.Lv4)
                    {
                        //���񶯻�
                        if (player.emCamp == EMUnitCamp.Red)
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                        else
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                    }
                    else if (frameEvent.UnitLev == (int)EMUnitLev.Lv5)
                    {
                        //���񶯻�
                        if (player.emCamp == EMUnitCamp.Red)
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                        else
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.���;�Ӣ��, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)frameEvent.Num, 0));
                        }
                    }
                }
            }
            else if(frameEvent.EventId == (int)EMLockStepEventType.JoinPlayer)  //��Ҽ����¼�
            {
                CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(frameEvent.Uid);
                if (pPlayer != null) return;

                pPlayer = new CPlayerBaseInfo(frameEvent.Uid, frameEvent.NickName, frameEvent.HeadIcon, 0, "",false,
                                                                  0,"", CPlayerBaseInfo.EMUserType.Guanzhong, frameEvent.TotalExp, frameEvent.Num, frameEvent.TotalWin);

                pPlayer.emCamp = (EMUnitCamp)frameEvent.Camp;
                pPlayer.emPathType = (EMStayPathType)frameEvent.Path;

                CPlayerMgr.Ins.AddPlayer(pPlayer);

                if (pPlayer.emCamp == EMUnitCamp.Red)
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.����Ҽ���, new IIPlayerJoin(pPlayer));
                }
                else if (pPlayer.emCamp == EMUnitCamp.Blue)
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.����Ҽ���, new IIPlayerJoin(pPlayer));
                }
            }
        }
    }
}

