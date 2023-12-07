using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenURPEmission : UITweenBase
{
    [ColorUsageAttribute(true, true)]
    public Color from;
    [ColorUsageAttribute(true, true)]
    public Color to;
    public Renderer objTarget;

    [ColorUsageAttribute(true, true)]
    public Color colorPlay = new Color();

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);
        //objTarget.material.EnableKeyword("_EMISSION");
        //objTarget.material.SetColor("_EmissionColor", from);
        objTarget.material.SetColor("_Emissive_Color", from);
        colorPlay = from;
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        colorPlay = from * (1 - curValue) + to * curValue;
        //objTarget.material.SetColor("_EmissionColor", colorPlay);
        objTarget.material.SetColor("_Emissive_Color", colorPlay);
    }
}
