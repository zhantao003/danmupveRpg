using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerAchivePointComp : MonoBehaviour
{
    public RawImage headImg;
    public Text playerName;
    public Text itemNumber;
    private long realItemNumber;
    private Vector2 refV2;
    //public CanvasGroup cg;
    private bool cgOn;
    private bool inited = false;
    private RectTransform rTransform;
    private Vector2 target;
    private bool redOrBlue;
    private float currentNumberCounter = 0;
    private float refF1;
    private float jumpTime;

    public Animator anim;

    public void Init(Color playerColor, string playerName, long itemNumber, bool redOrBlue, string faceUrl,float jumpTime)
    {
        CAysncImageDownload.Ins.setAsyncImage(faceUrl, headImg);
        this.playerName.text = playerName;
        this.playerName.color = playerColor;
        this.itemNumber.text = "0";
        this.realItemNumber = itemNumber;
        this.rTransform = GetComponent<RectTransform>();
        //this.cg.alpha = 0;
        this.redOrBlue = redOrBlue;
        this.jumpTime = jumpTime;
        StartCoroutine(Play());
    }

    //IEnumerator Play() {
    //    float animTime;
    //    if (redOrBlue)
    //    {
    //        anim.CrossFadeInFixedTime("PlayerAchivePointRed_01_appear", 0);
    //        animTime = CGameEffMgr.GetAnimatorLength(anim, "PlayerAchivePointRed_01_appear");

    //    }
    //    else {
    //        anim.CrossFadeInFixedTime("PlayerAchivePointBlue_01_appear", 0);
    //        animTime = CGameEffMgr.GetAnimatorLength(anim, "PlayerAchivePointBlue_01_appear");
    //    }
    //    yield return new WaitForSeconds(0.5f);
    //    CHelpTools.NumJump(itemNumber, 0, realItemNumber, "{0}", jumpTime>1?1: jumpTime);
    //    yield return new WaitForSeconds(animTime - 0.5f);
    //    Destroy(this.gameObject);
    //}

    private void Update()
    {
        if (!inited) return;
        rTransform.anchoredPosition = Vector2.SmoothDamp(rTransform.anchoredPosition, target, ref refV2, 0.2f);
        //cg.alpha = Mathf.MoveTowards(cg.alpha, cgOn ? 1 : 0, 3 * Time.deltaTime);
    }

    private IEnumerator Play()
    {

        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        rTransform.anchoredPosition = Vector2.right * width * (redOrBlue ? -1 : 1);
        target = Vector2.zero;
        yield return new WaitForEndOfFrame();
        this.inited = true;

        //cgOn = true;
        yield return new WaitForSeconds(0.3f);
        CHelpTools.NumLongJump(itemNumber, 0, realItemNumber, "{0}", jumpTime);

        float counter = 0;
        float time = jumpTime / 4f;
        float scale = 1;
        float refF = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
            scale = Mathf.SmoothDamp(scale, 1.4f, ref refF, time * 1 / 4);
            itemNumber.transform.localScale = Vector3.one * scale;
            yield return new WaitForEndOfFrame();
        }
        counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
            scale = Mathf.SmoothDamp(scale, 1f, ref refF, time * 1 / 4);
            itemNumber.transform.localScale = Vector3.one * scale;
            yield return new WaitForEndOfFrame();
        }
        counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
            scale = Mathf.SmoothDamp(scale, 1.4f, ref refF, time * 1 / 4);
            itemNumber.transform.localScale = Vector3.one * scale;
            yield return new WaitForEndOfFrame();
        }
        counter = 0;
        while (counter < time)
        {
            counter += Time.deltaTime;
            scale = Mathf.SmoothDamp(scale, 1f, ref refF, time * 1 / 4);
            itemNumber.transform.localScale = Vector3.one * scale;
            yield return new WaitForEndOfFrame();
        }
        itemNumber.transform.localScale = Vector3.one;

        yield return new WaitForSeconds(1f);
        //cgOn = false;
        target = Vector2.left * (width+30) * (redOrBlue ? 1 : -1);
        while (true)
        {
            if (Vector2.Distance(rt.anchoredPosition, target) < 1) Destroy(gameObject);
            yield return new WaitForEndOfFrame();
        }
    }
}
