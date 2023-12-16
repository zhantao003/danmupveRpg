using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMgr : CSingleMgrBase<CPlayerMgr>
{
    public delegate void DelegatePlayerChg(CPlayerBaseInfo player);

    public CPlayerBaseInfo pOwner = null;   //主播本人

    /// <summary>
    /// 所有玩家
    /// </summary>
    public Dictionary<string, CPlayerBaseInfo> dicAllPlayers = new Dictionary<string, CPlayerBaseInfo>();
    public DelegatePlayerChg dlgAllPlayerAdd;
    public DelegatePlayerChg dlgAllPlayerRemove;

    /// <summary>
    /// 玩家实体对象
    /// </summary>
    //玩家Avatar待机实体
    public Dictionary<string, List<CPlayerUnit>> dicPlayerIdleAvatar = new Dictionary<string, List<CPlayerUnit>>();

    public Dictionary<string, List<CPlayerUnit>> dicPlayerAliveAvatar = new Dictionary<string, List<CPlayerUnit>>();

    public List<CPlayerBaseInfo> top3ScorePlayersLeft = new List<CPlayerBaseInfo>();
    public List<CPlayerBaseInfo> top3ScorePlayersRight = new List<CPlayerBaseInfo>();

    FixVector3 vUnitIdlePos;

    public void Init()
    {
        vUnitIdlePos = new FixVector3((Fix64)10000, (Fix64)10000, Fix64.Zero);
    }

    public CPlayerUnit PopUnit(string szPrefabName)
    {
        CPlayerUnit unit = null;
        List<CPlayerUnit> units = null;
        if(dicPlayerIdleAvatar.TryGetValue(szPrefabName, out units))
        {
            if (units != null &&
                units.Count > 0)
            {
                unit = units[0];
                units.RemoveAt(0);
            }
        }
        return unit;
    }

    public int GetAllAliveCount()
    {
        if (dicPlayerAliveAvatar == null) return 0;
        int nAliveCount = 0;

        foreach(var value in dicPlayerAliveAvatar.Values)
        {
            nAliveCount += value.Count;
        }

        return nAliveCount;
    }

    public int GetAliveCountHighLev(EMUnitCamp camp)
    {
        if (dicPlayerAliveAvatar == null) return 0;
        int nAliveCount = 0;

        foreach (var value in dicPlayerAliveAvatar.Values)
        {
            if (value.Count > 0 &&
                    value[0].pUnitData.emUnitLev <= EMUnitLev.Lv2)
            {
                continue;
            }
            for(int i = 0;i < value.Count;i++)
            {
                if(value[i].emCamp == camp)
                {
                    nAliveCount++;
                }
            }
        }

        return nAliveCount;
    }

    public int GetAliveCountByLev(EMUnitLev lev, EMUnitCamp camp)
    {
        if (dicPlayerAliveAvatar == null) return 0;
        int nAliveCount = 0;

        foreach (var value in dicPlayerAliveAvatar.Values)
        {
            if (value.Count > 0 &&
                value[0].pUnitData.emUnitLev != lev)
            {
                continue;
            }
            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].emCamp == camp)
                {
                    nAliveCount++;
                }
            }
        }

        return nAliveCount;
    }

    /// <summary>
    /// 添加游戏实体
    /// </summary>
    /// <param name="unit"></param>
    public void AddPlayerUnit(CPlayerUnit unit)
    {
        string szPrefabName = unit.pUnitData.szPrefabName + "red";// (unit.emCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                  //            CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
        if (dicPlayerAliveAvatar.ContainsKey(szPrefabName))
        {
            dicPlayerAliveAvatar[szPrefabName].Add(unit);
        }
        else
        {
            List<CPlayerUnit> listUnits = new List<CPlayerUnit>();
            listUnits.Add(unit);
            dicPlayerAliveAvatar.Add(szPrefabName, listUnits);
        }
    }

    /// <summary>
    /// 获取对应阵营的游戏实体
    /// </summary>
    /// <param name="unitCamp"></param>
    /// <returns></returns>
    public List<CPlayerUnit> GetAliveUnitByCamp(EMUnitCamp unitCamp)
    {
        List<CPlayerUnit> playerUnits = new List<CPlayerUnit>();

        foreach (List<CPlayerUnit> units in dicPlayerAliveAvatar.Values)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].emCamp == unitCamp)
                {
                    playerUnits.Add(units[i]);
                }
            }
        }

        return playerUnits;
    }

    /// <summary>
    /// 获取对应阵营和路径的游戏实体
    /// </summary>
    /// <param name="unitCamp"></param>
    /// <returns></returns>
    public List<CPlayerUnit> GetAliveUnitByCampAndPath(EMUnitCamp unitCamp,EMStayPathType pathType)
    {
        List<CPlayerUnit> playerUnits = new List<CPlayerUnit>();

        foreach (List<CPlayerUnit> units in dicPlayerAliveAvatar.Values)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].emCamp == unitCamp &&
                    units[i].emPathType == pathType)
                {
                    playerUnits.Add(units[i]);
                }
            }
        }

        return playerUnits;
    }

    public void ClearAllPlayerInfo()
    {
        dicAllPlayers.Clear();
        top3ScorePlayersLeft.Clear();
        top3ScorePlayersRight.Clear();
    }

    public void ClearAllPlayerUnit()
    {
        //foreach (List<CPlayerUnit> units in dicPlayerAliveAvatar.Values)
        //{
        //    for (int i = 0; i < units.Count;)
        //    {
        //        if (units[i] == null ||
        //            units[i].gameObject == null)
        //        {
        //            i++;
        //            continue;
        //        }

        //        GameObject obj = units[i].gameObject;
        //        units[i].Recycle();
        //        GameObject.Destroy(obj);
        //    }

        //    units.Clear();
        //}

        dicPlayerAliveAvatar.Clear();
        dicPlayerIdleAvatar.Clear();
    }

    public void AddPlayer(CPlayerBaseInfo player)
    {
        if (!dicAllPlayers.ContainsKey(player.uid))
        {
            dicAllPlayers.Add(player.uid, player);
        }
        ///发送玩家加入的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetString("uid", player.uid);
        CGameObserverMgr.SendMsg(CGameObserverConst.PlayerJoin, msg);
        dlgAllPlayerAdd?.Invoke(player);
    }

    public void RemovePlayer(string uid)
    {
        if (dicAllPlayers.ContainsKey(uid))
        {
            dlgAllPlayerRemove?.Invoke(dicAllPlayers[uid]);
            dicAllPlayers.Remove(uid);
        }
    }

    public void RemovePlayerUnit(CPlayerUnit unit)
    {
        string szPrefabName = unit.pUnitData.szPrefabName;
        if(unit.emCamp == EMUnitCamp.Blue)
        {
            szPrefabName += CBattleMgr.Ins.pBlueCamp.szCampName;
        }
        else if(unit.emCamp == EMUnitCamp.Red)
        {
            szPrefabName += CBattleMgr.Ins.pRedCamp.szCampName;
        }

        if (dicPlayerAliveAvatar.ContainsKey(szPrefabName))
        {
            dicPlayerAliveAvatar[szPrefabName].Remove(unit);
        }
        else
        {

        }
        unit.tranSelf.position = vUnitIdlePos.ToVector3();
        unit.SetMapSlot(null);
        unit.enabled = false;
        unit.m_fixv3LogicPosition = vUnitIdlePos;
        unit.ForceRefreshPos();

        if (dicPlayerIdleAvatar.ContainsKey(szPrefabName))
        {
            dicPlayerIdleAvatar[szPrefabName].Add(unit);
        }
        else
        {
            List<CPlayerUnit> listUnits = new List<CPlayerUnit>();
            listUnits.Add(unit);
            dicPlayerIdleAvatar.Add(szPrefabName, listUnits);
        }
    }

    /// <summary>
    /// 获取指定ID玩家
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public CPlayerBaseInfo GetPlayer(string id)
    {
        CPlayerBaseInfo pInfo = null;
        if (dicAllPlayers.TryGetValue(id, out pInfo))
        {

        }

        return pInfo;
    }

    public int GetAllBaseInfoCount()
    {
        return dicAllPlayers.Count;
    }

}
