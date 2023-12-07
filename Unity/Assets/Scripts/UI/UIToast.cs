using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToast : UIBase
{
    public RectTransform rectBG;
    public RectTransform rectText;
    public Image uiBG;
    public Text uiTexContent;

    CPropertyTimer pTimer = new CPropertyTimer();

    DelegateNFuncCall callEventClose = null;

    public void SetContent(string content, float time = 2.5F, DelegateNFuncCall call = null)
    {
        callEventClose = call;

        uiBG.gameObject.SetActive(true);
        uiTexContent.gameObject.SetActive(true);

        //CTBLInfo.Inst.pHandlerGlobalValueInfo.GetStringValue(content);

        uiTexContent.text = content;
        pTimer.Value = time;
        pTimer.FillTime();

        LayoutRebuilder.ForceRebuildLayoutImmediate(rectText);
        rectBG.sizeDelta = new Vector2(rectText.sizeDelta.x + 30, rectBG.sizeDelta.y);

        uiBG.color = new Color(uiBG.color.r, uiBG.color.g, uiBG.color.b, 0.82F);
        uiTexContent.color = new Color(uiTexContent.color.r, uiTexContent.color.g, uiTexContent.color.b, 1F);
    }

    protected override void OnUpdate(float dt)
    {
        if(pTimer.Tick(dt))
        {
            callEventClose?.Invoke();

            UIManager.Instance.CloseUI(UIResType.Toast);
        }
        else
        {           
            if(pTimer.GetTimeLerp() <= 0.4f)
            {
                uiBG.color = new Color(uiBG.color.r, uiBG.color.g, uiBG.color.b, (pTimer.GetTimeLerp() / 0.4f) * 0.82F);
                uiTexContent.color = new Color(uiTexContent.color.r, uiTexContent.color.g, uiTexContent.color.b, pTimer.GetTimeLerp() / 0.4f);
            }
        }
    }

    public static void Show(string content, float time = 1.5F, DelegateNFuncCall call = null)
    {
        UIManager.Instance.OpenUI(UIResType.Toast);

        UIToast uiToast = UIManager.Instance.GetUI(UIResType.Toast) as UIToast;
        if (uiToast == null) return;
        uiToast.SetContent(content, time, call);
    }
}
