using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.Chi)]
public class CCmdChi : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        Debug.Log("³Ô£º" + addInfo);
    }
}
