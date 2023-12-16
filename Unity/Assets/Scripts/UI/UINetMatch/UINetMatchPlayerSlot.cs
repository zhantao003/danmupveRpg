using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetMatchPlayerSlot : MonoBehaviour
{
    public RawImage uiHeadIcon;
    public Text uiLabelName;

    public void SetInfo(EUserInfo userInfo)
    {
        if (userInfo == null)
        {
            uiLabelName.text = "???";
            uiHeadIcon.color = new Color(1, 1, 1, 0);
            return;
        }

        uiLabelName.text = userInfo.szNickName;

        CAysncImageDownload.Ins.setAsyncImage(userInfo.szHeadIcon, uiHeadIcon);
        uiHeadIcon.color = new Color(1, 1, 1, 1);
    }
}
