using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerSendHeroComp : MonoBehaviour
{
    //public CanvasGroup cg;
    public GameObject[] campHeroItems;
    public GameObject[] effItems;
    public Text txt_PlayerName;
    public Text txt_Des;
    public RawImage img_Head;
    public UITweenAlpha tween ;

    public void Init(bool redOrBlue, EMCamp camp,string faceUrl,string playerName,string rewardDes) {
        this.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        CAysncImageDownload.Ins.setAsyncImage(faceUrl, img_Head);
        txt_Des.text = rewardDes;
        txt_PlayerName.text = playerName;
        //txt_Des.color = redOrBlue ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
        //txt_PlayerName.color = redOrBlue ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
        if (camp == EMCamp.Camp1)
        {
            campHeroItems[0].gameObject.SetActive(true);
            effItems[0].gameObject.SetActive(true);
        }
        else if (camp == EMCamp.Camp2)
        {
            campHeroItems[1].gameObject.SetActive(true);
            effItems[1].gameObject.SetActive(true);
        }
        else if (camp == EMCamp.Camp3)
        {
            campHeroItems[2].gameObject.SetActive(true);
            effItems[2].gameObject.SetActive(true);
        }
        StartCoroutine(Play());
    }
     
    IEnumerator Play() {
        yield return new WaitForSeconds(2f);
        //while (cg.alpha>0.01f)
        //{
        //    cg.alpha = Mathf.MoveTowards(cg.alpha, 0, Time.deltaTime);
        //    yield return new WaitForEndOfFrame();
        //}
        tween.enabled = true;
        tween.Play(() =>
        {
            Destroy(this.gameObject);
        });
        //Destroy(this.gameObject);
    }
}
