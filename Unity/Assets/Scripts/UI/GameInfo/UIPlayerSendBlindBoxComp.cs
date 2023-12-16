using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerSendBlindBoxComp : MonoBehaviour
{
    public CSlotBlindBoxComp slotComp;
    //public CanvasGroup cg;
    public CanvasGroup desCg;
    public Text txt_Des;
    public GameObject objBG;
    public GameObject[] objAnima;
    public CEffectFramePlayUIImg[] pPlayAnima;

    private RectTransform rTransform;
    private bool redOrBlue;
    private bool cgOn;
    private bool inited = false;
    private Vector2 target;
    private Vector2 refV2;
    
    int drawIndex;
    string rewardDes;
    DelegateNFuncCall call;
    public void Init(int drawIndex ,string des, bool leftRight, DelegateNFuncCall call = null)
    {
        UnActiveObjAnima();
        //objBG.SetActive(true);
        this.call = call;
        this.drawIndex = drawIndex;
        this.rewardDes = des;
        this.rTransform = GetComponent<RectTransform>();
        //this.cg.alpha = 0;
        this.redOrBlue = leftRight;
        StartCoroutine(Play());
    }

    private void Update()
    {
        if (!inited) return;
        rTransform.anchoredPosition = Vector2.SmoothDamp(rTransform.anchoredPosition, target, ref refV2, 0.2f);
        //cg.alpha = Mathf.MoveTowards(cg.alpha, cgOn ? 1 : 0, 3 * Time.deltaTime);
    }

    private IEnumerator Play()
    {
        desCg.alpha = 0;
        float width = rTransform.rect.width;
        rTransform.anchoredPosition = Vector2.right * width * (redOrBlue ? -1 : 1);
        target = Vector2.zero;
        inited = true;
        cgOn = true;
        yield return new WaitUntil(() => Vector2.Distance(rTransform.anchoredPosition, target) < 1);
        yield return slotComp.DrawFun(drawIndex, 0.5f);
        yield return new WaitUntil(() => slotComp.isStopUpdatePos);
        txt_Des.text = rewardDes;
        yield return new WaitUntil(() => slotComp.isAnimEnd);
        call?.Invoke();
        //objBG.SetActive(false);
        PlayAnima(drawIndex);
        ActiveObjAnima(drawIndex);
        
        while (desCg.alpha < 0.99f) {
            desCg.alpha = Mathf.MoveTowards(desCg.alpha, 1, 3 * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
        //cgOn = false;
        //target = Vector2.left * width * (redOrBlue ? 1 : -1);
        //while (true)
        //{
        //    if (Vector2.Distance(rTransform.anchoredPosition, target) < 1) Destroy(gameObject);
        //    yield return new WaitForEndOfFrame();
        //}
    }

    public void PlayAnima(int nIdx)
    {
        for(int i = 0;i < pPlayAnima.Length;i++)
        {
            if(i == nIdx)
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

}
