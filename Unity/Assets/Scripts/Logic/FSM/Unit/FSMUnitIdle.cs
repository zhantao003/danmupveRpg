using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMUnitIdle : FSMUnitBase
{
    CPropertyTimer pStayTick;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Idle;
        //pStayTick = new CPropertyTimer();
        //pStayTick.Value = pUnit.fStayTime;
        //pStayTick.FillTime();
    }


    public override void OnUpdate(object obj, float delta)
    {
        //if(pStayTick != null)
        //{
        //    if(pStayTick.Tick(delta))
        //    {
        //        pUnit.SetState(CPlayerUnit.EMState.Move);
        //        pStayTick = null;
        //    }
        //}
    }

}
