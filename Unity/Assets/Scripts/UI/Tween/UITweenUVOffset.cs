using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenUVOffset : UITweenBase
{
    public Renderer pRenderer;
    public string szUVKey;
    public Vector2 from;
    public Vector2 to;
    public Vector2 cur;


    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        cur = from * (1 - curValue) + to * curValue;
        pRenderer.material.SetTextureOffset(szUVKey, cur);
    }
}
