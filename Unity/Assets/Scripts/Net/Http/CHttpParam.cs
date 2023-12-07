using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHttpParam
{
    public List<CHttpParamSlot> listParams = new List<CHttpParamSlot>();

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

    public CLocalNetMsg ToJsonMsg()
    {
        CLocalNetMsg pMsg = new CLocalNetMsg();
        for(int i=0; i< listParams.Count; i++)
        {
            pMsg.SetString(listParams[i].szKey, listParams[i].szValue);
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
