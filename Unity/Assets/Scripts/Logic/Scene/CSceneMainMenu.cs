using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneMainMenu : CSceneBase
{
    public override void OnSceneStart()
    {
        UIManager.Instance.RefreshUI();
        UIManager.Instance.CloseUI(UIResType.Loading);

        UIManager.Instance.OpenUI(UIResType.MainMenu);
    }

    public override void OnSceneLeave()
    {
        CAudioMgr.Ins.ClearAllAudio();
        UIManager.Instance.ClearUI();
    }
}
