using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CAudioAutoSlot : MonoBehaviour
{
    public bool bPlayOnAwake = true;

    public CAudioMgr.CAudioSlottInfo pAudioPlay;   //打牌音效

    CAudioMgr.CAudioSourcePlayer pPlayer = null;

    //void Awake()
    //{
    //    if (bPlayOnAwake)
    //    {
    //        Play();
    //    }
    //}

    // Use this for initialization
    void Start()
    {

    }

    void OnEnable()
    {
        if(bPlayOnAwake)
        {
            Play();
        }
    }

    void OnDisable()
    {
        Stop();
    }

    public void Play()
    {
        pPlayer = CAudioMgr.Ins.PlaySoundBySlot(pAudioPlay, transform.position);
    }

    public void Stop()
    {
        if (pAudioPlay.bLoop)
        {
            if (pPlayer != null && pPlayer.pSource != null)
            {
                pPlayer.pSource.Stop();
            }
        }
    }
}
