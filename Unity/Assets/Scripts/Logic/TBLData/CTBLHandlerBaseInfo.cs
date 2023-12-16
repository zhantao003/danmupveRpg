using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_BaseInfo : CTBLConfigSlot
{
    public int nExp;         //О­бщ

    public override void InitByLoader(CTBLLoader loader)
    {
        nExp = loader.GetIntByName("exp");
    }
}

[CTBLConfigAttri("BaseInfo")]
public class CTBLHandlerBaseInfo : CTBLConfigBaseWithDic<ST_BaseInfo>
{
    
}
