using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIETBoardLogin : MonoBehaviour
{
    public InputField uiInputCode;
    public Text uiLabelVtuberName;
    public RawImage uiIconHead;

    public Transform tranRoomRoot;
    public GameObject objRoomSlot;
    List<GameObject> listPublicRooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        objRoomSlot.SetActive(false);
    }

    public void RefreshRoomList()
    {
        for(int i=0; i<listPublicRooms.Count; i++)
        {
            Destroy(listPublicRooms[i].gameObject);
        }
        listPublicRooms.Clear();

        int nRoomCount = ERoomInfoMgr.Ins.GetPublicNum();
        for (int i=0; i< nRoomCount; i++)
        {
            ERoomSimpleInfo pRoomInfo = ERoomInfoMgr.Ins.GetPublicRoomByIdx(i);
            if (string.IsNullOrEmpty(pRoomInfo.szRoomId)) return;

            GameObject objNewRoom = GameObject.Instantiate(objRoomSlot) as GameObject;
            objNewRoom.SetActive(true);
            Transform tranNewRoom = objNewRoom.transform;
            tranNewRoom.SetParent(tranRoomRoot);
            tranNewRoom.localPosition = Vector3.zero;
            tranNewRoom.localRotation = Quaternion.identity;
            tranNewRoom.localScale = Vector3.one;

            UIETRoomListSlot uiNewRoomSlot = objNewRoom.GetComponent<UIETRoomListSlot>();
            uiNewRoomSlot.SetRoomInfo(pRoomInfo);
        }
    }

    public void OnClickConnectPlatform()
    {
        CDanmuSDKCenter.Ins.Login(uiInputCode.text, "", delegate (int value)
        {
            if (value != 0)
            {
                Debug.LogError("连接平台失败");
                return;
            }

            uiLabelVtuberName.text = CDanmuSDKCenter.Ins.szNickName;
            CAysncImageDownload.Ins.setAsyncImage(CDanmuSDKCenter.Ins.szHeadIcon, uiIconHead);
        });
    }

    public void OnClickLogin()
    {
        ETHandlerReqLogin.Login(CDanmuSDKCenter.Ins.szUid,
                                CDanmuSDKCenter.Ins.szNickName,
                                CDanmuSDKCenter.Ins.szHeadIcon, 0).Coroutine();
    }

    public void OnClickCreateRoom()
    {
        ETHandlerReqCreateRoom.Request(0, 2).Coroutine();
    }

    public void OnClickRefrehRoom()
    {
        ETHandlerReqGetRoomList.Request().Coroutine();
    }
}
