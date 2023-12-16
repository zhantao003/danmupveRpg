using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneGameModeSelect : CSceneBase
{
    public override void OnSceneStart()
    {
        UIManager.Instance.RefreshUI();
        UIManager.Instance.CloseUI(UIResType.Loading);
        UIManager.Instance.OpenUI(UIResType.ModeSelect);
    }

    public override void OnSceneLeave()
    {
        UIManager.Instance.ClearUI();
    }
}
