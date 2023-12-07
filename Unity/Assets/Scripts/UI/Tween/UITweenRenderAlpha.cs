using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITweenRenderAlpha : UITweenBase
{
    public float from;
    public float to;
    public SpriteRenderer[] objTargets;

    protected Color colorPlay = new Color();

    public override void Play(DelegateNFuncCall call = null)
    {
        base.Play(call);

        for (int i = 0; i < objTargets.Length; i++)
        {
            colorPlay = objTargets[i].color;
            colorPlay.a = from;
            objTargets[i].color = colorPlay;
        }

    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);

        colorPlay.a = from * (1 - curValue) + to * curValue;
        for (int i = 0; i < objTargets.Length; i++)
        {
            objTargets[i].color = colorPlay;
        }
    }

    [ContextMenu("GetTarget")]
    public void FindTweenRoot()
    {
        objTargets = new SpriteRenderer[2];
        objTargets[0] = transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        objTargets[1] = transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>();
    }
}
