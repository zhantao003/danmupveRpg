using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITopPlayerJoinComp : MonoBehaviour
{
    private float counter;
    private float lastPlayTime;

    public GameObject[] objTopPlays;
    public CEffectFramePlayUIImg pTopPlay;
    public CEffectFramePlayUIImg pTopPlay2;
    public CEffectFramePlayUIImg pTopPlay3;
    public UITweenBase[] pTween;

    public Image bg;
    public Image bgtop1;
    public RawImage[] avatars;
    public Text[] name;
    public Text[] addInfo;
    public Text rank;
    //public Text want;
    public Text team;
    //public Image joinTeamImg;
    //public Image seal;
    public Transform teamATs;
    public Transform teamBTs;
    //public Image vipIcon;

    public Animator animator;

    
    public void Play(CPlayerBaseInfo pInfo)
    {
        for(int i = 0;i < pTween.Length;i++)
        {
            pTween[i].Play();
        }
        int nShowIdx = 0;
        if (pInfo.nWorldRank <= 10)
        {
            bgtop1.gameObject.SetActive(true);
            bg.gameObject.SetActive(false);
            nShowIdx = 0;
            pTopPlay.PlayAnime();
        }
        else if (pInfo.nWorldRank <= 30)
        {
            nShowIdx = 1;
            bgtop1.gameObject.SetActive(false);
            bg.gameObject.SetActive(true);
            pTopPlay2.PlayAnime();
        }
        else if (pInfo.nWorldRank <= 100)
        {
            nShowIdx = 2;
            bgtop1.gameObject.SetActive(false);
            bg.gameObject.SetActive(true);
            pTopPlay3.PlayAnime();
        }
        ActiveTopPlay(nShowIdx);
        CAysncImageDownload.Ins.setAsyncImage(pInfo.userFace, this.avatars[nShowIdx]);
        //CAysncImageDownload.Ins.setAsyncImage(pInfo.userFace, this.avatar);

        //vipIcon.sprite = null; vipIcon.color = new Color(0, 0, 0, 0);
        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        {
            //switch (pInfo.guardLevel)
            //{
                //case 1: vipIcon.sprite = TotalDatas.Instance.bilibili_Lv1; vipIcon.color = Color.white; break;
                //case 2: vipIcon.sprite = TotalDatas.Instance.bilibili_Lv2; vipIcon.color = Color.white; break;
                //case 3: vipIcon.sprite = TotalDatas.Instance.bilibili_Lv3; vipIcon.color = Color.white; break;
                //default: vipIcon.sprite = null; vipIcon.color = new Color(0, 0, 0, 0); break;
            //}
        }
        else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Douyu)
        {
            //if (playerModel.roomVipIdentity != 0)
            //{
            //    vipIcon.sprite = TotalDatas.Instance.douyuVip; vipIcon.color = Color.white;
            //}
            //else
            //{
            //    vipIcon.sprite = null; vipIcon.color = new Color(0, 0, 0, 0);
            //}
        }

        string name = pInfo.userName;
        string rank = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "world") + pInfo.nWorldRank + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "ming");
        //string want ="悬赏:$" + string.Format("<color={0}>", "#FFB43D") + (pInfo.nTotalExp / 1000 + 1) * 1000 + "000" + "</color>"; ;



        Color color = new Color();
        string team="";
        if (pInfo.emCamp == EMUnitCamp.Red)
        {
            //this.team.color = CBattleMgr.Ins.pRedCamp.pColor;
            if (CBattleMgr.Ins.pRedCamp.emCamp == EMCamp.Camp1)
            {
               team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName);
            }
            else if (CBattleMgr.Ins.pRedCamp.emCamp == EMCamp.Camp2)
            {
                team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName);
            }
            else if (CBattleMgr.Ins.pRedCamp.emCamp == EMCamp.Camp3)
            {
                team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName);
            }
            color = CBattleMgr.Ins.pRedCamp.pColor;
        }
        else {
            //this.team.color = CBattleMgr.Ins.pBlueCamp.pColor;
            if (CBattleMgr.Ins.pBlueCamp.emCamp == EMCamp.Camp1)
            {
                team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName);
            }
            else if (CBattleMgr.Ins.pBlueCamp.emCamp == EMCamp.Camp2)
            {
                team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName);
            }
            else if (CBattleMgr.Ins.pBlueCamp.emCamp == EMCamp.Camp3)
            {
                team = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "join") + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName);
            }
            color = CBattleMgr.Ins.pBlueCamp.pColor;
        }

        //this.team.text = team;
        this.addInfo[nShowIdx].color = color;
        this.addInfo[nShowIdx].text = team;
        this.name[nShowIdx].text = name;
        this.rank.text = rank;
        //this.want.text = want;

        //Color blue;
        //ColorUtility.TryParseHtmlString("#008FFF", out blue);
        //this.team.color = playerModel.teamFlag == TeamFlag.Red ? TotalDatas.Instance.redTeamColor : blue;
        //this.team.transform.SetParent(pInfo.emCamp == EMUnitCamp.Red ? teamATs : teamBTs);
        //this.team.transform.localPosition = Vector3.zero;
        //this.team.transform.localScale = Vector3.one;

        if (pInfo.emCamp == EMUnitCamp.Red)
        {
            animator.Play("topPlayerJoin", 0, 0);
        }
        else
        {
            animator.Play("topPlayerJoinBlue", 0, 0);
        }
    }

    void ActiveTopPlay(int nIdx)
    {
        for(int i= 0;i < objTopPlays.Length;i++)
        {
            objTopPlays[i].SetActive(i == nIdx);
        }
    }

}
