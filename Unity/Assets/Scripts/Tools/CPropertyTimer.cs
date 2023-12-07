using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CPropertyTimer
{
    [SerializeField]
    protected float m_float;

    [SerializeField]
    protected float fCurParam;

    //值发生变化的委托
    public System.Action<float> pActionValue;

    public virtual float Value
    {
        get
        {
            return m_float;
        }
        set
        {
            m_float = value;
            if (pActionValue != null)
            {
                pActionValue(m_float);
            }
        }
    }

    public virtual float CurValue
    {
        get
        {
            return Mathf.Max(fCurParam, 0);
        }
        set
        {
            fCurParam = value;
        }
    }

    public float GetTimeLerp()
    {
        return Mathf.Max(0, fCurParam / Value);
    }

    public int GetTimeMSDiff()
    {
        int nDiff = (int)(Value * 1000) - (int)(fCurParam * 1000);
        return nDiff;
    }

    public void ClearTime()
    {
        fCurParam = 0F;
    }

    public bool Tick(float delta)
    {
        fCurParam -= delta;
        fCurParam = Mathf.Max(0F, fCurParam);
        return fCurParam <= 0F;
    }

    public void AddTickTime(float fTime)
    {
        Value += fTime;
        fCurParam += fTime;
    }

    public void FillTime(bool addLast = false)
    {
        if(addLast)
        {
            float fRes = fCurParam;
            fCurParam = Value;
            fCurParam += fRes;
        }
        else
        {
            fCurParam = Value;
        }
    }
}
