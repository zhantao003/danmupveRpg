using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITweenAlpha : UITweenBase
{
    public float from;
    public float to;
    public Graphic[] objTargets;

    protected Color colorPlay = new Color();

    public bool bInited = false;

    private void Start()
    {
        Init();
    }

    void Init()
    {
        if (bInited) return;

        for (int i = 0; i < objTargets.Length; i++)
        {
            colorPlay = objTargets[i].color;
            colorPlay.a = from;
            objTargets[i].color = colorPlay;
        }

        bInited = true;
    }

    public override void Play(DelegateNFuncCall call = null)
    {
        Init();

        base.Play(call);
        for(int i = 0;i < objTargets.Length;i++)
        {
            colorPlay = objTargets[i].color;
            colorPlay.a = from;
            objTargets[i].color = colorPlay;
        }
    }

    protected override void Refresh(float lerp)
    {
        base.Refresh(lerp);
        for (int i = 0; i < objTargets.Length; i++)
        {
            colorPlay = objTargets[i].color;
            colorPlay.a = from * (1 - curValue) + to * curValue;
            objTargets[i].color = colorPlay;
        }
    }

    [ContextMenu("GetTarget")]
    public void FindTweenRoot()
    {
        objTargets = new Graphic[2];
        objTargets[0] = transform.GetChild(0).GetChild(0).GetComponent<Graphic>();
        objTargets[1] = transform.GetChild(0).GetChild(1).GetComponent<Graphic>();
    }
}
