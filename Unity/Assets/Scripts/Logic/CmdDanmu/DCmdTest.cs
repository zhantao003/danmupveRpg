using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.Test)]
public class DCmdTest : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        Debug.Log("≤‚ ‘π¶ƒ‹£°£°");
    }
}
