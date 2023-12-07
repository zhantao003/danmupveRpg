using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��Ļ�ۺϹ�����
/// Ŀǰ�ѶԽӣ�Bվ�������ٷ�������Ұ��
/// </summary>
public class CDanmuSDKCenter : CSingleCompBase<CDanmuSDKCenter>
{
    public enum EMPlatform 
    {
        Bilibili,   //Bվ�ٷ�
        DouyinOpen, //�����ٷ�
        DouyinYS,   //����Ұ��
        Douyu,      //����
        KuaiShou,   //����
        QQNow,      //QQ
        TikTokYS,   //����TikTokҰ���ӿ�
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
    /// ��¼��Ļϵͳ
    /// </summary>
    /// <param name="code"></param>
    public void Login(string code, string roomId, System.Action<int> callSuc = null)
    {
        if(emPlatform == EMPlatform.Bilibili)
        {
            CDanmuBilibiliMgr pDanmMgr = arrPlatformMgr[(int)emPlatform].GetComponent<CDanmuBilibiliMgr>();
            if (pDanmMgr == null) return;

            //�¼�ע��
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
        //            szUid = pDouyinClient.szRoomID; //�����ö����ķ����������ƽ̨������ã�
        //                                            //������ȶ�����Ų�����Ϊ�������ݴ洢��Ψһ��ʶ��

        //            //����Ĭ���û�����
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
        //            szUid = pKsClient.szUid;        //����UID                
        //            szRoomId = pKsClient.szRoomID;  //���ֺţ����������ʹ�ã�
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
        //            szUid = pQQClient.szRoomID; //������QQ�������ţ��̶�

        //            //����Ĭ���û�����
        //            szRoomId = pQQClient.szRoomID;
        //        }

        //        callSuc?.Invoke(value);
        //    });
        //}
    }

    //�Ͽ���Ļ����
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
    /// �����޸�����
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

    //�ж��Ƿ�������
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
