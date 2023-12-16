using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINetMatch : UIBase
{
    public UINetMatchPlayerSlot pSlotSelf;
    public UINetMatchPlayerSlot pSlotEnemy;

    public GameObject objBoardSelect;
    public GameObject objBoardMatching;

    public Text uiLabelMatchTime;

    float fMatchTime = 0f;

    //CPropertyTimer pTimeStartGame = null;


    protected override void OnUpdate(float dt)
    {
        if (objBoardMatching.activeSelf)
        {
            fMatchTime += dt;
            uiLabelMatchTime.text = $"{CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "match")}{CHelpTools.GetTimeCounter((int)fMatchTime)}";
        }
    }

    public override void OnOpen()
    {
        pSlotSelf.SetInfo(EUserInfoMgr.Ins.pSelf);
        pSlotEnemy.SetInfo(null);
    }

    #region 网络回馈事件

    public void OnStartMatch()
    {
        fMatchTime = 0f;
        uiLabelMatchTime.text = $"{CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "match")}{CHelpTools.GetTimeCounter((int)fMatchTime)}";
        objBoardSelect.SetActive(false);
        objBoardMatching.SetActive(true);
    }

    public void OnCancelMatch()
    {
        objBoardSelect.SetActive(true);
        objBoardMatching.SetActive(false);
    }

    public void RecieveInvited(DUserListInfo inviter)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendFormat(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "receive2"), inviter.NickName);
        UIMsgBox.Show(stringBuilder.ToString(), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "receive"), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "refuse"), UIMsgBox.EMType.YesNo, delegate ()
        {
            ETHandlerReqAcceptInvitePk.Request(inviter.PlayerId).Coroutine();
        });
    }

    public void MatchSetEnemyInfo(EUserInfo enemyInfo) {
        pSlotEnemy.SetInfo(enemyInfo);
    }

    //进入游戏场景
    public IEnumerator OnStartGame() {
        yield return new WaitUntil(() => {
            return ERoomInfoMgr.Ins.pSelfRoom.GetPlayerCount() == 2;
        });

        UIManager.Instance.CloseUI(UIResType.ETNetUserList);
        objBoardSelect.SetActive(false);
        objBoardMatching.SetActive(false);
       
        UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "matchsuc"), 3);
        yield return new WaitForSeconds(3f);
        UIManager.Instance.OpenUI(UIResType.Loading);

        //进入场景  todo
        CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101Net);
    }

    #endregion


    public void OnClickMatch()
    {
        ETHandlerReqStartMatchGame.Request().Coroutine();
    }

    public void OnClickInvite()
    {
        EUserInfoMgr.Ins.ClearOnlineUser();
        UIManager.Instance.OpenUI(UIResType.ETNetUserList);

    }

    public void OnClickCancelMatch()
    {
        ETHandlerCancelMatchGame.Request().Coroutine();
    }

    public void OnClickLoginOut()
    {
        //匹配到了不让退出
        if (ERoomInfoMgr.Ins.pSelfRoom != null &&
            ERoomInfoMgr.Ins.pSelfRoom.GetPlayerCount() == 2)
            return;
        SessionComponent.Instance.Session.Dispose();
        CloseSelf();
        UIManager.Instance.OpenUI(UIResType.ModeSelect);
    }
}
