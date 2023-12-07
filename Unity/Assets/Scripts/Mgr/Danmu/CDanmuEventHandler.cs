using OpenBLive.Runtime;
using OpenBLive.Runtime.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class CDanmuEventSlot
{
    public CDanmuCmdAction cmd;
    public CDanmuChat dm;
    public string addInfo;
}

public class CDanmuLikeSlot
{
    public CDanmuLikeAction cmd;
    public CDanmuLike dm;
}

public class CDanmuEventHandler : SerializedMonoBehaviour
{
    public CDanmuEventMapConfig pMapConfig;

    List<CDanmuEventSlot> listWaitDM = new List<CDanmuEventSlot>();
    List<CDanmuLikeSlot> listWaitLike = new List<CDanmuLikeSlot>();

    [HideInInspector]
    public Dictionary<CDanmuEventConst, CDanmuCmdAction> dicDanmuCommands = new Dictionary<CDanmuEventConst, CDanmuCmdAction>();
    [HideInInspector]
    public Dictionary<CDanmuGiftConst, CDanmuGiftAction> dicGiftCommands = new Dictionary<CDanmuGiftConst, CDanmuGiftAction>();
    [HideInInspector]
    public Dictionary<string, CDanmuLikeAction> dicLikeCommands = new Dictionary<string, CDanmuLikeAction>();

    private void Start()
    {
        dicDanmuCommands = new Dictionary<CDanmuEventConst, CDanmuCmdAction>();
        dicGiftCommands = new Dictionary<CDanmuGiftConst, CDanmuGiftAction>();
        dicLikeCommands = new Dictionary<string, CDanmuLikeAction>();

        Assembly assembly = typeof(CDanmuEventHandler).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            InitDMEvent(type);
            InitGiftEvent(type);
            InitLikeEvent(type);
        }

        StartCoroutine(OnUpdateDanmu());
    }

    private void Update()
    {
           
    }

    IEnumerator OnUpdateDanmu()
    {
        while(true)
        {
            if (listWaitDM.Count > 0)
            {
                CheckDoDMEvent(listWaitDM[0]);

                listWaitDM.RemoveAt(0);
            }

            if (listWaitLike.Count > 0)
            {
                listWaitLike[0].cmd.DoAction(listWaitLike[0].dm);

                listWaitLike.RemoveAt(0);
            }

            yield return 0;
        }

        yield break;
    }

    void CheckDoDMEvent(CDanmuEventSlot dm)
    {
        dm.cmd.DoAction(dm.dm, dm.addInfo);
    }

    void InitDMEvent(Type type)
    {
        object[] objects = type.GetCustomAttributes(typeof(CDanmuCmdAttrite), false);
        if (objects.Length == 0)
        {
            return;
        }

        CDanmuCmdAttrite httAttri = (CDanmuCmdAttrite)objects[0];
        CDanmuCmdAction iHandler = Activator.CreateInstance(type) as CDanmuCmdAction;
        if (iHandler == null)
        {
            Debug.LogError("None Handler:" + httAttri.eventKey);
            return;
        }
        dicDanmuCommands.Add(httAttri.eventKey, iHandler);
    }

    void InitGiftEvent(Type type)
    {
        object[] objects = type.GetCustomAttributes(typeof(CDanmuGiftAttrite), false);
        if (objects.Length == 0)
        {
            return;
        }

        CDanmuGiftAttrite httAttri = (CDanmuGiftAttrite)objects[0];
        CDanmuGiftAction iHandler = Activator.CreateInstance(type) as CDanmuGiftAction;
        if (iHandler == null)
        {
            Debug.LogError("None Handler:" + httAttri.eventKey);
            return;
        }
        if(dicGiftCommands.ContainsKey(httAttri.eventKey))
        {
            Debug.LogWarning("Same Attri:" + iHandler.ToString());
        }
        dicGiftCommands.Add(httAttri.eventKey, iHandler);
    }

    void InitLikeEvent(Type type)
    {
        object[] objects = type.GetCustomAttributes(typeof(CDanmuLikeAttrite), false);
        if (objects.Length == 0)
        {
            return;
        }

        CDanmuLikeAttrite httAttri = (CDanmuLikeAttrite)objects[0];
        CDanmuLikeAction iHandler = Activator.CreateInstance(type) as CDanmuLikeAction;
        if (iHandler == null)
        {
            Debug.LogError("None Handler:" + httAttri.eventKey);
            return;
        }
        if (dicLikeCommands.ContainsKey(httAttri.eventKey))
        {
            Debug.LogWarning("Same Attri:" + iHandler.ToString());
        }
        dicLikeCommands.Add(httAttri.eventKey, iHandler);
    }

    //赠送VIP
    public void OnDanmuVipBuy(CDanmuVipInfo vip)
    {
        StringBuilder sb = new StringBuilder("收到VIP!");
        sb.AppendLine();
        sb.Append("来自用户：");
        sb.AppendLine(vip.nickName);
        sb.Append("赠送了");
        sb.Append(vip.vipLv);
        Debug.Log(sb);
    }

    //送礼消息
    public void OnDanmuSendGift(CDanmuGift sendGift)
    {
        StringBuilder sb = new StringBuilder("收到礼物!");
        sb.AppendLine();
        sb.Append("来自用户：");
        sb.AppendLine(sendGift.nickName);
        sb.Append("赠送了");
        sb.Append(sendGift.giftNum);
        sb.Append("个");
        sb.Append($"【{sendGift.giftName}】");
        sb.AppendLine();
        sb.Append("礼物价值 (人民币:角)：");
        sb.Append(sendGift.price.ToString());
        sb.AppendLine();
        sb.AppendLine();
        sb.Append("礼物ID：");
        sb.Append(sendGift.giftId.ToString());
        Debug.Log(sb);

        //执行通用的送礼动作
        CDanmuGiftAction pCommonCmd = null;
        if (dicGiftCommands.TryGetValue(CDanmuGiftConst.Common, out pCommonCmd))
        {
            pCommonCmd.DoAction(sendGift);
        }

        string szGiftName = sendGift.giftName.Trim();
        if(string.IsNullOrEmpty(szGiftName))
        {
            return;
        }

        CDanmuGiftEventInfo pEventInfo = pMapConfig.GetGiftEvent(szGiftName);

        CDanmuGiftAction pCmd = null;
        if (dicGiftCommands.TryGetValue(pEventInfo.eventType, out pCmd))
        {
            pCmd.DoAction(sendGift);
        }
    }

    //弹幕消息
    public void OnDanmuChatInfo(CDanmuChat dm)
    {
        //通用事件
        CDanmuCmdAction pCommonCdm = null;
        if (dicDanmuCommands.TryGetValue(CDanmuEventConst.Common_IdleUnitDialog, out pCommonCdm))
        {
            pCommonCdm.DoAction(dm, "");
        }

        //执行动作
        //这里要处理一些特殊的事件
        string szKey = dm.content.Trim();
        CDanmuChatEventInfo pEventInfo = pMapConfig.GetDanmuEvent(szKey);
        //if (pEventInfo == null) return;

        CDanmuCmdAction pCmd = null;
        if (dicDanmuCommands.TryGetValue(pEventInfo.eventType, out pCmd))
        {
            listWaitDM.Add(new CDanmuEventSlot() { 
                cmd = pCmd,
                dm = dm,
                addInfo = pEventInfo.szInfo
            });
        }
    }

    //点赞消息
    public void OnDanmuLikeInfo(CDanmuLike like)
    {
        //通用事件
        CDanmuLikeAction pCommonCdm = null;
        if (dicLikeCommands.TryGetValue(CDanmuLikeConst.Like, out pCommonCdm))
        {
            listWaitLike.Add(new CDanmuLikeSlot()
            {
                dm = like,
                cmd = pCommonCdm
            });
        }
    }
}
