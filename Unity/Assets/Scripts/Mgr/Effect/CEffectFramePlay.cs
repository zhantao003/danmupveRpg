using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Rendering;

[System.Serializable]
public class CEffectFrameSound
{
    public int nFrameIdx;
    public CAudioMgr.CAudioSlottInfo pSound;
}

public class CEffectFramePlay : MonoBehaviour
{
    public SpriteRenderer pRender;
    public Sprite[] arrFrames;
    public bool bLoop;
    public bool bRevert;
    public float fPerFrameTime = 0.15f;
    public Vector3 vScaleLerp = new Vector3(1,1,1);

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

    [ReadOnly]
    public int nCurSoundFrameIdx = 0;

    public CEffectFrameSound[] arrFrameSound;

    int nOriginLayer;

    public bool bAutoPlay = false;

    private void Start()
    {
        if(bAutoPlay)
        {
            PlayAnime();
        }
    }

    public void Init()
    {
        nOriginLayer = pRender.sortingOrder;
    }

    public void SetFrameLayer(int layer)
    {
        pRender.sortingOrder = nOriginLayer + layer;
    }

    private void Update()
    {
        if(fCurDelayTime > 0f)
        {
            //Debug.Log(fCurDelayTime);
            fCurDelayTime -= CTimeMgr.DeltaTime;
            if(fCurDelayTime <= 0f)
            {
                pRender.enabled = true;
            }

            return;
        }

        UpdateFrame(CTimeMgr.DeltaTime);
    }

    public void PlayAnime(bool force = false)
    {
        //重复播放直接返回
        nCurFrame = 0;
        nCurSoundFrameIdx = 0;
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

            if(arrFrameSound!=null && 
               arrFrameSound.Length > 0 &&
               nCurSoundFrameIdx < arrFrameSound.Length)
            {
                CheckSound();
            }

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

    void CheckSound()
    {
        if (nCurFrame < arrFrameSound[nCurSoundFrameIdx].nFrameIdx) return;

        if (arrFrameSound[nCurSoundFrameIdx].pSound != null)
        {
            CAudioMgr.Ins.PlaySoundBySlot(arrFrameSound[nCurSoundFrameIdx].pSound, transform.position);
            nCurSoundFrameIdx++;
        }
    }

    void SetAvatarSprite(Sprite sprite)
    {
        pRender.sprite = sprite;
    }
}
