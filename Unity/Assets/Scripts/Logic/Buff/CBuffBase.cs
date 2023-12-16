using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Buff单层信息
public class CBuffLayer
{
    public int nLayIdx;     //层级
    //计时器
    public float fTotalTime;
    Fix64 f64TotalTime;
    public Fix64 f64CurTime;
    Fix64 f64TimeScale;
}

public class CBuffBase 
{
    public int nTBID;

    public int nMaxLay;  //层数

    public ST_BuffInfo pBuffTBLInfo;   //Buff的基础信息

    //层数累积
    public List<CBuffLayer> listLayer = new List<CBuffLayer>();

    public CPlayerUnit pOwner;    //携带者

    public CPropertyTimer pActionTicker = null;

    public CLocalNetMsg[] arrAddPro;    //添加层属性

    public bool bActive = false;        //是否激活,为false时不会添加层或者刷新，先移除再添加

    public virtual void Init(ST_BuffInfo info, CPlayerUnit owner)
    {
        nTBID = info.nID;
        nMaxLay = info.nMaxLayer;
        pBuffTBLInfo = info;

        pOwner = owner;

        //获取属性增益
        arrAddPro = null;
        if (pBuffTBLInfo.arrAddPro != null)
        {
            arrAddPro = new CLocalNetMsg[pBuffTBLInfo.arrAddPro.Length];
            for (int i = 0; i < pBuffTBLInfo.arrAddPro.Length; i++)
            {
                arrAddPro[i] = new CLocalNetMsg(pBuffTBLInfo.arrAddPro[i]);
            }
        }

        listLayer.Clear();

        bActive = true;
    }

    protected void ChgPro(bool add, CLocalNetMsg msgProPack)
    {
        if (pOwner == null || pOwner.IsDead()) return;

        pOwner.pUnitData.AddProByMsg(msgProPack, add);
    }

    /// <summary>
    /// 添加层数
    /// </summary>
    /// <param name="num"></param>
    public virtual void AddLayer(int num)
    {
        //判断是否可以叠层
        for (int idx = 0; idx < num; idx++)
        {
            if (nMaxLay > 1)
            {
                //if (listLayer.Count >= nMaxLay)
                //{
                //    RefreshTime();
                //    break;
                //}

                //没有超出最大叠层数可以叠加
                CBuffLayer pLayer = new CBuffLayer();
                pLayer.nLayIdx = listLayer.Count;

                listLayer.Add(pLayer);
                //属性增益
                if (arrAddPro != null)
                {
                    int nBuffCount = arrAddPro.Length;
                    if(nBuffCount > nMaxLay)
                    {
                        //nBuffCount = nMaxLay;
                    }
                    else
                    {
                        for (int i = 0; i < nBuffCount; i++)
                        {
                            ChgPro(true, arrAddPro[i]);
                        }
                    }
                }
            }

            //刷新其它buff的层级
            RefreshTime();
        }
    }

    /// <summary>
    /// 获取层数
    /// </summary>
    /// <returns></returns>
    public virtual int GetLayer()
    {
        return listLayer.Count;
    }

    /// <summary>
    /// 移除增益层
    /// </summary>
    /// <param name="idx"></param>
    public virtual int RemoveLayer(int num)
    {
        int nRemoveLayer = 0;

        for (int i = 0; i < num; i++)
        {
            int buffIdx = listLayer.Count - 1;

            if (buffIdx >= 0)
            {
                if (arrAddPro != null &&
                    buffIdx < nMaxLay)
                {
                    for (int proIdx = 0; proIdx < arrAddPro.Length; proIdx++)
                    {
                        ChgPro(false, arrAddPro[proIdx]);
                    }
                }
            }
            else
            {
                break;
            }

            listLayer.RemoveAt(buffIdx);
            nRemoveLayer++;
        }

        bActive = (listLayer.Count > 0);
        if (!bActive)
        {
            OnEnd();
        }

        return nRemoveLayer;
    }

    void RefreshTime()
    {
        for (int i = 0; i < listLayer.Count; i++)
        {
            listLayer[i].f64CurTime = Fix64.Zero;
        }
    }

    public virtual void OnStart()
    {

    }

    public virtual void OnEnd()
    {
        if (listLayer.Count > 0)
        {
            RemoveLayer(listLayer.Count);
        }
    }


}
