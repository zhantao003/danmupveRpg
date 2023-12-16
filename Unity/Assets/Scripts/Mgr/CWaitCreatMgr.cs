using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CWaitCreatByPlayer
{
    public string szUID;
    public List<CWaitCreatInfo> listWaitCreatInfos = new List<CWaitCreatInfo>();
}

public class CWaitCreatMgr : CLockUnityObject
{
    static CWaitCreatMgr ins = null;
    public static CWaitCreatMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = FindObjectOfType<CWaitCreatMgr>();
            }

            return ins;
        }
    }
    public List<CWaitTeamInfo> listRedWaitCreatInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> listRedLev2WaitCreatInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> listBlueWaitCreatInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> listBlueLev2WaitCreatInfos = new List<CWaitTeamInfo>();
    [Header("单次生成最大数量")]
    public int nOnceCreatCount = 50;
    [Header("检测间隔")]
    public float fCheckTime;
    Fix64 f64CheckTotalTime;
    Fix64 f64CheckCurTime;

    public void InitInfo()
    {
        CLockStepMgr.Ins.pWaitCreatMgr = this;
        f64CheckTotalTime = (Fix64)fCheckTime;
        f64CheckCurTime = Fix64.Zero;
    }

    public void AddWaitInfo(CWaitCreatInfo waitInfo)
    {
        //if (waitInfo.emLev == EMUnitLev.Lv1)
        //{
        //    if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, waitInfo.unitCamp) >= 800)
        //    {
        //        CBattleMgr.Ins.AddWaitTeam(new CWaitCreatInfo(waitInfo.szUserUid, waitInfo.unitCamp, waitInfo.emPathType, waitInfo.szName, waitInfo.nID, waitInfo.emLev, waitInfo.pAddCall));
        //        return;
        //    }
        //}
        //else
        //{
        //    if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, waitInfo.unitCamp) >= 1200)
        //    {
        //        CBattleMgr.Ins.AddWaitTeam(new CWaitCreatInfo(waitInfo.szUserUid, waitInfo.unitCamp, waitInfo.emPathType, waitInfo.szName, waitInfo.nID, waitInfo.emLev, waitInfo.pAddCall));
        //        return;
        //    }
        //    else if (CPlayerMgr.Ins.GetAliveCountHighLev(waitInfo.unitCamp) >= 1000)
        //    {
        //        CBattleMgr.Ins.AddWaitTeam(new CWaitCreatInfo(waitInfo.szUserUid, waitInfo.unitCamp, waitInfo.emPathType, waitInfo.szName, waitInfo.nID, waitInfo.emLev, waitInfo.pAddCall));
        //        return;
        //    }
        //}
        if (waitInfo.unitCamp == EMUnitCamp.Blue)
        {
            bool bHaveInfo = false;
            for (int i = 0; i < listBlueWaitCreatInfos.Count; i++)
            {
                if (listBlueWaitCreatInfos[i].nID == waitInfo.nID)
                {
                    bHaveInfo = true;
                    listBlueWaitCreatInfos[i].listWaitInfo.Add(waitInfo);
                    break;
                }
            }
            if (!bHaveInfo)
            {
                CWaitTeamInfo waitTeamInfo = new CWaitTeamInfo();
                waitTeamInfo.nID = waitInfo.nID;
                waitTeamInfo.emUnitLev = waitInfo.emLev;
                waitTeamInfo.listWaitInfo.Add(waitInfo);
                listBlueWaitCreatInfos.Add(waitTeamInfo);
                listBlueWaitCreatInfos.Sort((a, b) => (b.emUnitLev.CompareTo(a.emUnitLev)));
            }
        }
        else if (waitInfo.unitCamp == EMUnitCamp.Red)
        {
            bool bHaveInfo = false;
            for (int i = 0; i < listRedWaitCreatInfos.Count; i++)
            {
                if (listRedWaitCreatInfos[i].nID == waitInfo.nID)
                {
                    bHaveInfo = true;
                    listRedWaitCreatInfos[i].listWaitInfo.Add(waitInfo);
                    break;
                }
            }
            if (!bHaveInfo)
            {
                CWaitTeamInfo waitTeamInfo = new CWaitTeamInfo();
                waitTeamInfo.nID = waitInfo.nID;
                waitTeamInfo.emUnitLev = waitInfo.emLev;
                waitTeamInfo.listWaitInfo.Add(waitInfo);
                listRedWaitCreatInfos.Add(waitTeamInfo);
                listRedWaitCreatInfos.Sort((a, b) => (b.emUnitLev.CompareTo(a.emUnitLev)));
            }
        }
    }

    /// <summary>
    /// 检测生成时间
    /// </summary>
    void CheckCreatTime()
    {
        if (listRedWaitCreatInfos.Count <= 0 &&
            listBlueWaitCreatInfos.Count <= 0) return;

        ///计时器
        f64CheckCurTime += CLockStepData.g_fixFrameLen;

        //大于1表示当前移动结束
        if (f64CheckCurTime >= f64CheckTotalTime)
        {
            f64CheckCurTime = Fix64.Zero;
            CheckWaitInfo();
        }
    }

    void CheckWaitInfo()
    {
        if (listRedWaitCreatInfos.Count > 0)
        {
            int nCurCreatCount = 0;
            CWaitCreatInfo waitCreatInfo = null;
            for (int i = 0; i < listRedWaitCreatInfos.Count;)
            {
                if (listRedWaitCreatInfos[i].listWaitInfo.Count > 0)
                {
                    for (int j = 0; j < listRedWaitCreatInfos[i].listWaitInfo.Count;)
                    {
                        if (nCurCreatCount >= nOnceCreatCount)
                            break;
                        if (listRedWaitCreatInfos[i].listWaitInfo.Count <= 0)
                            break;
                        nCurCreatCount++;
                        waitCreatInfo = listRedWaitCreatInfos[i].listWaitInfo[0];
                        CBattleMgr.Ins.AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                        listRedWaitCreatInfos[i].listWaitInfo.RemoveAt(0);
                    }

                }
                if (listRedWaitCreatInfos[i].listWaitInfo.Count <= 0)
                {
                    listRedWaitCreatInfos.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
        if (listBlueWaitCreatInfos.Count > 0)
        {
            int nCurCreatCount = 0;
            CWaitCreatInfo waitCreatInfo = null;
            for (int i = 0; i < listBlueWaitCreatInfos.Count;)
            {
                if (listBlueWaitCreatInfos[i].listWaitInfo.Count > 0)
                {
                    for (int j = 0; j < listBlueWaitCreatInfos[i].listWaitInfo.Count;)
                    {
                        if (nCurCreatCount >= nOnceCreatCount)
                            break;
                        if (listBlueWaitCreatInfos[i].listWaitInfo.Count <= 0)
                            break;
                        nCurCreatCount++;
                        waitCreatInfo = listBlueWaitCreatInfos[i].listWaitInfo[0];
                        CBattleMgr.Ins.AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                        listBlueWaitCreatInfos[i].listWaitInfo.RemoveAt(0);
                    }

                }
                if (listBlueWaitCreatInfos[i].listWaitInfo.Count <= 0)
                {
                    listBlueWaitCreatInfos.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public override void OnUpdateLogic()
    {
        if (CBattleMgr.Ins.emGameState != CBattleMgr.EMGameState.Gaming) return;
        CheckCreatTime();
    }

}
