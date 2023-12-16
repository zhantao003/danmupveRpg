using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITeamHeadComp : UIBuildHPComp
{
    public Text txt_OwerName;
    public List<CPlayerUnit> units;
    bool bWaitUnit;
    bool inited = false;
    CPlayerBaseInfo info;
    public Image headBg;
    public RawImage headIcon;
    public void Init(List<CPlayerUnit> units, Vector2 offset) {
        if(units == null)
        {
            bWaitUnit = true;
            this.offset = offset;
            originScale = this.transform.localScale.x;
        }
        else
        {
            this.units = units;
            base.Init(units[0], units[0].tranSelf, offset);
            if (unit != null)
            {
                CPlayerBaseInfo info = CPlayerMgr.Ins.GetPlayer(unit.szUserUid);
                if (info != null)
                {
                    CAysncImageDownload.Ins.setAsyncImage(info.userFace, headIcon);
                    txt_OwerName.color = info.emCamp == EMUnitCamp.Red ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
                    txt_OwerName.text = info.userName;
                    SetDisplay(CBattleMgr.Ins.emDisplayType);
                }
            }
            bWaitUnit = false;
        }
        inited = true;
    }

    public void AddUnit(CPlayerUnit playerUnit)
    {
        if(bWaitUnit)
        {
            units = new List<CPlayerUnit>();
            units.Add(playerUnit);
            if (units[0]!=null)
                base.Init(units[0], units[0].tranSelf, offset);
            bWaitUnit = false;
        }
        else
        {
            units.Add(playerUnit);
        }
    }

    public override void RecycleObj()
    {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.RecycleObject(UIWorldCanvas.WorldUIType.小队名字, this.gameObject);
        inited = false;
        bWaitUnit = false;
        unit = null;
        info = null;
        txt_OwerName.enabled = true;
        this.transform.localScale = Vector3.one;
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

    private void LateUpdate()
    {
        if (!inited)
            return;
        if (bWaitUnit) 
            return;
        if (unit==null||unit.pUnitData.nCurHP <= 0)
        {
            if (units.Count > 1) {
                units.RemoveAt(0);
                unit = units[0];
                if(unit !=null)
                    followTarget = unit.tranSelf;
                if (CBattleMgr.Ins.emDisplayType == CBattleMgr.EMDisPlayName.Name)
                {
                    txt_OwerName.enabled = false;
                }
                else if (CBattleMgr.Ins.emDisplayType == CBattleMgr.EMDisPlayName.Head)
                {
                    headBg.enabled = false;
                }else {
                    txt_OwerName.enabled = false;
                    headBg.enabled = false;
                }
               
            }
            else
            {
                RecycleObj();
            }
            return;
        }
        if (CBattleMgr.Ins.emDisplayType == CBattleMgr.EMDisPlayName.Name)
        {
            if (!txt_OwerName.enabled)
                txt_OwerName.enabled = true;
        }
        else if (CBattleMgr.Ins.emDisplayType == CBattleMgr.EMDisPlayName.Head)
        {
            if (!headBg.enabled)
                headBg.enabled = true;
        }
        else
        {
            if (txt_OwerName.enabled)
                txt_OwerName.enabled = false;
            if (headBg.enabled)
                headBg.enabled = false;
        }

       
        if (info == null)
        {
            info = CPlayerMgr.Ins.GetPlayer(unit.szUserUid);
            if (info != null)
            {
                CAysncImageDownload.Ins.setAsyncImage(info.userFace, headIcon);
                txt_OwerName.color = info.emCamp == EMUnitCamp.Red ? CBattleMgr.Ins.pRedCamp.pColor : CBattleMgr.Ins.pBlueCamp.pColor;
                txt_OwerName.text = info.userName;
                SetDisplay(CBattleMgr.Ins.emDisplayType);
            }
        }
        Vector3 viewPoint = worldCam.uiCam.WorldToViewportPoint(followTarget.position);
        this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((viewPoint.x + offset.x / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width, (viewPoint.y + offset.y / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height, 0);
        this.transform.localScale =Vector3.one* originScale / worldCam.zoneVal;
        if (hpImg != null)
            hpImg.fillAmount = 1.0f * unit.pUnitData.nCurHP / unit.pUnitData.nMaxHP;
    }
}
