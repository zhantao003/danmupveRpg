using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGMToast : MonoBehaviour
{
    public RectTransform rectBG;
    public RectTransform rectText;
    public Image uiBG;
    public Text uiTexContent;

    CPropertyTimer pTimer = new CPropertyTimer();

    DelegateNFuncCall callEventClose = null;

    public void SetContent(string content, float time = 2.5F, DelegateNFuncCall call = null)
    {
        gameObject.SetActive(true);

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

    void Update()
    {
        if (pTimer.Tick(CTimeMgr.DeltaTime))
        {
            callEventClose?.Invoke();

            gameObject.SetActive(false);
        }
        else
        {
            if (pTimer.GetTimeLerp() <= 0.4f)
            {
                uiBG.color = new Color(uiBG.color.r, uiBG.color.g, uiBG.color.b, (pTimer.GetTimeLerp() / 0.4f) * 0.82F);
                uiTexContent.color = new Color(uiTexContent.color.r, uiTexContent.color.g, uiTexContent.color.b, pTimer.GetTimeLerp() / 0.4f);
            }
        }
    }
}
