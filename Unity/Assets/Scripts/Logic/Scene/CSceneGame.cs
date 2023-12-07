using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneGame : CSceneBase
{
    public override void OnSceneStart()
    {
        //CGameColorPetMgr.Ins.pGachaMgr = GameObject.FindObjectOfType<CGachaMgr>();

        UIManager.Instance.RefreshUI(); 
        UIManager.Instance.CloseUI(UIResType.Loading);
    }

    public override void OnSceneLeave()
    {
        UIManager.Instance.ClearUI();

        CPlayerMgr.Ins.ClearAllPlayerUnit();
        CAudioMgr.Ins.ClearAllAudio();
    }
}
