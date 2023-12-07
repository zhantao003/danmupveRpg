using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerMgr : CSingleMgrBase<CPlayerMgr>
{
    public delegate void DelegatePlayerChg(CPlayerBaseInfo player);

    public CPlayerBaseInfo pOwner = null;   //��������

    /// <summary>
    /// �������
    /// </summary>
    public Dictionary<string, CPlayerBaseInfo> dicAllPlayers = new Dictionary<string, CPlayerBaseInfo>();
    public DelegatePlayerChg dlgAllPlayerAdd;
    public DelegatePlayerChg dlgAllPlayerRemove;

    /// <summary>
    /// ���ʵ�����
    /// </summary>
    public Dictionary<string, CPlayerUnit> dicPlayerUnits = new Dictionary<string, CPlayerUnit>();  //��Ϸʵ��
    public DelegatePlayerChg dlgPlayerUnitChg;

    /// <summary>
    /// ��ȡָ����ҵĶ���
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CPlayerUnit GetPlayerUnit(string id)
    {
        CPlayerUnit pUnit = null;
        if (!dicPlayerUnits.TryGetValue(id, out pUnit))
        {
            return null;
        }

        return pUnit;
    }

    /// <summary>
    /// �����Ϸʵ��
    /// </summary>
    /// <param name="unit"></param>
    public void AddPlayerUnit(CPlayerUnit unit)
    {
        if (dicPlayerUnits.ContainsKey(unit.uid))
        {
            if (dicPlayerUnits[unit.uid] != null)
            {
                GameObject.Destroy(dicPlayerUnits[unit.uid].gameObject);
            }
            else
            {
                Debug.Log("�ظ������ID��" + unit.uid);
            }

            dicPlayerUnits.Remove(unit.uid);
        }

        dicPlayerUnits.Add(unit.uid, unit);
    }

    public void ClearAllPlayerUnit()
    {
        foreach (CPlayerUnit unit in dicPlayerUnits.Values)
        {
            unit.Recycle();
            GameObject.Destroy(unit.gameObject);
        }

        dicPlayerUnits.Clear();
    }

    public void AddPlayer(CPlayerBaseInfo player)
    {
        dicAllPlayers.Add(player.uid, player);

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

    public void RemovePlayerUnit(string uid)
    {
        CPlayerUnit pUnit = GetPlayerUnit(uid);
        if(pUnit != null)
        {
            pUnit.Recycle();
            GameObject.Destroy(pUnit.gameObject);
            dicPlayerUnits.Remove(uid);
        }
    }

    /// <summary>
    /// ��ȡָ��ID���
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

}
