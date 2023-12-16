using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CDanmuSDKCenter;
using YYDanmu;

public class CWaitTeamInfo
{
    public int nID;
    public EMUnitLev emUnitLev;
    public List<CWaitCreatInfo> listWaitInfo = new List<CWaitCreatInfo>();
}

public class CWaitCreatInfo
{
    public string szUserUid;                //玩家的UID
    public EMUnitCamp unitCamp;             //所属阵营方
    public EMStayPathType emPathType;       //路线
    public string szName;                   //单位prefab名字
    public EMUnitLev emLev;                 //单位等级
    public int nID;                         //单位ID
    public DelegateUnituncCall pAddCall;    //加入事件

    public CWaitCreatInfo()
    {

    }

    public CWaitCreatInfo(string userUid, EMUnitCamp camp, EMStayPathType pathType, string name,int id, EMUnitLev lev,DelegateUnituncCall unitAddCall)
    {
        szUserUid = userUid;
        unitCamp = camp;
        emPathType = pathType;
        szName = name;
        emLev = lev;
        nID = id;
        pAddCall = unitAddCall;
    }
}

/// <summary>
/// 兵线生成信息
/// </summary>
[System.Serializable]
public class CAutoCreatInfo
{
    public int nNum;
    public string szPrefab;
    public EMStayPathType emPathType;
    public bool bNeedBarrckDead;
}

public class CBattleMgr : CLockUnityObject
{
    static CBattleMgr ins = null;
    public static CBattleMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = FindObjectOfType<CBattleMgr>();
            }

            return ins;
        }
    }

    public MapMgr mapMgr;

    /// <summary>
    /// 每个格子之间的间距
    /// </summary>
    public float fSlotLerp = 50f;

    Fix64 f64SlotLerp = Fix64.Zero;
    public Fix64 SlotLerp
    {
        get
        {
            return f64SlotLerp;
        }
    }

    public long nCurUnitCount = 0;  //当前一共生成的Unit数量

    public enum EMGameState
    {
        Start,
        Gaming,
        End,
    }

    public EMGameState emGameState;

    public enum EMGameType
    {
        Normal,             //普通
        TimeCountDown,      //倒计时
    }

    public enum EMDisPlayName
    { 
        Name,Head,None
    }

    public EMGameType emGameType;   //游戏类型
    public float fCountDownTime;
    Fix64 f64CountDownTotalTime;
    Fix64 f64CountDownCurTime;


    public Text text;
    [Header("红方阵营信息")]
    public CampInfo pRedCamp;
    [Header("蓝方阵营信息")]
    public CampInfo pBlueCamp;

    public bool bActiveSpecialHP;

    public EMDisPlayName emDisplayType;

    #region 双方兵线生成

    [Header("上路生成信息")]
    public CAutoCreatInfo[] pUpCreatInfo;
    [Header("中路生成信息")]
    public CAutoCreatInfo[] pCenterCreatInfo;
    [Header("下路生成信息")]
    public CAutoCreatInfo[] pDownCreatInfo;
    [Header("红方生成时间")]
    public float fRedCreatTime;
    Fix64 f64RedTotalTime;
    Fix64 f64RedCurTime;
    [Header("蓝方生成时间")]
    public float fBlueCreatTime;
    Fix64 f64BlueTotalTime;
    Fix64 f64BlueCurTime;

    #endregion

    #region 点赞队列检测

    [Header("上路点赞生成信息")]
    public CAutoCreatInfo[] pUpLikesCreatInfo;
    [Header("中路点赞生成信息")]
    public CAutoCreatInfo[] pCenterLikesCreatInfo;
    [Header("下路点赞生成信息")]
    public CAutoCreatInfo[] pDownLikesCreatInfo;

    public List<CPlayerBaseInfo> listLikesInfos = new List<CPlayerBaseInfo>();
    [Header("点赞队列生成时间")]
    public float fLikesCreatTime;
    Fix64 f64LikesTotalTime;
    Fix64 f64LikesCurTime;

    #endregion

    #region 等待队列检测
    public List<CWaitTeamInfo> ListRedLev1WaitTeamInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> ListRedLev2WaitTeamInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> ListRedWaitTeamInfos = new List<CWaitTeamInfo>();
    [Header("红方队列检测时间")]
    public float fRedTeamCreatTime;
    Fix64 f64RedTeamTotalTime;
    Fix64 f64RedTeamCurTime;

    public List<CWaitTeamInfo> ListBlueLev1WaitTeamInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> ListBlueLev2WaitTeamInfos = new List<CWaitTeamInfo>();
    public List<CWaitTeamInfo> ListBlueWaitTeamInfos = new List<CWaitTeamInfo>();
    [Header("蓝方队列检测时间")]
    public float fBlueTeamCreatTime;
    Fix64 f64BlueTeamTotalTime;
    Fix64 f64BlueTeamCurTime;
    
    public int nOnceCheckCount = 20;

    #endregion

    public int nPreLoadCount = 6;
    public string[] szPreLoadSoliders;

    /// <summary>
    /// 倒计时事件(当前时间，总时间)
    /// </summary>
    public DelegateFFFuncCall delCountDownEvent;

    /// <summary>
    /// 倒计时事件(当前剩余时间)
    /// </summary>
    public DelegateFFuncCall delCountDownEventNet;

    /// <summary>
    /// 显示切换事件
    /// </summary>
    public DelegateNameHeadIconSwitchChg displayChangeEvent;

    public List<ST_RandomName> listRobotNamePool = new List<ST_RandomName>();
    public List<ST_RandomIcon> listRobotIconPool = new List<ST_RandomIcon>();

    protected const int nNormalLev3 = 1000;
    protected const int nNormalLev2 = 1200;
    protected const int nNormalLev1 = 800;
    protected const int nNetLev3 = 99900;
    protected const int nNetLev2 = 500;
    protected const int nNetLev1 = 100;

    public void Init()
    {
        emGameState = EMGameState.Start;
        CLockStepMgr.Ins.pGameMgr = this;
        f64SlotLerp = (Fix64)fSlotLerp;
        //UIManager.Instance.RefreshUI();
        
        CampInfoMgr.Ins.InitCampInfo();

        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            CLockStepData.pRand = new SRandom((uint)Random.Range(0, 999999));
            GetRandomCampInfo();
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            CLockStepData.pRand = new SRandom(ERoomInfoMgr.Ins.pSelfRoom.nRandSeed);

            ERoom.RoomSlot pSeatRed = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(0);
            ERoom.RoomSlot pSeatBlue = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(1);

            if(pSeatRed!=null && pSeatBlue!=null)
            {
                SetBattleCamp(pSeatRed.camp, pSeatBlue.camp);
            }
            else
            {
                SetBattleCamp(0, 1);
            }
        }

        mapMgr.Init();
        CPlayerMgr.Ins.Init();
        CBulletMgr.Ins.Init();
        //GameStart();
        listRobotNamePool = CTBLHandlerRandName.Ins.GetInfos();
        listRobotIconPool = CTBLHandlerRandIcon.Ins.GetInfos();
    }

    string GetRandomName()
    {
        if (listRobotNamePool.Count <= 0)
        {
            listRobotNamePool = CTBLHandlerRandName.Ins.GetInfos();
        }

        int nIdx = Random.Range(0, listRobotNamePool.Count);
        ST_RandomName pNameInfo = listRobotNamePool[nIdx];
        listRobotNamePool.RemoveAt(nIdx);
        return pNameInfo.szName;
    }

    string GetRandomIcon()
    {
        if (listRobotIconPool.Count <= 0)
        {
            listRobotIconPool = CTBLHandlerRandIcon.Ins.GetInfos();
        }

        int nIdx = Random.Range(0, listRobotIconPool.Count);
        ST_RandomIcon pIconInfo = listRobotIconPool[nIdx];
        listRobotIconPool.RemoveAt(nIdx);
        return pIconInfo.szName;
    }

    public void StartChouJiangUI()
    {
        StartCoroutine(CheckCampSetThenGameStart());
    }

    UIGameInfo uiGameInfo;
    IEnumerator CheckCampSetThenGameStart() {
        uiGameInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;

        //while(!uiGameInfo.redSlot.isStopUpdatePos || 
        //      !uiGameInfo.blueSlot.isStopUpdatePos)
        //{
        //    Debug.Log("Red:" + uiGameInfo.redSlot.isStopUpdatePos + "   Blue:" + uiGameInfo.blueSlot.isStopUpdatePos);
        //    yield return 0;
        //}

        yield return new WaitUntil(() =>
        {
            return uiGameInfo.redSlot.isStopUpdatePos && uiGameInfo.blueSlot.isStopUpdatePos;
        });

        yield return new WaitForSeconds(2f);
        //uiGameInfo.redSlot.gameObject.SetActive(false);
        //uiGameInfo.blueSlot.gameObject.SetActive(false);

        SetCampUI();
        float animSec = CGameEffMgr.GetAnimatorLength(uiGameInfo.slotAnim, "SlotHideEff");
        uiGameInfo.slotAnim.CrossFadeInFixedTime("SlotHideEff", 0f);
        yield return new WaitForSeconds(animSec + 0.2f);
        uiGameInfo.campTileImg.gameObject.SetActive(false);

        animSec =  CGameEffMgr.GetAnimatorLength(uiGameInfo.battleStartAnim, "Eff");
        uiGameInfo.battleStartAnim.gameObject.SetActive(true);
        yield return new WaitForSeconds(animSec + 0.5f);

        animSec = CGameEffMgr.GetAnimatorLength(uiGameInfo.slotAnim, "SlotBGHideEff");
        uiGameInfo.slotAnim.CrossFadeInFixedTime("SlotBGHideEff", 0f);
        yield return new WaitForSeconds(animSec + 0.2f);
        //BattleAnim
        uiGameInfo.slotBG.gameObject.SetActive(false);
        //uiGameInfo.startFightAnim.gameObject.SetActive(true);
        //uiGameInfo.startFightAnim.CrossFadeInFixedTime("Eff", 0f);

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            GameStart();
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            GameStartByNet();
        }
    }

    void SetCampUI() {
        uiGameInfo.img_battleStartCampHeadLeft.sprite = pRedCamp.pCampHead;
        uiGameInfo.img_battleStartCampHeadRight.sprite = pBlueCamp.pCampHead;
        uiGameInfo.img_battleStartCampBGLeft.sprite = pRedCamp.pStartFightBG;
        uiGameInfo.img_battleStartCampBGRight.sprite = pBlueCamp.pStartFightBG;
        uiGameInfo.img_battleStartCampNameBGLeft.sprite = pRedCamp.pStartFightNameBG;
        uiGameInfo.img_battleStartCampNameBGRight.sprite = pBlueCamp.pStartFightNameBG;

        uiGameInfo.campHeroImgsLeft[(int)pRedCamp.emCamp].gameObject.SetActive(true);
        uiGameInfo.campHeroImgsRight[(int)pBlueCamp.emCamp].gameObject.SetActive(true);

        uiGameInfo.img_CampHeadBGLeft.sprite = pRedCamp.pCampHeadBG;
        uiGameInfo.img_CampHeadBGRight.sprite = pBlueCamp.pCampHeadBG;
        uiGameInfo.img_CampHeadLeft.sprite = pRedCamp.pCampHead;
        uiGameInfo.img_CampHeadRight.sprite = pBlueCamp.pCampHead;

        for (int i = 0; i < uiGameInfo.c_LeftTop3.nameBGs.Length; i++)
        {
            uiGameInfo.c_LeftTop3.nameBGs[i].sprite = pRedCamp.pPlayerScoreBG;
        }
        for (int i = 0; i < uiGameInfo.c_RightTop3.nameBGs.Length; i++)
        {
            uiGameInfo.c_RightTop3.nameBGs[i].sprite = pBlueCamp.pPlayerScoreBG;
        }
        uiGameInfo.txt_CampNameLeft.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, pRedCamp.szCampCNName);
        uiGameInfo.txt_CampNameRight.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, pBlueCamp.szCampCNName);
        uiGameInfo.txt_LeftBuff.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, pRedCamp.szBuffName);// "阵营效果:增加" + mapMgr.pRedBase.GetAddProByCamp(false);
        uiGameInfo.txt_RightBuff.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, pBlueCamp.szBuffName);// "阵营效果:增加" + mapMgr.pBlueBase.GetAddProByCamp(false);
    }

    public void GameStartByNet()
    {
        ETModel.Actor_LockStepGameStart_C2M pReq = new ETModel.Actor_LockStepGameStart_C2M();
        ETModel.SessionComponent.Instance.Session.Send(pReq);
    }

    public void GameStart()
    {
        if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            YYOpenClient pYYClient = CDanmuSDKCenter.Ins.arrPlatformMgr[(int)CDanmuSDKCenter.Ins.emPlatform].GetComponent<YYOpenClient>();
            pYYClient.StartRound();
        }
        //CWaitCreatMgr.Ins.InitInfo();

        emGameState = EMGameState.Gaming;

        f64LikesTotalTime = (Fix64)fLikesCreatTime;
        f64LikesCurTime = Fix64.Zero;

        f64CountDownTotalTime = (Fix64)fCountDownTime;
        f64CountDownCurTime = Fix64.Zero;

        f64RedTotalTime = (Fix64)fRedCreatTime;
        f64RedCurTime = Fix64.Zero;

        f64BlueTotalTime = (Fix64)fBlueCreatTime;
        f64BlueCurTime = Fix64.Zero;

        f64RedTeamTotalTime = (Fix64)fRedTeamCreatTime;
        f64RedTeamCurTime = Fix64.Zero;

        f64BlueTeamTotalTime = (Fix64)fBlueTeamCreatTime;
        f64BlueTeamCurTime = Fix64.Zero;
        UIGameInfo gameInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
        if (gameInfo != null)
        {
            gameInfo.RefreshBaseInfo(true);
            gameInfo.RefreshBaseInfo(false);
        }
        mapMgr.RefreshSearch();
    }

    public void GameEnd()
    {
        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            YYOpenClient pYYClient = CDanmuSDKCenter.Ins.arrPlatformMgr[(int)CDanmuSDKCenter.Ins.emPlatform].GetComponent<YYOpenClient>();
            pYYClient.EndRound();
        }
        emGameState = EMGameState.End;
    }

    /// <summary>
    /// 检测倒计时
    /// </summary>
    void CheckCountDown()
    {
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (emGameType != EMGameType.TimeCountDown) return;
            if (emGameState != EMGameState.Gaming) return;
            ///倒计时
            f64CountDownCurTime += CLockStepData.g_fixFrameLen;
            //大于1表示当前移动结束
            if (f64CountDownCurTime >= f64CountDownTotalTime)
            {
                f64CountDownCurTime = Fix64.Zero;
                delCountDownEvent?.Invoke((float)f64CountDownTotalTime, (float)f64CountDownTotalTime);
                GameTimeOut();
            }
            else
            {
                delCountDownEvent?.Invoke((float)f64CountDownCurTime, (float)f64CountDownTotalTime);
            }
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            delCountDownEventNet?.Invoke(CLockStepData.g_uServerGameTime);
        }
    }

    /// <summary>
    /// 游戏倒计时结束
    /// </summary>
    public void GameTimeOut()
    {
        int nRedTotalHP = mapMgr.GetAllBuildHP(EMUnitCamp.Red);
        int nBlueTotalHP = mapMgr.GetAllBuildHP(EMUnitCamp.Blue);
        EMUnitCamp unitCamp = EMUnitCamp.Blue;
        if (nRedTotalHP > nBlueTotalHP)
        {
            unitCamp = EMUnitCamp.Blue;
        }
        else if(nRedTotalHP == nBlueTotalHP)
        {
            if(CLockStepMgr.Ins.GetRandomInt(1,101) >= 50)
            {
                unitCamp = EMUnitCamp.Blue;
            }
            else
            {
                unitCamp = EMUnitCamp.Red;
            }
        }
        else
        {
            unitCamp = EMUnitCamp.Red;
        }
        GameEnd();
        CLockStepMgr.Ins.ClearAllList();
        AStarFindPath.Ins.dicMapSlots.Clear();
        UIGameEndResult uiResult = UIManager.Instance.GetUI(UIResType.GameEndResult) as UIGameEndResult;
        uiResult.GameEndRequest(unitCamp);
    }

    /// <summary>
    /// 增加点赞队列信息
    /// </summary>
    /// <param name="baseInfo"></param>
    public void AddLikesPlayerInfo(CPlayerBaseInfo baseInfo)
    {
        for(int i = 0;i < listLikesInfos.Count;i++)
        {
            if(listLikesInfos[i].uid.Equals(baseInfo.uid))
            {
                return;
            }
        }
        listLikesInfos.Add(baseInfo);
    }

    /// <summary>
    /// 检测点赞队列的生成时间
    /// </summary>
    void CheckLikesTeamTime()
    {
        if (emGameState != EMGameState.Gaming) return;
        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP) return;
        if (CDanmuSDKCenter.Ins.emPlatform != CDanmuSDKCenter.EMPlatform.DouyinOpen &&
            CDanmuSDKCenter.Ins.emPlatform != CDanmuSDKCenter.EMPlatform.DouyinYS) return;
        if (listLikesInfos.Count <= 0) return;

        f64LikesCurTime += CLockStepData.g_fixFrameLen;
        //大于1表示当前移动结束
        if (f64LikesCurTime >= f64LikesTotalTime)
        {
            f64LikesCurTime = Fix64.Zero;
            CreatLikesTeamUnit();
        }
    }

    /// <summary>
    /// 检测双方兵线的生成时间
    /// </summary>
    void CheckCreatTime()
    {
        if (emGameState != EMGameState.Gaming) return;
        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP) return;
        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen ||
            CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS) return;
        ///红方计时器
        f64RedCurTime += CLockStepData.g_fixFrameLen;
        //大于1表示当前移动结束
        if (f64RedCurTime >= f64RedTotalTime)
        {
            f64RedCurTime = Fix64.Zero;
            CreatUnit(EMUnitCamp.Red, mapMgr.pRedBase.pCampInfo);
        }
        ///蓝方计时器
        f64BlueCurTime += CLockStepData.g_fixFrameLen;
        //大于1表示当前移动结束
        if (f64BlueCurTime >= f64BlueTotalTime)
        {
            f64BlueCurTime = Fix64.Zero;
            CreatUnit(EMUnitCamp.Blue, mapMgr.pBlueBase.pCampInfo);
        }
    }

    public void AddWaitTeam(CWaitCreatInfo waitInfo)
    {
        List<CWaitTeamInfo> listWaitTeamInfos = new List<CWaitTeamInfo>();
        if(waitInfo.unitCamp == EMUnitCamp.Blue)
        {
            if (waitInfo.emLev == EMUnitLev.Lv1)
            {
                listWaitTeamInfos = ListBlueLev1WaitTeamInfos;
            }
            else if (waitInfo.emLev == EMUnitLev.Lv2)
            {
                listWaitTeamInfos = ListBlueLev2WaitTeamInfos;
            }
            else
            {
                listWaitTeamInfos = ListBlueWaitTeamInfos;
            }
            bool bHaveInfo = false;
            for (int i = 0; i < listWaitTeamInfos.Count; i++)
            {
                if (listWaitTeamInfos[i].nID == waitInfo.nID)
                {
                    bHaveInfo = true;
                    listWaitTeamInfos[i].listWaitInfo.Add(waitInfo);
                    break;
                }
            }
            if (!bHaveInfo)
            {
                CWaitTeamInfo waitTeamInfo = new CWaitTeamInfo();
                waitTeamInfo.nID = waitInfo.nID;
                waitTeamInfo.emUnitLev = waitInfo.emLev;
                waitTeamInfo.listWaitInfo.Add(waitInfo);
                listWaitTeamInfos.Add(waitTeamInfo);
                listWaitTeamInfos.Sort((a, b) => (b.emUnitLev.CompareTo(a.emUnitLev)));
            }
        }
        else if(waitInfo.unitCamp == EMUnitCamp.Red)
        {
            if (waitInfo.emLev == EMUnitLev.Lv1)
            {
                listWaitTeamInfos = ListRedLev1WaitTeamInfos;
            }
            else if (waitInfo.emLev == EMUnitLev.Lv2)
            {
                listWaitTeamInfos = ListRedLev2WaitTeamInfos;
            }
            else
            {
                listWaitTeamInfos = ListRedWaitTeamInfos;
            }
            bool bHaveInfo = false;
            for(int i = 0;i < listWaitTeamInfos.Count;i++)
            {
                if(listWaitTeamInfos[i].nID == waitInfo.nID)
                {
                    bHaveInfo = true;
                    listWaitTeamInfos[i].listWaitInfo.Add(waitInfo);
                    break;
                }
            }
            if(!bHaveInfo)
            {
                CWaitTeamInfo waitTeamInfo = new CWaitTeamInfo();
                waitTeamInfo.nID = waitInfo.nID;
                waitTeamInfo.emUnitLev = waitInfo.emLev;
                waitTeamInfo.listWaitInfo.Add(waitInfo);
                listWaitTeamInfos.Add(waitTeamInfo);
                listWaitTeamInfos.Sort((a, b) => (b.emUnitLev.CompareTo(a.emUnitLev)));
            }
        }
    }

    /// <summary>
    /// 检测等待队列的计时器
    /// </summary>
    void CheckWaitTeamTime()
    {
        if (emGameState != EMGameState.Gaming) return;
        ///红方计时器
        if (ListRedWaitTeamInfos.Count > 0 ||
            ListRedLev1WaitTeamInfos.Count > 0 ||
            ListRedLev2WaitTeamInfos.Count > 0)
        {
            f64RedTeamCurTime += CLockStepData.g_fixFrameLen;
            //大于1表示当前移动结束
            if (f64RedTeamCurTime >= f64RedTeamTotalTime)
            {
                f64RedTeamCurTime = Fix64.Zero;
                int nCurCreatCount = 0;
                CWaitCreatInfo waitCreatInfo = null;
                
                while(nCurCreatCount < nOnceCheckCount)
                {
                    bool bMaxLev3 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                         CPlayerMgr.Ins.GetAliveCountHighLev(EMUnitCamp.Red) >= nNormalLev3 :
                         CPlayerMgr.Ins.GetAliveCountHighLev(EMUnitCamp.Red) >= nNetLev3;
                    bool bMaxLev2 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, EMUnitCamp.Red) >= nNormalLev2 :
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, EMUnitCamp.Red) >= nNetLev2;
                    bool bMaxLev1 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, EMUnitCamp.Red) >= nNormalLev1 :
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, EMUnitCamp.Red) >= nNetLev1;
                    if (ListRedWaitTeamInfos.Count > 0 &&
                        !bMaxLev3)
                    {
                        if (ListRedWaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListRedWaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListRedWaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListRedWaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListRedWaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }
                        }
                        if (ListRedWaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListRedWaitTeamInfos.RemoveAt(0);
                        }
                    }
                    else if(ListRedLev2WaitTeamInfos.Count > 0 &&
                            !bMaxLev2)
                    {
                        if (ListRedLev2WaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListRedLev2WaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListRedLev2WaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListRedLev2WaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListRedLev2WaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }

                        }
                        if (ListRedLev2WaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListRedLev2WaitTeamInfos.RemoveAt(0);
                        }
                    }
                    else if(ListRedLev1WaitTeamInfos.Count > 0 &&
                            !bMaxLev1)
                    {
                        if (ListRedLev1WaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListRedLev1WaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListRedLev1WaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListRedLev1WaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListRedLev1WaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }

                        }
                        if (ListRedLev1WaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListRedLev1WaitTeamInfos.RemoveAt(0);
                        }
                    }
                    if (ListRedWaitTeamInfos.Count > 0 ? bMaxLev3 : true &&
                        ListRedLev1WaitTeamInfos.Count > 0 ? bMaxLev1 : true &&
                        ListRedLev2WaitTeamInfos.Count > 0 ? bMaxLev2 : true)
                        break;
                    if (nCurCreatCount >= nOnceCheckCount)
                        break;
                }
            }
        }
        ///蓝方计时器
        if (ListBlueWaitTeamInfos.Count > 0 ||
            ListBlueLev1WaitTeamInfos.Count > 0 ||
            ListBlueLev2WaitTeamInfos.Count > 0)
        {
            f64BlueTeamCurTime += CLockStepData.g_fixFrameLen;
            //大于1表示当前移动结束
            if (f64BlueTeamCurTime >= f64BlueTeamTotalTime)
            {
                f64BlueTeamCurTime = Fix64.Zero;
                int nCurCreatCount = 0;
                CWaitCreatInfo waitCreatInfo = null;
                while (nCurCreatCount < nOnceCheckCount)
                {
                    bool bMaxLev3 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                       CPlayerMgr.Ins.GetAliveCountHighLev(EMUnitCamp.Blue) >= nNormalLev3 :
                       CPlayerMgr.Ins.GetAliveCountHighLev(EMUnitCamp.Blue) >= nNetLev3;
                    bool bMaxLev2 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, EMUnitCamp.Blue) >= nNormalLev2 :
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, EMUnitCamp.Blue) >= nNetLev2;
                    bool bMaxLev1 = CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP ?
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, EMUnitCamp.Blue) >= nNormalLev1 :
                       CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, EMUnitCamp.Blue) >= nNetLev1;
                    if (ListBlueWaitTeamInfos.Count > 0 &&
                        !bMaxLev3)
                    {
                        if (ListBlueWaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListBlueWaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListBlueWaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListBlueWaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListBlueWaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }

                        }
                        if (ListBlueWaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListBlueWaitTeamInfos.RemoveAt(0);
                        }
                    }
                    else if(ListBlueLev2WaitTeamInfos.Count > 0 &&
                            !bMaxLev2)
                    {
                        if (ListBlueLev2WaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListBlueLev2WaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListBlueLev2WaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListBlueLev2WaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListBlueLev2WaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }

                        }
                        if (ListBlueLev2WaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListBlueLev2WaitTeamInfos.RemoveAt(0);
                        }
                    }
                    else if(ListBlueLev1WaitTeamInfos.Count > 0 &&
                            !bMaxLev1)
                    {
                        if (ListBlueLev1WaitTeamInfos[0].listWaitInfo.Count > 0)
                        {
                            for (int j = 0; j < ListBlueLev1WaitTeamInfos[0].listWaitInfo.Count;)
                            {
                                if (nCurCreatCount >= nOnceCheckCount)
                                    break;
                                if (ListBlueLev1WaitTeamInfos[0].listWaitInfo.Count <= 0)
                                    break;
                                nCurCreatCount++;
                                waitCreatInfo = ListBlueLev1WaitTeamInfos[0].listWaitInfo[0];
                                AddNewPlayer(waitCreatInfo.unitCamp, waitCreatInfo.emPathType, waitCreatInfo.szName, waitCreatInfo.szUserUid, waitCreatInfo.nID, waitCreatInfo.emLev, waitCreatInfo.pAddCall);
                                ListBlueLev1WaitTeamInfos[0].listWaitInfo.RemoveAt(0);
                            }

                        }
                        if (ListBlueLev1WaitTeamInfos[0].listWaitInfo.Count <= 0)
                        {
                            ListBlueLev1WaitTeamInfos.RemoveAt(0);
                        }
                    }
                    if(ListBlueWaitTeamInfos.Count > 0 ? bMaxLev3 : true &&
                       ListBlueLev1WaitTeamInfos.Count > 0 ? bMaxLev1 : true &&
                       ListBlueLev2WaitTeamInfos.Count > 0 ? bMaxLev2 : true)
                        break;
                    if (nCurCreatCount >= nOnceCheckCount)
                        break;
                }
                   
               
            }
        }
    }

    public void CreatLikesTeamUnit()
    {
        if (listLikesInfos.Count <= 0) return;
        for (int i = 0; i < listLikesInfos.Count; i++)
        {
            CreatLikesSoliders(listLikesInfos[i].emCamp, listLikesInfos[i].emPathType);
        }
    }

    /// <summary>
    /// 双方获取随机的阵营
    /// </summary>
    public void GetRandomCampInfo()
    {
        List<CampInfo> listCampInfos = new List<CampInfo>();
        listCampInfos.AddRange(CampInfoMgr.Ins.pCampInfos);

        //pRedCamp = new CampInfo(listCampInfos[0]);
        //pBlueCamp = new CampInfo(listCampInfos[2]);
        //return;

        int nRandomIdx = Random.Range(0, listCampInfos.Count);
        pRedCamp = new CampInfo(listCampInfos[nRandomIdx]);
        listCampInfos.RemoveAt(nRandomIdx);

        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen ||
           CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS)
        {
            if(pRedCamp.emCamp == EMCamp.Camp1 ||
               pRedCamp.emCamp == EMCamp.Camp2)
            {
                pBlueCamp = new CampInfo(listCampInfos[1]);
            }
            else
            {
                nRandomIdx = Random.Range(0, listCampInfos.Count);
                pBlueCamp = new CampInfo(listCampInfos[nRandomIdx]);
            }
        }
        else
        {
            nRandomIdx = Random.Range(0, listCampInfos.Count);
            pBlueCamp = new CampInfo(listCampInfos[nRandomIdx]);
        }
    }

    /// <summary>
    /// 设置双方固定阵营
    /// </summary>
    /// <param name="redCamp"></param>
    /// <param name="blueCamp"></param>
    public void SetBattleCamp(int redCamp, int blueCamp)
    {
        pRedCamp = new CampInfo(CampInfoMgr.Ins.pCampInfos[redCamp]);
        pBlueCamp = new CampInfo(CampInfoMgr.Ins.pCampInfos[blueCamp]);
    }

    /// <summary>
    /// 对应的阵营生成对应的兵种
    /// </summary>
    /// <param name="unitCamp"></param>
    /// <param name="pCamp"></param>
    public void CreatUnit(EMUnitCamp unitCamp,CampInfo pCamp)
    {
        ///判断对应阵营的基地是否被摧毁
        if (unitCamp == EMUnitCamp.Blue &&
            mapMgr.pBlueBase.IsDead())
        {
            return;
        }
        else if (unitCamp == EMUnitCamp.Red &&
                 mapMgr.pRedBase.IsDead())
        {
            return;
        }
        //Debug.LogError(unitCamp + "=====" + pCamp.bUpDead + "====" + pCamp.bCenterDead + "====" + pCamp.bDownDead);
        //Debug.LogError(emUpUnitLev + "====" + emCenterUnitLev + "====" + emDownUnitLev + "====" + unitCamp);
        ///生成对应的兵种
        AutoCreatSoliders(unitCamp, pUpCreatInfo, pCamp.bUpDead);
        AutoCreatSoliders(unitCamp, pCenterCreatInfo, pCamp.bCenterDead);
        AutoCreatSoliders(unitCamp, pDownCreatInfo, pCamp.bDownDead);
    }

    void AutoCreatSoliders(EMUnitCamp unitCamp, CAutoCreatInfo[] creatInfos,bool bBarrackDead)
    {
        string szCamp = (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
        bool bCreatInfo = false;
        for (int i = 0; i < creatInfos.Length; i++)
        {
            if (creatInfos[i].bNeedBarrckDead)
            {
                if (bBarrackDead)
                {
                    bCreatInfo = true;
                }
                else
                {
                    bCreatInfo = false;
                }
            }
            else
            {
                bCreatInfo = true;
            }
            if (bCreatInfo)
            {
                for (int j = 0; j < creatInfos[i].nNum; j++)
                {
                    AddNewPlayer(unitCamp, creatInfos[i].emPathType, creatInfos[i].szPrefab + szCamp, "", 0, EMUnitLev.Lv1);
                }
            }
        }
    }

    void CreatLikesSoliders(EMUnitCamp unitCamp,EMStayPathType pathType)
    {
        CBaseUnit baseUnit = null;
        if (unitCamp == EMUnitCamp.Blue)
        {
            baseUnit = mapMgr.pBlueBase;
        }
        else if (unitCamp == EMUnitCamp.Red)
        {
            baseUnit = mapMgr.pRedBase;
        }
        switch (pathType)
        {
            case EMStayPathType.Up:
                {
                    if (baseUnit.pCampInfo.bUpDead)
                    {
                        for (int i = 0; i < pUpLikesCreatInfo.Length; i++)
                        {
                            for (int j = 0; j < pUpLikesCreatInfo[i].nNum; j++)
                            {
                                AddNewPlayer(unitCamp, pUpLikesCreatInfo[i].emPathType, pUpLikesCreatInfo[i].szPrefab + (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                                                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName));
                            }
                        }
                       
                    }
                }
                break;
            case EMStayPathType.Center:
                {
                    if (baseUnit.pCampInfo.bCenterDead)
                    {
                        for (int i = 0; i < pCenterLikesCreatInfo.Length; i++)
                        {
                            for (int j = 0; j < pCenterLikesCreatInfo[i].nNum; j++)
                            {
                                AddNewPlayer(unitCamp, pCenterLikesCreatInfo[i].emPathType, pCenterLikesCreatInfo[i].szPrefab + (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                                                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName));
                            }
                        }
                    }
                }
                break;
            case EMStayPathType.Down:
                {
                    if (baseUnit.pCampInfo.bDownDead)
                    {
                        for (int i = 0; i < pDownLikesCreatInfo.Length; i++)
                        {
                            for (int j = 0; j < pDownLikesCreatInfo[i].nNum; j++)
                            {
                                AddNewPlayer(unitCamp, pDownLikesCreatInfo[i].emPathType, pDownLikesCreatInfo[i].szPrefab + (unitCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                                                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName));
                            }
                        }
                    }
                }
                break;
        }
    }

    public override void OnUpdateLogic()
    {
        CheckCountDown();
        CheckWaitTeamTime();
        CheckLikesTeamTime();
        CheckCreatTime();
    }

    public bool bUpSpeed;
    public float fUpValue;
    private void Update()
    {
        //CheckCreatTime();
#if UNITY_EDITOR
        TestEvent();
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(bUpSpeed)
            {
                CTimeMgr.TimeScale = 1f;
            }
            else
            {
                CTimeMgr.TimeScale = fUpValue;
            }
            bUpSpeed = !bUpSpeed;
        }
#endif
    }

    /// <summary>
    /// 增加新的单位
    /// </summary>
    /// <param name="unitCamp"></param>
    /// <param name="szName"></param>
    public void AddNewPlayer(EMUnitCamp unitCamp,EMStayPathType pathType,string szResName,string userUid = "",int nID = 0,EMUnitLev lev = EMUnitLev.Lv1, DelegateUnituncCall unitAddCall = null)
    {
        //Debug.LogError(lev + "===" + CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, unitCamp) + "==Lv1 Count===" +
        //                   CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, unitCamp) + "==Lv2 Count===" +
        //                   CPlayerMgr.Ins.GetAliveCountHighLev(unitCamp) + "==Lv3 Count===");
        
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (lev == EMUnitLev.Lv1)
            {
                if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, unitCamp) >= nNormalLev1)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
            else if (lev == EMUnitLev.Lv2)
            {
                if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, unitCamp) >= nNormalLev2)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
            else
            {
                if (CPlayerMgr.Ins.GetAliveCountHighLev(unitCamp) >= nNormalLev3)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if (lev == EMUnitLev.Lv1)
            {
                if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv1, unitCamp) >= nNetLev1)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
            else if (lev == EMUnitLev.Lv2)
            {
                if (CPlayerMgr.Ins.GetAliveCountByLev(EMUnitLev.Lv2, unitCamp) >= nNetLev2)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
            else
            {
                if (CPlayerMgr.Ins.GetAliveCountHighLev(unitCamp) >= nNetLev3)
                {
                    AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
                    return;
                }
            }
        }
       

        ///从生存池子中取出单位
        CPlayerUnit playerUnit = CPlayerMgr.Ins.PopUnit(szResName);
        if (playerUnit == null)
        {
            GameObject objNewPlayer = GameObject.Instantiate(Resources.Load("Soldier/" + szResName) as GameObject);
            playerUnit = objNewPlayer.GetComponent<CPlayerUnit>();
            playerUnit.InitUniqueIdx();
        }

        //Debug.Log("当前索引：" + playerUnit.nUniqueIdx);

        ///设置对应单位的信息
        playerUnit.szUserUid = CHelpTools.GenerateId().ToString();
        MapSlot mapSlot = mapMgr.GetRandomIdleSlot();// GetCreatPos(unitCamp, pathType, playerUnit);
        if(mapSlot == null)
        {
            playerUnit.emCamp = unitCamp;
            CPlayerMgr.Ins.RemovePlayerUnit(playerUnit);
            AddWaitTeam(new CWaitCreatInfo(userUid, unitCamp, pathType, szResName, nID, lev, unitAddCall));
            return;
        }
        if(!CHelpTools.IsStringEmptyOrNone(userUid))
        {
            playerUnit.szUserUid = userUid;
        }
        playerUnit.emPathType = pathType;
        playerUnit.SetMapSlot(mapSlot);
        playerUnit.m_fixv3LogicPosition = mapSlot.GetV64Pos();
        playerUnit.Init(unitCamp);
        CLockStepMgr.Ins.AddLockUnit(playerUnit);
        CPlayerMgr.Ins.AddPlayerUnit(playerUnit);
        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  "
        //          + $"出生者位置【{mapSlot.vecPos.x},{mapSlot.vecPos.y}】"
        //          + $"出生目标索引【{playerUnit.nUniqueIdx}】【出生事件】  ");
        ///发送单位加入的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", (int)unitCamp);
        msg.SetInt("pathtype", (int)pathType);
        msg.SetInt("buffid", playerUnit.pUnitData.nBuffID);
        msg.SetLong("gold", playerUnit.pUnitData.nlGold);
        CGameObserverMgr.SendMsg(CGameObserverConst.UnitJoin, msg);
        unitAddCall?.Invoke(playerUnit);
        return;
    }

    public void PreLoadEvent()
    {
        for(int i = 0;i < nPreLoadCount;i++)
        {
            for(int j = 0;j < szPreLoadSoliders.Length;j++)
            {
                AddPreLoadUnit(szPreLoadSoliders[i] + pRedCamp.szCampName, EMUnitCamp.Red);
                AddPreLoadUnit(szPreLoadSoliders[i] + pBlueCamp.szCampName, EMUnitCamp.Blue);
            }
            //Buff怪单位
            ST_UnitBattleInfo redBuffInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(pRedCamp.nBuffSolderTBLID);
            AddPreLoadUnit(redBuffInfo.szPrefab + pRedCamp.szCampName, EMUnitCamp.Red);
            ST_UnitBattleInfo blueBuffInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(pBlueCamp.nBuffSolderTBLID);
            AddPreLoadUnit(blueBuffInfo.szPrefab + pBlueCamp.szCampName, EMUnitCamp.Blue);
            //英雄单位
            ST_UnitBattleInfo redHeroInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(pRedCamp.nHeroSolderTBLID);
            AddPreLoadUnit(redHeroInfo.szPrefab + pRedCamp.szCampName, EMUnitCamp.Red);
            ST_UnitBattleInfo blueHeroInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(pBlueCamp.nHeroSolderTBLID);
            AddPreLoadUnit(blueHeroInfo.szPrefab + pBlueCamp.szCampName, EMUnitCamp.Blue);
        }
        if (!CHelpTools.IsStringEmptyOrNone(mapMgr.pRedBase.szBoomEffect))
        {
            CEffectMgr.Instance.CreateEffSyncNoPlay(mapMgr.pRedBase.szBoomEffect, new Vector3(-10000f, -10000f, -10000f), Quaternion.identity, 0);
        }
        if (!CHelpTools.IsStringEmptyOrNone(mapMgr.pRedBuilds[0].szBoomEffect))
        {
            CEffectMgr.Instance.CreateEffSyncNoPlay(mapMgr.pRedBuilds[0].szBoomEffect, new Vector3(-10000f, -10000f, -10000f), Quaternion.identity, 0);
        }
    }

    /// <summary>
    /// 预加载对象
    /// </summary>
    /// <param name="szResName"></param>
    public void AddPreLoadUnit(string szResName,EMUnitCamp camp)
    {
        ///从生存池子中取出单位
        //Debug.Log("预加载资源:" + szResName);
        CPlayerUnit playerUnit = CPlayerMgr.Ins.PopUnit(szResName);
        if (playerUnit == null)
        {
            CResLoadMgr.Inst.SynLoad("Soldier/" + szResName, CResLoadMgr.EM_ResLoadType.CanbeUnloadAssetbundle,
                delegate (Object res, object data, bool bSuc)
                {
                    GameObject objNewUnitRoot = res as GameObject;
                    GameObject objNewPlayer = GameObject.Instantiate(objNewUnitRoot) as GameObject;
                    playerUnit = objNewPlayer.GetComponent<CPlayerUnit>();
                    if (!CHelpTools.IsStringEmptyOrNone(playerUnit.szSkillEffect))
                    {
                        CEffectMgr.Instance.CreateEffSyncNoPlay(playerUnit.szSkillEffect, new Vector3(-10000f, -10000f, -10000f), Quaternion.identity, 0);
                    }
                    ///设置对应单位的信息
                    playerUnit.emCamp = camp;
                    playerUnit.InitUniqueIdx();
                    playerUnit.szUserUid = CHelpTools.GenerateId().ToString();
                    CPlayerMgr.Ins.RemovePlayerUnit(playerUnit);
                });

            //GameObject objNewPlayer = GameObject.Instantiate(Resources.Load("Soldier/" + szResName) as GameObject);
            //Transform tranNewPlayer = objNewPlayer.GetComponent<Transform>();
            //playerUnit = objNewPlayer.GetComponent<CPlayerUnit>();
        }
    }

    public EMUnitCamp emTestCamp;
    public EMStayPathType emTestPath;

    public int nCreatNum = 10;
    /// <summary>
    /// 测试用
    /// </summary>
    public void TestEvent()
    {
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl)){
            if (emDisplayType == EMDisPlayName.Name) {
                emDisplayType = EMDisPlayName.Head;
            } else if (emDisplayType == EMDisPlayName.Head) {
                emDisplayType = EMDisPlayName.None;
            }
            else {
                emDisplayType = EMDisPlayName.Name;
            }
            displayChangeEvent?.Invoke(emDisplayType);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            List<CPlayerUnit> playerRedUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(EMUnitCamp.Red);
            for (int i = 0; i < playerRedUnits.Count; i++)
            {
                playerRedUnits[i].OnHit(CBattleMgr.ins.mapMgr.pBlueBase, 9999999);
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            List<CPlayerUnit> playerBlueUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(EMUnitCamp.Blue);
            for (int i = 0; i < playerBlueUnits.Count; i++)
            {
                playerBlueUnits[i].OnHit(CBattleMgr.ins.mapMgr.pRedBase, 9999999);
            }
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            mapMgr.pBlueBase.OnHit(mapMgr.pRedTowers[0], 1000000);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            mapMgr.pRedBase.OnHit(mapMgr.pBlueTowers[0], 1000000);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //MapSlot mapSlot = AStarFindPath.Ins.GetMapSlot(new Vector2Int(94, 48));
            //Fix64 f64Radius = (Fix64)1.7f;
            //Debug.LogError((f64Radius * f64Radius) + "===value");
            //Debug.LogError(mapMgr.pBlueBarracks[0].GetBestMoveTarget(mapSlot.v64SlotPos).vecPos + "===Pos");
            //Debug.LogError("value ===" + FixVector3.SqrMagnitude(mapSlot.v64SlotPos - mapMgr.pBlueBarracks[0].GetBestMoveTarget(mapSlot.v64SlotPos).v64SlotPos));
            emTestCamp = EMUnitCamp.Red;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            emTestCamp = EMUnitCamp.Blue;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            emTestPath = EMStayPathType.Up;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            emTestPath = EMStayPathType.Center;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            emTestPath = EMStayPathType.Down;
        }
        if(Input.GetKeyDown(KeyCode.T))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 10);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);

                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv2);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider1"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(201);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider6"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(202);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider7"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(203);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 3500);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = null;
                    if (player.emCamp == EMUnitCamp.Blue)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nBuffSolderTBLID);
                    }
                    else if (player.emCamp == EMUnitCamp.Red)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nBuffSolderTBLID);
                    }
                    int num = 1;
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);

                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)num, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)num, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 0,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 10000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = null;
                    if (player.emCamp == EMUnitCamp.Blue)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nHeroSolderTBLID);
                    }
                    else if (player.emCamp == EMUnitCamp.Red)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nHeroSolderTBLID);
                    }
                    int num = 1;
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)num, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)num, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 10);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv2);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider1"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Archer, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(201);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider6"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv1, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(202);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider7"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv2, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 1000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    ST_UnitBattleInfo pTBLInfo = CTBLHandlerUnitBattleInfo.Ins.GetInfo(203);

                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", CGameAntGlobalMgr.Ins.pStaticConfig.GetInt("GiftSolider8"));

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)1, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_Lv3, pTBLInfo.szName, (int)1, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 3500);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = null;
                    if (player.emCamp == EMUnitCamp.Blue)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nBuffSolderTBLID);
                    }
                    else if (player.emCamp == EMUnitCamp.Red)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nBuffSolderTBLID);
                    }
                    int num = 1;
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);

                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)num, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Soldier_BoxLv2, pTBLInfo.szName, (int)num, 0));
                    }
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0, 1,
            delegate ()
            {
                CPlayerBaseInfo player = CPlayerMgr.Ins.GetPlayer(uid);
                if (player != null)
                {
                    ///发送基地经验增加的监听事件
                    CLocalNetMsg msg = new CLocalNetMsg();
                    msg.SetInt("camp", (int)player.emCamp);
                    msg.SetInt("exp", 10000);
                    CGameObserverMgr.SendMsg(CGameObserverConst.AddBaseExp, msg);
                    player.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    ST_UnitBattleInfo pTBLInfo = null;
                    if (player.emCamp == EMUnitCamp.Blue)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nHeroSolderTBLID);
                    }
                    else if (player.emCamp == EMUnitCamp.Red)
                    {
                        pTBLInfo = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nHeroSolderTBLID);
                    }
                    int num = 1;
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", player.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)player.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)player.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", pTBLInfo.nID);
                    pLSEvent.msgParams.SetString("nickname", player.userName);
                    pLSEvent.msgParams.SetString("headIcon", player.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", player.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);

                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                    //送礼动画
                    if (player.emCamp == EMUnitCamp.Red)
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)num, 0));
                    }
                    else
                    {
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右送精英怪, new IIPlayerSendGift(player, CDanmuGiftConst.Hero, pTBLInfo.szName, (int)num, 0));
                    }
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0,0,
            delegate ()
            {
                CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(uid);
                if (baseInfo != null)
                {
                    baseInfo.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    int num = CLockStepMgr.Ins.GetRandomInt(1, 15);
                    int nID = 0;
                    int nRandomValue = CLockStepMgr.Ins.GetRandomInt(1, 100);
                    if (nRandomValue < 20)
                    {
                        nID = 101;
                    }
                    else if (nRandomValue < 40)
                    {
                        nID = 102;
                    }
                    else if (nRandomValue < 60)
                    {
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv3).nID;
                    }
                    else if (nRandomValue < 80)
                    {
                        num = 1;
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nBuffSolderTBLID).nID;
                    }
                    else
                    {
                        num = 1;
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nHeroSolderTBLID).nID;
                    }
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", baseInfo.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)baseInfo.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)baseInfo.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", nID);
                    pLSEvent.msgParams.SetString("nickname", baseInfo.userName);
                    pLSEvent.msgParams.SetString("headIcon", baseInfo.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", baseInfo.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);
                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                }
            });
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            string uid = CHelpTools.GenerateId().ToString();
            //先登录
            CGameAntGlobalMgr.Ins.LoginPlayer(uid, GetRandomName(), GetRandomIcon(), 0,1,
            delegate ()
            {
                CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(uid);
                if (baseInfo != null)
                {
                    baseInfo.emPathType = (EMStayPathType)CLockStepMgr.Ins.GetRandomInt(0, 3);
                    int num = CLockStepMgr.Ins.GetRandomInt(1, 15);
                    int nID = 0;
                    int nRandomValue = CLockStepMgr.Ins.GetRandomInt(1, 100);
                    if(nRandomValue < 20)
                    {
                        nID = 101;
                    }
                    else if (nRandomValue < 40)
                    {
                        nID = 102;
                    }
                    else if (nRandomValue < 60)
                    {
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetRandomUnitInfoByLev(EMUnitLev.Lv3).nID;
                    }
                    else if (nRandomValue < 80)
                    {
                        num = 1;
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nBuffSolderTBLID).nID;
                    }
                    else
                    {
                        num = 1;
                        nID = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nHeroSolderTBLID).nID;
                    }
                    CLockStepEvent_CreateManySoldier pLSEvent = new CLockStepEvent_CreateManySoldier();
                    pLSEvent.msgParams.SetString("uid", baseInfo.uid);
                    pLSEvent.msgParams.SetInt("camp", (int)baseInfo.emCamp);
                    pLSEvent.msgParams.SetInt("path", (int)baseInfo.emPathType);
                    pLSEvent.msgParams.SetInt("tblId", nID);
                    pLSEvent.msgParams.SetString("nickname", baseInfo.userName);
                    pLSEvent.msgParams.SetString("headIcon", baseInfo.userFace);
                    pLSEvent.msgParams.SetLong("vipLv", baseInfo.guardLevel);
                    pLSEvent.msgParams.SetLong("num", num);
                    CLockStepMgr.Ins.AddLSEvent(pLSEvent);
                }
            });
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            for (int i = 0; i < nCreatNum; i++)
            {
                string szPrefab = "soldier_small_001" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                   CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv1);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            for (int i = 0; i < nCreatNum; i++)
            {
                string szPrefab = "soldier_small_002" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                   CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv2);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            for (int i = 0; i < nCreatNum; i++)
            {
                string szPrefab = "soldier_middle_001" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                   CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv3);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            for (int i = 0; i < nCreatNum; i++)
            {
                string szPrefab = "soldier_middle_002" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                  CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv3);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            for (int i = 0; i < nCreatNum; i++)
            {
                string szPrefab = "soldier_middle_003" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                                                  CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv3);
            }
        }
        //if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    for (int i = 0; i < nCreatNum; i++)
        //    {
        //        string szPrefab = "soldier_middle_004" + (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
        //                                                                          CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
        //        AddNewPlayer(emTestCamp, emTestPath, szPrefab);
        //    }
        //}
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            //for (int i = 0; i < nCreatNum; i++)
            //{
                string szPrefab = "";
                if (emTestCamp == EMUnitCamp.Blue)
                {
                    szPrefab = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nBuffSolderTBLID).szPrefab;
                }
                else if (emTestCamp == EMUnitCamp.Red)
                {
                    szPrefab = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nBuffSolderTBLID).szPrefab;
                }
                szPrefab += (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv4);
            //}
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            //for (int i = 0; i < nCreatNum; i++)
            //{
                string szPrefab = "";
                if (emTestCamp == EMUnitCamp.Blue)
                {
                    szPrefab = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.nHeroSolderTBLID).szPrefab;
                }
                else if (emTestCamp == EMUnitCamp.Red)
                {
                    szPrefab = CTBLHandlerUnitBattleInfo.pIns.GetInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.nHeroSolderTBLID).szPrefab;
                }
                szPrefab += (emTestCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.szCampName :
                                                       CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.szCampName);
                AddNewPlayer(emTestCamp, emTestPath, szPrefab, "", 0, EMUnitLev.Lv5);
            //}
        }

        if (text != null)
            text.text = CPlayerMgr.Ins.GetAllAliveCount().ToString();
    }

}
