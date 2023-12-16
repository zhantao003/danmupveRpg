using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_RandomName : CTBLConfigSlot
{
    public string szName;

    public override void InitByLoader(CTBLLoader loader)
    {
        szName = loader.GetStringByName("name");
    }
}

[CTBLConfigAttri("RandName")]
public class CTBLHandlerRandName : CTBLConfigBaseWithDic<ST_RandomName>
{
    
}
