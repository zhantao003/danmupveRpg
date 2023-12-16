using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBulletLineUnit : CBulletBeizierUnit
{
    public string szEffDead;

    public CPlayerUnit.EMAtkRange emAtkRange;

    public override void Init(FixVector3 startPos, FixVector3 endPos, CPlayerUnit traget, CPlayerUnit unit)
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
        vCurOffset = new Vector3(Random.Range(fTargetPosOffsetX[0], fTargetPosOffsetX[1]),
                                 Random.Range(fTargetPosOffsetY[0], fTargetPosOffsetY[1]),
                                 0);
        vEndPos += vCurOffset;

        f64MoveSpd = (Fix64)fMoveSpd;

        //计算移动参数
        f64MoveTime = (FixVector3.Distance(startPos, endPos) / f64MoveSpd);
        if(f64MoveTime <= Fix64.Zero)
        {
            f64MoveTime = (Fix64)0.05f;
        }

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

        if (bMoveOver)
        {
            DoHit();
            Recycle();
        }
    }

    void FixedUpdate()
    {
        fCurMoveTime += CTimeMgr.FixedDeltaTime;
        fCurBeizierTimeScale = Mathf.Min(fCurMoveTime / fMoveTime, 1f);

        vNextPos = Vector3.Lerp(vStartPos, vEndPos, fCurBeizierTimeScale);

        if (fCurBeizierTimeScale < 1f)
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

    public override void DoHit()
    {
        //播放受击特效
        CEffectMgr.Instance.CreateEffSync(szEffDead, tranSelf, 0);

        if (pTarget == null || pTarget.IsDead())
        {
            return;
        }
        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " + $"子弹索引【{nUniqueIdx}】"
        //            + $"发射者索引【{pBindUnit.nUniqueIdx}】" +
        //               $"攻击目标索引【{pTarget.nUniqueIdx}】【子弹攻击目标】  ");
        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
            return;
        //TODO:受击事件
        if (emAtkRange == CPlayerUnit.EMAtkRange.Normal)
        {
            if (pTarget.emUnitType == CPlayerUnit.EMUnitType.Unit)
            {
                if (pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Unit)
                {
                    pTarget.OnHit(pBindUnit, pBindUnit.pUnitData.AtkDmg, pBindUnit.pStayMapSlot);
                }
                else
                {
                    if(pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Tower)
                    {
                        pTarget.OnHit(pBindUnit, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetTowerDmg(pTarget.pUnitData.nTBLID), pBindUnit.pStayMapSlot);
                    }
                    else if(pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Base)
                    {
                        pTarget.OnHit(pBindUnit, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetBaseDmg(pTarget.pUnitData.nTBLID), pBindUnit.pStayMapSlot);
                    }
                }
            }
            else
            {
                pTarget.OnHit(pBindUnit, pBindUnit.pUnitData.nAtkBuildDmg, pBindUnit.pStayMapSlot);
            }
        }
        else if(emAtkRange == CPlayerUnit.EMAtkRange.AtkAround)
        {
            AtkAround();
        }
    }
    List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
    /// <summary>
    /// 攻击周围
    /// </summary>
    void AtkAround()
    {
        listTarget.Clear();
        List<MapSlot> slots = new List<MapSlot>();
        AStarFindPath.Ins.GetAroundSlot(ref slots, pBindUnit.pUnitData.nDmgRange, pTarget.pStayMapSlot.vecPos);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].pStayGroundUnit != null &&
               !slots[i].pStayGroundUnit.IsDead() &&
               slots[i].pStayGroundUnit.emCamp != pBindUnit.emCamp &&
               !listTarget.Contains(slots[i].pStayGroundUnit))
            {
                listTarget.Add(slots[i].pStayGroundUnit);
            }
            if (pBindUnit.pUnitData.bCanAtkFly &&
               slots[i].pStayFlyUnit != null &&
               !slots[i].pStayFlyUnit.IsDead() &&
               slots[i].pStayFlyUnit.emCamp != pBindUnit.emCamp &&
               !listTarget.Contains(slots[i].pStayFlyUnit))
            {
                listTarget.Add(slots[i].pStayFlyUnit);
            }
        }
        for (int i = 0; i < listTarget.Count; i++)
        {
            if (listTarget[i] == null) continue;
            if (listTarget[i].emUnitType == CPlayerUnit.EMUnitType.Unit)
            {
                if (pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Unit)
                {
                    listTarget[i].OnHit(pBindUnit, pBindUnit.pUnitData.AtkDmg, pBindUnit.pStayMapSlot);
                }
                else
                {
                    if (pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Tower)
                    {
                        listTarget[i].OnHit(pBindUnit, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetTowerDmg(listTarget[i].pUnitData.nTBLID), pBindUnit.pStayMapSlot);
                    }
                    else if (pBindUnit.emUnitType == CPlayerUnit.EMUnitType.Base)
                    {
                        listTarget[i].OnHit(pBindUnit, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetBaseDmg(listTarget[i].pUnitData.nTBLID), pBindUnit.pStayMapSlot);
                    }
                }
            }
            else
            {
                listTarget[i].OnHit(pBindUnit, pBindUnit.pUnitData.nAtkBuildDmg, pBindUnit.pStayMapSlot);
            }
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

    public override void Recycle()
    {
        //子弹对象池事件

        OnRecycleLockStepBullet();
        CBulletMgr.Ins.RemoveBulletUnit(this);
    }

}
