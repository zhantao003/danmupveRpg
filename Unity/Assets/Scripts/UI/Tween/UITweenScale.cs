using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenScale : UITweenBase
{
    public Vector3 from;
    public Vector3 to;
    public Transform tranTarget;

    Vector3 curScale;

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);
        tranTarget.localScale = from;
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);

        tranTarget.localScale = from * (1 - curValue) + to * curValue;
    }
}
