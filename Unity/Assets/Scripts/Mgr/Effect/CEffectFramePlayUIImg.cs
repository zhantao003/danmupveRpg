using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CEffectFramePlayUIImg : MonoBehaviour
{
    public Image pRender;
    public Sprite[] arrFrames;
    public bool bLoop;
    public bool bRevert;
    public float fPerFrameTime = 0.15f;
    public Vector3 vScaleLerp = new Vector3(1, 1, 1);

    public float fDelayTime;

    [ReadOnly]
    public float fAnimeTime = 0f;
    [ReadOnly]
    public bool bPlayAnime = false;
    [ReadOnly]
    public float fPlayTime = 0f;
    [ReadOnly]
    public int nCurFrame = 0;
    [ReadOnly]
    public float fCurDelayTime = 0f;

    public bool bAutoPlay = false;

    private void Start()
    {
        
    }

    private void OnEnable()
    {
        if (bAutoPlay)
        {
            PlayAnime();
        }
    }

    private void Update()
    {
        if (fCurDelayTime > 0f)
        {
            //Debug.Log(fCurDelayTime);
            fCurDelayTime -= CTimeMgr.DeltaTimeUnScale;
            if (fCurDelayTime <= 0f)
            {
                pRender.enabled = true;
            }

            return;
        }

        UpdateFrame(CTimeMgr.DeltaTimeUnScale);
    }

    public void PlayAnime()
    {
        //重复播放直接返回
        nCurFrame = 0;
        fPlayTime = 0f;
        bPlayAnime = true;
        fCurDelayTime = fDelayTime;

        fAnimeTime = fPerFrameTime * arrFrames.Length;

        if (bRevert)
        {
            transform.localScale = new Vector3(-vScaleLerp.x, vScaleLerp.y, vScaleLerp.z);
        }
        else
        {
            transform.localScale = new Vector3(vScaleLerp.x, vScaleLerp.y, vScaleLerp.z);
        }

        if (fCurDelayTime <= 0f)
        {
            pRender.enabled = true;
        }
        else
        {
            pRender.enabled = false;
        }

        SetAvatarSprite(arrFrames[nCurFrame]);
    }

    public void StopAnime()
    {
        bPlayAnime = false;
        pRender.enabled = false;
    }

    public void UpdateFrame(float delta)
    {
        if (!bPlayAnime) return;

        fPlayTime += delta;
        if (fPlayTime > (nCurFrame + 1) * fPerFrameTime)
        {
            nCurFrame++;

            if (nCurFrame >= arrFrames.Length)
            {
                if (bLoop)
                {
                    nCurFrame = 0;
                    fPlayTime -= fAnimeTime;
                    SetAvatarSprite(arrFrames[nCurFrame]);
                }
                else
                {
                    bPlayAnime = false;
                }
            }
            else
            {
                SetAvatarSprite(arrFrames[nCurFrame]);
            }
        }
    }

    void SetAvatarSprite(Sprite sprite)
    {
        pRender.sprite = sprite;
    }
}
