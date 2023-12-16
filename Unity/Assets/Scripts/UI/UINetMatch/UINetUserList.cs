using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetUserList : UIBase
{
    public GameObject objSlotRoot;
    public Transform tranGrid;

    public Scrollbar bar_Players;
    public UINetUserSlot playerItemPrefab;
    Dictionary<string, UINetUserSlot> playerItemDic;
    protected override void OnStart()
    {
        //objSlotRoot.gameObject.SetActive(false);
    }

    public override void OnOpen()
    {
        GetServerPlayer();
    }
   
    int page = 1;
    bool initFirstPage = false;
    float pageAddTimeGap = 3f;
    float pageAddTimeCount = 0;
    public void GetServerPlayer()
    {
        ClearAllChild(tranGrid);
        if (playerItemDic == null)
            playerItemDic = new Dictionary<string, UINetUserSlot>();
        else
            playerItemDic.Clear();
        page = 1;
        initFirstPage = false;
        ETHandlerReqUserOnlineList.Request(page).Coroutine();
    }

    #region 网络消息返回

    public void GetNextPagePlayer()
    {
        //Debug.Log("page" + page + "back");
        List<EUserInfo> players = EUserInfoMgr.Ins.listUserOnlineInfo;
        for (int i = 0; i < players.Count; i++)
        {
            if (playerItemDic.ContainsKey(players[i].nUserId.ToString()))
            {
                //刷
                playerItemDic[players[i].nUserId.ToString()].Init(players[i]);
            }
            else
            {
                UINetUserSlot item = Instantiate(playerItemPrefab.gameObject, tranGrid).GetComponent<UINetUserSlot>();
                item.gameObject.SetActive(true);
                item.Init(players[i]);
                playerItemDic.Add(players[i].nUserId.ToString(), item);
            }
        }
        initFirstPage = true;
        //page++;
    }


    #endregion



    public void OnClickClose()
    {
        CloseSelf();

     
    }

    float refreshTimeGap = 8f;
    float refreshTimeCount = 0;
    public void OnClickRefresh() {
        if (refreshTimeCount < refreshTimeGap)
        {
            UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "refreshwait"));
            return;
        }
        EUserInfoMgr.Ins.ClearOnlineUser();
        GetServerPlayer();
        refreshTimeCount = 0;
    }

    protected override void OnUpdate(float dt)
    {
        if (initFirstPage && bar_Players.value > 0.95f && pageAddTimeCount > pageAddTimeGap && playerItemDic.Count == (page - 1) * 25)
        {
            ETHandlerReqUserOnlineList.Request(page).Coroutine();
            pageAddTimeCount = 0;
        }
        refreshTimeCount += dt;
    }

    public static void ClearAllChild(Transform transform)
    {
        while (transform.childCount > 0)
        {
            GameObject.DestroyImmediate(transform.GetChild(0).gameObject);
        }
        transform.DetachChildren();
    }
}
