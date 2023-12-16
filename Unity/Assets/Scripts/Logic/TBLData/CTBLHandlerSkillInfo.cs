using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_SkillInfo : CTBLConfigSlot
{
    public int nSkillDmg;               //�����˺�
    public int nAtkBuildDmg;            //��������
    public int nSkillRange;             //���ܷ�Χ
    public int nSkillCD;              //����CD
    public int nSkillTime;            //���ܶ���ʱ��
    public int nSkillActiveTime;      //������Чʱ��
    public Fix64 f64SkillCD;            //����CD
    public Fix64 f64SkillTime;          //���ܶ���ʱ��
    public Fix64 f64SkillActiveTime;    //������Чʱ��
    public string szSkillEffect;        //������Ч
    public bool bDmg;                   //�Ƿ�Ϊ�˺�����
    public int nValue;                  //������ֵ
    public string szSkillName;          //��������


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
