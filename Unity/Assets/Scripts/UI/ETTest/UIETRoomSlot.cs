using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIETRoomSlot : MonoBehaviour
{
    public Text uiLabelVtuberName;
    public RawImage uiIconHead;

    public void SetUserInfo(EUserInfo info)
    {
        uiLabelVtuberName.text = info.szNickName;
        CAysncImageDownload.Ins.setAsyncImage(info.szHeadIcon, uiIconHead);
    }

    public void Clear()
    {
        uiLabelVtuberName.text = "";
        uiIconHead.texture = null;
    }
}
