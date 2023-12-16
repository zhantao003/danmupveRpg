using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CEffectFrameGroup : CEffectBase
{
    public CEffectFramePlay[] arrFrameEff;

    public UITweenBase[] arrTween;

    public override void Init(bool play = true)
    {
        if(!bInited)
        {
            for(int i=0; i<arrFrameEff.Length; i++)
            {
                arrFrameEff[i].Init();
            }
        }

        base.Init(play);
    }

    public void SetFrameLayer(int layer)
    {
        for(int i=0; i<arrFrameEff.Length; i++)
        {
            arrFrameEff[i].SetFrameLayer(layer);
        }
    }

    public override void Play(bool refresh = true)
    {
        for (int i = 0; i < arrFrameEff.Length; i++)
        {
            arrFrameEff[i].PlayAnime();
        }

        for(int i=0; i<arrTween.Length; i++)
        {
            arrTween[i].Play();
        }

        base.Play(refresh);
    }

    [ContextMenu("PlayFrame")]
    public void TestPlay()
    {
        transform.position = new Vector3(28.5f, 19.5f, 0f);
        Init();
    }

    [ContextMenu("StopFrame")]
    public void TestStop()
    {
        StopEffect();
    }

    public override void StopEffect()
    {
        for (int i = 0; i < arrFrameEff.Length; i++)
        {
            arrFrameEff[i].StopAnime();
        }

        base.StopEffect();
    }

    public override void Recycle()
    {
        for (int i = 0; i < arrFrameEff.Length; i++)
        {
            arrFrameEff[i].StopAnime();
        }

        base.Recycle();
    }

    protected override void OnUpdate(float dt)
    {
        base.OnUpdate(dt);

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    TestPlay();
        //}
    }
}
