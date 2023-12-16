using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetUserSlot : MonoBehaviour
{
    public RawImage img_HeadIcon;
    public Text txt_Name;
    public Text txt_Status;
    //public Text txt_Score;
    public Button btn_Invite;

    long userID;
    public void Init(EUserInfo pInfo)
    {
        txt_Name.text = pInfo.szNickName;
        //txt_Score.text =  "排位分" + ":" + pInfo.nScore.ToString();
        CAysncImageDownload.Ins.setAsyncImage(pInfo.szHeadIcon, img_HeadIcon);
     
        bool isFight = !string.IsNullOrEmpty(pInfo.szRoomId);
        txt_Status.text = isFight ? CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "battling") : "";

        if (isFight)
            btn_Invite.enabled = false;
        else
            btn_Invite.enabled = true;
        userID = pInfo.nUserId;
        btn_Invite.onClick.RemoveAllListeners();
        btn_Invite.onClick.AddListener(InvitePK);
    }

    float inviteGap = 10f;
    float inviteCount = 10f;
    void InvitePK()
    {
        if (inviteCount > inviteGap)
        {
            ETHandlerReqInvitePk.Request(userID).Coroutine();
            inviteCount = 0;
            UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "sendinvite"));
        }
        else {
            UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "invitewait"));
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        inviteCount += Time.deltaTime;
    }
}
