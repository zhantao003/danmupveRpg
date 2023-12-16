using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenRenderColor : UITweenBase
{
    public Color from;
    public Color to;
    public SpriteRenderer objTarget;

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
