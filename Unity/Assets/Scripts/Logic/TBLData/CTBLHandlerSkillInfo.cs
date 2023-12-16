using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_SkillInfo : CTBLConfigSlot
{
    public int nSkillDmg;               //技能伤害
    public int nAtkBuildDmg;            //打建筑增伤
    public int nSkillRange;             //技能范围
    public int nSkillCD;              //技能CD
    public int nSkillTime;            //技能动作时间
    public int nSkillActiveTime;      //技能生效时间
    public Fix64 f64SkillCD;            //技能CD
    public Fix64 f64SkillTime;          //技能动作时间
    public Fix64 f64SkillActiveTime;    //技能生效时间
    public string szSkillEffect;        //技能特效
    public bool bDmg;                   //是否为伤害技能
    public int nValue;                  //技能用值
    public string szSkillName;          //技能名字


    public override void InitByLoader(CTBLLoader loader)
    {
        nSkillDmg = loader.GetIntByName("skilldmg");
        nAtkBuildDmg = loader.GetIntByName("AtkBuildDmg");
        nSkillRange = loader.GetIntByName("skillrange");
        nSkillCD = loader.GetIntByName("skillcd");// ((float)loader.GetIntByName("skillcd") * 0.01f);
        f64SkillCD = (Fix64)nSkillCD * (Fix64)0.01f;
        nSkillTime = loader.GetIntByName("skilltime");// ((float)loader.GetIntByName("skilltime") * 0.01f);
        f64SkillTime = (Fix64)nSkillTime * (Fix64)0.01f;
        nSkillActiveTime = loader.GetIntByName("skillactivetime");// ((float)loader.GetIntByName("skillactivetime") * 0.01f);
        f64SkillActiveTime = (Fix64)nSkillActiveTime * (Fix64)0.01f;
        szSkillEffect = loader.GetStringByName("skilleffect");
        bDmg = loader.GetIntByName("dmg") > 0;
        nValue = loader.GetIntByName("value");
        szSkillName = loader.GetStringByName("skillname");
    }
}
[CTBLConfigAttri("SkillInfo")]
public class CTBLHandlerSkillInfo : CTBLConfigBaseWithDic<ST_SkillInfo>
{
    
}
