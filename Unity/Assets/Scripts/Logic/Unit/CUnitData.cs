using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAddValue
{
    public List<int> nValues = new List<int>();
    public List<Fix64> f64Values = new List<Fix64>();

    public void Clear()
    {
        nValues.Clear();
        f64Values.Clear();
    }
}

[Serializable]
public class CUnitData
{
    public EMUnitLev emUnitLev;         //兵种等级

    public string szPrefabName;         //预制体名字

    public int nTBLID;                  //表ID

    public int nDmgRange = 1;           //攻击伤害范围默认为1

    public int nAtkDmg;                 //攻击伤害

    public int AtkDmg
    {
        get
        {
            return nAtkDmg + (emCampAddType == AddTypeByCamp.Atk ? nCampAddValue : 0) + GetPropertyValueByInt(AddTypeByCamp.Atk);
        }
    }

    public int nAtkBuildDmg;                 //攻击伤害

    public int AtkBuildDmg
    {
        get
        {
            return nAtkBuildDmg + (emCampAddType == AddTypeByCamp.Atk ? nCampAddValue : 0) + GetPropertyValueByInt(AddTypeByCamp.Atk);
        }
    }

    public int nAtkCD;              //攻击时间 

    Fix64 f64AtkCD;                     //攻击CD

    public Fix64 AtkCD
    {
        get
        {
            return f64AtkCD - (emCampAddType == AddTypeByCamp.AtkSpeed ? f64AddValue3 : Fix64.Zero) - GetPropertyValueByF64(AddTypeByCamp.AtkSpeed, 3);
        }
    }

    public int nAlertRange;             //警戒距离

    public int nAtkRange;               //攻击距离

    public int nCurHP;                  //当前血量
        
    public int nMaxHP;                  //最大血量
    public int MaxHP
    {
        get
        {
            return nMaxHP + (emCampAddType == AddTypeByCamp.Hp ? nCampAddValue : 0) + GetPropertyValueByInt(AddTypeByCamp.Hp);
        }
    }

    public int nAtkTime;              //攻击时间 

    Fix64 f64AtkTime;

    public Fix64 AtkTime
    {
        get
        {
            return f64AtkTime - (emCampAddType == AddTypeByCamp.AtkSpeed ? f64AddValue : Fix64.Zero) - GetPropertyValueByF64(AddTypeByCamp.AtkSpeed);
        }
    }

    public int nAtkActiveTime;        //攻击生效时间 

    Fix64 f64AtkActiveTime;

    public Fix64 AtkActiveTime
    {
        get
        {
            return f64AtkActiveTime - (emCampAddType == AddTypeByCamp.AtkSpeed ? f64AddValue2 : Fix64.Zero) - GetPropertyValueByF64(AddTypeByCamp.AtkSpeed, 1);
        }
    }

    public int nMoveSpeed = 30;      //移动速度

    Fix64 f64MoveSpeed;

    public Fix64 MoveSpd
    {
        get
        {
            return f64MoveSpeed + (emCampAddType == AddTypeByCamp.MoveSpeed ? f64AddValue : Fix64.Zero) + (Fix64)GetPropertyValueByF64(AddTypeByCamp.MoveSpeed);
        }
    }

    public int nDashSpeed = 50;
    Fix64 f64DashSpeed;

    public Fix64 DashSpd
    {
        get
        {
            return f64DashSpeed + (emCampAddType == AddTypeByCamp.MoveSpeed ? f64AddValue : Fix64.Zero) + (Fix64)GetPropertyValueByF64(AddTypeByCamp.MoveSpeed);
        }
    }

    public int nDeadTime;              //死亡动画时间 

    Fix64 f64DeadTime;

    public Fix64 DeadTime
    {
        get
        {
            return f64DeadTime;
        }
    }

    public ST_SkillInfo pSkillInfo;

    [HideInInspector]
    public CPlayerUnit pOwner;

    /// <summary>
    /// 是否可以攻击飞行单位
    /// </summary>
    public bool bCanAtkFly;
    /// <summary>
    /// 子弹Prefab
    /// </summary>
    public string szBullet;

    public int nBuffID;

    public int nSkillID;

    /// <summary>
    /// 阵营增益属性 
    /// </summary>
    public int nCampAddValue;
    public Fix64 f64AddValue;
    public Fix64 f64AddValue2;
    public Fix64 f64AddValue3;
    public Fix64 f64AddValue4;
    /// <summary>
    /// 属性增加类型
    /// </summary>
    AddTypeByCamp emCampAddType;

    ///Buff增益属性
    public Dictionary<AddTypeByCamp, CAddValue> dicAddInfoByBuff = new Dictionary<AddTypeByCamp, CAddValue>();

    /// <summary>
    /// 技能效果
    /// </summary>
    public CLocalNetMsg pAtkSkillMsg;
    /// <summary>
    /// 分数
    /// </summary>
    public long nlGold;

    public string szName;

    bool bInit = false;

    /// <summary>
    /// 初始化阵营加成信息
    /// </summary>
    /// <param name="campInfo"></param>
    /// <param name="nLev"></param>
    /// <param name="nSoliderLev"></param>
    public void InitAddInfo(CampInfo campInfo,int nLev,int nSoliderLev)
    {
        emCampAddType = campInfo.emAddType;
        f64AddValue = Fix64.Zero;
        if (emCampAddType == AddTypeByCamp.Hp ||
            emCampAddType == AddTypeByCamp.Atk)
        {
            if (nCampAddValue > 0 &&
                nCurHP > 0 &&
                (int)campInfo.nSolderAddValue[nLev - 1].fAddValue[nSoliderLev] > nCampAddValue)
            {
                nCurHP += (int)campInfo.nSolderAddValue[nLev - 1].fAddValue[nSoliderLev] - nCampAddValue;
            }
            nCampAddValue = (int)campInfo.nSolderAddValue[nLev-1].fAddValue[nSoliderLev];
            f64AddValue = Fix64.Zero;
        }
        else
        {
            nCampAddValue = 0;
            f64AddValue = (Fix64)campInfo.nSolderAddValue[nLev-1].fAddValue[nSoliderLev];
            f64AddValue2 = (Fix64)campInfo.nSolderAddValue[nLev - 1].fAddValue2[nSoliderLev];
            f64AddValue4 = campInfo.nSolderAddValue[nLev - 1].fAddValue3[nSoliderLev];
            f64AddValue3 = (Fix64)campInfo.nSolderAddValue[nLev - 1].fAddValue4[nSoliderLev];
            //Debug.LogError(f64AddValue + "=====" + f64AddValue2 + "====" + fAddValue + "===Add");
        }
    }

    public void Init(CPlayerUnit unit)
    {
        pOwner = unit;

        if (!bInit)
        {
            dicAddInfoByBuff.Add(AddTypeByCamp.Atk, new CAddValue());
            dicAddInfoByBuff.Add(AddTypeByCamp.Hp, new CAddValue());
            dicAddInfoByBuff.Add(AddTypeByCamp.MoveSpeed, new CAddValue());
            dicAddInfoByBuff.Add(AddTypeByCamp.AtkSpeed, new CAddValue());
            bInit = true;
        }
        else
        {
            dicAddInfoByBuff[AddTypeByCamp.Atk].Clear();
            dicAddInfoByBuff[AddTypeByCamp.Hp].Clear();
            dicAddInfoByBuff[AddTypeByCamp.MoveSpeed].Clear();
            dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].Clear();
        }

        

        ST_UnitBattleInfo unitBattleInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(nTBLID);
        if (unitBattleInfo == null)
        {
            f64MoveSpeed = (Fix64)nMoveSpeed;
            f64DashSpeed = (Fix64)nDashSpeed;
            f64AtkTime = (Fix64)nAtkTime;
            f64AtkActiveTime = (Fix64)nAtkActiveTime;
            f64AtkCD = (Fix64)nAtkCD;
            f64DeadTime = (Fix64)nDeadTime;
            nCurHP = MaxHP;
        }
        else
        {
            emUnitLev = unitBattleInfo.emUnitLev;

            nAtkBuildDmg = unitBattleInfo.nAtkBuildDmg;
            nAtkDmg = unitBattleInfo.nAtkDmg;
            nAlertRange = unitBattleInfo.nAlertRange;
            nAtkRange = unitBattleInfo.nAtkRange;
            if (pOwner.emUnitType == CPlayerUnit.EMUnitType.Unit)
            {
                nMaxHP = unitBattleInfo.nMaxHP;
            }
            else
            {
                if(pOwner.emUnitType == CPlayerUnit.EMUnitType.Base)
                {
                    nMaxHP = CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).nBaseHP;
                }
                else if (pOwner.emUnitType == CPlayerUnit.EMUnitType.Barracks)
                {
                    nMaxHP = CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).nBarretHP;
                }
                else if (pOwner.emUnitType == CPlayerUnit.EMUnitType.Tower)
                {
                    nMaxHP = CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).nTowerHP;
                }
                else if (pOwner.emUnitType == CPlayerUnit.EMUnitType.Build)
                {
                    if(nTBLID == 1004)
                    {
                        nMaxHP = CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).nIdle1HP;
                    }
                    else if(nTBLID == 1005)
                    {
                        nMaxHP = CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).nIdle2HP;
                    }
                }
            }
            nMoveSpeed = unitBattleInfo.nMoveSpeed;
            nDashSpeed = unitBattleInfo.nMoveSpeed;
            nAtkTime = unitBattleInfo.nAtkTime;
            nAtkActiveTime = unitBattleInfo.nAtkActiveTime;
            nAtkCD = unitBattleInfo.nAtkCD;

            f64MoveSpeed = unitBattleInfo.f64MoveSpeed;
            f64DashSpeed = unitBattleInfo.f64DashSpeed;
            f64AtkTime = unitBattleInfo.f64AtkTime;
            f64AtkActiveTime = unitBattleInfo.f64AtkActiveTime;
            f64AtkCD = unitBattleInfo.f64AtkCD;

            nDeadTime = unitBattleInfo.nDeadTime;
            f64DeadTime = unitBattleInfo.f64DeadTime;
            bCanAtkFly = unitBattleInfo.bCanAtkFly;
            szBullet = unitBattleInfo.szBullet;
            szPrefabName = unitBattleInfo.szPrefab;
            nBuffID = unitBattleInfo.nBuffID;
            nSkillID = unitBattleInfo.nSkillID;
            nlGold = unitBattleInfo.nlGold;
            szName = unitBattleInfo.szName;

            nCurHP = nMaxHP;
        }

        pSkillInfo = CTBLHandlerSkillInfo.Ins.GetInfo(nSkillID);
        if (pOwner.pAnimeCtrl != null)
            pOwner.pAnimeCtrl.fAddAnimaSpeed = (emCampAddType == AddTypeByCamp.AtkSpeed ? (float)f64AddValue4 : 0f) + (float)GetPropertyValueByF64(AddTypeByCamp.AtkSpeed, 2);
    }

   


    public virtual void AddProByMsg(CLocalNetMsg msgPro, bool add)
    {
        string szKey = msgPro.GetString("id");
        int nValue = msgPro.GetInt("data");
        switch (szKey)
        {
            case "hp_r":            //血量上限
                {
                    nValue = nValue * nMaxHP / 100 ;
                    int nPreHpMax = MaxHP;
                    if(dicAddInfoByBuff[AddTypeByCamp.Hp].nValues.Count <= 0)
                    {
                        dicAddInfoByBuff[AddTypeByCamp.Hp].nValues.Add(0);
                    }
                    dicAddInfoByBuff[AddTypeByCamp.Hp].nValues[0] += nValue * (add ? 1 : -1);
                    int nCurHpMax = MaxHP;
                    if (add)
                    {
                        nCurHP += nCurHpMax - nPreHpMax;
                    }
                    else
                    {
                        if (nCurHP >= nCurHpMax)
                        {
                            nCurHP = nCurHpMax;
                        }
                    }
                }
                break;
            case "atk_r":           //攻击伤害
                {
                    nValue = nValue * nAtkDmg / 100 ;// System.Convert.ToInt32((float)nValue * 0.01f * (float)nAtkDmg);
                    if (dicAddInfoByBuff[AddTypeByCamp.Atk].nValues.Count <= 0)
                    {
                        dicAddInfoByBuff[AddTypeByCamp.Atk].nValues.Add(0);
                    }
                    dicAddInfoByBuff[AddTypeByCamp.Atk].nValues[0] += nValue * (add ? 1 : -1);
                }
                break;
            case "movespeed_r":     //移动速度
                {
                    Fix64 f64Value = (Fix64)nValue * (Fix64)0.01f * f64MoveSpeed;
                    //nValue = System.Convert.ToInt32((float)nValue * 0.01f * nMoveSpeed);
                    if (dicAddInfoByBuff[AddTypeByCamp.MoveSpeed].f64Values.Count <= 0)
                    {
                        dicAddInfoByBuff[AddTypeByCamp.MoveSpeed].f64Values.Add(Fix64.Zero);
                    }
                    dicAddInfoByBuff[AddTypeByCamp.MoveSpeed].f64Values[0] += f64Value * (Fix64)0.01f * f64MoveSpeed * (add ? (Fix64)1f : (Fix64)(-1f));
                }
                break;
            case "atkspeed_r":     //攻击速度
                {
                    if (dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values.Count <= 3)
                    {
                        dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values.Add(Fix64.Zero);
                        dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values.Add(Fix64.Zero);
                        dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values.Add(Fix64.Zero);
                        dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values.Add(Fix64.Zero);
                    }
                    dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values[0] += (Fix64)nValue * (Fix64)0.01f * f64AtkActiveTime * (add ? (Fix64)1 : (Fix64)(-1));
                    dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values[1] += (Fix64)nValue * (Fix64)0.01f * f64AtkTime * (add ? (Fix64)1 : (Fix64)(-1));
                    dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values[2] += (Fix64)nValue * (Fix64)0.01f * (add ? (Fix64)1 : (Fix64)(-1));
                    dicAddInfoByBuff[AddTypeByCamp.AtkSpeed].f64Values[3] += (Fix64)nValue * (Fix64)0.01f * f64AtkCD * (add ? (Fix64)1 : (Fix64)(-1));
                   
                    if (pOwner.pAnimeCtrl != null)
                        pOwner.pAnimeCtrl.fAddAnimaSpeed = (emCampAddType == AddTypeByCamp.AtkSpeed ? (float)f64AddValue4 : 0f) + (float)GetPropertyValueByF64(AddTypeByCamp.AtkSpeed, 2);
                }
                break;
        }
    }


    /// <summary>
    /// 获取角色属性总值
    /// </summary>
    public int GetPropertyValueByInt(AddTypeByCamp pro,int nIdx = 0)
    {
        CAddValue addValue = new CAddValue();
        dicAddInfoByBuff.TryGetValue(pro, out addValue);
        if (nIdx < addValue.nValues.Count)
        {
            return addValue.nValues[nIdx];
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 获取角色属性总值
    /// </summary>
    public Fix64 GetPropertyValueByF64(AddTypeByCamp pro, int nIdx = 0)
    {
        CAddValue addValue = new CAddValue();
        dicAddInfoByBuff.TryGetValue(pro, out addValue);
        if(nIdx < addValue.f64Values.Count)
        {
            return addValue.f64Values[nIdx];
        }
        else
        {
            return Fix64.Zero;
        }
        
    }

}
