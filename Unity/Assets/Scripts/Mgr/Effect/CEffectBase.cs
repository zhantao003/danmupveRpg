using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEffectBase : MonoBehaviour
{
    [ReadOnly]
    public long lGuid;

    public bool bAutoRecycle = false;
    public CPropertyTimer pLifeTicker = new CPropertyTimer();

    public CAudioMgr.CAudioSlottInfo pAudioPlay;

    [ReadOnly]
    public bool bPlaying = false;

    [ReadOnly]
    public bool bInited = false;

    private List<SpriteRenderer> listSprites = new List<SpriteRenderer>();
    private List<int> listSpritesOriginLayer = new List<int>();

    private List<ParticleSystem> listParticleSys = new List<ParticleSystem>();
    private List<int> listParticleOriginLayer = new List<int>();

    private List<Animator> listAnimator = new List<Animator>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    void Update()
    {
        OnUpdate(CTimeMgr.DeltaTime);
    }

    protected virtual void OnUpdate(float dt)
    {
        if (bAutoRecycle && bPlaying)
        {
            if (pLifeTicker.Tick(dt))
            {
                bPlaying = false;
                Recycle();
            }
        }
    }

    public virtual void Init()
    {
        enabled = true;

        if (!bInited)
        {
            lGuid = CEffectMgr.Instance.GenerateID();

            SpriteRenderer selfSprite = GetComponent<SpriteRenderer>();
            if(selfSprite!=null)
            {
                listSprites.Add(selfSprite);
                listSpritesOriginLayer.Add(selfSprite.sortingOrder);
            }

            ParticleSystem self = GetComponent<ParticleSystem>();
            if (self != null)
            {
                listParticleSys.Add(self);
                listParticleOriginLayer.Add(self.GetComponent<Renderer>().sortingOrder);
            }

            Animator selfAnime = GetComponent<Animator>();
            if (selfAnime != null)
            {
                listAnimator.Add(selfAnime);
            }

            SpriteRenderer[] arrSprites = GetComponentsInChildren<SpriteRenderer>();
            listSprites.AddRange(arrSprites);
            for(int i=0; i<arrSprites.Length; i++)
            {
                listSpritesOriginLayer.Add(arrSprites[i].sortingOrder);
            }

            ParticleSystem[] arrChilds = GetComponentsInChildren<ParticleSystem>();
            listParticleSys.AddRange(arrChilds);
            for (int i = 0; i < arrChilds.Length; i++)
            {
                listParticleOriginLayer.Add(arrChilds[i].GetComponent<Renderer>().sortingOrder);
            }

            Animator[] arrAnimation = GetComponentsInChildren<Animator>();
            listAnimator.AddRange(arrAnimation);

            bInited = true;
        }

        Play();
    }

    public virtual void Recycle()
    {
        //Debug.Log("基础回收");
        CEffectMgr.Instance.Recycle(this);
    }

    public virtual void SetLayer(int layer)
    {
        for(int i=0; i<listSprites.Count; i++)
        {
            listSprites[i].sortingOrder = listSpritesOriginLayer[i] + layer;
        }

        for(int i=0; i<listParticleSys.Count; i++)
        {
            listParticleSys[i].GetComponent<Renderer>().sortingOrder = listParticleOriginLayer[i] + layer;
        }
    }

    /// <summary>
    /// 暂停播放
    /// </summary>
    [ContextMenu("Stop")]
    public virtual void StopEffect()
    {
        listParticleSys.ForEach(t => t.Stop());
        listAnimator.ForEach(t =>
        {
            t.speed = 0;
        });

        bPlaying = false;
    }

    [ContextMenu("Pause")]
    public virtual void PauseEffect()
    {
        listParticleSys.ForEach(t => t.Pause());
        listAnimator.ForEach(t =>
        {
            t.speed = 0;
        });

        bPlaying = false;
    }

    /// <summary>
    /// 播放粒子特效
    /// </summary>
    [ContextMenu("Play")]
    public virtual void Play(bool refresh = true)
    {
        bPlaying = true;

        listParticleSys.ForEach(t =>
        {
            if (t == null) return;

            if(refresh)
            {
                t.Simulate(0, false, true);
            }
            t.Play();
        });

        listAnimator.ForEach(t =>
        {
            if (t == null) return;
            t.speed = 1;
            if(refresh)
            {
                t.Play(t.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
            }
        });

        if (pAudioPlay != null && refresh)
        {
            CAudioMgr.Ins.PlaySoundBySlot(pAudioPlay, transform.position);
        }

        if (bAutoRecycle && refresh)
        {
            pLifeTicker.FillTime();
        }
    }
}
