using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSendGiftSpecialComp : MonoBehaviour
{
    public GameObject[] BGCamps;
    public GameObject[] SoldierCamps;
    public RawImage iconHead;
    public Text playerName;
    public Text itemName;
    public Text itemName_BG;
    private int realItemNumber;
    public CanvasGroup cg;
    private bool redOrBlue;
    public Animator anim;
    public UITweenAlpha tween;
    public void Init(Color playerColor, string playerFace, string playerName, string itemName, CDanmuGiftConst giftType, int itemNumber, bool leftRight/*, int blindBox = 0*/)
    {
        EMCamp camp = leftRight ? CBattleMgr.Ins.pRedCamp.emCamp : CBattleMgr.Ins.pBlueCamp.emCamp;
        if (camp == EMCamp.Camp1)
        {
            BGCamps[0].gameObject.SetActive(true);
            switch (giftType)
            {
                case CDanmuGiftConst.Soldier_Lv1:
                    {
                        SoldierCamps[0].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv2:
                    {
                        SoldierCamps[3].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv3:
                    {
                        SoldierCamps[6].gameObject.SetActive(true);
                    }
                    break;
            }

            
        }
        else if (camp == EMCamp.Camp2)
        {
            BGCamps[1].gameObject.SetActive(true);
            switch (giftType)
            {
                case CDanmuGiftConst.Soldier_Lv1:
                    {
                        SoldierCamps[1].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv2:
                    {
                        SoldierCamps[4].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv3:
                    {
                        SoldierCamps[7].gameObject.SetActive(true);
                    }
                    break;
            }
        }
        else if (camp == EMCamp.Camp3)
        {
            BGCamps[2].gameObject.SetActive(true);
            switch (giftType)
            {
                case CDanmuGiftConst.Soldier_Lv1:
                    {
                        SoldierCamps[2].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv2:
                    {
                        SoldierCamps[5].gameObject.SetActive(true);
                    }
                    break;
                case CDanmuGiftConst.Soldier_Lv3:
                    {
                        SoldierCamps[8].gameObject.SetActive(true);
                    }
                    break;
            }
        }
        CAysncImageDownload.Ins.setAsyncImage(playerFace, iconHead);
        this.playerName.text = playerName;
        this.itemName.text = leftRight ? CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "summon") + " " + itemName + " x " : " x " + itemName + " "+ CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "summon");
        this.itemName_BG.text = "1";

        this.cg.alpha = 0;
        this.realItemNumber = itemNumber;
        this.redOrBlue = leftRight;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        float animTime;
        if (redOrBlue)
        {
            anim.CrossFadeInFixedTime("PlayerSendGiftRedRare_01_appear", 0);
            animTime = CGameEffMgr.GetAnimatorLength(anim, "PlayerSendGiftRedRare_01_appear");

        }
        else
        {
            anim.CrossFadeInFixedTime("PlayerSendGiftBlueRare_01_appear", 0);
            animTime = CGameEffMgr.GetAnimatorLength(anim, "PlayerSendGiftBlueRare_01_appear");
        }
        yield return new WaitForSeconds(0.2f);
        CHelpTools.NumJump(itemName_BG, 1, realItemNumber, "{0}", 1);
        yield return new WaitForSeconds(animTime - 0.2f + 0.5f);
        tween.enabled = true;
        tween.Play(() =>
        {
            Destroy(this.gameObject);
        });
    }
}
