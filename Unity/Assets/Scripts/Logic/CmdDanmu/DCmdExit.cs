using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.ExitGame)]
public class DCmdExit : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        string uid = dm.uid.ToString();
        CPlayerBaseInfo pPlayer = CPlayerMgr.Ins.GetPlayer(uid);
        if (pPlayer == null)
        {
            return;
        }
        CPlayerMgr.Ins.RemovePlayerUnit(uid);
        CPlayerMgr.Ins.RemovePlayer(uid);

        return;

    }
}
