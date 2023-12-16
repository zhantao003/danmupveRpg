using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPropertyTimerFix64
{
    public Fix64 m_float;

    public Fix64 fCurParam;

    public virtual Fix64 Value
    {
        get
        {
            return m_float;
        }
        set
        {
            m_float = value;
        }
    }

    public virtual Fix64 CurValue
    {
        get
        {
            return fCurParam;
        }
        set
        {
            fCurParam = value;
        }
    }

    public Fix64 GetTimeLerp()
    {
        return fCurParam / Value;
    }

    public void ClearTime()
    {
        fCurParam = Fix64.Zero;
    }

    public bool Tick(Fix64 delta)
    {
        fCurParam -= delta;

        return fCurParam <= Fix64.Zero;
    }

    public void AddTickTime(Fix64 fTime)
    {
        Value += fTime;
        fCurParam += fTime;
    }

    public void FillTime(bool addLast = false)
    {
        if (addLast)
        {
            Fix64 fRes = fCurParam;
            fCurParam = Value;
            fCurParam += fRes;
        }
        else
        {
            fCurParam = Value;
        }
    }
}
