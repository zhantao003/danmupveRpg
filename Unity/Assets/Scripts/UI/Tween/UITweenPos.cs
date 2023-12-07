using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenPos : UITweenBase
{
    public Vector3 from;
    public Vector3 to;
    public Transform tranTarget;    //Tween目标
    public bool bLocal;             //本地坐标

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);

        if (bLocal)
        {
            tranTarget.localPosition = from;
        }
        else
        {
            tranTarget.position = from;
        }
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        if (bLocal)
        {
            tranTarget.localPosition = from * (1 - curValue) + to * curValue;
        }
        else
        {
            tranTarget.position = from * (1 - curValue) + to * curValue;
        }
    }
}
