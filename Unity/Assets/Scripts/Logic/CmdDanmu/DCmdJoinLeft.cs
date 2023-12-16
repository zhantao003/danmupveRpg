using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.JoinLeft)]
public class DCmdJoinLeft : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        //if (CGameColorFishMgr.Ins.pMap == null) return;

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101)
            {
                if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                    return;
            }
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            return;
            //if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101Net)
            //{
            //    if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
            //        return;
            //}
        }

        Debug.Log("玩家：" + dm.nickName + " 想加入游戏");
        CPlayerBaseInfo pPlayerInfo = CPlayerMgr.Ins.GetPlayer(dm.uid.ToString());
        if (pPlayerInfo == null)
        {
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, dm.vipLv,0,
            delegate ()
            {
                CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(dm.uid);
                if (baseInfo != null &&
                    baseInfo.emCamp == EMUnitCamp.Max)
                {
                    baseInfo.emCamp = EMUnitCamp.Red;
                }
            });
        }
        else
        {
            CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(dm.uid);
            if (baseInfo != null &&
                baseInfo.emCamp == EMUnitCamp.Max)
            {
                baseInfo.emCamp = EMUnitCamp.Red;

                if (baseInfo.emCamp == EMUnitCamp.Red)
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左玩家加入, new IIPlayerJoin(baseInfo));
                }
                else if (baseInfo.emCamp == EMUnitCamp.Blue)
                {
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右玩家加入, new IIPlayerJoin(baseInfo));
                }
            }
        }
    }

}
