using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainOpController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public CanvasGroup cg;
    float showTime = 0.3f;
    bool isShow = false;
    public bool panelOn = true;

    public GameObject objSelf;

    CDanmuSDKCenter.EMPlatform platform;

    public List<OperatingPanelComponent> operatings;

    public void Start()
    {
        if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            objSelf.SetActive(false);
        }
        foreach (var comp in operatings) {
            if(comp!=null)
                comp.gameObject.SetActive(false);
        }
        platform = CDanmuSDKCenter.Ins.emPlatform;
        if(operatings[(int)platform]!=null)
            operatings[(int)platform].gameObject.SetActive(true);

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isShow = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isShow = false;
    }

    public void TempShowOpenBtn()
    {
        v = 0f;
        panelOn = true;
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
        isShow = false;
    }

    public void HideOpenBtn()
    {
        v = 0;
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
        isShow = false;
        panelOn = false;
    }
    float v;
    void Update()
    {
        if (isShow && panelOn)
        {
            if (cg.alpha == 1)
                return;
            cg.alpha = Mathf.MoveTowards(cg.alpha, 1, showTime);
            if (cg.alpha > 0.99f)
            {
                cg.alpha = 1;
                cg.interactable = true;
                cg.blocksRaycasts = true;
            }
        }
        else
        {
            if (cg.alpha == 0)
                return;
            cg.alpha = Mathf.SmoothDamp(cg.alpha, 0, ref v, showTime);
            if (cg.alpha < 0.01f)
            {
                cg.alpha = 0;
                cg.interactable = false;
                cg.blocksRaycasts = false;
            }
        }
    }
}