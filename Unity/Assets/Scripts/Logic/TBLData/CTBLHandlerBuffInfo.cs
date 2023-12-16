using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_BuffInfo : CTBLConfigSlot
{
    public string szName;
    public string szDes;
    public int nMaxLayer;
    public string szAddPro;
    public string[] arrAddPro;
    public string szEffect;

    public override void InitByLoader(CTBLLoader loader)
    {
        szName = loader.GetStringByName("name");
        szDes = loader.GetStringByName("des");
        nMaxLayer = loader.GetIntByName("overlay");
        string szAddPro = loader.GetStringByName("addpro");
        arrAddPro = null;
        if (!szAddPro.ToLower().Equals("none"))
        {
            arrAddPro = CHelpTools.StringCutByString(szAddPro, new char[] { '|' });
        }
        szEffect = loader.GetStringByName("acteffect");
    }
}

[CTBLConfigAttri("BuffInfo")]
public class CTBLHandlerBuffInfo : CTBLConfigBaseWithDic<ST_BuffInfo>
{

}
