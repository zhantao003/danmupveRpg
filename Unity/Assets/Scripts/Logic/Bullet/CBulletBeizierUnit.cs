using FixMath.NET;
using ILRuntime.Runtime;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletBeizierUnit : CLockUnityObject
{
    public float fMoveSpd;
    public float fHeight;

    public string szPrefabName;
    public string szEffHit;

    public enum EMDir
    {
        Up,
        Right,
    }

    public EMDir emBulletDir = EMDir.Right;

    public float[] fTargetPosOffsetX;    //目标点偏移
    public float[] fTargetPosOffsetY;    //目标点偏移

    //protected FixVector3 v64StartPos;
    //protected FixVector3 v64EndPos;
    //protected FixVector3 v64CenterPos;

    protected Fix64 f64MoveTime;
    protected Fix64 f64CurMoveTime;

    protected Fix64 f64MoveSpd;
    protected Fix64 f64Height;

    protected Vector3 vStartPos;
    protected Vector3 vEndPos;
    protected Vector3 vCenterPos;
    protected Vector3 vNextPos;
    protected Vector3 vCurOffset;

    protected float fMoveTime;
    protected float fCurMoveTime;
    protected float fCurBeizierTimeScale;

    public Fix64 MoveSpd
    {
        get
        {
            return f64MoveSpd;
        }
    }

    public Fix64 Height
    {
        get
        {
            return f64Height;
        }
    }

    [ReadOnly]
    public CPlayerUnit pTarget;

    [ReadOnly]
    public CPlayerUnit pBindUnit;

    [ReadOnly]
    public bool bMoveOver = false;

    public virtual void Init(FixVector3 startPos, FixVector3 endPos, CPlayerUnit traget,CPlayerUnit unit)
    {
        enabled = true;
        pBindUnit = unit;
        pTarget = traget;
        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " + $"子弹索引【{nUniqueIdx}】"
        //    + $"发射者索引【{pBindUnit.nUniqueIdx}】"
        //    + $"攻击目标索引【{pTarget.nUniqueIdx}】【出现子弹，攻击目标】  ");
        //v64StartPos = startPos;
        //v64EndPos = endPos;
        //f64Height = (Fix64)fHeight;
        //v64CenterPos = startPos + (endPos - startPos) * (Fix64)0.5f + new FixVector3(Fix64.Zero, Height, Fix64.Zero);

        vStartPos = startPos.ToVector3();
        vEndPos = endPos.ToVector3();
        vCenterPos = vStartPos + (vEndPos - vStartPos) * 0.5f + Vector3.up * fHeight;
        vCurOffset = new Vector3(Random.Range(fTargetPosOffsetX[0], fTargetPosOffsetX[1]),
                                 Random.Range(fTargetPosOffsetY[0], fTargetPosOffsetY[1]),
                                 0);
        vEndPos += vCurOffset;

        f64MoveSpd = (Fix64)fMoveSpd;

        //计算移动参数
        f64MoveTime = (FixVector3.Distance(startPos, endPos) / f64MoveSpd);
        fMoveTime = (float)f64MoveTime;

        f64CurMoveTime = Fix64.Zero;
        fCurMoveTime = 0f;

        fCurBeizierTimeScale = 0f;

        bMoveOver = false;

        m_fixv3LogicPosition = startPos;
        ForceRefreshPos();
        RecordLastPos();
    }

    public override void OnUpdateLogic()
    {
        RecordLastPos();

        f64CurMoveTime += CLockStepData.g_fixFrameLen;

        //大于1表示当前移动结束
        if (f64CurMoveTime >= f64MoveTime)
        {
            f64CurMoveTime = Fix64.Zero;
            bMoveOver = true;
        }

        //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
    
        if(bMoveOver)
        {
            DoHit();
            Recycle();
        }
    }

    void FixedUpdate()
    {
        fCurMoveTime += CTimeMgr.FixedDeltaTime;
        fCurBeizierTimeScale = Mathf.Min(fCurMoveTime / fMoveTime, 1f);

        vNextPos = CHelpTools.GetCurvePoint(vStartPos, vCenterPos, vEndPos, fCurBeizierTimeScale);

        if(fCurBeizierTimeScale < 1f)
        {
            if (emBulletDir == EMDir.Up)
            {
                tranSelf.up = (vNextPos - tranSelf.position).normalized;
            }
            else if (emBulletDir == EMDir.Right)
            {
                tranSelf.right = (vNextPos - tranSelf.position).normalized;
            }
        }

        tranSelf.position = vNextPos;
    }

    public virtual void DoHit()
    {
        if(pTarget == null || pTarget.IsDead())
        {
            return;
        }

        if(!CHelpTools.IsStringEmptyOrNone(szEffHit))
        {
            CEffectMgr.Instance.CreateEffSync(szEffHit, pTarget.tranSelf, 0);
        }
        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " + $"子弹索引【{nUniqueIdx}】"
        //             + $"发射者索引【{pBindUnit.nUniqueIdx}】" +
        //                $"攻击目标索引【{pTarget.nUniqueIdx}】【子弹攻击目标】  ");
        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            return;
        //TODO:受击事件
        if (pTarget.emUnitType == CPlayerUnit.EMUnitType.Unit)
        {
            pTarget.OnHit(pBindUnit, pBindUnit.pUnitData.AtkDmg, pBindUnit.pStayMapSlot);
        }
        else
        {
            pTarget.OnHit(pBindUnit, pBindUnit.pUnitData.nAtkBuildDmg, pBindUnit.pStayMapSlot);
        }
    }

    public override void UpdatePos(float interpolation)
    {
        //base.UpdatePos(interpolation);

        //if(emBulletDir == EMDir.Up)
        //{
        //    tranSelf.up = (m_fixv3LogicPosition - m_fixv3LastPosition).GetNormalized().ToVector3();
        //}
        //else if(emBulletDir == EMDir.Right)
        //{
        //    tranSelf.right = (m_fixv3LogicPosition - m_fixv3LastPosition).GetNormalized().ToVector3();
        //}
    }

    public virtual void Recycle()
    {
        //子弹对象池事件

        OnRecycleLockStepBullet();
        CBulletMgr.Ins.RemoveBulletUnit(this);
    }
}
