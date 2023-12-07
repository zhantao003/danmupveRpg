using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLocalNetMsg
{
    public bool bIsCorrectJson = false;
    protected string szMsgContent;
    protected JSONClass m_pJsonData;

    public CLocalNetMsg()
    {
        m_pJsonData = new JSONClass();
    }

    public CLocalNetMsg(string json)
    {
        szMsgContent = json;
        m_pJsonData = new JSONClass();
        bIsCorrectJson = InitMsg(json);
    }

    public virtual void SetString(string key, string value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetInt(string key, int value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetFloat(string key, float value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetBool(string key, bool value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetLong(string key, long value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetDouble(string key, double value)
    {
        m_pJsonData.Add(key, new JSONData(value));
    }

    public virtual void SetVector3(string key, Vector3 v3)
    {
        m_pJsonData.Add(key + "x", new JSONData(v3.x));
        m_pJsonData.Add(key + "y", new JSONData(v3.y));
        m_pJsonData.Add(key + "z", new JSONData(v3.z));
    }

    public virtual void SetNetMsg(string key, CLocalNetMsg pSlot)
    {
        m_pJsonData.Add(key, pSlot.GetJSON());
    }

    public virtual void SetNetMsgArr(string key, CLocalNetArrayMsg pSlotArr)
    {
        m_pJsonData.Add(key, pSlotArr.GetJSON());
    }

    public virtual JSONNode GetJSON()
    {
        return m_pJsonData;
    }

    public virtual string GetData()
    {
        return m_pJsonData.ToString();
    }

    public virtual string GetDataByContent()
    {
        return szMsgContent;
    }

    public virtual int GetSize()
    {
        return m_pJsonData.ToString().Length;
    }

    public string GetString(string key)
    {
        return m_pJsonData[key];
    }

    public int GetInt(string key)
    {
        return m_pJsonData[key].AsInt;
    }

    public bool GetBool(string key)
    {
        return m_pJsonData[key].AsBool;
    }

    public float GetFloat(string key)
    {
        return m_pJsonData[key].AsFloat;
    }

    public long GetLong(string key)
    {
        return m_pJsonData[key].AsLong;
    }

    public virtual Vector3 GetVector3(string key)
    {
        Vector3 vRes = new Vector3();
        vRes.x = m_pJsonData[key + "x"].AsFloat;
        vRes.y = m_pJsonData[key + "y"].AsFloat;
        vRes.z = m_pJsonData[key + "z"].AsFloat;

        return vRes;
    }

    public float[] GetFloatArr(string key)
    {
        JSONArray arrData = m_pJsonData[key].AsArray;
        float[] arrFloat = new float[arrData.Count];
        for (int i = 0; i < arrData.Count; i++)
        {
            arrFloat[i] = arrData[i].AsFloat;
        }

        return arrFloat;
    }

    public int[] GetIntArr(string key)
    {
        JSONArray arrData = m_pJsonData[key].AsArray;
        int[] arrFloat = new int[arrData.Count];
        for (int i = 0; i < arrData.Count; i++)
        {
            arrFloat[i] = arrData[i].AsInt;
        }

        return arrFloat;
    }

    public CLocalNetMsg GetNetMsg(string key)
    {
        //JSONClass pContent = m_pJsonData[key].AsObject;
        //CLocalNetMsg pMsg = new CLocalNetMsg();
        if (m_pJsonData[key] == null)
        {
            return new CLocalNetMsg();
        }

        return new CLocalNetMsg(m_pJsonData[key].ToString());
    }

    public CLocalNetArrayMsg GetNetMsgArr(string key)
    {
        CLocalNetArrayMsg arrMsg = new CLocalNetArrayMsg(m_pJsonData[key].AsArray);
        return arrMsg;
    }

    public virtual bool InitMsg(string data)
    {
        if (string.IsNullOrEmpty(data)) return false;

        try
        {
            m_pJsonData = JSONNode.Parse(data) as JSONClass;

            if (m_pJsonData == null)
            {
                Debug.LogError("【Error JSON Data】 " + data);
                return false;
            }
        }
        catch(System.Exception e)
        {
            Debug.LogError("【Error JSON Data】 " + data);
            Debug.LogError(e.Message);
        }
       
        return true;
    }
}

//网络消息（数组消息结构）
public class CLocalNetArrayMsg
{
    public bool bIsCorrectJson = false;
    JSONArray pJSONDataArray;

    public CLocalNetArrayMsg()
    {
        pJSONDataArray = new JSONArray();
    }

    public CLocalNetArrayMsg(string szJson)
    {
        pJSONDataArray = new JSONArray();
        bIsCorrectJson = InitMsg(szJson);
    }

    public CLocalNetArrayMsg(JSONArray pJson)
    {
        pJSONDataArray = pJson;
        bIsCorrectJson = (pJSONDataArray != null);
    }

    public bool InitMsg(string data)
    {
        pJSONDataArray = JSONNode.Parse(data) as JSONArray;
        if (pJSONDataArray == null)
        {
            Debug.LogError("【Error JSON Data】 " + data);
            return false;
        }

        return true;
    }

    public void AddMsg(CLocalNetMsg msg)
    {
        pJSONDataArray.Add(msg.GetJSON());
    }

    public CLocalNetMsg GetNetMsg(int nIdx)
    {
        return new CLocalNetMsg(pJSONDataArray[nIdx].ToString());
    }

    public int GetSize()
    {
        return pJSONDataArray.Count;
    }

    public string GetData()
    {
        return pJSONDataArray.ToString();
    }

    public JSONNode GetJSON()
    {
        return pJSONDataArray;
    }
}