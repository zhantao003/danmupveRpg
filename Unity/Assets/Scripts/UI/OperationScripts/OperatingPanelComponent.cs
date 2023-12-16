using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OperatingPanelComponent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public MainOpController mainOp;
    public MovePanelComponent mp;
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointEnter = true;
        pointStayTime = 0f;
        refF = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointEnter = false;
        pointStayTime = 1f;
        refF = 0f;
    }
    [Header("操作说明图")]
    public Image opPanel;
    public CanvasGroup cgPanel;
    public Button btn_Open;
    public Button btn_Close;
    public Button btn_Magnify;
    public Button btn_Shrink;
    //public Button btn_HoverInfo;

    public CanvasGroup cgFunctions;
    [HideInInspector]
    public bool isPointEnter = false;
    [HideInInspector]
    public float pointStayTime = 0;
    [Header("鼠标悬停显示CgFunctions所需时间")]
    public float editWaitTime = 0.5f;
    [Header("UI渐显渐隐时间")]
    public float cgShowTime = 0.2f;
    float refF = 0f;
    // Start is called before the first frame update
    Vector2 deltaZoom;
    [Header("UI放大缩小的当前档位，每档像素增大缩小原图的10%")]
    public int nowSizeBox = 4;
    [Header("UI放大缩小的最大挡位")]
    public int maxSizeBox = 10;

    public Transform tranZiRoot;

    int originSizeBox ;

    public GameObject[] objNormal;
    public GameObject[] objNet;
    public Sprite pNormalSprite;
    public Sprite pNetSprite;

    void Start()
    {
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            opPanel.sprite = pNormalSprite;
        }
        else if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            opPanel.sprite = pNetSprite;
        }
        if (objNormal != null)
        {
            for (int i = 0; i < objNormal.Length; i++)
            {
                objNormal[i].SetActive(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP);
            }
        }
        if (objNet != null)
        {
            for (int i = 0; i < objNet.Length; i++)
            {
                objNet[i].SetActive(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP);
            }
        }
        CheckDefaultOpen();
        InitSizeBox();
        btn_Open.onClick.AddListener(OpenPanel);
        btn_Magnify.onClick.AddListener(MagnifyPanel);
        btn_Shrink.onClick.AddListener(ShrinkPanel);
        btn_Close.onClick.AddListener(HidePanel);
    }

    void CheckDefaultOpen()
    {
        int panelStatu = PlayerPrefs.GetInt("OpenPanel", 1);
        panelStatu = 1;
        if (panelStatu == 1)
        {
            cgPanel.alpha = 1;
            cgPanel.blocksRaycasts = true;
            cgPanel.interactable = true;

            cgFunctions.alpha = 1;
            cgFunctions.blocksRaycasts = true;
            cgFunctions.interactable = true;

            //OpenPanel();
            OnPointerExit(null);
            mainOp.HideOpenBtn();
        }
        else
        {
            cgPanel.alpha = 0;
            cgPanel.blocksRaycasts = false;
            cgPanel.interactable = false;

            cgFunctions.alpha = 1;
            cgFunctions.blocksRaycasts = false;
            cgFunctions.interactable = false;
            mainOp.TempShowOpenBtn();
        }
        //btn_Open.gameObject.SetActive(true);
    }


    float panelShowHideTime = 0.2f;
    float _;
    Vector2 __;
    Vector2 showHidePos;
    WaitForEndOfFrame wtf = new WaitForEndOfFrame();

    void OpenPanel()
    {
        PlayerPrefs.SetInt("OpenPanel", 1);
        mainOp.HideOpenBtn();
        _ = 0f;
        __ = Vector2.zero;
        panelShowHideTime = cgShowTime;
        //showHidePos = opPanel.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0f, 0);
        StartCoroutine(ShowPanelIE());
    }

    IEnumerator ShowPanelIE()
    {
        tranZiRoot.gameObject.SetActive(true);
        while (panelShowHideTime > 0 || cgPanel.alpha < 0.999f)
        {
            cgPanel.alpha = Mathf.SmoothDamp(cgPanel.alpha, 1, ref _, cgShowTime);
            //opPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(opPanel.GetComponent<RectTransform>().anchoredPosition, showHidePos, ref __, cgShowTime);
            panelShowHideTime -= Time.deltaTime;
            yield return wtf;
        }
        
        cgPanel.alpha = 1;
        cgPanel.blocksRaycasts = true;
        cgPanel.interactable = true;

        cgFunctions.alpha = 1;
        cgFunctions.blocksRaycasts = true;
        cgFunctions.interactable = true;
        OnPointerExit(null);
        //btn_Open.gameObject.SetActive(false);
    }

    void HidePanel()
    {
        PlayerPrefs.SetInt("OpenPanel", 0);
        _ = 0f;
        __ = Vector2.zero;
        panelShowHideTime = cgShowTime;
        //showHidePos = opPanel.GetComponent<RectTransform>().anchoredPosition += new Vector2(0f, 0);
        StartCoroutine(HidePanelIE());
    }

    IEnumerator HidePanelIE()
    {
        tranZiRoot.gameObject.SetActive(false);
        while (panelShowHideTime > 0 || cgPanel.alpha > 0.001f)
        {
            cgPanel.alpha = Mathf.SmoothDamp(cgPanel.alpha, 0, ref _, cgShowTime);
            //opPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.SmoothDamp(opPanel.GetComponent<RectTransform>().anchoredPosition, showHidePos, ref __, cgShowTime);
            panelShowHideTime -= Time.deltaTime;
            yield return wtf;
        }
        
        cgPanel.alpha = 0;
        cgPanel.blocksRaycasts = false;
        cgPanel.interactable = false;
        mainOp.TempShowOpenBtn();
    }

    void InitSizeBox()
    {
        deltaZoom = new Vector2(opPanel.GetComponent<RectTransform>().sizeDelta.x / 10f, opPanel.GetComponent<RectTransform>().sizeDelta.y / 10f);
        originSizeBox = nowSizeBox;
        //originLH = tranLeft.GetComponent<RectTransform>().anchoredPosition.y;
        //originRH = tranRight.GetComponent<RectTransform>().anchoredPosition.y;
        nowSizeBox = PlayerPrefs.GetInt("SizeBox", nowSizeBox);
        opPanel.GetComponent<RectTransform>().sizeDelta += (nowSizeBox - originSizeBox) * deltaZoom;

        tranZiRoot.localScale =Vector3.one*((nowSizeBox - originSizeBox) * 0.1f + 1);
        SetMinX();
        mp.InitBoxPos();


    }

    void MagnifyPanel()
    {
        if (nowSizeBox < maxSizeBox)
        {
            opPanel.GetComponent<RectTransform>().sizeDelta += deltaZoom;
            nowSizeBox++;
            tranZiRoot.localScale = Vector3.one * ((nowSizeBox - originSizeBox) * 0.1f + 1);
            PlayerPrefs.SetInt("SizeBox", nowSizeBox);
            SetMinX();
        }
    }

    void ShrinkPanel()
    {
        if (nowSizeBox > 0)
        {
            opPanel.GetComponent<RectTransform>().sizeDelta -= deltaZoom;
            nowSizeBox--;
            tranZiRoot.localScale = Vector3.one * ((nowSizeBox - originSizeBox) * 0.1f + 1);
            PlayerPrefs.SetInt("SizeBox", nowSizeBox);
            SetMinX();
        }
    }

    void SetMinX() {
        bool isHor = false;
        //if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Bilibili || CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.Douyu)
        //{
        //    isHor = true;
        //}
        //else
        //{
        //    isHor = false;
        //}
        if (isHor)
        {
            mp.xMin = opPanel.GetComponent<RectTransform>().sizeDelta.x - 1920f + 40f;
        }
        else {
            mp.xMin = opPanel.GetComponent<RectTransform>().sizeDelta.x - (1080f/16f*9f) + 40f;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (isPointEnter)
        {
            if (pointStayTime < editWaitTime)
            {
                pointStayTime += Time.deltaTime;
            }
            else
            {
                if (cgFunctions.alpha == 1)
                    return;
                cgFunctions.alpha = Mathf.MoveTowards(cgFunctions.alpha, 1, cgShowTime);
                if (cgFunctions.alpha > 0.9f)
                {
                    cgFunctions.alpha = 1f;
                    cgFunctions.blocksRaycasts = true;
                    cgFunctions.interactable = true;
                }
            }
        }
        else
        {
            if (pointStayTime > 0)
            {
                pointStayTime -= Time.deltaTime;
            }
            else
            {
                if (cgFunctions.alpha == 0)
                    return;
                cgFunctions.alpha = Mathf.SmoothDamp(cgFunctions.alpha, 0, ref refF, cgShowTime);
                if (cgFunctions.alpha < 0.1f)
                {
                    cgFunctions.alpha = 0f;
                    cgFunctions.blocksRaycasts = false;
                    cgFunctions.interactable = false;
                }
            }
        }
    }
}
