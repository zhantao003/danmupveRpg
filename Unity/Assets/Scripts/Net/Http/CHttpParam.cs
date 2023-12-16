using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHttpParam
{
    public List<CHttpParamSlot> listParams = new List<CHttpParamSlot>();
    public List<CHttpParamSlotInt> listParamsInt = new List<CHttpParamSlotInt>();

    public CHttpParam(params CHttpParamSlot[] slots)
    {
        for(int i=0; i<slots.Length; i++)
        {
            AddSlot(slots[i]);
        }
    }

    public void AddSlot(CHttpParamSlot pSlot)
    {
        listParams.Add(pSlot);
    }

    public void AddSlotInt(CHttpParamSlotInt pSlot)
    {
        listParamsInt.Add(pSlot);
    }

    public CLocalNetMsg ToJsonMsg()
    {
        CLocalNetMsg pMsg = new CLocalNetMsg();
        for(int i=0; i< listParams.Count; i++)
        {
            pMsg.SetString(listParams[i].szKey, listParams[i].szValue);
        }

        for (int i = 0; i < listParamsInt.Count; i++)
        {
            pMsg.SetLong(listParamsInt[i].szKey, listParamsInt[i].nValue);
        }

        return pMsg;
    }

    public override string ToString()
    {
        string szRes = "";
        for (int i = 0; i < listParams.Count; i++)
        {
            if (i > 0)
            {
                szRes += "&";
            }

            szRes += listParams[i].ToString();
        }

        return szRes;
    }
}

public class CHttpParamSlot
{
    public string szKey;
    public string szValue;

    public CHttpParamSlot(string key, string value)
    {
        szKey = key;
        szValue = value;
    }

    public override  string ToString()
    {
        return szKey + "=" + szValue;
    }
}

public class CHttpParamSlotInt
{
    public string szKey;
    public long nValue;

    public CHttpParamSlotInt(string key, long value)
    {
        szKey = key;
        nValue = value;
    }

    public override string ToString()
    {
        return szKey + "=" + nValue;
    }
}
