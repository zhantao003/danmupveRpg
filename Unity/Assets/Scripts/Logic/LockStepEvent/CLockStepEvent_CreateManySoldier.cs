using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepEvent_CreateManySoldier : CLockStepEvent
{
    public override void DoEvent()
    {
        string uid = msgParams.GetString("uid");
        CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(uid);
        if (baseInfo == null)
        {
            CGameAntGlobalMgr.Ins.AddNewPlayerByLocal(msgParams.GetString("uid"),
                                                      msgParams.GetString("nickname"),
                                                      msgParams.GetString("headIcon"),
                                                      msgParams.GetLong("vipLv"),
                                                      (EMUnitCamp)msgParams.GetInt("camp"),
                                                      (EMStayPathType)msgParams.GetInt("path"));
        }

        ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(msgParams.GetInt("tblId"));

        long num = msgParams.GetLong("num");
        EMStayPathType emPath = (EMStayPathType)msgParams.GetInt("path");
        EMUnitCamp unitCamp = (EMUnitCamp)msgParams.GetInt("camp");
        string szPrefab = pTBLInfo.szPrefab + (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                             CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        int nMaxPlayer = CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("最大队伍人数");
        List<CPlayerUnit> listPlayers = new List<CPlayerUnit>();
        for (int i = 0; i < num; )
        {
            UITeamHeadComp teamHeadComp = null;
            if (pTBLInfo.emUnitLev  > EMUnitLev.Lv1 &&
                pTBLInfo.emUnitLev < EMUnitLev.Lv4)
            {
                teamHeadComp = worldUI.SetTeamUnit(null);
            }
            for (int j = 0;j < nMaxPlayer;j++)
            {
                if (i >= num) break;
                //CPlayerUnit unit = null;
                if (pTBLInfo.emUnitLev >= EMUnitLev.Lv4)
                {
                    CBattleMgr.Ins.AddWaitTeam(new CWaitCreatInfo(uid, unitCamp, emPath, szPrefab, pTBLInfo.nID, pTBLInfo.emUnitLev, delegate (CPlayerUnit unit)
                    {
                        worldUI.SetRareUnit(unit);
                    }));
                }
                else
                {
                    CBattleMgr.Ins.AddWaitTeam(new CWaitCreatInfo(uid, unitCamp, emPath, szPrefab, pTBLInfo.nID, pTBLInfo.emUnitLev, delegate (CPlayerUnit unit)
                    {
                        if (pTBLInfo.emUnitLev > EMUnitLev.Lv1)
                        {
                            teamHeadComp.AddUnit(unit);
                        }
                    }));
                }
                i++;
            }
        }
    }
}
