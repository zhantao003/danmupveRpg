using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffPack
{
    public CPlayerUnit pOwner = null; //������

    public List<CBuffBase> listBuff = new List<CBuffBase>();    // Buff�б�

    public Dictionary<int, CEffectBase> dicBuffLoopEffs = new Dictionary<int, CEffectBase>();   // Buff��ѭ����Ч

    public Dictionary<int, int> dicBuffLock = new Dictionary<int, int>();   //buff������

    public void Init(CPlayerUnit owner)
    {
        pOwner = owner;
    }

    /// <summary>
    /// ���Buff
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
            //�����buff�Ѿ����������Ƴ������
            if (!pBuff.bActive)
            {
                RemoveBuff(pBuff);

                pBuff = new CBuffBase();
                pBuff.Init(pBuffTBLInfo, pOwner);
                listBuff.Add(pBuff);

            }
            else
            {
                //ˢ������
            }
        }

        pBuff.AddLayer(num);

    }

    /// <summary>
    /// �Ƴ�buff
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
    /// ͨ��ID����ָ����Buff
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
