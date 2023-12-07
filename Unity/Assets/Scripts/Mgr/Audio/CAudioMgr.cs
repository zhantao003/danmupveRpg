using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CAudioMgr : CSingleCompBase<CAudioMgr>
{
    #region 基本数据

    public delegate void DlgLoadClip(AudioClip clip);

    //音频表地址
    public static readonly string TBL_AUDIO_PATH = "TBL/Audio";

    //更新间隔
    public CPropertyTimer pUpdateTimer;

    //音频信息单元ByID
    [System.Serializable]
    public class CAudioSlottInfo
    {
        public AudioClip clip;
        public Vector2 vClipRange = new Vector2(1, 8);
        public bool bLoop = false;
        public float fVolum = 1F;
        public float fPitch = 1F;
        public float fBlend = 1F;
    }

    //音频播放单元
    public class CAudioSourcePlayer
    {
        public AudioSource pSource;
        public float fOriginVolum;
        public float fVolumLerp;    //音量插值
    }

    //音频基本信息
    public class ST_AudioInfo
    {
        public string nID;
        public bool bSync;   //同步加载还是异步加载
        public string szClipRes;
        public string szPath;
    }

    protected Dictionary<string, ST_AudioInfo> dicAudioInfo = new Dictionary<string, ST_AudioInfo>();

    //初始化(只需要调用一次)
    bool bInited = false;
    public void Init()
    {
        if (bInited) return;

        //CTBLInfo.Inst.LoadTBL(TBL_AUDIO_PATH, OnLoadAudioInfo);

        //CAudioModelMgr.Instance.Init();

        pUpdateTimer = new CPropertyTimer();
        pUpdateTimer.Value = 0.5F;

        bInited = true;

        StartCoroutine(UpdateAudioSource());
    }

    //加载表数据
    public void OnLoadAudioInfo(CTBLLoader loader)
    {
        for (int i = 0; i < loader.GetLineCount(); i++)
        {
            loader.GotoLineByIndex(i);

            ST_AudioInfo pInfo = new ST_AudioInfo();
            pInfo.nID = loader.GetStringByName("clip");
            //pInfo.bLoop = ((loader.GetIntByName("loop") == 1) ? true : false);
            //pInfo.fVolum = loader.GetFloatByName("volum");
            //pInfo.fBlend = loader.GetFloatByName("blend");
            //pInfo.fMinRange = loader.GetFloatByName("minRange");
            //pInfo.fMaxRange = loader.GetFloatByName("maxRange");
            pInfo.szClipRes = pInfo.nID;
            pInfo.bSync = (loader.GetIntByName("sync") == 1 );
            pInfo.szPath = loader.GetStringByName("path");

            dicAudioInfo.Add(pInfo.nID, pInfo);
        }
    }

    #endregion

    #region 通用容器与接口

    public Dictionary<string, AudioClip> dicAudioClip = new Dictionary<string, AudioClip>();  //所有加载过的音频Clip
    protected List<CAudioSourcePlayer> listAudioSourceIdle = new List<CAudioSourcePlayer>();     //待机AudioSource
    protected List<CAudioSourcePlayer> listAudioSourceActive = new List<CAudioSourcePlayer>();   //播放中的AudioSource

    protected float fMainVolum = 1F;         //主音量
    protected float fAudioSoundVolum = 1F;   //全局音效音量
    protected float fAudioMusicVolum = 1F;   //全局音乐音量

    public float MainVolum
    {
        get
        {
            return fMainVolum;
        }
        set
        {
            fMainVolum = value;

            for (int i = 0; i < listAudioSourceActive.Count; i++)
            {
                if (listAudioSourceActive[i].pSource == null ||
                   !listAudioSourceActive[i].pSource.isPlaying) continue;

                listAudioSourceActive[i].pSource.volume = listAudioSourceActive[i].fOriginVolum * fAudioSoundVolum * fMainVolum;
            }

            for (int i = 0; i < (int)MusicTrac.Max; i++)
            {
                if (dicAudioSourceMusic.ContainsKey((MusicTrac)i) &&
                   dicAudioSourceMusic[(MusicTrac)i] != null &&
                   dicAudioSourceMusic[(MusicTrac)i].pSource.isPlaying)
                {
                    dicAudioSourceMusic[(MusicTrac)i].pSource.volume = dicAudioSourceMusic[(MusicTrac)i].fOriginVolum * fAudioMusicVolum * fMainVolum;
                }
            }
        }
    }

    public float VolumSound
    {
        get { return fAudioSoundVolum; }
        set
        {
            fAudioSoundVolum = value;

            for (int i = 0; i < listAudioSourceActive.Count; i++)
            {
                if (listAudioSourceActive[i].pSource == null ||
                   !listAudioSourceActive[i].pSource.isPlaying) continue;

                listAudioSourceActive[i].pSource.volume = listAudioSourceActive[i].fOriginVolum * fAudioSoundVolum * fMainVolum;
            }
        }
    }

    public float VolumMusic
    {
        get { return fAudioMusicVolum; }
        set
        {
            fAudioMusicVolum = value;

            for (int i = 0; i < (int)MusicTrac.Max; i++)
            {
                if (dicAudioSourceMusic.ContainsKey((MusicTrac)i) &&
                   dicAudioSourceMusic[(MusicTrac)i] != null &&
                   dicAudioSourceMusic[(MusicTrac)i].pSource.isPlaying)
                {
                    dicAudioSourceMusic[(MusicTrac)i].pSource.volume = dicAudioSourceMusic[(MusicTrac)i].fOriginVolum * fAudioMusicVolum * fMainVolum;
                }
            }
        }
    }

    private void Update()
    {
        OnUpdate(CTimeMgr.DeltaTime);
    }

    public void OnUpdate(float delta)
    {
        //音乐动作刷新
        if (listMusicAction.Count > 0)
        {
            for (int i = 0; i < listMusicAction.Count;)
            {
                listMusicAction[i].OnUpdate(delta);

                if(listMusicAction[i].IsActive())
                {
                    i++;
                }
                else
                {
                    listMusicAction.RemoveAt(i);
                }
            }
        }

        //if (pUpdateTimer != null)
        //{
        //    if (!pUpdateTimer.Tick(delta))
        //    {
        //        return;
        //    }

        //    pUpdateTimer.FillTime();
        //}
        //else
        //{
        //    pUpdateTimer = new CPropertyTimer();
        //    pUpdateTimer.Value = 0.2F;

        //    return;
        //}


        //for (int i = 0; i < listAudioSourceActive.Count;)
        //{
        //    if (listAudioSourceActive[i].pSource == null)
        //    {
        //        listAudioSourceActive.RemoveAt(i);
        //        continue;
        //    }

        //    if (!listAudioSourceActive[i].pSource.isPlaying)
        //    {
        //        listAudioSourceIdle.Add(listAudioSourceActive[i]);
        //        //listAudioSourceActive[i].pSource.gameObject.SetActive(false);
        //        listAudioSourceActive.RemoveAt(i);
        //    }
        //    else
        //    {
        //        i++;
        //    }
        //}
    }

    IEnumerator UpdateAudioSource()
    {
        while(true)
        {
            for (int i = 0; i < listAudioSourceActive.Count;)
            {
                if (listAudioSourceActive[i].pSource == null)
                {
                    listAudioSourceActive.RemoveAt(i);
                    continue;
                }

                if (!listAudioSourceActive[i].pSource.isPlaying)
                {
                    listAudioSourceIdle.Add(listAudioSourceActive[i]);
                    //listAudioSourceActive[i].pSource.gameObject.SetActive(false);
                    listAudioSourceActive.RemoveAt(i);
                }
                else
                {
                    i++;
                }

                yield return 0;
            }

            yield return new WaitForSeconds(0.2F);
        }
    }

    //获取指定ID的音频基本信息
    public ST_AudioInfo GetAudioInfo(string nID)
    {
        ST_AudioInfo pInfo = null;
        if (dicAudioInfo.TryGetValue(nID, out pInfo))
        {
            return pInfo;
        }
        return pInfo;
    }

    //public Config_Sound GetAudioInfo(int nID)
    //{
    //    if (ConfigureManager.Inst == null ||
    //        ConfigureManager.Inst.pSound == null) return null;
    //    Config_Sound pInfo = ConfigureManager.Inst.pSound.GetSound(nID);

    //    return pInfo;
    //}

    /// <summary>
    /// 加载音频资源 
    /// </summary>
    /// <param name="nID"></param>
    /// <param name="dlg"></param>
    public void GetClipRes(ST_AudioInfo pInfo, DlgLoadClip dlg = null)
    {
        if (pInfo == null)
        {
            Debug.LogWarning("None Audio Info");
            return;
        }

        AudioClip pClip = null;
        if (dicAudioClip.TryGetValue(pInfo.nID, out pClip))
        {
            //已经加载过了
            if (dlg != null)
            {
                dlg(pClip);
            }

            return;
        }

        if (pInfo.bSync)
        {
            //同步加载
            CResLoadMgr.Inst.SynLoad(pInfo.szPath + "/" + pInfo.szClipRes, CResLoadMgr.EM_ResLoadType.Audio,
            delegate (Object res, object data, bool bSuc)
            {
                pClip = res as AudioClip;
                dicAudioClip.Add(pInfo.nID, pClip);

                if (dlg != null)
                {
                    dlg(pClip);
                }
            });
        }
        else
        {
            //异步加载
            //加载bundle
            CResLoadMgr.Inst.ACreateAssetByType(pInfo.szPath, pInfo.szClipRes, typeof(AudioClip),
                delegate (Object pRes) {
                    pClip = pRes as AudioClip;
                    if (!dicAudioClip.ContainsKey(pInfo.nID))
                    {
                        dicAudioClip.Add(pInfo.nID, pClip);
                    }

                    if (dlg != null)
                    {
                        dlg(pClip);
                    }
                }, 
                CResLoadMgr.EM_ResLoadType.Audio);
        }
    }

    //取待机链表中的AudioSource
    protected CAudioSourcePlayer PopIdleAudioSource()
    {
        bool bAddNew = true;
        CAudioSourcePlayer pSource = null;
        if (listAudioSourceIdle.Count > 0)
        {
            pSource = listAudioSourceIdle[0];
            listAudioSourceIdle.RemoveAt(0);
            if (pSource != null && pSource.pSource != null)
            {
                bAddNew = false;
            }
        }

        if (bAddNew)
        {
            pSource = new CAudioSourcePlayer();
            GameObject objNew = new GameObject();
            objNew.transform.parent = null;
            pSource.pSource = objNew.AddComponent<AudioSource>();
        }

        //Debug.Log("Pos Sound:" + bAddNew);

        return pSource;
    }

    #endregion

    #region Music

    //音轨
    public enum MusicTrac
    {
        BGM_1 = 0,
        BGM_2 = 1,
        BGM_3 = 2,
        BGM_4 = 3,

        Max,
    }

    protected class MusicAction
    {
        public enum ActionType
        {
            EasyOut,
            EasyIn,
        }

        public MusicAction(CAudioSourcePlayer player, float time, ActionType emType)
        {
            pPlayer = player;
            fTime = time;
            emActionType = emType;
        }

        protected CAudioSourcePlayer pPlayer;

        protected float fTime;

        protected ActionType emActionType = ActionType.EasyIn;

        bool bActive = false;

        CPropertyTimer pTicker;

        public bool IsActive()
        {
            return bActive;
        }

        public void Start()
        {
            pTicker = new CPropertyTimer();
            pTicker.Value = fTime;
            pTicker.FillTime();

            bActive = true;

            switch (emActionType)
            {
                case ActionType.EasyIn:
                    {
                        pPlayer.fVolumLerp = 0f;
                        pPlayer.pSource.volume = 0F;
                    }
                    break;
                case ActionType.EasyOut:
                    {
                        pPlayer.fVolumLerp = CAudioMgr.Ins.VolumMusic * CAudioMgr.Ins.MainVolum;
                        pPlayer.pSource.volume = pPlayer.fVolumLerp *
                                                 pPlayer.fOriginVolum *
                                                 Ins.fAudioMusicVolum * 
                                                 Ins.fMainVolum;
                    }
                    break;
            }
        }

        public void OnUpdate(float dt)
        {
            if (bActive)
            {
                if (pTicker.Tick(dt))
                {
                    bActive = false;
                }

                switch (emActionType)
                {
                    case ActionType.EasyIn:
                        {
                            pPlayer.fVolumLerp = Mathf.Clamp01(1F - pTicker.GetTimeLerp()) * CAudioMgr.Ins.MainVolum * CAudioMgr.Ins.VolumMusic;
                            //Debug.Log(pPlayer.fVolumLerp);
                            pPlayer.pSource.volume = pPlayer.fVolumLerp *
                                                 pPlayer.fOriginVolum;
                        }
                        break;
                    case ActionType.EasyOut:
                        {
                            pPlayer.fVolumLerp = Mathf.Clamp01(pTicker.GetTimeLerp()) * CAudioMgr.Ins.MainVolum * CAudioMgr.Ins.VolumMusic;
                            pPlayer.pSource.volume = pPlayer.fVolumLerp *
                                                 pPlayer.fOriginVolum;

                            if (!bActive)
                            {
                                pPlayer.pSource.Stop();
                            }
                        }
                        break;
                }
            }
        }
    }

    protected MusicTrac emCurPlayingTrac = MusicTrac.BGM_1; //当前播放的音轨

    protected bool bFirstPlayBGM = true;

    protected Dictionary<MusicTrac, CAudioSourcePlayer> dicAudioSourceMusic = new Dictionary<MusicTrac, CAudioSourcePlayer>();

    protected List<MusicAction> listMusicAction = new List<MusicAction>();

    public CAudioSourcePlayer PlayMusicByID(CAudioSlottInfo pSlot)
    {
        if (!bFirstPlayBGM)
        {
            if (IsCurTracBGMPlaying())
            {
                CAudioSourcePlayer pPrePlayer = GetAudioSourceMusicByTrac(emCurPlayingTrac);
                AddMusicAction(new MusicAction(pPrePlayer, 0.5F, MusicAction.ActionType.EasyOut));

                emCurPlayingTrac = (MusicTrac)((int)(emCurPlayingTrac + 1) % (int)MusicTrac.Max);
            }
        }

        CAudioSourcePlayer pSource = GetAudioSourceMusicByTrac(emCurPlayingTrac);
        //ST_AudioInfo pInfo = GetAudioInfo(pSlot.nID);
        PlayAudioClipByPlayer(ref pSource, pSlot.fVolum, pSlot.fBlend, pSlot.bLoop,
                                  new Vector2(pSlot.vClipRange.x, pSlot.vClipRange.y),
                                  pSlot.clip, 
                                  VolumMusic * MainVolum);

        AddMusicAction(new MusicAction(pSource, 0.8F, MusicAction.ActionType.EasyIn));

        if (bFirstPlayBGM)
        {
            bFirstPlayBGM = false;
        }

        return pSource;
    }

    //设定指定音轨的音量
    public void SetMusicTracVolum(MusicTrac emTrac, float fVolum)
    {
        CAudioSourcePlayer pSource = GetAudioSourceMusicByTrac(emTrac);
        pSource.pSource.volume = fVolum;
    }

    //停止指定音轨的BGM
    public void StopMusicTrac()
    {
        CAudioSourcePlayer pSource = GetAudioSourceMusicByTrac(emCurPlayingTrac);

        AddMusicAction(new MusicAction(pSource, 0.5F, MusicAction.ActionType.EasyOut));

        //pSource.pSource.Stop();
        //pSource.pSource.gameObject.SetActive(false);
    }

    //获取BGM指定音轨
    protected CAudioSourcePlayer GetAudioSourceMusicByTrac(MusicTrac emTrac)
    {
        CAudioSourcePlayer pSource = null;
        bool bAddNew = true;
        if (dicAudioSourceMusic.TryGetValue(emTrac, out pSource))
        {
            if (pSource == null)
            {
                dicAudioSourceMusic.Remove(emTrac);
            }
            else
            {
                return pSource;
            }
        }

        //添加新音轨
        if (bAddNew)
        {
            pSource = new CAudioSourcePlayer();
            GameObject objNew = new GameObject();
            pSource.pSource = objNew.AddComponent<AudioSource>();
            pSource.pSource.gameObject.name = "Music_" + emTrac.ToString();
            pSource.pSource.transform.parent = transform;
            dicAudioSourceMusic.Add(emTrac, pSource);
        }

        return pSource;
    }

    /// <summary>
    /// 当前BGM音轨是否在播放
    /// </summary>
    /// <returns></returns>
    protected bool IsCurTracBGMPlaying()
    {
        CAudioSourcePlayer pPlayer = GetAudioSourceMusicByTrac(emCurPlayingTrac);
        if (pPlayer == null) return false;

        return pPlayer.pSource.isPlaying;
    }

    void AddMusicAction(MusicAction pAction)
    {
        pAction.Start();
        listMusicAction.Add(pAction);
    }

    #endregion

    #region Sound

    public CAudioSourcePlayer PlaySoundBySlot(CAudioSlottInfo pSlot, Vector3 vPos)
    {
        if (pSlot.clip == null)
        {
            return null;
        }

        CAudioSourcePlayer pSource = PopIdleAudioSource();
        pSource.pSource.transform.position = vPos;

       

        PlayAudioClipByPlayer(ref pSource, pSlot.fVolum, pSlot.fBlend, pSlot.bLoop,
                              new Vector2(pSlot.vClipRange.x, pSlot.vClipRange.y),
                              pSlot.clip, VolumSound * MainVolum, pSlot.fPitch);

        listAudioSourceActive.Add(pSource);

        return pSource;
    }
    
    #endregion

    //通用播放声音接口
    protected void PlayAudioClipByPlayer(ref CAudioSourcePlayer pSourcePlay, float fVolum, float fBlend, bool bLoop, Vector2 vRange, AudioClip pClip, float fLerp, float fPitch = 1f)
    {
        if (pSourcePlay == null || pSourcePlay.pSource == null) return;

        pSourcePlay.fOriginVolum = fVolum;

        AudioSource pSource = pSourcePlay.pSource;
        //pSource.gameObject.SetActive(true);
        pSource.clip = pClip;
        pSource.volume = fVolum * fLerp;
        //Debug.Log("FVol: " + fVolum + "  " + fLerp + "    Res:" + pSource.volume);
        pSource.spatialBlend = fBlend;
        pSource.loop = bLoop;
        pSource.pitch = fPitch;
        pSource.minDistance = vRange.x;
        pSource.maxDistance = vRange.y;
        pSource.rolloffMode = AudioRolloffMode.Linear;
        pSource.Play();
    }

    public void ClearAllAudio()
    {
        ////Music清空
        //foreach(CAudioSourcePlayer ele in dicAudioSourceMusic.Values)
        //{
        //    ele.pSource.Stop();
        //    GameObject.Destroy(ele.pSource.gameObject);
        //}
        //dicAudioSourceMusic.Clear();

        //Sound清空
        for (int i = 0; i < listAudioSourceActive.Count; i++)
        {
            if (listAudioSourceActive[i].pSource == null) continue;
            listAudioSourceActive[i].pSource.Stop();
            GameObject.Destroy(listAudioSourceActive[i].pSource.gameObject);
        }
        listAudioSourceActive.Clear();

        for (int i = 0; i < listAudioSourceIdle.Count; i++)
        {
            if (listAudioSourceIdle[i].pSource == null) continue;
            listAudioSourceIdle[i].pSource.Stop();
            GameObject.Destroy(listAudioSourceIdle[i].pSource.gameObject);
        }
        listAudioSourceIdle.Clear();

        //Clip资源清空
        dicAudioClip.Clear();
    }
}
