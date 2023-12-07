using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIRawIconLoad : MonoBehaviour
{
    public RawImage uiIcon;

    public float fFixedLerp = 1f;

    [ReadOnly]
    public Vector2Int vOriginWH = new Vector2Int();
    [ReadOnly]
    public Vector2Int vCurWH = new Vector2Int();

    [ReadOnly]
    public string szBundle;

    [ReadOnly]
    public string szAsset;

    RectTransform tranIcon;

    /// <summary>
    /// 设置图标（同步版）
    /// </summary>
    /// <param name="assetPath"></param>
    /// <param name="deleEvent"></param>
    public virtual void SetIconSync(string assetPath, DelegateNFuncCall deleEvent = null)
    {
        if (uiIcon == null) return;

        if (string.IsNullOrEmpty(assetPath))
        {
            return;
        }

        CResLoadMgr.Inst.SynLoad(assetPath, CResLoadMgr.EM_ResLoadType.CanbeUnloadAssetbundle,
            delegate (Object pRes, object data, bool bSuc)
            {
                if (this == null) return;

                Texture2D pResObj = pRes as Texture2D;
                if (pResObj == null) return;

                if (uiIcon == null)
                {
                    uiIcon = GetComponent<RawImage>();
                }

                if (uiIcon == null) return;

                uiIcon.texture = pResObj;

                RectTransform tranOrigin = uiIcon.GetComponent<RectTransform>();
                vOriginWH.x = (int)tranOrigin.rect.width;
                vOriginWH.y = (int)tranOrigin.rect.height;

                RefreshSize();

                if (deleEvent != null)
                    deleEvent();
            });
    }

    public void SetActiveIcon(bool value)
    {
        uiIcon.gameObject.SetActive(value);
    }

    //刷新大小
    protected virtual void RefreshSize()
    {
        uiIcon.SetNativeSize();
        if (tranIcon == null)
            tranIcon = uiIcon.GetComponent<RectTransform>();

        tranIcon.sizeDelta = new Vector2(vOriginWH.x * fFixedLerp, vOriginWH.y * fFixedLerp);
    }

    [ContextMenu("Refresh")]
    public void Refresh()
    {
        if (uiIcon == null) return;
        RefreshSize();
    }

}