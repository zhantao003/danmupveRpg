using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UIIdleNetPlayerInviteWindow : MonoBehaviour
{
    public Text txt_Des;
    public Button btn_Cancel;
    public Button btn_Accpet;

    public void Init(DUserListInfo inviter)
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.AppendFormat(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "receivecontent"), inviter.NickName, inviter.Score);
        btn_Cancel.onClick.Add(() =>
        {
            Destroy(this.gameObject);
        });
        btn_Accpet.onClick.Add(() =>
        {
            ETHandlerReqAcceptInvitePk.Request(inviter.PlayerId).Coroutine();
            Destroy(this.gameObject);
        });
    }
}
