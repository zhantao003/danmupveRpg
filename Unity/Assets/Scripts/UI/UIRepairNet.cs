using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRepairNet : UIBase
{
    bool bReconnect = false;
    public CPropertyTimer pTickerConnect = new CPropertyTimer();

    public override void OnOpen()
    {
        bReconnect = true;
        pTickerConnect.FillTime();

        if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            CDanmuSDKCenter.Ins.EndGame(true);
        }
    }

    protected override void OnUpdate(float dt)
    {
        if(bReconnect &&
           pTickerConnect.Tick(dt))
        {
            bReconnect = false;

            CDanmuSDKCenter.Ins.RepairNet(delegate ()
            {
                CloseSelf();
            });
        }
    }
}
