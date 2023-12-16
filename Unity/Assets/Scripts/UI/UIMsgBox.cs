using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMsgBox : UIBase
{
    public enum EMType
    {
        YesNo,
        OK,
    }

    //public Text uiLabelTitle;
    public Text uiLabelContent;
    public Text uiLabelOK;
    public Text uiLabelCancel;

    public GameObject objBtnYes;
    public GameObject objBtnNo;
    public GameObject objBtnOK;

    public DelegateNFuncCall pDlgOk = null;
    public DelegateNFuncCall pDlgCancel = null;

    public void OnClickOK()
    {
        pDlgOk?.Invoke();

        CloseSelf();
    }

    public void OnClickCancel()
    {
        pDlgCancel?.Invoke();

        CloseSelf();
    }

    public static void Show(string content, string labelOk, string labelCancel, EMType type, 
                            DelegateNFuncCall callOk, DelegateNFuncCall callCancel = null)
    {
        UIManager.Instance.OpenUI(UIResType.MsgBox);

        UIMsgBox uiMsgBox = UIManager.Instance.GetUI(UIResType.MsgBox) as UIMsgBox;
        if (uiMsgBox == null) return;

        //uiMsgBox.uiLabelTitle.text = title;
        uiMsgBox.uiLabelContent.text = content;
        uiMsgBox.uiLabelOK.text = labelOk;
        uiMsgBox.uiLabelCancel.text = labelCancel;

        uiMsgBox.objBtnYes.SetActive(type == EMType.YesNo);
        uiMsgBox.objBtnNo.SetActive(type == EMType.YesNo);
        uiMsgBox.objBtnOK.SetActive(type == EMType.OK);

        uiMsgBox.pDlgOk = callOk;
        uiMsgBox.pDlgCancel = callCancel;
    }
}
