using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UINetWait : UIBase
{
    public CPropertyTimer pTickerOvertime;

    public override void OnOpen()
    {
        pTickerOvertime.FillTime();
    }

    protected override void OnUpdate(float dt)
    {
        if (pTickerOvertime.Tick(dt))
        {
            UIToast.Show("ÍøÂç³¬Ê±");

            CDanmuSDKCenter.Ins.RepairNet(delegate ()
            {
                CloseSelf();
            });
        }
    }
}
