using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class CUnitAnimeCtrl : SerializedMonoBehaviour
{
    public SpriteRenderer pAvatar;
    public EMUnitAnimeState emCurState;
    public EMUnitAnimeDir emCurDir = EMUnitAnimeDir.Up;
    public Dictionary<EMUnitAnimeState, CUnitAnimeStateSlot> dicAnimeSlots = new Dictionary<EMUnitAnimeState, CUnitAnimeStateSlot>();

    public float fAddAnimaSpeed;

    [ReadOnly]
    public float fAnimeTime = 0f;
    [ReadOnly]
    public bool bPlayAnime = false;
    public float fFrameTime = 0f;
    public int nCurFrame = 0;
    public CUnitAnimeStateSlot pCurStateSlot = null;
    public CUnitAnimeDirSlot pCurDirSlot = null;

    public bool bTest;

    public void PlayAnime(EMUnitAnimeState state, EMUnitAnimeDir dir, bool force = false)
    {
        //重复播放直接返回
        if (!force && 
            (emCurState == state && emCurDir == dir)) return;

        emCurState = state;
        emCurDir = dir;
        nCurFrame = 0;
        fFrameTime = 0f;
        bPlayAnime = true;

        pCurStateSlot = GetAnimeSlot(emCurState);
        pCurDirSlot = pCurStateSlot.GetDirSlot(emCurDir);
        fAnimeTime = pCurDirSlot.fFrameTime * pCurDirSlot.arrFrames.Length;

        if(pCurDirSlot.bRevert)
        {
            pAvatar.transform.localScale = new Vector3(-1,1,1);
        }
        else
        {
            pAvatar.transform.localScale = Vector3.one;
        }

        SetAvatarSprite(pCurDirSlot.arrFrames[nCurFrame]);
    }

    public void UpdateFrame(float delta)
    {
        if (!bPlayAnime) return;

        fFrameTime += delta * (1 + fAddAnimaSpeed);
        if (fFrameTime > (nCurFrame + 1) * pCurDirSlot.fFrameTime)
        {
            nCurFrame++;

            if(nCurFrame >= pCurDirSlot.arrFrames.Length)
            {
                if(pCurDirSlot.bLoop)
                {
                    nCurFrame = 0;
                    fFrameTime -= fAnimeTime;
                    SetAvatarSprite(pCurDirSlot.arrFrames[nCurFrame]);
                }
                else
                {
                    bPlayAnime = false;
                }
            }
            else
            {
                SetAvatarSprite(pCurDirSlot.arrFrames[nCurFrame]);
            }
        }
    }

    CUnitAnimeStateSlot GetAnimeSlot(EMUnitAnimeState state)
    {
        CUnitAnimeStateSlot pRes = null;
        if(dicAnimeSlots.TryGetValue(state, out pRes))
        {

        }

        return pRes;
    }

    void SetAvatarSprite(Sprite sprite)
    {
        pAvatar.sprite = sprite;
    }

    private void Update()
    {
        if(bTest)
        {
            UpdateFrame(CTimeMgr.DeltaTime);
        }
    }

    [ContextMenu("创建动画模板")]
    void InitAnimeCtrl()
    {
        dicAnimeSlots.Clear();
        for (int i = 0; i < (int)EMUnitAnimeState.Max; i++)
        {
            CUnitAnimeStateSlot pStateSlot = new CUnitAnimeStateSlot();
            pStateSlot.emState = (EMUnitAnimeState)i;

            pStateSlot.dicDirSlots.Clear();
            for (int dir = 0; dir < (int)EMUnitAnimeDir.Max; dir++)
            {
                CUnitAnimeDirSlot pDirSlot = new CUnitAnimeDirSlot();
                pDirSlot.bLoop = true;
                pDirSlot.fFrameTime = 0.15f;

                pStateSlot.dicDirSlots.Add((EMUnitAnimeDir)dir, pDirSlot);
            }

            dicAnimeSlots.Add((EMUnitAnimeState)i, pStateSlot);
        }
    }

    [ContextMenu("设置时间")]
    void ResetAnimeFrameTime()
    {
        foreach(CUnitAnimeStateSlot state in dicAnimeSlots.Values)
        {
            foreach(CUnitAnimeDirSlot slot in state.dicDirSlots.Values)
            {
                slot.fFrameTime = 0.08f;
            }
        }
    }
}
