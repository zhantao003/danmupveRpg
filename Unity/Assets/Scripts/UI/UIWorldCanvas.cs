using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;
public class UIWorldCanvas : UIBase
{
    public enum WorldUIType { 
        建筑血条,
        精英怪血条,
        小队名字,
        击杀信息,
    }

    public UIBindCamera uiCamera;
    [Header("世界UI资产")]
    public UIBuildHPComp buildingHP;
    public UIRareUnitComp rareUnit;
    public UITeamHeadComp teamUnit;
    public UIKillInfoComp killInfo;


    [Header("UI资产生成位置")]
    public Transform buildingHPTs;
    public Transform rareUnitTs;
    public Transform teamUnitTs;
    public Transform killInfoTs;

    public Dictionary<WorldUIType,List<GameObject>> assetObjectsPool;
    public Dictionary<WorldUIType, List<GameObject>> allAssets;
    protected override void OnStart()
    {
        assetObjectsPool = new Dictionary<WorldUIType, List<GameObject>>();
        allAssets = new Dictionary<WorldUIType, List<GameObject>>();
        CBattleMgr.Ins.displayChangeEvent = ChangeDisplayNameOrHeadIcon;
    }

    public override void OnClose()
    {
        assetObjectsPool.Clear();
    }

    void ChangeDisplayNameOrHeadIcon(CBattleMgr.EMDisPlayName disPlayType) {
        if (allAssets.ContainsKey(WorldUIType.精英怪血条))
        {
            foreach (var item in allAssets[WorldUIType.精英怪血条])
            {
                item.GetComponent<UIRareUnitComp>().SetDisplay(disPlayType);
            }
        }
        if (allAssets.ContainsKey(WorldUIType.小队名字))
        {
            foreach (var item in allAssets[WorldUIType.小队名字])
            {
                item.GetComponent<UITeamHeadComp>().SetDisplay(disPlayType);
            }
        }
    }

    public void RecycleObject(WorldUIType type, GameObject obj) {
        obj.transform.position += new Vector3(0, 1000, 0);
        if (assetObjectsPool.ContainsKey(type))
        {
            assetObjectsPool[type].Add(obj);
        }
        else {
            List<GameObject> list = new List<GameObject>();
            list.Add(obj);
            assetObjectsPool.Add(type, list);
        }
    }

    GameObject GetAssetObj(WorldUIType type) {
        if (assetObjectsPool.ContainsKey(type)&& assetObjectsPool[type].Count > 0)
        {
            GameObject obj = assetObjectsPool[type][0];
            assetObjectsPool[type].RemoveAt(0);
            return obj;
        }
        else {
            if (!allAssets.ContainsKey(type)) {
                List<GameObject> list = new List<GameObject>();
                allAssets[type] = list;
            }

            if (type == WorldUIType.建筑血条)
            {
                return Instantiate(buildingHP.gameObject, buildingHPTs);
            }
            else if (type == WorldUIType.精英怪血条)
            {
                GameObject asset = Instantiate(rareUnit.gameObject, rareUnitTs);
                allAssets[type].Add(asset);
                return asset;
            }
            else if (type == WorldUIType.小队名字)
            {
                GameObject asset = Instantiate(teamUnit.gameObject, teamUnitTs);
                allAssets[type].Add(asset);
                return asset;
            }
            else if (type == WorldUIType.击杀信息) {
                return Instantiate(killInfo.gameObject, killInfoTs);
            }
        }
        return null;
    }

    /// <summary>
    /// 建筑血条绑定
    /// </summary>
    /// <param name="unit"></param>
    public void SetBuildHp(CPlayerUnit unit) {
        GameObject assetObj = GetAssetObj(WorldUIType.建筑血条);
        UIBuildHPComp hpSli = assetObj.GetComponent<UIBuildHPComp>();
        //hpSli.gameObject.SetActive(true);
        Vector2 offset = new Vector2(0, 0.05f);
        if (unit.emUnitType == CPlayerUnit.EMUnitType.Base)
        {
            offset = new Vector2(0, 0.07f);
        }
        else if (unit.emUnitType == CPlayerUnit.EMUnitType.Barracks) {
            offset = new Vector2(0, 0.06f);
        }
        hpSli.enabled = true;
        hpSli.Init(unit, unit.transform, offset);
    }

    /// <summary>
    /// 英雄精英怪，绑定血条，玩家名字
    /// </summary>
    /// <param name="unit"></param>
    public void SetRareUnit(CPlayerUnit unit) {
        if (unit == null) return;
        GameObject assetObj = GetAssetObj(WorldUIType.精英怪血条);
        UIRareUnitComp rare = assetObj.GetComponent<UIRareUnitComp>();
        rare.enabled = true;
        //rare.gameObject.SetActive(true);
        rare.Init(unit, unit.tranSelf, new Vector2(0, 0.08f));
    }

    /// <summary>
    /// 普通怪组刷，玩家名字传递
    /// </summary>
    /// <param name="units"></param>
    public UITeamHeadComp SetTeamUnit(List<CPlayerUnit> units)
    {
        GameObject assetObj = GetAssetObj(WorldUIType.小队名字);
        UITeamHeadComp team = assetObj.GetComponent<UITeamHeadComp>();
        team.enabled = true;
        //team.gameObject.SetActive(true);
        team.Init(units, new Vector2(0, 0.03f));
        return team;
    }

    /// <summary>
    /// 单位击杀单位跳出信息
    /// </summary>
    /// <param name="killer"></param>
    /// <param name="beKillUnit"></param>
    /// <returns></returns>
    public UIKillInfoComp SetKillInfo(CPlayerUnit killer, string beKillUnitName)
    {
        CPlayerBaseInfo info = CPlayerMgr.Ins.GetPlayer(killer.szUserUid);
        if (info != null)
        {
            GameObject assetObj = GetAssetObj(WorldUIType.击杀信息);
            UIKillInfoComp killInfoComp = assetObj.GetComponent<UIKillInfoComp>();
            killInfoComp.enabled = true;
            //killInfoComp.gameObject.SetActive(true);
            killInfoComp.Init(killer.tranSelf, info.userName, beKillUnitName, info.emCamp == EMUnitCamp.Red, 1, new Vector2(0, 0.01f));
            return killInfoComp;
        }
        return null;
    }
}
