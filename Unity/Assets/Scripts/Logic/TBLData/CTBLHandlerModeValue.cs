using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAtkUnitDmgInfo
{
    public string szKey;
    public int nDmg;

    public CAtkUnitDmgInfo(string key,int dmg)
    {
        szKey = key;
        nDmg = dmg;
    }

}


public class ST_ModeValue : CTBLConfigSlot
{
    public string szModelName;
    public string szModelDes;
    public int nTowerHP;
    public int nIdle1HP;
    public int nIdle2HP;
    public int nBarretHP;
    public int nBaseHP;
    public List<CAtkUnitDmgInfo> pTowerDmgInfos = new List<CAtkUnitDmgInfo>();
    public List<CAtkUnitDmgInfo> pBaseDmgInfos = new List<CAtkUnitDmgInfo>();
    public override void InitByLoader(CTBLLoader loader)
    {
        szModelName = loader.GetStringByName("name");
        szModelDes = loader.GetStringByName("des");
        nTowerHP = loader.GetIntByName("hpTower");
        nIdle1HP = loader.GetIntByName("hpIdle1");
        nIdle2HP = loader.GetIntByName("hpIdle2");
        nBarretHP = loader.GetIntByName("hpBarret");
        nBaseHP = loader.GetIntByName("hpBase");

        pTowerDmgInfos = new List<CAtkUnitDmgInfo>();
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_101", loader.GetIntByName("t_dmg_101")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_102", loader.GetIntByName("t_dmg_102")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_201", loader.GetIntByName("t_dmg_201")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_202", loader.GetIntByName("t_dmg_202")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_203", loader.GetIntByName("t_dmg_203")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_301", loader.GetIntByName("t_dmg_301")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_302", loader.GetIntByName("t_dmg_302")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_303", loader.GetIntByName("t_dmg_303")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_401", loader.GetIntByName("t_dmg_401")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_402", loader.GetIntByName("t_dmg_402")));
        pTowerDmgInfos.Add(new CAtkUnitDmgInfo("t_dmg_403", loader.GetIntByName("t_dmg_403")));

        pBaseDmgInfos = new List<CAtkUnitDmgInfo>();
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_101", loader.GetIntByName("b_dmg_101")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_102", loader.GetIntByName("b_dmg_102")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_201", loader.GetIntByName("b_dmg_201")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_202", loader.GetIntByName("b_dmg_202")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_203", loader.GetIntByName("b_dmg_203")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_301", loader.GetIntByName("b_dmg_301")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_302", loader.GetIntByName("b_dmg_302")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_303", loader.GetIntByName("b_dmg_303")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_401", loader.GetIntByName("b_dmg_401")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_402", loader.GetIntByName("b_dmg_402")));
        pBaseDmgInfos.Add(new CAtkUnitDmgInfo("b_dmg_403", loader.GetIntByName("b_dmg_403")));
    }

    public int GetTowerDmg(int nID)
    {
        string key = "t_dmg_" + nID;
        int nDmg = 0;

        for(int i= 0;i < pTowerDmgInfos.Count;i++)
        {
            if(pTowerDmgInfos[i].szKey.Equals(key))
            {
                nDmg = pTowerDmgInfos[i].nDmg;
                break;
            }
        }
        //Debug.LogError(nDmg + "===Tower Dmg===" + nID);
        return nDmg;
    }

    public int GetBaseDmg(int nID)
    {
        string key = "b_dmg_" + nID;
        int nDmg = 0;

        for (int i = 0; i < pBaseDmgInfos.Count; i++)
        {
            if (pBaseDmgInfos[i].szKey.Equals(key))
            {
                nDmg = pBaseDmgInfos[i].nDmg;
                break;
            }
        }
        //Debug.LogError(nDmg + "===Base Dmg===" + nID);
        return nDmg;
    }

}

[CTBLConfigAttri("ModeValue")]
public class CTBLHandlerModeValue : CTBLConfigBaseWithDic<ST_ModeValue>
{

}
