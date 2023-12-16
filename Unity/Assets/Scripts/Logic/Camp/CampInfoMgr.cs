using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMCamp
{
    Camp1,
    Camp2,
    Camp3
}

public class CampInfoMgr : MonoBehaviour
{
    static CampInfoMgr ins = null;
    public static CampInfoMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = FindObjectOfType<CampInfoMgr>();
            }

            return ins;
        }
    }

    public CampInfo[] pCampInfos;

    public void InitCampInfo()
    {
        List<ST_UnitBattleInfo> unitBattleInfos = CTBLHandlerUnitBattleInfo.Ins.GetInfos();
        for(int i = 0;i < pCampInfos.Length;i++)
        {
            pCampInfos[i].nSolderAddValue = new CPreSolderAddValue[pCampInfos[i].nValues.Length];
            for (int j = 0;j < pCampInfos[i].nValues.Length;j++)
            {
                pCampInfos[i].nSolderAddValue[j] = new CPreSolderAddValue();
                pCampInfos[i].nSolderAddValue[j].fAddValue = new Fix64[unitBattleInfos.Count];
                pCampInfos[i].nSolderAddValue[j].fAddValue2 = new Fix64[unitBattleInfos.Count];
                pCampInfos[i].nSolderAddValue[j].fAddValue3 = new Fix64[unitBattleInfos.Count];
                pCampInfos[i].nSolderAddValue[j].fAddValue4 = new Fix64[unitBattleInfos.Count];
                for (int k = 0;k < unitBattleInfos.Count;k++)
                {
                    switch(pCampInfos[i].emAddType)
                    {
                        case AddTypeByCamp.Hp:
                            {
                                pCampInfos[i].nSolderAddValue[j].fAddValue[k] = (Fix64)unitBattleInfos[k].nMaxHP * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                                if (pCampInfos[i].nSolderAddValue[j].fAddValue[k] < (Fix64)1)
                                {
                                    pCampInfos[i].nSolderAddValue[j].fAddValue[k] = (Fix64)1f;
                                }
                            }
                            break;
                        case AddTypeByCamp.Atk:
                            {
                                pCampInfos[i].nSolderAddValue[j].fAddValue[k] = (Fix64)unitBattleInfos[k].nAtkDmg * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                                if (pCampInfos[i].nSolderAddValue[j].fAddValue[k] < (Fix64)1)
                                {
                                    pCampInfos[i].nSolderAddValue[j].fAddValue[k] = (Fix64)1f;
                                }
                            }
                            break;
                        case AddTypeByCamp.MoveSpeed:
                            {
                                pCampInfos[i].nSolderAddValue[j].fAddValue[k] = (Fix64)unitBattleInfos[k].nMoveSpeed * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                            }
                            break;
                        case AddTypeByCamp.AtkSpeed:
                            {
                                pCampInfos[i].nSolderAddValue[j].fAddValue[k] = unitBattleInfos[k].f64AtkTime * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                                pCampInfos[i].nSolderAddValue[j].fAddValue2[k] = unitBattleInfos[k].f64AtkActiveTime * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                                pCampInfos[i].nSolderAddValue[j].fAddValue3[k] = (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                                pCampInfos[i].nSolderAddValue[j].fAddValue4[k] = unitBattleInfos[k].f64AtkCD * (Fix64)pCampInfos[i].nValues[j] * (Fix64)0.01f;
                            }
                            break;
                    }
                }
            }
        }
    }

    public CampInfo GetCampInfo(int nIdx)
    {
        CampInfo campInfo = null;

        campInfo = pCampInfos[nIdx];

        return campInfo;
    }

    int nRandomIdx = 0;
    public CampInfo GetRandomCampInfo(out int index)
    {
        nRandomIdx = Random.Range(0, pCampInfos.Length);
        index = nRandomIdx;
        return pCampInfos[nRandomIdx];
    }
}
