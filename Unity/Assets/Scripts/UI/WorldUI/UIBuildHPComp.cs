using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildHPComp : MonoBehaviour
{
    public CanvasGroup cg;
    public Image hpImg;
    protected CPlayerUnit unit;
    protected Transform followTarget;
    protected Vector2 offset = Vector2.zero;
    protected UIBindCamera worldCam;
    protected UIWorldCanvas uiCanvas;
    protected float originScale;
    protected float lastFrameHp;
    protected bool allowShowHp;
    protected float allowShowHpTimeGap = 5f;
    protected float allowShoHpCount = 0f;
    public void Init(CPlayerUnit unit, Transform followTarget, Vector2 offset)
    {
        this.unit = unit;
        this.followTarget = followTarget;
        this.offset = offset;
        this.originScale = this.transform.localScale.x;
        if (hpImg != null)
        {
            if (unit.emCamp == EMUnitCamp.Red)
            {
                hpImg.sprite = CBattleMgr.Ins.pRedCamp.pHpSprite;
            }
            else
            {
                hpImg.sprite = CBattleMgr.Ins.pBlueCamp.pHpSprite;
            }
        }
        if (cg != null)
            cg.alpha = 0;
        lastFrameHp = unit.pUnitData.nCurHP;
        uiCanvas = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldCam = uiCanvas.uiCamera;

        Vector3 viewPoint = worldCam.uiCam.WorldToViewportPoint(followTarget.position);
        this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((viewPoint.x + offset.x / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width, (viewPoint.y + offset.y / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height, 0);
    }
    public virtual void RecycleObj()
    {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.RecycleObject(UIWorldCanvas.WorldUIType.建筑血条, this.gameObject);
        allowShoHpCount = 0;
        unit = null;
        this.enabled = false;
    }

    protected void SynUI()
    {
        if (unit.pUnitData.nCurHP <= 0 || unit == null)
        {
            RecycleObj();
            return;
        }

        if (lastFrameHp > unit.pUnitData.nCurHP)
        {
            allowShoHpCount = 0;
            lastFrameHp = unit.pUnitData.nCurHP;
            this.transform.SetAsLastSibling();
        }

        if (allowShoHpCount > allowShowHpTimeGap)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 0, Time.deltaTime);
        }
        else
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 1, Time.deltaTime * 2);
        }

        Vector3 viewPoint = worldCam.uiCam.WorldToViewportPoint(followTarget.position);

        this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((viewPoint.x + offset.x / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width, (viewPoint.y + offset.y / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height, 0);
        this.transform.localScale = Vector3.one * originScale / worldCam.zoneVal;
        if (hpImg != null)
            hpImg.fillAmount = 1.0f * unit.pUnitData.nCurHP / unit.pUnitData.nMaxHP;
        allowShoHpCount += Time.deltaTime;
    }
    // Update is called once per frame
    void LateUpdate()
    {
        SynUI();
    }
}
