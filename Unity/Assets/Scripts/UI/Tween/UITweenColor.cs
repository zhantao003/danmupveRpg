using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITweenColor : UITweenBase
{
    public Color from;
    public Color to;
    public Graphic objTarget;

    protected Color colorPlay = new Color();

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);
        objTarget.color = from;
        colorPlay = from;
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        colorPlay = from * (1 - curValue) + to * curValue;
        objTarget.color = colorPlay;
    }
}
