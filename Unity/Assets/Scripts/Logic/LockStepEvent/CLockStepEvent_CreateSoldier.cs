using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepEvent_CreateSoldier : CLockStepEvent
{
    public override void DoEvent()
    {
        CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(msgParams.GetString("uid"));
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

        EMUnitCamp unitCamp = (EMUnitCamp)msgParams.GetInt("camp");

        string szPrefab = pTBLInfo.szPrefab + (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName:
                                                                             CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
        
        CBattleMgr.Ins.AddNewPlayer(unitCamp,
                                    (EMStayPathType)msgParams.GetInt("path"),
                                    szPrefab,
                                    msgParams.GetString("uid"),
                                    pTBLInfo.nID,
                                    pTBLInfo.emUnitLev);
    }
}
