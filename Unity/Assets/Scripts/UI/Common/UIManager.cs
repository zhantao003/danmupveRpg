using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Instance

    private static UIManager __instatnc__ = null;
    public static UIManager Instance
    {
        get
        {
            if (__instatnc__ == null)
            {
                GameObject gameobject = new GameObject("[Game - UIManger]");
                DontDestroyOnLoad(gameobject);
                __instatnc__ = gameobject.AddComponent<UIManager>();
            }

            return __instatnc__;
        }

    }

    #endregion

    public UICamera uiCamDefault;
    public UICamera uiCamUppon;

    //UI对象管理容器
    protected Dictionary<UIResType, UIBase> dicUIs = new Dictionary<UIResType, UIBase>();

    /// <summary>
    /// 刷新UI管理器
    /// </summary>
    public void RefreshUI()
    {
        //找到UI摄像机
        UICamera[] arrCam = FindObjectsOfType<UICamera>();
        for(int i=0; i<arrCam.Length; i++)
        {
            if(arrCam[i].emType == UICamera.EMType.Default)
            {
                uiCamDefault = arrCam[i];
            }
            else if(arrCam[i].emType == UICamera.EMType.Uppon)
            {
                uiCamUppon = arrCam[i];
            }
        }

        //遍历所有UI
        UISceneRoot uiRoot = FindObjectOfType<UISceneRoot>();
        if (uiRoot == null) return;

        UIBase[] arrUI = uiRoot.transform.GetComponentsInChildren<UIBase>(true);
        for (int i=0; i<arrUI.Length; i++)
        {
            UIBase uiRes = arrUI[i];
            if (uiRes == null) continue;

            if (dicUIs.ContainsKey(uiRes.emType))
            {
                Destroy(uiRes.gameObject);
                continue;
            }
                
            dicUIs.Add(uiRes.emType, uiRes);

            CloseUI(uiRes.emType);

            if (uiRes.bDontDestroy)
            {
                arrUI[i].transform.SetParent(null);
                DontDestroyOnLoad(arrUI[i].gameObject);
            }
        }

        //绑定摄像机
        if (uiCamDefault == null) return;
        foreach (UIBase uiRes in dicUIs.Values)
        {
            if (uiRes == null ||
                uiRes.emCamType == UIBase.EMCamType.None) continue;
            Canvas uiCanvas = uiRes.GetComponent<Canvas>();

            if (uiRes.emCamType == UIBase.EMCamType.Default)
            {
                uiCanvas.worldCamera = uiCamDefault.uiCam;
            }
            else if(uiRes.emCamType == UIBase.EMCamType.Uppon)
            {
                uiCanvas.worldCamera = uiCamUppon.uiCam;
            }
        }
    }

    /// <summary>
    /// 清空当前的所有UI
    /// </summary>
    public void ClearUI()
    {
        List<UIResType> listDel = new List<UIResType>();
        foreach(UIBase uiRes in dicUIs.Values)
        {
            if (uiRes.bDontDestroy) continue;

            uiRes.OnClose();
            listDel.Add(uiRes.emType);
        }

        for(int i=0; i<listDel.Count; i++)
        {
            dicUIs.Remove(listDel[i]);
        }
    }

    public void OpenUI(UIResType uiType)
    {
        Debug.Log("Open UI:" + uiType.ToString());
        UIBase uiRes = GetUI(uiType);
        if (uiRes == null)
        {
            Debug.LogWarning("UI并没有加入管理器:" + uiType);
            return;
        }

        uiRes.gameObject.SetActive(true);
        uiRes.OnOpen();
    }

    public void CloseUI(UIResType uiType)
    {
        UIBase uiRes = GetUI(uiType);
        if (uiRes == null)
        {
            Debug.LogWarning("UI并没有加入管理器:" + uiType);
            return;
        }

        uiRes.gameObject.SetActive(false);
        uiRes.OnClose();
    }

    /// <summary>
    /// 获取UI
    /// </summary>
    public UIBase GetUI(UIResType uiType)
    {
        UIBase uiRes = null;
        if (dicUIs.TryGetValue(uiType, out uiRes))
        {

        }
        return uiRes;
    }

    private void OnDestroy()
    {
        ClearUI();
    }
}
