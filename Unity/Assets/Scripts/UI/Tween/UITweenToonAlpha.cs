using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenToonAlpha : UITweenBase
{
    public Renderer[] pRenderer;
    public string szKey = "_Tweak_transparency";
    public float from;
    public float to;
    public float cur;


    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        cur = from * (1 - curValue) + to * curValue;
        for(int i=0; i<pRenderer.Length; i++)
        {
            pRenderer[i].material.SetFloat(szKey, cur);
        }
    }
}
