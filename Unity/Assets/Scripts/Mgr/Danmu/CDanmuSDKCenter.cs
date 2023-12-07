using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 弹幕聚合管理器
/// 目前已对接：B站、抖音官方、抖音野生
/// </summary>
public class CDanmuSDKCenter : CSingleCompBase<CDanmuSDKCenter>
{
    public enum EMPlatform 
    {
        Bilibili,   //B站官方
        DouyinOpen, //抖音官方
        DouyinYS,   //抖音野生
        Douyu,      //斗鱼
        KuaiShou,   //快手
        QQNow,      //QQ
        TikTokYS,   //海外TikTok野生接口
    }

    public EMPlatform emPlatform = EMPlatform.Bilibili;

    public GameObject[] arrPlatformMgr;

    public CDanmuEventHandler pEventHandler;

    [ReadOnly]
    public string szUid = "";

    [ReadOnly]
    public string szRoomId = "";

    [ReadOnly]
    public string szNickName = "";

    [ReadOnly]
    public string szHeadIcon = "";

    public bool bDevType = false;

    private void Awake()
    {
        //if (Ins != null)
        //{
        //    Destroy(gameObject);
        //    return;
        //}

        //Ins = this;
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CDanmuMockTool.handler = pEventHandler;
    }

    /// <summary>
    /// 登录弹幕系统
    /// </summary>
    /// <param name="code"></param>
    public void Login(string code, string roomId, System.Action<int> callSuc = null)
    {
        if(emPlatform == EMPlatform.Bilibili)
        {
            CDanmuBilibiliMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<CDanmuBilibiliMgr>();
            if (pDanmMgr == null) return;

            //事件注册
            if(pEventHandler!=null)
            {
                pDanmMgr.pEvent.onEventDM = pEventHandler.OnDanmuChatInfo;
                pDanmMgr.pEvent.onEventGift = pEventHandler.OnDanmuSendGift;
                pDanmMgr.pEvent.onEventVip = pEventHandler.OnDanmuVipBuy;
            }

            pDanmMgr.bDevMode = bDevType;
            pDanmMgr.StartGame(code, delegate (int value)
            {
                if (value == 0)
                {
                    szUid = pDanmMgr.lUserId;
                    szRoomId = pDanmMgr.lRoomID;
                    szNickName = pDanmMgr.gameAnchorInfo.UName;
                    szHeadIcon = pDanmMgr.gameAnchorInfo.UFace;
                }

                callSuc?.Invoke(value);
            });
        }
        //else if(emPlatform == EMPlatform.DouyinOpen)
        //{
        //    DouyinOpenClient pDouyinClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyinOpenClient>();
        //    if (pDouyinClient == null) return;

        //    if(pEventHandler!=null)
        //    {
        //        pDouyinClient.pEvent.onEventDM = pEventHandler.OnDanmuChatInfo;
        //        pDouyinClient.pEvent.onEventGift = pEventHandler.OnDanmuSendGift;
        //        pDouyinClient.pEvent.onEventLike = pEventHandler.OnDanmuLikeInfo;
        //    }

        //    pDouyinClient.bDebug = bDevType;
        //    pDouyinClient.StartConnect(code, delegate (int value)
        //    {
        //        if (value == 0)
        //        {
        //            szUid = pDouyinClient.szRoomID; //这里用抖音的房间号连接玩平台后便弃用，
        //                                            //这个不稳定房间号不能作为房间数据存储的唯一标识符

        //            //抖音默认用机器码
        //            szRoomId = SystemInfo.deviceUniqueIdentifier;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
        //else if(emPlatform == EMPlatform.DouyinYS)
        //{
        //    DouyinYSClient pDouyinClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyinYSClient>();
        //    if (pDouyinClient == null) return;

        //    if (pEventHandler != null)
        //    {
        //        pDouyinClient.pEvent.onEventDM = pEventHandler.OnDanmuChatInfo;
        //        pDouyinClient.pEvent.onEventGift = pEventHandler.OnDanmuSendGift;
        //        pDouyinClient.pEvent.onEventLike = pEventHandler.OnDanmuLikeInfo;
        //    }

        //    pDouyinClient.bDebug = bDevType;
        //    pDouyinClient.StartConnect(code, delegate (int value)
        //    {
        //        if (value == 0)
        //        {
        //            szUid = pDouyinClient.szRoomID;
        //            szRoomId = pDouyinClient.szRoomID;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
        //else if(emPlatform == EMPlatform.Douyu)
        //{
        //    DouyuSDKMgr pDouyuClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyuSDKMgr>();
        //    if (pDouyuClient == null) return;

        //    if (pEventHandler != null)
        //    {
        //        pDouyuClient.pEventHandler.onEventDM = pEventHandler.OnDanmuChatInfo;
        //        pDouyuClient.pEventHandler.onEventGift = pEventHandler.OnDanmuSendGift;
        //    }

        //    pDouyuClient.bDevMode = bDevType;
        //    pDouyuClient.StartGame(code, delegate (int value)
        //    {
        //        if (value == 0)
        //        {
        //            szUid = pDouyuClient.szUid;
        //            szRoomId = pDouyuClient.szRoomID;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
        //else if(emPlatform == EMPlatform.KuaiShou)
        //{
        //    KsOpenClient pKsClient = arrPlatformMgr[(int)emPlatform].GetComponent<KsOpenClient>();
        //    if (pKsClient == null) return;

        //    if(pKsClient.pEventHandler!=null)
        //    {
        //        pKsClient.pEventHandler.onEventDM = pEventHandler.OnDanmuChatInfo;
        //        pKsClient.pEventHandler.onEventGift = pEventHandler.OnDanmuSendGift;
        //        pKsClient.pEventHandler.onEventLike = pEventHandler.OnDanmuLikeInfo;
        //    }

        //    pKsClient.bDebug = bDevType;
        //    pKsClient.StartConnect(code, delegate (int value)
        //    {
        //        if (value == 0)
        //        {
        //            szUid = pKsClient.szUid;        //主播UID                
        //            szRoomId = pKsClient.szRoomID;  //快手号（当做房间号使用）
        //            szNickName = pKsClient.szNickName;
        //            szHeadIcon = pKsClient.szHeadIcon;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
        //else if(emPlatform == EMPlatform.QQNow)
        //{
        //    QQOpenClient pQQClient = arrPlatformMgr[(int)emPlatform].GetComponent<QQOpenClient>();
        //    if (pQQClient == null) return;

        //    if (pEventHandler != null)
        //    {
        //        pQQClient.pEvent.onEventDM = pEventHandler.OnDanmuChatInfo;
        //        pQQClient.pEvent.onEventGift = pEventHandler.OnDanmuSendGift;
        //        pQQClient.pEvent.onEventLike = pEventHandler.OnDanmuLikeInfo;
        //    }

        //    pQQClient.bDebug = bDevType;
        //    pQQClient.StartConnect(code, roomId, delegate (int value)
        //    {
        //        if (value == 0)
        //        {
        //            szUid = pQQClient.szRoomID; //这里用QQ的手填房间号，固定

        //            //抖音默认用机器码
        //            szRoomId = pQQClient.szRoomID;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
    }

    //断开弹幕连接
    public void EndGame(bool clearData, System.Action callSuc = null)
    {
        if(emPlatform == EMPlatform.Bilibili)
        {
            CDanmuBilibiliMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<CDanmuBilibiliMgr>();
            if (pDanmMgr == null) return;

            pDanmMgr.EndGame(clearData, callSuc);
        }
        //else if(emPlatform == EMPlatform.DouyinOpen)
        //{
        //    DouyinOpenClient pDouyinClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyinOpenClient>();
        //    if (pDouyinClient == null) return;

        //    pDouyinClient.CloseConnect(callSuc);
        //}
        //else if (emPlatform == EMPlatform.Douyu)
        //{
        //    DouyuSDKMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<DouyuSDKMgr>();
        //    if (pDanmMgr == null) return;

        //    pDanmMgr.EndGame(callSuc);
        //}
        //else if(emPlatform == EMPlatform.KuaiShou)
        //{
        //    KsOpenClient pKsClient = arrPlatformMgr[(int)emPlatform].GetComponent<KsOpenClient>();
        //    if (pKsClient == null) return;

        //    pKsClient.CloseConnect(callSuc);
        //}
        //else if(emPlatform == EMPlatform.QQNow)
        //{
        //    QQOpenClient pQQClient = arrPlatformMgr[(int)emPlatform].GetComponent<QQOpenClient>();
        //    if (pQQClient == null) return;

        //    pQQClient.CloseConnect(callSuc);
        //}
    }

    /// <summary>
    /// 主动修复网络
    /// </summary>
    public void RepairNet(System.Action callSuc = null)
    {
        if (emPlatform == EMPlatform.Bilibili)
        {
            CDanmuBilibiliMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<CDanmuBilibiliMgr>();
            if (pDanmMgr == null) return;

            pDanmMgr.RepairAllConnect(callSuc);
        }
        //else if (emPlatform == EMPlatform.DouyinOpen)
        //{
        //    DouyinOpenClient pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<DouyinOpenClient>();
        //    if (pDanmMgr == null) return;

        //    pDanmMgr.RepairNet(callSuc);
        //}
        //else if(emPlatform == EMPlatform.Douyu)
        //{
        //    DouyuSDKMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<DouyuSDKMgr>();
        //    if (pDanmMgr == null) return;

        //    pDanmMgr.RepaireNet(callSuc);
        //}
        else
        {
            callSuc?.Invoke();
        }
    }

    //判断是否连接中
    public bool IsGaming()
    {
        if (emPlatform == EMPlatform.Bilibili)
        {
            CDanmuBilibiliMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<CDanmuBilibiliMgr>();
            if (pDanmMgr == null) return false;

            return pDanmMgr.IsGaming();
        }
        //else if (emPlatform == EMPlatform.DouyinOpen)
        //{
        //    DouyinOpenClient pDouyinClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyinOpenClient>();
        //    if (pDouyinClient == null) return false;

        //    return pDouyinClient.IsGaming();
        //}
        //else if(emPlatform == EMPlatform.KuaiShou)
        //{
        //    KsOpenClient pKsClient = arrPlatformMgr[(int)emPlatform].GetComponent<KsOpenClient>();
        //    if (pKsClient == null) return false;

        //    return pKsClient.IsGaming();
        //}
        //else if(emPlatform == EMPlatform.Douyu)
        //{
        //    DouyuSDKMgr pDouyuClient = arrPlatformMgr[(int)emPlatform].GetComponent<DouyuSDKMgr>();
        //    if (pDouyuClient == null) return false;

        //    return pDouyuClient.IsGaming();
        //}
        //else if(emPlatform == EMPlatform.QQNow)
        //{
        //    QQOpenClient pQQClient = arrPlatformMgr[(int)emPlatform].GetComponent<QQOpenClient>();
        //    if (pQQClient == null) return false;

        //    return pQQClient.IsGaming();
        //}

        return false;
    }
}
