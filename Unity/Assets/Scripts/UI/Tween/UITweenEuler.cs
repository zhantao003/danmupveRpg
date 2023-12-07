using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenEuler : UITweenBase
{
    public Vector3 from;
    public Vector3 to;
    public Transform tranTarget;    //Tween目标
    public bool bLocal;

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);

        if (bLocal)
        {
            tranTarget.localEulerAngles = from;
        }
        else
        {
            tranTarget.eulerAngles = from;
        }
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);

        //Debug.Log("Lerp:" + lerp + "   Cur:" + curValue);

        if (bLocal)
        {
            tranTarget.localEulerAngles = from * (1 - curValue) + to * curValue;
        }
        else
        {
            tranTarget.eulerAngles = from * (1 - curValue) + to * curValue;
        }
    }

    [ContextMenu("GetTarget")]
    public void FindTweenRoot()
    {
        tranTarget = transform;
    }
}
