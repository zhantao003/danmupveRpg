using ETModel;
using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGameEndResult : UIBase
{
    [Header("胜利弹窗")]
    public GameObject go_Win;
    public GameObject objWinTeamImg;
    public Image[] win_TeamImg;
    public Text txt_winTeamName;
    public Animator anim_Win;

    public CanvasGroup alphaGroup;
    public GameObject awardPanel;
    public GameObject worldRankPanel;
    public CanvasGroup wrGroup;
    public GameObject winerRed;
    public GameObject winerBlue;
    public Text winMemo;

    public Button closeAwardPanelButton;
    public Button nextRoundGameButton;
    public Button backToMenuButton;
    public Button backToMenuButtonNet;

    public Animator endResultAnim;
    public GameObject endResultEff1;
    public GameObject endResultEff2;

    //public Button worldRankQuery;
    //public Button worldRankBack;
    public GameObject[] objAnima;
    public CEffectFramePlayUIImg[] pPlayAnima;

    public List<CPlayerAwardTop3Component> top3Comps;
    public Transform playerAwardContent;
    public PlayerAwardComponent playerAwardItemComponent;
    public Transform playerWorldRankContent_Top3;
    public Transform playerWorldRankContent_Other;
    public PlayerWorldRankComponent playerWorldRankItemComponent;
    //public bool beatMotherShip = false;

    public GameObject objLocal;
    public GameObject objNet;

    public void GameEndRequest(EMUnitCamp loseCamp) {
        UnActiveObjAnima();
        UIManager.Instance.CloseUI(UIResType.GiftEff);
        UIManager.Instance.OpenUI(UIResType.GameEndResult);
        StartCoroutine(ShowWinerWindow(loseCamp));
        objLocal.SetActive(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP);
        objNet.SetActive(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP);
        List<GameResultContent> gameResults = new List<GameResultContent>();
        foreach (var playerInfo in CPlayerMgr.Ins.dicAllPlayers)
        {
            //Debug.Log(playerInfo.Value.userName + "===" +  playerInfo.Value.emCamp + "===Camp===" + EUserInfoMgr.Ins.emSelfCamp);
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            {
                if (playerInfo.Value.emCamp != EUserInfoMgr.Ins.emSelfCamp)
                    continue;
            }
            //Debug.Log(playerInfo.Value.userName + "===" + playerInfo.Value.emCamp + "===Enter    Camp===" + EUserInfoMgr.Ins.emSelfCamp);
            bool win = playerInfo.Value.emCamp != loseCamp;
            if (win) {
                playerInfo.Value.nGameEarnExp = (long)(playerInfo.Value.nGameEarnExp * (1 + playerInfo.Value.nWinTimes * 0.03f));
            }
            GameResultContent grc = new GameResultContent(playerInfo.Key, playerInfo.Value.nGameEarnExp);
            grc.Win = playerInfo.Value.emCamp != loseCamp;

            gameResults.Add(grc);
        }

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if (EUserInfoMgr.Ins.emSelfCamp == loseCamp)
            {
                CTimeTickMgr.Inst.PushTicker(0.5f, delegate (object[] objs)
                 {
                     GameResultRequest req = new GameResultRequest(gameResults);
                     CHttpMgr.Instance.SendHttpMsg(CHttpConst.GameResult, req.GetJsonMsg().GetData(), new HHandlerGetGameResult(), true, 1, true);
                     Actor_LockStepGameEnd_C2M pReq = new Actor_LockStepGameEnd_C2M();
                     SessionComponent.Instance.Session.Send(pReq);
                 });
            }
            else
            {
                GameResultRequest req = new GameResultRequest(gameResults);
                CHttpMgr.Instance.SendHttpMsg(CHttpConst.GameResult, req.GetJsonMsg().GetData(), new HHandlerGetGameResult(), true, 1, true);
                Actor_LockStepGameEnd_C2M pReq = new Actor_LockStepGameEnd_C2M();
                SessionComponent.Instance.Session.Send(pReq);
            }
        }
        else
        {
            GameResultRequest req = new GameResultRequest(gameResults);
            CHttpMgr.Instance.SendHttpMsg(CHttpConst.GameResult, req.GetJsonMsg().GetData(), new HHandlerGetGameResult(), true, 1, true);
        }
    }

    public void PlayAnima(int nIdx)
    {
        for (int i = 0; i < pPlayAnima.Length; i++)
        {
            if (i == nIdx)
            {
                pPlayAnima[i].PlayAnime();
                break;
            }
        }
    }

    public void ActiveObjAnima(int nIdx)
    {
        for (int i = 0; i < objAnima.Length; i++)
        {
            objAnima[i].SetActive(i == nIdx);
        }
    }

    public void UnActiveObjAnima()
    {
        for (int i = 0; i < objAnima.Length; i++)
        {
            objAnima[i].SetActive(false);
        }
    }

    IEnumerator ShowWinerWindow(EMUnitCamp loseCamp) {
        if (loseCamp == EMUnitCamp.Red)
        {
            txt_winTeamName.text = string.Format("<color={0}>{1}</color> "+ CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "win"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName));
            objWinTeamImg.SetActive(true);
            win_TeamImg[(int)CBattleMgr.Ins.pBlueCamp.emCamp].gameObject.SetActive(true);
            ActiveObjAnima((int)CBattleMgr.Ins.pBlueCamp.emCamp);
            PlayAnima((int)CBattleMgr.Ins.pBlueCamp.emCamp);
        }
        else
        {
            txt_winTeamName.text = string.Format("<color={0}>{1}</color> "+ CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "win"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName));
            objWinTeamImg.SetActive(true);
            win_TeamImg[(int)CBattleMgr.Ins.pRedCamp.emCamp].gameObject.SetActive(true);
            ActiveObjAnima((int)CBattleMgr.Ins.pRedCamp.emCamp);
            PlayAnima((int)CBattleMgr.Ins.pRedCamp.emCamp);
        }
        yield return new WaitForSeconds(2.6f);
        go_Win.gameObject.SetActive(true);
        float animTime = CGameEffMgr.GetAnimatorLength(anim_Win, "winerwindoweff");
        if(animTime < 0.5f)
        {
            animTime = 0.5f;
            yield return new WaitForSeconds(animTime);
            UnActiveObjAnima();
        }
        else if(animTime > 0.5f)
        {
            animTime = animTime - 0.5f;
            yield return new WaitForSeconds(0.5f);
            UnActiveObjAnima();
            yield return new WaitForSeconds(animTime);
        }
        StartCoroutine(ShowPanel());
        go_Win.gameObject.SetActive(false);

    }

    List<WorldRankPlayerInfoContent> topPlayer;
    List<WorldRankCombineSamePointRank> topPlayerCombine;
    public void GameEndResponse(GameResultResponse response) {
       
        List<GameResultResponseCPRContent> playerRanks = response.CurrentPlayersRanks;
        topPlayer = response.WorldTopPlayers;
        topPlayerCombine = new List<WorldRankCombineSamePointRank>();
        for (int i = 0; i < topPlayer.Count; i++) {
            WorldRankPlayerInfoContent p = topPlayer[i];
            WorldRankCombineSamePointRank player = new WorldRankCombineSamePointRank();
            player.U = p.U;
            if (i != 0) {
                if (p.E == topPlayer[i - 1].E)
                    player.Rank = topPlayerCombine[i - 1].Rank;
                else
                    player.Rank = i + 1;
            }else
                player.Rank = 1;
            topPlayerCombine.Add(player);
        }
        StartCoroutine(ShowRoundRankPlayers(playerRanks));
    }

    public IEnumerator ShowRoundRankPlayers(List<GameResultResponseCPRContent> playerRanks)
    {
        yield return new WaitUntil(() => !go_Win.gameObject.activeSelf);
        //StartCoroutine(ShowPanel());
        endResultAnim.CrossFadeInFixedTime("Eff", 0f);
        endResultEff1.gameObject.SetActive(true);
        float EffTime = CGameEffMgr.GetAnimatorLength(endResultAnim, "UIGameEndResult_RoundRank_01_appear");
        yield return new WaitForSeconds(EffTime);
        endResultEff2.gameObject.SetActive(true);

        List<CPlayerBaseInfo> playInfos = new List<CPlayerBaseInfo>();
        foreach (var playerInfo in CPlayerMgr.Ins.dicAllPlayers)
        {
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            {
                if (playerInfo.Value.emCamp != EUserInfoMgr.Ins.emSelfCamp)
                    continue;
            }
            Debug.LogWarning("Result Info {uid:" + playerInfo.Value.uid + "} {NickName:" + playerInfo.Value.userName + "}  {IsJoinLeft0Right1:" + (EUserInfoMgr.Ins.emSelfCamp == EMUnitCamp.Red ? 0 : 1).ToString() + "}");
            playInfos.Add(playerInfo.Value);
        }
        playInfos.Sort((a, b) => b.nGameEarnExp.CompareTo(a.nGameEarnExp));
        int maxRankCount = 10;
        int maxPoint = 0;
        if (playInfos.Count > 0)
            maxPoint = (int)playerRanks.Find((x) => x.UId == playInfos[0].uid).Exp;
        List<PlayerAwardComponent> pacs = new List<PlayerAwardComponent>();
        for (int i = 0; i < (playInfos.Count > maxRankCount ? maxRankCount : playInfos.Count); i++)
        {
            CPlayerBaseInfo playerInfo = playInfos[i];
            GameResultResponseCPRContent result = playerRanks.Find((x) => x.UId == playerInfo.uid);
            int worldRank = 1001;
            if (topPlayerCombine.Count > 0)
            {
                WorldRankCombineSamePointRank p = topPlayerCombine.Find((x) => x.U == playerInfo.uid);
                if (p != null)
                    worldRank = p.Rank;
            }

            int wintimes = 0;
            if (playerRanks != null)
            {
                var content = playerRanks.Find(o => o.UId == playerInfo.uid);
                if (content != null)
                {
                    //rank = (int)content.Rank;
                    wintimes = (int)content.WinTimes;
                    if (!playerInfo.hasEarnToToalExp)
                    {
                        playerInfo.nTotalExp += content.Exp;
                        playerInfo.hasEarnToToalExp = true;
                    }
                }
            }

            if (i >= 3)
            {
                PlayerAwardComponent pa = Instantiate(playerAwardItemComponent.gameObject, playerAwardContent).GetComponent<PlayerAwardComponent>();
                if (pacs.Count != 0)
                {
                    if (playerInfo.nTotalExp == pacs[pacs.Count - 1].playerTotalExp)
                        worldRank = pacs[pacs.Count - 1].rankInt + 1;
                }
                pa.Init(playerInfo, result, i + 1, worldRank, wintimes, maxPoint);
                pa.gameObject.SetActive(true);
                pacs.Add(pa);
            }
            else {
                top3Comps[i].Init(playerInfo, result, i + 1, worldRank, wintimes, maxPoint);
            }
          
            yield return new WaitForSeconds(0.2f);
        }

        if (playInfos.Count > maxRankCount)
        {
            StartCoroutine(ShowRest(maxRankCount - 3, playInfos, playerRanks, pacs, maxPoint));
        }
        nextRoundGameButton.onClick.RemoveAllListeners(); 
        nextRoundGameButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            nextRoundGameButton.onClick.RemoveAllListeners();
            CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);
        }));
        backToMenuButton.onClick.RemoveAllListeners();
        backToMenuButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            backToMenuButton.onClick.RemoveAllListeners();
            CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameModeSelect102);
        }));
        backToMenuButtonNet.onClick.RemoveAllListeners();
        backToMenuButtonNet.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
        {
            backToMenuButtonNet.onClick.RemoveAllListeners();

            //TODO:断开网络链接
            if (SessionComponent.Instance != null &&
                SessionComponent.Instance.Session != null)
            {
                SessionComponent.Instance.Session.Dispose();
            }

            CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameModeSelect102);
        }));
        

        //float endCounter = 0;
        //float endTime = 90 + (playInfos.Count - 3f) / maxRankCount * 20;
        //Text endButtonText = nextRoundGameButton.GetComponentInChildren<Text>();
        //while (endCounter < endTime)
        //{
        //    endCounter += CTimeMgr.DeltaTime;
        //    endButtonText.text = "回到主菜单" + "(" + (endTime - endCounter).ToString("F0") + "秒" + ")";
        //    yield return new WaitForEndOfFrame();
        //}
        //CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameModeSelect102);
    }
    private IEnumerator ShowRest(int maxRankCount, List<CPlayerBaseInfo> playerInfos, List<GameResultResponseCPRContent> playerRanks, List<PlayerAwardComponent> playerAwardComponents,int maxPoint)
    {
        int startIndex = maxRankCount + 3;

        while (true)
        {
            yield return new WaitForSeconds(15);
            for (int i = 0; i < playerAwardComponents.Count; i++)
            {
                playerAwardComponents[i].Release();
            }
            yield return new WaitForSeconds(0.6f);
            for (int i = 0; i < playerAwardComponents.Count; i++)
            {
                Destroy(playerAwardComponents[i].gameObject);
                playerAwardComponents.RemoveAt(0);
                i--;
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < (playerInfos.Count > maxRankCount ? maxRankCount : playerInfos.Count); i++)
            {
                if (startIndex + i < playerInfos.Count)
                {
                    CPlayerBaseInfo playerInfo = playerInfos[startIndex + i];

                    int worldRank = 1001;
                    int wintimes = 0;
                    if (playerRanks != null)
                    {
                        if (topPlayerCombine.Count > 0)
                        {
                            WorldRankCombineSamePointRank p = topPlayerCombine.Find((x) => x.U == playerInfo.uid);
                            if (p != null)
                                worldRank = p.Rank;
                        }
                        var content = playerRanks.Find(o => o.UId == playerInfo.uid);
                        if (content != null)
                        {
                            wintimes = (int)content.WinTimes;
                        }
                        if (!playerInfo.hasEarnToToalExp)
                        {
                            playerInfo.nTotalExp += content.Exp;
                            playerInfo.hasEarnToToalExp = true;
                        }
                        PlayerAwardComponent pa = Instantiate(playerAwardItemComponent.gameObject, playerAwardContent).GetComponent<PlayerAwardComponent>();
                        if (playerAwardComponents.Count != 0)
                        {
                            if (playerInfo.nTotalExp == playerAwardComponents[playerAwardComponents.Count - 1].playerTotalExp)
                                worldRank = playerAwardComponents[playerAwardComponents.Count - 1].rankInt + 1;
                        }
                        pa.Init(playerInfo, content, startIndex + i + 1, worldRank, wintimes, maxPoint);
                        pa.gameObject.SetActive(true);
                        playerAwardComponents.Add(pa);
                        yield return new WaitForSeconds(0.2f);
                    }
                }
            }
            startIndex += playerAwardComponents.Count;
            if (startIndex == playerInfos.Count)
            {
                startIndex = 3;
            }
        }
    }
    protected override void OnStart()
    {
        //ShowPanel();
    }

    public IEnumerator ShowPanel()
    {
        alphaGroup.alpha = 0;
        while (alphaGroup.alpha != 1)
        {
            alphaGroup.alpha = Mathf.MoveTowards(alphaGroup.alpha, 1, 2 * CTimeMgr.DeltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    public override void OnOpen()
    {
       
    }

    public override void OnClose()
    {


    }

    protected override void OnUpdate(float dt)
    {

    }




    //List<PlayerInfoContent> responContents = new List<PlayerInfoContent>();
    //bool hasShowWorldRank = false;
    //IEnumerator ShowWorldRanks()
    //{
    //    while (wrGroup.alpha != 1)
    //    {
    //        wrGroup.alpha = Mathf.MoveTowards(wrGroup.alpha, 1, 2 * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }
    //    wrGroup.interactable = true;
    //    wrGroup.blocksRaycasts = true;
    //    if (!hasShowWorldRank)
    //    {
    //        hasShowWorldRank = true;
    //        for (int i = 0; i < responContents.Count; i++)
    //        {
    //            PlayerWorldRankComponent item;
    //            if (i < 3)
    //            {
    //                item = Instantiate(playerWorldRankItemComponent.gameObject, playerWorldRankContent_Top3).GetComponent<PlayerWorldRankComponent>();
    //            }
    //            else
    //                item = Instantiate(playerWorldRankItemComponent.gameObject, playerWorldRankContent_Other).GetComponent<PlayerWorldRankComponent>();
    //            item.Init(i + 1, responContents[i]);
    //            item.gameObject.SetActive(true);
    //            yield return new WaitForSeconds(0.2f);
    //        }
    //    }
    //}


    //public IEnumerator ShowPanel(string titleTxt)
    //{
    //    this.titleText.text = titleTxt;
    //    while (alphaGroup.alpha != 1)
    //    {
    //        alphaGroup.alpha = Mathf.MoveTowards(alphaGroup.alpha, 1, 2 * Time.deltaTime);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}


    //protected int maxRankCount;
    //public virtual IEnumerator SynRoundPlayerInfo(SailTeam whichWin, bool beatMotherShip)
    //{
    //    this.beatMotherShip = beatMotherShip;
    //    bool teamAwin = false;
    //    if (whichWin == MotherShipController.Instance.teamA && whichWin != null) teamAwin = true;
    //    if (whichWin != null)
    //    {
    //        if (teamAwin)
    //        {
    //            winerRed.gameObject.SetActive(true);
    //            winerBlue.gameObject.SetActive(false);
    //        }
    //        else
    //        {
    //            winerRed.gameObject.SetActive(false);
    //            winerBlue.gameObject.SetActive(true);
    //        }
    //    }
    //    else
    //    {
    //        titleText.gameObject.SetActive(true);
    //    }

    //    for (int i = 0; i < MotherShipController.Instance.teamA.players.Count; i++)
    //    {
    //        PlayerPoint pp = MotherShipController.Instance.teamA.players[i].localPlayer.pp;
    //        if (pp == null) continue;
    //        pp.win = teamAwin && whichWin != null;
    //        pp.totalResult = SailMath.CalcPlayerPoint(pp, beatMotherShip ? TotalDatas.Instance.winIfMotherShipSinkMult : TotalDatas.Instance.winTeamExpMult);
    //        pp.totalResult = SailMath.CalcBroadCasterLvToPlayerPoint(pp.totalResult);
    //        //pp.player.exp += (int)pp.totalResult;
    //    }
    //    for (int i = 0; i < MotherShipController.Instance.teamB.players.Count; i++)
    //    {
    //        PlayerPoint pp = MotherShipController.Instance.teamB.players[i].localPlayer.pp;
    //        if (pp == null) continue;
    //        pp.win = !teamAwin && whichWin != null;
    //        pp.totalResult = SailMath.CalcPlayerPoint(pp, beatMotherShip ? TotalDatas.Instance.winIfMotherShipSinkMult : TotalDatas.Instance.winTeamExpMult);
    //        pp.totalResult = SailMath.CalcBroadCasterLvToPlayerPoint(pp.totalResult);
    //        //pp.player.exp += (int)pp.totalResult;
    //    }
    //    List<GameResultContent> contents = new List<GameResultContent>();
    //    for (int i = 0; i < PointController.Instance.playerPoints.Count; i++)
    //    {
    //        GameResultContent content = new GameResultContent(PointController.Instance.playerPoints[i].localPlayer.uId, (long)PointController.Instance.playerPoints[i].totalResult);
    //        content.Win = PointController.Instance.playerPoints[i].win;
    //        contents.Add(content);
    //    }
    //    GameResultRequest request = new GameResultRequest(contents);
    //    List<GameResultResponseCPRContent> playerRanks = null;
    //    int retryCount = 0;
    //    bool responseSucceed = false;
    //    do
    //    {

    //        yield return ServerQuery.GameResult(request, o =>
    //        {
    //            if (o != null)
    //            {
    //                playerRanks = o.CurrentPlayersRanks;
    //                responseSucceed = true;
    //            }
    //            else
    //            {
    //                responseSucceed = false;
    //                retryCount++;
    //            }
    //        });
    //    } while (!responseSucceed && retryCount < 10);

    //    PointController.Instance.playerPoints.Sort((a, b) => b.totalResult.CompareTo(a.totalResult));
    //    //PointController.Instance.playerPoints.Sort((a, b) => b.isChampion.CompareTo(a.isChampion));
    //    this.gameObject.SetActive(true);
    //    //winMemo.text = beatMotherShip ? "时间到，获胜方获得额外" + ((TotalDatas.Instance.winTeamExpMult - 1) * 100).ToString("F0") + "%功勋！并且连胜获取额外功勋！" : "击沉母舰，获胜方获得额外" + ((TotalDatas.Instance.winIfMotherShipSinkMult - 1) * 100).ToString("F0") + "%功勋！并且连胜获取额外功勋！";
    //    winMemo.text = "";
    //    //PlatFormUIController.Instance.uIIndexComponents.winMemo.text = "连胜获取额外功勋！";
    //    yield return new WaitForSeconds(3);
    //    yield return ShowPanel(whichWin != null ? (teamAwin ? "红队获胜" : "蓝队获胜") : "平局");
    //    for (int i = playerAwardContent.childCount - 1; i >= 0; i--)
    //    {
    //        if (i < 2)
    //            continue;
    //        Destroy(playerAwardContent.GetChild(i).gameObject);
    //    }
    //    if (responseSucceed)
    //    {
    //        //排行榜限定最多显示20个人
    //        for (int i = 0; i < (PointController.Instance.playerPoints.Count > 20 ? 20 : PointController.Instance.playerPoints.Count); i++)
    //        {
    //            LocalPlayerModel lpm = PointController.Instance.playerPoints[i].localPlayer;
    //            int rank = 1000;
    //            int wintimes = 0;
    //            if (playerRanks != null)
    //            {
    //                var content = playerRanks.Find(o => o.UId == lpm.uId);
    //                if (content != null)
    //                {
    //                    rank = (int)content.Rank;
    //                    wintimes = (int)content.WinTimes;
    //                    PointController.Instance.playerPoints[i].totalResult = content.Exp - PointController.Instance.playerPoints[i].localPlayer.exp;
    //                    PointController.Instance.playerPoints[i].localPlayer.exp = (int)content.Exp;
    //                }
    //            }
    //        }
    //        PointController.Instance.playerPoints.Sort((a, b) => b.totalResult.CompareTo(a.totalResult));

    //        //if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Douyu || CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
    //        //{
    //        if (ResolutionController.Instance.isWidth)
    //        {
    //            maxRankCount = 13;
    //        }
    //        else
    //            maxRankCount = 10;

    //        List<PlayerAwardComponent> pacs = new List<PlayerAwardComponent>();
    //        for (int i = 0; i < (PointController.Instance.playerPoints.Count > maxRankCount ? maxRankCount : PointController.Instance.playerPoints.Count); i++)
    //        {
    //            LocalPlayerModel lpm = PointController.Instance.playerPoints[i].localPlayer;
    //            int rank = 1000;
    //            int wintimes = 0;
    //            if (playerRanks != null)
    //            {
    //                var content = playerRanks.Find(o => o.UId == lpm.uId);
    //                if (content != null)
    //                {
    //                    rank = (int)content.Rank;
    //                    wintimes = (int)content.WinTimes;
    //                }
    //            }
    //            PlayerAwardComponent pa = Instantiate(playerAwardItemComponent.gameObject, playerAwardContent).GetComponent<PlayerAwardComponent>();
    //            pa.Init(i + 1, PointController.Instance.playerPoints[i], rank, wintimes, beatMotherShip);
    //            pa.gameObject.SetActive(true);
    //            if (i >= 3) pacs.Add(pa);
    //            yield return new WaitForSeconds(0.2f);
    //        }
    //        if (PointController.Instance.playerPoints.Count > maxRankCount)
    //        {
    //            StartCoroutine(ShowRest(maxRankCount - 3, playerRanks, pacs));
    //        }
    //    }

    //    retryCount = 0;
    //    responseSucceed = false;
    //    responContents = new List<PlayerInfoContent>();
    //    int topNum = 100;
    //    GetTopRanksListRequest req = new GetTopRanksListRequest(topNum);
    //    do
    //    {
    //        yield return ServerQuery.GetWorldRankList(req, o =>
    //        {
    //            if (o != null)
    //            {
    //                responContents = o.result;
    //                Debug.Log(responContents.Count);
    //                responseSucceed = true;
    //            }
    //            else
    //            {
    //                responseSucceed = false;
    //                retryCount++;
    //            }
    //        });
    //    } while (!responseSucceed && retryCount < 10);

    //    if (responseSucceed)
    //    {
    //        worldRankQuery.onClick.AddListener(WorldRankQuery);
    //    }
    //    else
    //    {
    //        worldRankQuery.interactable = false;
    //    }
    //    nextRoundGameButton.onClick.AddListener(new UnityEngine.Events.UnityAction(() =>
    //    {
    //        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //        CSceneLoadMgr.Ins.LoadGameSceneToHarbor();
    //    }));
    //    worldRankBack.onClick.AddListener(HideWorldRankPanel);

    //    float endCounter = 0;
    //    float endTime = 90 + (PointController.Instance.playerPoints.Count - 3f) / maxRankCount * 20;
    //    Text endButtonText = nextRoundGameButton.GetComponentInChildren<Text>();
    //    while (endCounter < endTime)
    //    {
    //        endCounter += Time.deltaTime;
    //        //endButtonText.text = "再来一把（" + (endTime - endCounter).ToString("F0") + "秒）";
    //        endButtonText.text = LanguageController.GetStaticText(LanguageStaticTextContent.TextContentType.返回, "回到港口") + "(" + (endTime - endCounter).ToString("F0") + LanguageController.GetDynamicText(LanguageDynamicTextContent.DynamicTextContentType.秒, "秒") + ")";
    //        yield return new WaitForEndOfFrame();
    //    }
    //}
    //private IEnumerator ShowRest(int maxRankCount, List<GameResultResponseCPRContent> playerRanks, List<PlayerAwardComponent> playerAwardComponents)
    //{
    //    int startIndex = maxRankCount + 3;

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(15);
    //        for (int i = 0; i < playerAwardComponents.Count; i++)
    //        {
    //            playerAwardComponents[i].Release();
    //        }
    //        yield return new WaitForSeconds(0.6f);
    //        for (int i = 0; i < playerAwardComponents.Count; i++)
    //        {
    //            Destroy(playerAwardComponents[i].gameObject);
    //            playerAwardComponents.RemoveAt(0);
    //            i--;
    //        }
    //        yield return new WaitForEndOfFrame();
    //        for (int i = 0; i < (PointController.Instance.playerPoints.Count > maxRankCount ? maxRankCount : PointController.Instance.playerPoints.Count); i++)
    //        {
    //            if (startIndex + i < PointController.Instance.playerPoints.Count)
    //            {
    //                LocalPlayerModel lpm = PointController.Instance.playerPoints[startIndex + i].localPlayer;
    //                int rank = 1000;
    //                int wintimes = 0;
    //                if (playerRanks != null)
    //                {
    //                    var content = playerRanks.Find(o => o.UId == lpm.uId);
    //                    if (content != null)
    //                    {
    //                        rank = (int)content.Rank;
    //                        wintimes = (int)content.WinTimes;
    //                    }
    //                }
    //                PlayerAwardComponent pa = Instantiate(playerAwardItemComponent.gameObject, playerAwardContent).GetComponent<PlayerAwardComponent>();
    //                pa.Init(startIndex + i + 1, PointController.Instance.playerPoints[startIndex + i], rank, wintimes, beatMotherShip);
    //                pa.gameObject.SetActive(true);
    //                playerAwardComponents.Add(pa);
    //                yield return new WaitForSeconds(0.2f);
    //            }
    //        }
    //        startIndex += playerAwardComponents.Count;
    //        if (startIndex == PointController.Instance.playerPoints.Count) startIndex = 3;
    //    }
    //}


}
