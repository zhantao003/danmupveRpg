using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIGameInfo : UIBase
{
    public bool isPvPScene;
    [Header("PVP展示")]
    public GameObject leftPlayer;
    public GameObject rightPlayer;
    public RawImage img_LeftPlayerHead;
    public RawImage img_RightPlayerHead;
    public Text txt_LeftPlayerName;
    public Text txt_RightPlayerName;

    public GameObject leftPlayerInFight;
    public GameObject rightPlayerInFight;
    public RawImage img_LeftPlayerHeadInFight;
    public RawImage img_RightPlayerHeadInFight;
    public Text txt_LeftPlayerNameInFight;
    public Text txt_RightPlayerNameInFight;

    [Header("老虎机")]
    public Image slotBG;
    public CSlotMachineController redSlot;
    public CSlotMachineController blueSlot;
    public Animator slotAnim;
    public Animator battleStartAnim;
    public Image[] campHeroImgsLeft;
    public Image[] campHeroImgsRight;
    public Image campTileImg;
    public Image img_battleStartCampHeadLeft;
    public Image img_battleStartCampHeadRight;
    public Image img_battleStartCampBGLeft;
    public Image img_battleStartCampBGRight;
    public Image img_battleStartCampNameBGLeft;
    public Image img_battleStartCampNameBGRight;
    public Text txt_CampNameLeft;
    public Text txt_CampNameRight;
    public Text txt_LeftBuff;
    public Text txt_RightBuff;

    public Animator startFightAnim;

    [Header("插入信息TS")]
    public Transform ts_LeftJoin;
    public Transform ts_RightJoin;
    public Transform ts_LeftAchivePoint;
    public Transform ts_RightAchivePoint;
    public Transform ts_LeftCampLevelAchive;
    public Transform ts_RightCampLevelAchive;
    public Transform ts_LeftBuildDestroy;
    public Transform ts_RightBuildDestroy;
    public Transform ts_LeftSendGift;
    public Transform ts_RightSendGift;
    public Transform ts_LeftSendBlindBox;
    public Transform ts_RightSendBlindBox;
    public Transform ts_LeftSendRare;
    public Transform ts_RightSendRare;
    public Transform ts_LeftSendHero;
    public Transform ts_RightSendHero;
    public Transform ts_ComboKill;
    public UITopPlayerJoinComp c_TopPlayerJoin;

    [Header("界面信息")]
    public Text txt_TimeRemain;
    public Text txt_LeftForce;
    public Text txt_RightForce;
    public Text txt_uiRedLev;
    public Text txt_uiRedAddPro;
    public Image img_uiRedAddPro;
    public Text txt_uiBlueLev;
    public Text txt_uiBlueAddPro;
    public Image img_uiBlueAddPro;
    public Image img_CampHeadBGLeft;
    public Image img_CampHeadBGRight;
    public Image img_CampHeadLeft;
    public Image img_CampHeadRight;

    public GameObject hp_Left;
    public GameObject hp_Right;
    public GameObject hp_LeftSpe;
    public GameObject hp_RightSpe;
    public Image img_HpSliLeftSpe;
    public Image img_HpSliRightSpe;
    public Image img_HpSliLeft;
    public Image img_HpSliRight;
    public Text txt_HpLeft;
    public Text txt_HpRight;
    public Image img_ExpSliLeft;
    public Image img_ExpSliRight;
    public Text txt_ExpLeft;
    public Text txt_ExpRight;

    public UIPlayerScoreTop3Comp c_LeftTop3;
    public UIPlayerScoreTop3Comp c_RightTop3;

    public Image pLeftImg;
    public Image pRightImg;
    public Image pSixImg;

    public Sprite[] pColorSixImg;
    public Sprite[] pColorLeftImg;
    public Sprite[] pColorRightImg;
    public Sprite[] pBuffImg;

    public GameObject[] objNets;
    public RawImage pLeftPlyaerImg;
    public RawImage pRightPlyaerImg;

    public override void OnOpen()
    {
        if (CBattleMgr.Ins.emGameType == CBattleMgr.EMGameType.TimeCountDown)
        {
            CBattleMgr.Ins.delCountDownEvent = SetTimeRemain;
            CBattleMgr.Ins.delCountDownEventNet = SetTimeRemainByNet;
        }
        if (objNets != null)
        {
            for (int i = 0; i < objNets.Length; i++)
            {
                objNets[i].SetActive(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP);
            }
        }

        img_uiBlueAddPro.gameObject.SetActive(false);
        img_uiRedAddPro.gameObject.SetActive(false);
        if (CBattleMgr.Ins.bActiveSpecialHP)
        {
            hp_Left.gameObject.SetActive(false);
            hp_Right.gameObject.SetActive(false);
            hp_LeftSpe.gameObject.SetActive(true);
            hp_RightSpe.gameObject.SetActive(true);

            img_HpSliLeft = img_HpSliLeftSpe;
            img_HpSliRight = img_HpSliRightSpe;

            img_HpSliLeft.sprite = CBattleMgr.Ins.pRedCamp.pHpSpriteSpe;
            img_HpSliRight.sprite = CBattleMgr.Ins.pBlueCamp.pHpSpriteSpe;
        }
        else {
            hp_Left.gameObject.SetActive(true);
            hp_Right.gameObject.SetActive(true);
            hp_LeftSpe.gameObject.SetActive(false);
            hp_RightSpe.gameObject.SetActive(false);
            img_HpSliLeft.sprite = CBattleMgr.Ins.pRedCamp.pHpSprite;
            img_HpSliRight.sprite = CBattleMgr.Ins.pBlueCamp.pHpSprite;
        }

        if (isPvPScene) {
            ERoom.RoomSlot left = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(0);
            if (left != null) {
                leftPlayer.gameObject.SetActive(true);
                leftPlayerInFight.gameObject.SetActive(true);
                CAysncImageDownload.Ins.setAsyncImage(left.player.szHeadIcon, img_LeftPlayerHead);
                CAysncImageDownload.Ins.setAsyncImage(left.player.szHeadIcon, img_LeftPlayerHeadInFight);

                txt_LeftPlayerName.text = left.player.szNickName;
                txt_LeftPlayerNameInFight.text = left.player.szNickName;
            }

            ERoom.RoomSlot right = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerBySeat(1);
            if (right != null)
            {
                rightPlayer.gameObject.SetActive(true);
                rightPlayerInFight.gameObject.SetActive(true);
                CAysncImageDownload.Ins.setAsyncImage(right.player.szHeadIcon, img_RightPlayerHead);
                CAysncImageDownload.Ins.setAsyncImage(right.player.szHeadIcon, img_RightPlayerHeadInFight);

                txt_RightPlayerName.text = right.player.szNickName;
                txt_RightPlayerNameInFight.text = right.player.szNickName;
            }
        }

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            pLeftImg.sprite = pColorLeftImg[(int)CBattleMgr.Ins.pRedCamp.emCamp];
            pRightImg.sprite = pColorRightImg[(int)CBattleMgr.Ins.pBlueCamp.emCamp];
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            if(EUserInfoMgr.Ins.emSelfCamp == EMUnitCamp.Red)
            {
                pSixImg.sprite = pColorSixImg[(int)CBattleMgr.Ins.pRedCamp.emCamp];
            }
            else
            {
                pSixImg.sprite = pColorSixImg[(int)CBattleMgr.Ins.pBlueCamp.emCamp];
            }
        }
        CBattleMgr.Ins.mapMgr.pRedBase.delBattleChg = SetLeftForce;
        CBattleMgr.Ins.mapMgr.pBlueBase.delBattleChg = SetRightForce;
    }

    long lastSetForceLeft = 0;
    void SetLeftForce(long force,EMUnitCamp camp) {
        CHelpTools.NumJump(txt_LeftForce, (int)lastSetForceLeft, (int)force, "{0}",0.5f);
        lastSetForceLeft = force;
    }

    long lastSetForceRight = 0;
    void SetRightForce(long force, EMUnitCamp camp)
    {
        CHelpTools.NumJump(txt_RightForce, (int)lastSetForceRight, (int)force, "{0}", 0.5f);
        lastSetForceRight = force;
    }

    void SetTimeRemain(float pastTime,float totalTime) {
        txt_TimeRemain.text = CHelpTools.GetTimeCounter(Mathf.FloorToInt(totalTime - pastTime));
    }

    /// <summary>
    /// 设置时间
    /// </summary>
    public void SetTimeRemainByNet(float time)
    {
        txt_TimeRemain.text = CHelpTools.GetTimeCounter(Mathf.FloorToInt(time / 1000));
    }

    /// <summary>
    /// 刷新基地信息
    /// </summary>
    /// <param name="bRed"></param>
    public void RefreshBaseInfo(bool bRed)
    {
        if (bRed)
        {
            img_uiRedAddPro.gameObject.SetActive(true);
            int buffID = ((int)CBattleMgr.Ins.pRedCamp.emCamp + 1);
            txt_uiRedLev.text = /*"Lv" +*/ CBattleMgr.Ins.mapMgr.pRedBase.nLev.ToString();
            List<CPlayerUnit> playerUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(EMUnitCamp.Red);
            img_uiRedAddPro.sprite = pBuffImg[(int)CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp];
            if (CBattleMgr.Ins.mapMgr.pRedBase.GetBuffLayer(buffID) > 0)
            {
                txt_uiRedAddPro.text = "x" + (CBattleMgr.Ins.mapMgr.pRedBase.GetAddValueByCamp() + CBattleMgr.Ins.mapMgr.pRedBase.GetBuffNumPer(buffID));
                //txt_uiRedAddPro.text = CBattleMgr.Ins.mapMgr.pRedBase.GetAddProByCamp() + "(" + "光环+" + CBattleMgr.Ins.mapMgr.pRedBase.GetBuffNumPer(buffID) + "%)";
            }
            else
            {
                txt_uiRedAddPro.text = "x" + CBattleMgr.Ins.mapMgr.pRedBase.GetAddValueByCamp();
                //txt_uiRedAddPro.text =/*"阵营效果:"+*/ CBattleMgr.Ins.mapMgr.pRedBase.GetAddProByCamp();
            }
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP &&
                pLeftPlyaerImg != null)
            {
                if (EUserInfoMgr.Ins.emSelfCamp == EMUnitCamp.Red)
                {
                    CAysncImageDownload.Ins.setAsyncImage(ERoomInfoMgr.Ins.pSelfRoom.GetSelfSlot().player.szHeadIcon, pLeftPlyaerImg);
                }
                else
                {
                    CAysncImageDownload.Ins.setAsyncImage(ERoomInfoMgr.Ins.pSelfRoom.GetEnemySlot().player.szHeadIcon, pLeftPlyaerImg);
                }
            }
        }
        else
        {
            img_uiBlueAddPro.gameObject.SetActive(true);
            int buffID = ((int)CBattleMgr.Ins.pBlueCamp.emCamp + 1);
            txt_uiBlueLev.text = /*"Lv" + */CBattleMgr.Ins.mapMgr.pBlueBase.nLev.ToString();
            List<CPlayerUnit> playerUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(EMUnitCamp.Blue);
            img_uiBlueAddPro.sprite = pBuffImg[(int)CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp];
            if (CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffLayer(buffID) > 0)
            {
                txt_uiBlueAddPro.text = (CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp()+ CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(buffID)) + "x";
                //txt_uiBlueAddPro.text = CBattleMgr.Ins.mapMgr.pBlueBase.GetAddProByCamp() + "(" + "光环+" + CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(buffID) + "%)";
            }
            else
            {
                txt_uiBlueAddPro.text =CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp() + "x";
                //txt_uiBlueAddPro.text =/* "阵营效果:" +*/ CBattleMgr.Ins.mapMgr.pBlueBase.GetAddProByCamp();
            }
            if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP && 
                pRightPlyaerImg != null)
            {
                if (EUserInfoMgr.Ins.emSelfCamp == EMUnitCamp.Blue)
                {
                    CAysncImageDownload.Ins.setAsyncImage(ERoomInfoMgr.Ins.pSelfRoom.GetSelfSlot().player.szHeadIcon, pRightPlyaerImg);
                }
                else
                {
                    CAysncImageDownload.Ins.setAsyncImage(ERoomInfoMgr.Ins.pSelfRoom.GetEnemySlot().player.szHeadIcon, pRightPlyaerImg);
                }
            }
        }
    }
}
