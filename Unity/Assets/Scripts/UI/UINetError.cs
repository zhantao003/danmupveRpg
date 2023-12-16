using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UINetError : UIBase
{
    public Text uiLabelText;

    public void SetContent(string content)
    {
        uiLabelText.text = content;
    }

    public void OnClickOK()
    {
        CloseSelf();
    }
}
