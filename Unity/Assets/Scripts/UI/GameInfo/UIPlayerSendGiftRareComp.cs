using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSendGiftRareComp : MonoBehaviour
{
    public GameObject[] BGCamps;
    public GameObject[] SoldierCamps;
    public RawImage iconHead;
    public Text playerName;
    public Text itemName;
    public Text itemNum;
    //public Image giftIcon;
    private int realItemNumber;
    private Vector2 refV2;
    public CanvasGroup cg;
    private bool cgOn;
    private bool inited = false;
    private RectTransform rTransform;
    private Vector2 target;
    private bool redOrBlue;

    public Animator anim;
    public UITweenAlpha tween;
    public void Init(Color playerColor, string playerFace, string playerName, string itemName, int itemNumber, bool leftRight, EMCamp camp)
    {
        if (camp == EMCamp.Camp1)
        {
            //giftIcon.sprite = UIScreenInfoInsertionMgr.Ins.Camp1GiftNameToSprite[CDanmuGiftConst.Soldier_BoxLv2][0];
            BGCamps[0].gameObject.SetActive(true);
            SoldierCamps[0].gameObject.SetActive(true);
        }
        else if (camp == EMCamp.Camp2)
        {
            //giftIcon.sprite = UIScreenInfoInsertionMgr.Ins.Camp2GiftNameToSprite[CDanmuGiftConst.Soldier_BoxLv2][0];
            BGCamps[1].gameObject.SetActive(true);
            SoldierCamps[1].gameObject.SetActive(true);
        }
        else if (camp == EMCamp.Camp3)
        {
            //giftIcon.sprite = UIScreenInfoInsertionMgr.Ins.Camp3GiftNameToSprite[CDanmuGiftConst.Soldier_BoxLv2][0];
            BGCamps[2].gameObject.SetActive(true);
            SoldierCamps[2].gameObject.SetActive(true);
        }

        CAysncImageDownload.Ins.setAsyncImage(playerFace, iconHead);
        this.playerName.text = playerName;
        //this.playerName.color = playerColor;
        this.itemName.text = leftRight ? CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "summon") +" " + itemName + " x "  :  " x " +   itemName+ " " + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "summon");
        this.itemNum.text = "1";
        //this.itemNumber.text = " ";

        this.realItemNumber = itemNumber;
        //this.rTransform = GetComponent<RectTransform>();
        this.cg.alpha = 0;
        this.redOrBlue = leftRight;

        //campHero[(int)camp].gameObject.SetActive(true);
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
        CHelpTools.NumJump(itemNum, 1, realItemNumber, "{0}", 1);
        yield return new WaitForSeconds(animTime - 0.2f + 0.5f);
        tween.enabled = true;
        tween.Play(() =>
        {
            Destroy(this.gameObject);
        });
    }

    private void Update()
    {
        //if (!inited) return;
        //rTransform.anchoredPosition = Vector2.SmoothDamp(rTransform.anchoredPosition, target, ref refV2, 0.4f);
        //cg.alpha = Mathf.MoveTowards(cg.alpha, cgOn ? 1 : 0, cgOn ? 3 * Time.deltaTime : Time.deltaTime);
    }

    //private IEnumerator Play()
    //{
    //    RectTransform rt = GetComponent<RectTransform>();
    //    float width = rt.rect.width;
    //    rTransform.anchoredPosition = Vector2.right * width * (redOrBlue ? -1 : 1);
    //    target = Vector2.zero;
    //    yield return new WaitForEndOfFrame();
    //    this.inited = true;

    //    cgOn = true;
    //    yield return new WaitForSeconds(0.3f);
    //    float counter = 0;
    //    float time = 0.2f;
    //    float scale = 1;
    //    float refF = 0;
    //    while (counter < time)
    //    {
    //        counter += Time.deltaTime;
    //        scale = Mathf.SmoothDamp(scale, 1.4f, ref refF, time * 1 / 4);
    //        itemNum.transform.localScale = Vector3.one * scale;
    //        yield return new WaitForEndOfFrame();
    //    }
    //    counter = 0;
    //    while (counter < time)
    //    {
    //        counter += Time.deltaTime;
    //        scale = Mathf.SmoothDamp(scale, 1f, ref refF, time * 1 / 4);
    //        itemNum.transform.localScale = Vector3.one * scale;
    //        yield return new WaitForEndOfFrame();
    //    }
    //    itemNum.transform.localScale = Vector3.one;
    //    counter = 0;
    //    time = 0.2f;
    //    float currentNumberCounter = 0;
    //    while (counter < time)
    //    {
    //        counter += Time.deltaTime;
    //        currentNumberCounter = Mathf.SmoothDamp(currentNumberCounter, realItemNumber, ref refF, time * 1 / 2);
    //        int i = currentNumberCounter == realItemNumber ? 0 : 1;
    //        this.itemNum.text = redOrBlue ? ((int)currentNumberCounter + i).ToString() : ((int)currentNumberCounter + i).ToString() ;
    //        yield return new WaitForEndOfFrame();
    //    }
    //    this.itemNum.text = redOrBlue ?  realItemNumber.ToString() : realItemNumber.ToString() ;
    //    yield return new WaitForSeconds(0.5f);
    //    cgOn = false;
    //    //target = Vector2.left * width * (redOrBlue ? 1 : -1);
    //    while (true)
    //    {
    //        if (cg.alpha < 0.01f) Destroy(gameObject);
    //        //if (Vector2.Distance(rt.anchoredPosition, target) < 1) Destroy(gameObject);
    //        yield return new WaitForEndOfFrame();
    //    }
    //}


}
