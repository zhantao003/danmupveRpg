using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIPlayerScoreTop3Comp : MonoBehaviour
{
    public GameObject[] rankItems;
    public Image[] nameBGs;
    public List<RawImage> headIcons;
    public List<Text> txt_name;
    public List<Text> txt_liansheng;
    //public List<Image> vipImg;

    private void Start()
    {
        for (int i = 0; i < rankItems.Length; i++)
        {
            rankItems[i].gameObject.SetActive(false);
        }
    }

    public void SetRankInfo(int rank, string name, int liansheng, string head, int vipLv = 0)
    {
        if (!rankItems[rank].gameObject.activeSelf)
            rankItems[rank].gameObject.SetActive(true);
        CAysncImageDownload.Ins.setAsyncImage(head, headIcons[rank]);
        txt_name[rank].text = name;
        //if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili)
        //{
        //    Sprite sprite = null;
        //    switch (vipLv)
        //    {
        //        case 1: sprite = TotalDatas.Instance.bilibili_Lv1; break;
        //        case 2: sprite = TotalDatas.Instance.bilibili_Lv2; break;
        //        case 3: sprite = TotalDatas.Instance.bilibili_Lv3; break;
        //    }
        //    if (sprite != null)
        //    {
        //        vipImg[rank].gameObject.SetActive(true);
        //        vipImg[rank].sprite = sprite;
        //    }
        //    else
        //    {
        //        vipImg[rank].gameObject.SetActive(false);
        //    }
        //}
        //else if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Douyu)
        //{
        //    Sprite sprite = null;
        //    if (vipLv != 0)
        //    {
        //        sprite = TotalDatas.Instance.douyuVip;
        //    }
        //    if (sprite != null)
        //    {
        //        vipImg[rank].gameObject.SetActive(true);
        //        vipImg[rank].sprite = sprite;
        //    }
        //    else
        //    {
        //        vipImg[rank].gameObject.SetActive(false);
        //    }
        //}
        //else
        //{
        //vipImg[rank].gameObject.SetActive(false);
        //}
        if (liansheng > 0)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.AppendFormat(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "liansheng"), liansheng);
            txt_liansheng[rank].text = stringBuilder.ToString();// "连胜" + liansheng + "场";// CTBLLanguageInfo.Inst.GetContent( EMLanguageContentType.Game, "liansheng")
        }
        else
        {
            txt_liansheng[rank].text = "";
        }
    }
}
