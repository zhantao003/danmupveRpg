using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMUnitLev
{
    Lv1,
    Lv2,
    Lv3,
    Lv4,
    Lv5
}

public class ST_UnitBattleInfo : CTBLConfigSlot
{
    public EMUnitLev emUnitLev;         //兵种等级
    public int nAtkDmg;                 //攻击伤害
    public int nAtkBuildDmg;            //攻击建筑伤害
    public int nAlertRange;             //警戒距离
    public int nAtkRange;               //攻击距离
    public int nMaxHP;                  //最大血量
    //public int[] nBuildHPs;              //塔的血量
    public int nAtkCD;              //攻击CD
    public int nAtkTime;            //攻击动作时间
    public int nAtkActiveTime;      //攻击生效时间
    public int nMoveSpeed;          //移动速度
    public int nDashSpeed;          //冲刺速度
    public int nDeadTime;           //死亡动作时间
    public Fix64 f64AtkCD;              //攻击CD
    public Fix64 f64AtkTime;            //攻击动作时间
    public Fix64 f64AtkActiveTime;      //攻击生效时间
    public Fix64 f64MoveSpeed;          //移动速度
    public Fix64 f64DashSpeed;          //冲刺速度
    public Fix64 f64DeadTime;           //死亡动作时间

    public bool bCanAtkFly;             //是否可以攻击飞行单位
    public string szBullet;             //子弹Prefab
    public string szPrefab;             //预制体名字
    public int nWeight;                 //权重
    public int nBuffID;                 //Buff光环ID
    public int nSkillID;                //技能ID
    public long nlGold;                 //分数
    public string szName;               //名字


    public override void InitByLoader(CTBLLoader loader)
    {
        emUnitLev = (EMUnitLev)loader.GetIntByName("lv");
        nAtkDmg = loader.GetIntByName("atkdmg");
        nAtkBuildDmg = loader.GetIntByName("AtkBuildDmg");
        nAlertRange = loader.GetIntByName("alertrange");
        //if(nAtkBuildDmg > 0)
        //{
            nMaxHP = loader.GetIntByName("maxhp");
        //}
        //else
        //{
        //    nMaxHP = 0;
        //    string szBuildHP = loader.GetStringByName("maxhp");
        //    string[] buildhp = szBuildHP.Split(',');
        //    nBuildHPs = new int[buildhp.Length];
        //    for(int i = 0;i < buildhp.Length;i++)
        //    {
        //        nBuildHPs[i] = int.Parse(buildhp[i]);
        //    }
        //}
        nAtkRange = loader.GetIntByName("atkrange");
        
        nAtkCD = loader.GetIntByName("atkcd");
        f64AtkCD = (Fix64)nAtkCD * (Fix64)0.01f;
        nAtkTime = loader.GetIntByName("atktime");
        f64AtkTime = (Fix64)nAtkTime * (Fix64)0.01f;
        nAtkActiveTime = loader.GetIntByName("atkactivetime");
        f64AtkActiveTime = (Fix64)nAtkActiveTime * (Fix64)0.01f;
        nMoveSpeed = loader.GetIntByName("movespeed");
        f64MoveSpeed = (Fix64)nMoveSpeed * (Fix64)0.01f;
        nDashSpeed = loader.GetIntByName("dashspeed");
        f64DashSpeed = (Fix64)nDashSpeed * (Fix64)0.01f;
        nDeadTime = loader.GetIntByName("deadtime");
        f64DeadTime = (Fix64)nDeadTime * (Fix64)0.01f;
        bCanAtkFly = loader.GetIntByName("canatkfly") > 0;
        szBullet = loader.GetStringByName("bullet");
        szPrefab = loader.GetStringByName("prefab");
        nWeight = loader.GetIntByName("weight");
        nBuffID = loader.GetIntByName("buffid");
        nSkillID = loader.GetIntByName("skillid");
        nlGold = loader.GetLongByName("gold");
        szName = loader.GetStringByName("name");// CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, loader.GetStringByName("name"));
    }

    public void GetRealName()
    {
        szName = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, szName);
    }

}

[CTBLConfigAttri("UnitBattleInfo")]
public class CTBLHandlerUnitBattleInfo : CTBLConfigBaseWithDic<ST_UnitBattleInfo>
{
    public static CTBLHandlerUnitBattleInfo pIns = null;

    public Dictionary<EMUnitLev, List<ST_UnitBattleInfo>> dicUnitInfoByLev = new Dictionary<EMUnitLev, List<ST_UnitBattleInfo>>();

    public override void LoadInfo(CTBLLoader loader)
    {
        pIns = this;
        Ins = this;
        
        for (int i = 0; i < loader.GetLineCount(); i++)
        {
            loader.GotoLineByIndex(i);

            ST_UnitBattleInfo pInfo = (ST_UnitBattleInfo)Activator.CreateInstance(typeof(ST_UnitBattleInfo), true);
            pInfo.nID = loader.GetIntByName("id");
            pInfo.InitByLoader(loader);
            dicInfos.Add(pInfo.nID, pInfo);

            List<ST_UnitBattleInfo> listInfos = null;
            dicUnitInfoByLev.TryGetValue(pInfo.emUnitLev, out listInfos);
            if (listInfos == null)
            {
                listInfos = new List<ST_UnitBattleInfo>();
                listInfos.Add(pInfo);
                dicUnitInfoByLev.Add(pInfo.emUnitLev, listInfos);
            }
            else
            {
                listInfos.Add(pInfo);
            }

            if (i == 0)
            {
                nMinId = pInfo.nID;
            }
            else
            {
                if (nMinId > pInfo.nID)
                {
                    nMinId = pInfo.nID;
                }
            }

            if (nMaxId < pInfo.nID)
            {
                nMaxId = pInfo.nID;
            }
        }
    }

    int nTotalWeight = 0;
    int nRandomWeight = 0;
    public ST_UnitBattleInfo GetRandomUnitInfoByLev(EMUnitLev unitLev)
    {
        ST_UnitBattleInfo unitInfo = null;
        List<ST_UnitBattleInfo> listInfos = null;
        dicUnitInfoByLev.TryGetValue(unitLev, out listInfos);
        if (listInfos != null)
        {
            if (listInfos != null && listInfos.Count > 0)
            {
                if (unitLev < EMUnitLev.Lv3)
                {
                    unitInfo = listInfos[0];
                }
                else
                {
                    nTotalWeight = 0;
                    for (int i = 0; i < listInfos.Count; i++)
                    {
                        nTotalWeight += listInfos[i].nWeight;
                    }
                    nRandomWeight = UnityEngine.Random.Range(1, nTotalWeight + 1);// CLockStepMgr.Ins.GetRandomInt(1, nTotalWeight);
                    
                    for (int i = 0; i < listInfos.Count; i++)
                    {
                        if (listInfos[i].nWeight <= 0) continue;
                        nRandomWeight -= listInfos[i].nWeight;
                        if (nRandomWeight > 0) continue;
                        unitInfo = listInfos[i];
                        break;
                    }
                }
            }
        }
        return unitInfo;
    }

}
