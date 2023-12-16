using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIKillInfoComp : MonoBehaviour
{
  
    public Text killer;
    public Text beKillName;
    public Text s;
    public Image killIcon;
    public CanvasGroup cg;
    private float deadTime = 3;
    private float deadCounter = 3;

    Transform killerTarget;
    Vector2 offset = Vector2.zero;
    float originScale;
    UIBindCamera worldCam;
    UIWorldCanvas uiCanvas;
    Vector3 killWorldPos;
    Vector3 viewPoint;
    public void Init(Transform killerTarget, string killer, string enemyName, bool redOrBlue, int level ,Vector2 offset)
    {
        uiCanvas = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldCam = uiCanvas.uiCamera;

        this.killerTarget = killerTarget;
        killWorldPos = killerTarget.position;
       
        this.offset = offset;
       
        this.originScale = this.transform.localScale.x;
        this.transform.localScale = new Vector3(originScale, originScale, originScale) * (1+level * 0.1f);
      
        this.killer.text = killer;
        //this.shipName.text = /*shipName*/ "+" + shipLv;
        this.beKillName.text = enemyName;
  
        this.cg.alpha = 0;
        if (redOrBlue)
        {
            this.killer.color = CBattleMgr.Ins.pRedCamp.pColor;
            this.beKillName.color = CBattleMgr.Ins.pBlueCamp.pColor;
        }
        else
        {
            this.killer.color = CBattleMgr.Ins.pBlueCamp.pColor;
            this.beKillName.color = CBattleMgr.Ins.pRedCamp.pColor;
        }
        this.deadCounter = 0;
    }
    float flyOffset = 0;
    private void LateUpdate()
    {
        viewPoint = worldCam.uiCam.WorldToViewportPoint(killWorldPos);
        viewPoint.y += flyOffset;
        flyOffset +=  0.0003f;
        this.GetComponent<RectTransform>().anchoredPosition3D = new Vector3((viewPoint.x + offset.x / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.width, (viewPoint.y + offset.y / worldCam.zoneVal) * uiCanvas.GetComponent<Canvas>().GetComponent<RectTransform>().rect.height, 0);
        this.transform.localScale = new Vector3(originScale, originScale, originScale) * originScale / worldCam.zoneVal;
        

        deadCounter += Time.deltaTime;
        if (deadCounter < deadTime * 0.2f)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 1, 3 * Time.deltaTime);
        }
        if (deadCounter > deadTime * 0.5f)
        {
            cg.alpha = Mathf.MoveTowards(cg.alpha, 0, 3 * Time.deltaTime);

            if (cg.alpha <= 0)
                RecycleObj();
        }
    }

    void RecycleObj() {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.RecycleObject(UIWorldCanvas.WorldUIType.击杀信息, this.gameObject);
        this.transform.localScale = Vector3.one;
        flyOffset=0f;
        this.enabled = false;
    }
}
