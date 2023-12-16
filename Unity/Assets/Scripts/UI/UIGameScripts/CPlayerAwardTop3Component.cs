using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CPlayerAwardTop3Component : MonoBehaviour
{
    public CanvasGroup alphaGroup;
    public Text winTimesTxt;
    public RawImage playerFace;
    public Text playerName;
    public Text playerPoint;
    public Text playerTotalPoint;

    public Text rank;
    public Slider expSlider;

    public GameObject tipPoint;
    public Text txt_PointDes;

    private float fillStartAt;
    private float fillEndAt;
    private float fillSpeed;

    private bool inited = false;
    public bool show = true;

    public void Init(CPlayerBaseInfo playerInfo, GameResultResponseCPRContent roundInfo, int roundRank, int rank, int winTimes, int maxExp)
    {
        alphaGroup.alpha = 0;
        expSlider = GetComponentInChildren<Slider>();
        playerName.color = playerInfo.emCamp == EMUnitCamp.Red ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
        playerName.text = playerInfo.userName;
        CAysncImageDownload.Ins.setAsyncImage(playerInfo.userFace, playerFace);

        if (winTimes > 1)
        {
            winTimesTxt.gameObject.SetActive(true);
            winTimesTxt.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "sheng") + " x" + winTimes;
        }
        else
        {
            winTimesTxt.gameObject.SetActive(false);
        }

        if (rank <= 1000)
        {
            this.rank.text = rank + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "ming");
        }
        else
        {
            this.rank.text = "1000+" + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "ming");
        }
        long baseExp = (long)(playerInfo.nGameEarnExp / (1 + (winTimes - 1) * 0.03f));
        CHelpTools.NumJump(playerPoint, 0, (int)playerInfo.nGameEarnExp, CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "benchang"));
        if (winTimes > 1) {
            //tipPoint.gameObject.SetActive(true);
            txt_PointDes.text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "base") + ":" + baseExp + "\n" +CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "sheng")+ ":" + (playerInfo.nGameEarnExp - baseExp);
        }
        CHelpTools.NumJump(playerTotalPoint,(int) playerInfo.nTotalExp -(int)playerInfo.nGameEarnExp, (int)playerInfo.nTotalExp, CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "saiji"));

        inited = true;
        if (maxExp <= 0)
            maxExp = 1;
        fillStartAt = 1.0f * playerInfo.nTotalExp / maxExp;
        expSlider.value = fillStartAt;
        fillEndAt = 1.0f * roundInfo.Exp / maxExp;
        fillSpeed = (fillEndAt - fillStartAt) / 5f;
    }

    private void Update()
    {
        if (!inited)
        {
            alphaGroup.alpha = 0f;
            return;
        }
        alphaGroup.alpha = Mathf.MoveTowards(alphaGroup.alpha, show ? 1 : 0, 2f * CTimeMgr.DeltaTime);
        fillStartAt = Mathf.MoveTowards(fillStartAt, fillEndAt, fillSpeed * CTimeMgr.DeltaTime);
        expSlider.value = fillStartAt % 1f;

    }
}
