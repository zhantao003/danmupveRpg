using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICampLevelUpComp : MonoBehaviour
{
    public Image headImg;
    public Text playerName;
    public Text itemNumber;
    private int realItemNumber;
    private Vector2 refV2;
    //public CanvasGroup cg;
    private bool cgOn;
    private bool inited = false;
    private RectTransform rTransform;
    private Vector2 target;
    private bool redOrBlue;
    private float currentNumberCounter = 0;
    private float refF1;
    public Animator anim;

    public void Init(Color playerColor, string playerName,int originNumber, int itemNumber, bool redOrBlue, string faceUrl)
    {
        //CAysncImageDownload.Ins.localImageAction(faceUrl, headImg);
        EMCamp camp = redOrBlue ? CBattleMgr.Ins.pRedCamp.emCamp : CBattleMgr.Ins.pBlueCamp.emCamp;
        if (camp == EMCamp.Camp1)
        {
            headImg.sprite = UIScreenInfoInsertionMgr.Ins.Camp1GiftNameToSprite[CDanmuGiftConst.Hero][0];
        }
        else if (camp == EMCamp.Camp2)
        {
            headImg.sprite = UIScreenInfoInsertionMgr.Ins.Camp2GiftNameToSprite[CDanmuGiftConst.Hero][0];
        }
        else if (camp == EMCamp.Camp3)
        {
            headImg.sprite = UIScreenInfoInsertionMgr.Ins.Camp3GiftNameToSprite[CDanmuGiftConst.Hero][0];
        }

        this.playerName.text = playerName;
        this.playerName.color = playerColor;
        this.itemNumber.text = " ";
        this.currentNumberCounter = originNumber;
        this.realItemNumber = itemNumber;
        this.rTransform = GetComponent<RectTransform>();
        //this.cg.alpha = 0;
        this.redOrBlue = redOrBlue;
        StartCoroutine(Play());
    }

    private void Update()
    {
        if (!inited) return;
        //rTransform.anchoredPosition = Vector2.SmoothDamp(rTransform.anchoredPosition, target, ref refV2, 0.2f);
        //cg.alpha = Mathf.MoveTowards(cg.alpha, cgOn ? 1 : 0, 3 * Time.deltaTime);
        currentNumberCounter = Mathf.MoveTowards(currentNumberCounter, realItemNumber, realItemNumber / 2 * Time.deltaTime);
        int i = currentNumberCounter == realItemNumber ? 0 : 1;
        itemNumber.text = ((int)currentNumberCounter + i).ToString();
    }

    private IEnumerator Play()
    {
        this.inited = true;
        cgOn = true;
        anim.CrossFadeInFixedTime("Eff", 0);
        float animTime = CGameEffMgr.GetAnimatorLength(anim, "CampAchiveLevelBlue_01_appear");
        yield return new WaitForSeconds(animTime-0.5f);
        currentNumberCounter = realItemNumber;
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
        //cgOn = false;
        ////target = Vector2.left * width * (redOrBlue ? 1 : -1);
        //while (true)
        //{
        //    if (cg.alpha < 0.01f) Destroy(gameObject);
        //    //if (Vector2.Distance(rt.anchoredPosition, target) < 1) Destroy(gameObject);
        //    yield return new WaitForEndOfFrame();
        //}
    }
}
