using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBase : MonoBehaviour
{
    public UIResType emType = UIResType.None;
    public bool bDontDestroy = false;

    public enum EMCamType
    {
        None,
        Default,
        Uppon,
    }
    public EMCamType emCamType = EMCamType.None;

    void Start()
    {
        OnStart();
    }

    void Update()
    {
        OnUpdate(CTimeMgr.DeltaTime);
    }

    protected virtual void OnStart()
    {

    }

    protected virtual void OnUpdate(float dt)
    {

    }

    public virtual void OnOpen()
    {

    }

    public virtual void OnClose()
    {

    }

    public virtual bool IsOpen()
    {
        return gameObject.activeSelf;
    }

    public virtual void CloseSelf()
    {
        UIManager.Instance.CloseUI(emType);
    }
}
