using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIETRoomListSlot : MonoBehaviour
{
    string szRoomId;
    public Text uiLabelRoomID;
    public Text uiLabelPlayerNum; 

    public void SetRoomInfo(ERoomSimpleInfo info)
    {
        szRoomId = info.szRoomId;
        uiLabelRoomID.text = info.szRoomId;
        uiLabelPlayerNum.text = "最大人数:" + info.nMaxPlayer.ToString();
    }

    public void OnClickJoin()
    {
        ETHandlerReqJoinRoom.Request(szRoomId).Coroutine();
    }
}
