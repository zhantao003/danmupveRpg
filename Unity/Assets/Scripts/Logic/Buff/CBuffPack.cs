using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffPack
{
    public CPlayerUnit pOwner = null; //持有者

    public List<CBuffBase> listBuff = new List<CBuffBase>();    // Buff列表

    public Dictionary<int, CEffectBase> dicBuffLoopEffs = new Dictionary<int, CEffectBase>();   // Buff的循环特效

    public Dictionary<int, int> dicBuffLock = new Dictionary<int, int>();   //buff互斥锁

    public void Init(CPlayerUnit owner)
    {
        pOwner = owner;
    }

    /// <summary>
    /// 添加Buff
    /// </summary>
    /// <param name="tbid"></param>
    /// <param name="num"></param>
    public void AddBuff(int tbid, int num)
    {
        ST_BuffInfo pBuffTBLInfo = CTBLHandlerBuffInfo.Ins.GetInfo(tbid);
        if (pBuffTBLInfo == null)
        {
            Debug.LogError("None Buff Info:" + tbid);
            return;
        }
        CBuffBase pBuff = GetBuffByID(tbid);

        if (pBuff == null)
        {
            pBuff = new CBuffBase();
            pBuff.Init(pBuffTBLInfo, pOwner);
            listBuff.Add(pBuff);

        }
        else
        {
            //如果旧buff已经结束，先移除后添加
            if (!pBuff.bActive)
            {
                RemoveBuff(pBuff);

                pBuff = new CBuffBase();
                pBuff.Init(pBuffTBLInfo, pOwner);
                listBuff.Add(pBuff);

            }
            else
            {
                //刷新数据
            }
        }

        pBuff.AddLayer(num);

    }

    /// <summary>
    /// 移除buff
    /// </summary>
    /// <param name="buff"></param>
    /// <param name="idx"></param>
    public void RemoveBuff(CBuffBase buff, int idx = -1)
    {
        if (buff == null) return;

        buff.OnEnd();

        if (idx >= 0)
        {
            listBuff.RemoveAt(idx);
        }
        else
        {
            for (int i = 0; i < listBuff.Count; i++)
            {
                if (listBuff[i].nTBID == buff.nTBID)
                {
                    listBuff.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public void Update(float dt)
    {
        for (int i = 0; i < listBuff.Count;)
        {
            if (listBuff[i].bActive)
            {
                //listBuff[i].OnUpdate(dt);
            }

            if (i >= listBuff.Count)
            {
                return;
            }

            if (!listBuff[i].bActive)
            {
                RemoveBuff(listBuff[i], i);
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    /// 通过ID查找指定的Buff
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public CBuffBase GetBuffByID(int id)
    {
        return listBuff.Find(buff => buff.nTBID == id);
    }

    public void Clear()
    {
        listBuff.Clear();
        foreach (CEffectBase eff in dicBuffLoopEffs.Values)
        {
            eff.Recycle();
        }
        dicBuffLoopEffs.Clear();
    }


}
