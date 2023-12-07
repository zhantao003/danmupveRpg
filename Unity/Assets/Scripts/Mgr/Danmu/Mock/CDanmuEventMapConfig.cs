using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[UnityEngine.CreateAssetMenu(fileName = "EventMapData", menuName = "Game/EventMapData")]
public class CDanmuEventMapConfig : SerializedMonoBehaviour
{
    [DictionaryDrawerSettings(KeyLabel = "弹幕内容", ValueLabel = "事件映射")]
    public Dictionary<string, CDanmuChatEventInfo> dicNormalChat = new Dictionary<string, CDanmuChatEventInfo>();
    [DictionaryDrawerSettings(KeyLabel = "弹幕内容", ValueLabel = "事件映射")]
    public Dictionary<string, CDanmuChatEventInfo> dicFollowNumberChat = new Dictionary<string, CDanmuChatEventInfo>();
    [DictionaryDrawerSettings(KeyLabel = "礼物", ValueLabel = "事件映射")]
    public Dictionary<string, CDanmuGiftEventInfo> dicGift = new Dictionary<string, CDanmuGiftEventInfo>();

    public CDanmuChatEventInfo GetDanmuEvent(string content)
    {
        CDanmuChatEventInfo pRes;
        if (dicNormalChat.TryGetValue(content, out pRes))
        {
            return pRes;
        }

        foreach(string keys in dicFollowNumberChat.Keys)
        {
            if(content.StartsWith(keys))
            {
                pRes = new CDanmuChatEventInfo();
                pRes.emType = CDanmuChatEventInfo.EMType.FollowNum;
                pRes.eventType = dicFollowNumberChat[keys].eventType;
                pRes.szInfo = GetChatFollowNum(keys, content);

                return pRes;
            }
        }

        pRes = new CDanmuChatEventInfo();
        pRes.emType = CDanmuChatEventInfo.EMType.Direct;
        pRes.eventType = CDanmuEventConst.None;
        pRes.szInfo = "";

        return pRes;
    }

    public CDanmuGiftEventInfo GetGiftEvent(string content)
    {
        CDanmuGiftEventInfo pRes;
        if (dicGift.TryGetValue(content, out pRes))
        {
            return pRes;
        }

        pRes = new CDanmuGiftEventInfo();
        pRes.eventType = CDanmuGiftConst.None;

        return pRes;
    }

    string GetChatFollowNum(string key, string content)
    {
        string res = "";
        try
        {
            int nLen = content.Length;
            int nCmdIdx = content.IndexOf(key) + key.Length;
            res = content.Substring(nCmdIdx, nLen - nCmdIdx).Trim();

            return res;
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }

        return res;
    }
}
