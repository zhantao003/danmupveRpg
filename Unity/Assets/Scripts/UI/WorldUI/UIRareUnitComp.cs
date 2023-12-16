using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRareUnitComp : UIBuildHPComp
{
    public Text txt_OwerName;
    public Image headBg;
    public RawImage headIcon;

    public void Init(CPlayerUnit unit, Transform followTarget, Vector2 offset)
    {
        base.Init(unit, followTarget, offset);
        CPlayerBaseInfo info =  CPlayerMgr.Ins.GetPlayer(unit.szUserUid);
        if (info != null)
        {
            CAysncImageDownload.Ins.setAsyncImage(info.userFace, headIcon);
            txt_OwerName.color = info.emCamp == EMUnitCamp.Red ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
            txt_OwerName.text = info.userName;
            SetDisplay(CBattleMgr.Ins.emDisplayType);
        }
    }

    public override void RecycleObj() {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.RecycleObject(UIWorldCanvas.WorldUIType.精英怪血条, this.gameObject);
        allowShoHpCount = 0;
        this.transform.localScale = Vector3.one;
        unit = null;
        this.enabled = false;
    }

    public void SetDisplay(CBattleMgr.EMDisPlayName displayType)
    {
        if (displayType == CBattleMgr.EMDisPlayName.Name)
        {
            txt_OwerName.enabled = true;
            headBg.enabled = false;
            headIcon.enabled = false;
        }
        else if (displayType == CBattleMgr.EMDisPlayName.Head)
        {
            txt_OwerName.enabled = false;
            headBg.enabled = true;
            headIcon.enabled = true;
        }
        else
        {
            txt_OwerName.enabled = false;
            headBg.enabled = false;
            headIcon.enabled = false;
        }
    }


    // Update is called once per frame
    void LateUpdate()
    {
        SynUI();
    }
}
