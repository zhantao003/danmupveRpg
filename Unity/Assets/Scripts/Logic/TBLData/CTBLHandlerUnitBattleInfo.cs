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
    public EMUnitLev emUnitLev;         //���ֵȼ�
    public int nAtkDmg;                 //�����˺�
    public int nAtkBuildDmg;            //���������˺�
    public int nAlertRange;             //�������
    public int nAtkRange;               //��������
    public int nMaxHP;                  //���Ѫ��
    //public int[] nBuildHPs;              //����Ѫ��
    public int nAtkCD;              //����CD
    public int nAtkTime;            //��������ʱ��
    public int nAtkActiveTime;      //������Чʱ��
    public int nMoveSpeed;          //�ƶ��ٶ�
    public int nDashSpeed;          //����ٶ�
    public int nDeadTime;           //��������ʱ��
    public Fix64 f64AtkCD;              //����CD
    public Fix64 f64AtkTime;            //��������ʱ��
    public Fix64 f64AtkActiveTime;      //������Чʱ��
    public Fix64 f64MoveSpeed;          //�ƶ��ٶ�
    public Fix64 f64DashSpeed;          //����ٶ�
    public Fix64 f64DeadTime;           //��������ʱ��

    public bool bCanAtkFly;             //�Ƿ���Թ������е�λ
    public string szBullet;             //�ӵ�Prefab
    public string szPrefab;             //Ԥ��������
    public int nWeight;                 //Ȩ��
    public int nBuffID;                 //Buff�⻷ID
    public int nSkillID;                //����ID
    public long nlGold;                 //����
    public string szName;               //����


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
