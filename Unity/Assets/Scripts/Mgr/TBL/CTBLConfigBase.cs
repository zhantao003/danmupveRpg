using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTBLConfigBase 
{
    public virtual void LoadInfo(CTBLLoader loader)
    {

    }
}

public class CTBLConfigBaseWithDic<T>: CTBLConfigBase where T:CTBLConfigSlot
{
    public static CTBLConfigBaseWithDic<T> Ins = null;

    protected Dictionary<int, T> dicInfos = new Dictionary<int, T>();

    public int nMinId = 0;
    public int nMaxId = 0;

    public override void LoadInfo(CTBLLoader loader)
    {
        Ins = this;
        for (int i = 0; i < loader.GetLineCount(); i++)
        {
            loader.GotoLineByIndex(i);

            T pInfo = (T)Activator.CreateInstance(typeof(T), true);
            pInfo.nID = loader.GetIntByName("id");
            pInfo.InitByLoader(loader);

            dicInfos.Add(pInfo.nID, pInfo);

            if(i == 0)
            {
                nMinId = pInfo.nID;
            }
            else
            {
                if(nMinId > pInfo.nID)
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

    public virtual T GetInfo(int id)
    {
        T pRes = null;
        if (dicInfos.TryGetValue(id, out pRes))
        {

        }

        return pRes;
    }

    public virtual List<T> GetInfos()
    {
        List<T> listRes = new List<T>();

        foreach (T slot in dicInfos.Values)
        {
            listRes.Add(slot);
        }

        return listRes;
    }

    public virtual int GetMaxId()
    {
        return 0;
    }
}

public class CTBLConfigBaseWithDicNoIns<T> : CTBLConfigBase where T : CTBLConfigSlot
{
    protected Dictionary<int, T> dicInfos = new Dictionary<int, T>();

    public int nMinId = 0;
    public int nMaxId = 0;

    public override void LoadInfo(CTBLLoader loader)
    {
        for (int i = 0; i < loader.GetLineCount(); i++)
        {
            loader.GotoLineByIndex(i);

            T pInfo = (T)Activator.CreateInstance(typeof(T), true);
            pInfo.InitByLoader(loader);

            dicInfos.Add(pInfo.nID, pInfo);

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

    public virtual T GetInfo(int id)
    {
        T pRes = null;
        if (dicInfos.TryGetValue(id, out pRes))
        {

        }

        return pRes;
    }

    public virtual List<T> GetInfos()
    {
        List<T> listRes = new List<T>();

        foreach (T slot in dicInfos.Values)
        {
            listRes.Add(slot);
        }

        return listRes;
    }

    public virtual int GetMaxId()
    {
        return nMaxId;
    }

    public virtual void Clear()
    {
        dicInfos.Clear();
    }
}
