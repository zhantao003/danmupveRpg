using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_RandomIcon : CTBLConfigSlot
{
    public string szName;

    public override void InitByLoader(CTBLLoader loader)
    {
        szName = loader.GetStringByName("name");
    }
}

[CTBLConfigAttri("RandIcon")]
public class CTBLHandlerRandIcon : CTBLConfigBaseWithDic<ST_RandomIcon>
{
    
}
