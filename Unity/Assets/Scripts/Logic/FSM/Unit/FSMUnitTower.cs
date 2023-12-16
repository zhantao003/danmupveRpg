using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMUnitTower_Idle : FSMUnitBase
{
    Fix64 f64TotalIdleTime;
    Fix64 f64CurIdleTime;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Idle;
        f64TotalIdleTime = (Fix64)0.3f;
        f64CurIdleTime = Fix64.Zero;
    }

    public override void OnUpdate(object obj, float delta)
    {
        f64CurIdleTime += CLockStepData.g_fixFrameLen;

        //大于1表示当前移动结束
        if (f64CurIdleTime >= f64TotalIdleTime)
        {
            f64CurIdleTime = Fix64.Zero;

            if (pUnit.pAtkTarget == null)
            {

            }
            else
            {
                if (pUnit.pAtkTarget.IsDead())
                {
                    pUnit.pAtkTarget = null;
                    pUnit.pMoveTarget = null;
                    pUnit.RefreshSearch();
                    //pUnit.GetAtkTargetByAlertRange();
                }

                if (pUnit.pAtkTarget == null)
                {

                }
                else
                {
                    if (pUnit.pAtkTarget.IsDead())
                    {
                        pUnit.pAtkTarget = null;
                        pUnit.pMoveTarget = null;
                        pUnit.RefreshSearch();
                        //pUnit.GetAtkTargetByAlertRange();
                    }

                    if (CFindTargetHelp.IsUnitInAtkRange(pUnit.pUnitData.nAtkRange, pUnit.pAtkTarget, pUnit))
                    {
                        if (pUnit.IsAtkAble())
                        {
                            pUnit.SetState(CPlayerUnit.EMState.Attack);
                        }
                    }
                    else
                    {
                        if (pUnit.GetAtkTargetByAtkRange())
                        {
                            if (pUnit.IsAtkAble())
                            {
                                pUnit.SetState(CPlayerUnit.EMState.Attack);
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }
        }
    }
}

public class FSMUnitTower_Atk : FSMUnitBase
{
    Fix64 f64TotalAtkActiveTime;
    Fix64 f64CurAtkActiveTime;
    bool bAtkActiveOver;

    Fix64 f64TotalAtkTime;
    Fix64 f64CurAtkTime;
    bool bAtkOver;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Attack;
        DoAtk();
    }
    
    public override void OnUpdate(object obj, float delta)
    {
        
        if (pUnit.pAtkTarget == null ||
            pUnit.pAtkTarget.IsDead())
        {
            pUnit.pAtkTarget = null;
            pUnit.pMoveTarget = null;
            pUnit.RefreshSearch();
            pUnit.SetState(CPlayerUnit.EMState.Idle);
            return;
        }
        //return;
        if (!bAtkActiveOver)
        {
            f64CurAtkActiveTime += CLockStepData.g_fixFrameLen;
            if (f64CurAtkActiveTime >= f64TotalAtkActiveTime)
            {
                f64CurAtkActiveTime -= f64TotalAtkActiveTime;
                bAtkActiveOver = true;
                pUnit.AtkTarget();
            }
        }

        if (!bAtkOver)
        {
            f64CurAtkTime += CLockStepData.g_fixFrameLen;
            if (f64CurAtkTime >= f64TotalAtkTime)
            {
                f64CurAtkTime -= f64TotalAtkTime;
                bAtkOver = true;
                //pUnit.pAtkTarget = null;
                //pUnit.pMoveTarget = null;
                pUnit.SetState(CPlayerUnit.EMState.Idle);
                //DoAtk();
            }
        }
    }

    void DoAtk()
    {
        if (pUnit.pAtkTarget == null ||
           pUnit.pAtkTarget.IsDead())
        {
            pUnit.pAtkTarget = null;
            pUnit.pMoveTarget = null;
            pUnit.RefreshSearch();
            pUnit.SetState(CPlayerUnit.EMState.Idle);
        }
        else
        {
            pUnit.RefreshAtkCD();
            AStarFindPath.Ins.GetNextMoveDir(ref pUnit.emMoveDir,pUnit.pStayMapSlot, pUnit.pAtkTarget.pStayMapSlot);

            f64TotalAtkTime = pUnit.pUnitData.AtkTime;
            f64TotalAtkActiveTime = pUnit.pUnitData.AtkActiveTime;
            f64CurAtkTime = Fix64.Zero;
            f64CurAtkActiveTime = Fix64.Zero;
            bAtkOver = false;
            bAtkActiveOver = false;
        }
    }

}