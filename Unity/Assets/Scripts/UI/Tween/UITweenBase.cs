using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UITweenEvent
{
    public float fTriggerTime;  //触发时间

    public DelegateNFuncCall pEvent = null;

    CPropertyTimer pTicker = null;

    public UITweenEvent(float time, DelegateNFuncCall call)
    {
        fTriggerTime = time;
        pEvent = call;
    }

    public void Start()
    {
        pTicker = new CPropertyTimer();
        pTicker.Value = fTriggerTime;
        pTicker.FillTime();
    }

    public void Update(float dt)
    {
        if (pTicker != null && pTicker.Tick(dt))
        {
            DoEvent();
            pTicker = null;
        }
    }

    public virtual void DoEvent()
    {
        pEvent?.Invoke();
    }
}

public class UITweenBase : MonoBehaviour
{
    public float playTime;
    public float delayTime;
    public AnimationCurve curve;
    public bool fixedUpdate = false;
    public bool unscaleTime = false;
    public bool bLoop;

    public DelegateNFuncCall callOver = null;

    //当前播放时间
    [ReadOnly][ShowInInspector]
    protected float curTime = 0F;
    [ReadOnly][ShowInInspector]
    protected float curDelayTime = 0F;
    [ReadOnly][ShowInInspector]
    protected float curValue = 0F;

    List<UITweenEvent> listEvents = new List<UITweenEvent>();

    bool skipFrame;

    public virtual void RegistEvent(UITweenEvent call)
    {
        listEvents.Add(call);
    }

    public virtual void Play(DelegateNFuncCall call = null)
    {
        enabled = true;
        curTime = 0F;
        curDelayTime = delayTime;
        callOver = call;

        skipFrame = true;

        for(int i=0; i<listEvents.Count; i++)
        {
            listEvents[i].Start();
        }

        Refresh(0F);
    }

    protected virtual void Update()
    {
        if (fixedUpdate) return;

        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        if(curDelayTime > 0F)
        {
            curDelayTime -= (unscaleTime ? CTimeMgr.DeltaTimeUnScale : CTimeMgr.DeltaTime);
            return;
        }

        Refresh(curTime / playTime);

        UpdateEvent(unscaleTime ? CTimeMgr.DeltaTimeUnScale : CTimeMgr.DeltaTime);

        if (curTime >= playTime)
        {
            if (bLoop)
            {
                Play(callOver);
            }
            else
            {
                enabled = false;
            }
            callOver?.Invoke();
        }

        //Debug.Log(CTimeMgr.DeltaTime + "   Cur:" + curTime + "    Delta:" + Time.deltaTime + "    UnscaleDelta:" + Time.unscaledDeltaTime);
        curTime += (unscaleTime ? CTimeMgr.DeltaTimeUnScale : CTimeMgr.DeltaTime);
    }

    void FixedUpdate()
    {
        if (!fixedUpdate) return;

        if (skipFrame)
        {
            skipFrame = false;
            return;
        }

        if (curDelayTime > 0F)
        {
            curDelayTime -= (unscaleTime ? CTimeMgr.FixedTimeUnScale : CTimeMgr.FixedDeltaTime);
            return;
        }

        Refresh(curTime / playTime);

        UpdateEvent(unscaleTime ? CTimeMgr.FixedTimeUnScale : CTimeMgr.FixedDeltaTime);

        if (curTime >= playTime)
        {
            callOver?.Invoke();
            if(bLoop)
            {
                Play(callOver);
            }
            else
            {
                enabled = false;
            }
        }

        curTime += (unscaleTime ? CTimeMgr.FixedTimeUnScale : CTimeMgr.FixedDeltaTime);
    }

    protected virtual void Refresh(float lerp)
    {
        lerp = Mathf.Min(lerp, 1F);
        if(curve!=null)
        {
            curValue = curve.Evaluate(lerp);
        }
    }

    protected virtual void UpdateEvent(float dt)
    {
        for (int i = 0; i < listEvents.Count; i++)
        {
            listEvents[i].Update(dt);
        }
    }

    [ContextMenu("Play")]
    public void EditPlay()
    {
        Play();
    }

    public void Stop()
    {
        curTime = playTime;
        Refresh(curTime / playTime);
        callOver = null;
        enabled = false;

        listEvents.Clear();
    }

    public virtual void Reset()
    {
        curTime = 0;
        Refresh(curTime / playTime);
        callOver = null;
        enabled = false;

        listEvents.Clear();
    }
}
