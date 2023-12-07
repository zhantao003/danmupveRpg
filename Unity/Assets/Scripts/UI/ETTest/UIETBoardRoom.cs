using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIETBoardRoom : MonoBehaviour
{
    public Text uiLabelStartTime;
    public UIETRoomSlot[] arrRoomSlot;

    float fTimeWaitStart = 0f;

    void Update()
    {
        if(fTimeWaitStart > 0F)
        {
            fTimeWaitStart -= CTimeMgr.DeltaTime;
            RefreshWaitTime();
        }
        else
        {
            uiLabelStartTime.text = "";
        }
    }

    public void SetSlotInfo(ERoom.RoomSlot slotInfo)
    {
        if (slotInfo.nSeatIdx < 0 || slotInfo.nSeatIdx >= arrRoomSlot.Length) return;

        arrRoomSlot[slotInfo.nSeatIdx].SetUserInfo(slotInfo.player);
    }

    public void ClearSlot(int idx)
    {
        if (idx < 0 || idx >= arrRoomSlot.Length) return;

        arrRoomSlot[idx].Clear();
    }

    /// <summary>
    /// 判断是否进入开始倒计时
    /// </summary>
    public void CheckGameStart()
    {
        if(ERoomInfoMgr.Ins.pSelfRoom.IsAllReady())
        {
            fTimeWaitStart = 3.05f;
            RefreshWaitTime();
        }
    }

    void RefreshWaitTime()
    {
        uiLabelStartTime.text = "开始倒计时:" + (int)fTimeWaitStart;
    }
}
