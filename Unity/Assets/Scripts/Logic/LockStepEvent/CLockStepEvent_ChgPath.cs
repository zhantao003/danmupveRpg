using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockStepEvent_ChgPath : CLockStepEvent
{
    public override void DoEvent()
    {
        CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(msgParams.GetString("uid"));
        if(baseInfo == null)
        {
            CGameAntGlobalMgr.Ins.AddNewPlayerByLocal(msgParams.GetString("uid"),
                                                     msgParams.GetString("nickname"),
                                                     msgParams.GetString("headIcon"),
                                                     msgParams.GetLong("vipLv"),
                                                     (EMUnitCamp)msgParams.GetInt("camp"),
                                                     (EMStayPathType)msgParams.GetInt("chgpath"));
        }
        else
        {
            baseInfo.emPathType = (EMStayPathType)msgParams.GetInt("chgpath");
        }
    }

    

}
