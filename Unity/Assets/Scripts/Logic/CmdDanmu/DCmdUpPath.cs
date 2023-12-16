using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.UpPath)]
public class DCmdUpPath : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        //if (CGameColorFishMgr.Ins.pMap == null) return;

        Debug.Log("玩家：" + dm.nickName + " 想加入游戏");

        CPlayerBaseInfo pPlayerInfo = CPlayerMgr.Ins.GetPlayer(dm.uid.ToString());
        if (pPlayerInfo == null)
        {
            //先登录
            //CGameAntGlobalMgr.Ins.LoginPlayer(dm.uid, dm.nickName, dm.headIcon, dm.vipLv, -1);
            return;
        }
        else
        {
            CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(dm.uid);
            ChgPath(baseInfo, EMStayPathType.Up);
        }
    }

    void ChgPath(CPlayerBaseInfo player, EMStayPathType pathType)
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
                CLockStepEvent_ChgPath pLSEvent = new CLockStepEvent_ChgPath();
                pLSEvent.msgParams.SetString("uid", player.uid);
                pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                pLSEvent.msgParams.SetInt("chgpath", (int)pathType);
                pLSEvent.msgParams.SetString("nickname", player.userName);
                pLSEvent.msgParams.SetString("headIcon", player.userFace);
                pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);

                CLockStepMgr.Ins.AddLSEvent(pLSEvent);
            }
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameMap101)
            {
                if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming)
                    return;
                CLockStepEvent_ChgPath pLSEvent = new CLockStepEvent_ChgPath();
                pLSEvent.msgParams.SetString("uid", player.uid);
                pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                pLSEvent.msgParams.SetInt("chgpath", (int)pathType);
                pLSEvent.msgParams.SetString("nickname", player.userName);
                pLSEvent.msgParams.SetString("headIcon", player.userFace);
                pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);

                CLockStepMgr.Ins.AddLSEvent(pLSEvent);
            }
        }
    }

}
