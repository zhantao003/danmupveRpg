using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEffectBeizier : CEffectBase
{
    Transform tranSelf;
    [ReadOnly]
    public Vector3 vStart;
    [ReadOnly]
    public Transform tranEnd;
    [ReadOnly]
    public Vector3 vCenter;
    public float fSpd;
    public float fCenterHeight;
    public float fTargetAddHeight;
    CPropertyTimer pMoveTicker = null;

    public string szBoomEff;

    public bool bRecyleMoveEnd = true;

    public void SetTarget(Vector3 start, Transform end)
    {
        vStart = start;
        tranEnd = end;
        vCenter = (tranEnd.position + vStart) * 0.5F + Vector3.up * fCenterHeight;

        float fMoveTime = (tranEnd.position - vStart).magnitude / fSpd;
        pMoveTicker = new CPropertyTimer();
        pMoveTicker.Value = fMoveTime;
        pMoveTicker.FillTime();

        tranSelf = gameObject.GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        if (pMoveTicker == null || !bPlaying) return;

        if(tranEnd == null)
        {
            pMoveTicker = null;
            Recycle();
            return;
        }

        if(pMoveTicker.Tick(CTimeMgr.FixedDeltaTime))
        {
            pMoveTicker = null;

            tranSelf.position = (tranEnd.position + Vector3.up * fTargetAddHeight);

            CEffectMgr.Instance.CreateEffSync(szBoomEff, tranSelf, 0);

            if(bRecyleMoveEnd)
                Recycle();
        }
        else
        {
            tranSelf.position = CHelpTools.GetCurvePoint(vStart, vCenter, (tranEnd.position + Vector3.up * fTargetAddHeight), 1F - pMoveTicker.GetTimeLerp());
        }
    }
}
