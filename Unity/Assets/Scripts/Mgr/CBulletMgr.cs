using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletMgr : CSingleMgrBase<CBulletMgr>
{
    //玩家Avatar待机实体
    public Dictionary<string, List<CBulletBeizierUnit>> dicBulletIdleUnit = new Dictionary<string, List<CBulletBeizierUnit>>();

    public Dictionary<string, List<CBulletBeizierUnit>> dicBulletAliveUnit = new Dictionary<string, List<CBulletBeizierUnit>>();

    FixVector3 vUnitIdlePos;

    public void Init()
    {
        vUnitIdlePos = new FixVector3((Fix64)10000, (Fix64)10000, Fix64.Zero);
    }

    public CBulletBeizierUnit PopBullet(string szPrefabName)
    {
        CBulletBeizierUnit bullet = null;
        List<CBulletBeizierUnit> bullets = null;
        if (dicBulletIdleUnit.TryGetValue(szPrefabName, out bullets))
        {
            if (bullets != null &&
                bullets.Count > 0)
            {
                bullet = bullets[0];
                bullets.RemoveAt(0);
            }
        }
        return bullet;
    }

    /// <summary>
    /// 添加游戏实体
    /// </summary>
    /// <param name="unit"></param>
    public void AddBulletUnit(CBulletBeizierUnit unit)
    {
        string szPrefabName = unit.szPrefabName;
        if (dicBulletAliveUnit.ContainsKey(szPrefabName))
        {
            dicBulletAliveUnit[szPrefabName].Add(unit);
        }
        else
        {
            List<CBulletBeizierUnit> listUnits = new List<CBulletBeizierUnit>();
            listUnits.Add(unit);
            dicBulletAliveUnit.Add(szPrefabName, listUnits);
        }
    }

    public void RemoveBulletUnit(CBulletBeizierUnit unit)
    {
        string szPrefabName = unit.szPrefabName;
        if (dicBulletAliveUnit.ContainsKey(szPrefabName))
        {
            dicBulletAliveUnit[szPrefabName].Remove(unit);
        }
        else
        {

        }
        unit.enabled = false;
        unit.m_fixv3LogicPosition = vUnitIdlePos;
        unit.ForceRefreshPos();

        if (dicBulletIdleUnit.ContainsKey(szPrefabName))
        {
            dicBulletIdleUnit[szPrefabName].Add(unit);
        }
        else
        {
            List<CBulletBeizierUnit> listUnits = new List<CBulletBeizierUnit>();
            listUnits.Add(unit);
            dicBulletIdleUnit.Add(szPrefabName, listUnits);
        }
    }

    public void ClearAllBulletUnit()
    {
        foreach (List<CBulletBeizierUnit> units in dicBulletAliveUnit.Values)
        {
            for (int i = 0; i < units.Count; i++)
            {
                if (i >= units.Count) continue;
                if (units[i] == null ||
                    units[i].gameObject == null) continue;
                units[i].Recycle();
                GameObject.Destroy(units[i].gameObject);
            }
            units.Clear();
        }

        dicBulletAliveUnit.Clear();
    }

}
