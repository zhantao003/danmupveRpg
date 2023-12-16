using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;
using static UnityEngine.UI.CanvasScaler;

public class FSMUnitRole_Idle : FSMUnitBase
{
    Fix64 f64TotalIdleTime;
    Fix64 f64CurIdleTime;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Idle;
        if (pUnit.pUnitData.nMoveSpeed > 0)
        {
            pUnit.PlayAnima(EMUnitAnimeState.Idle, pUnit.emMoveDir, true);
        }
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

            //pUnit.GetAtkTargetByAlertRange();
            
            if (pUnit.pAtkTarget == null)
            {
                if (pUnit.pUnitData.nMoveSpeed > 0)
                    MoveCheck();
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

                if(pUnit.pAtkTarget == null)
                {
                    if (pUnit.pUnitData.nMoveSpeed > 0)
                        MoveCheck();
                }
                else
                {
                    if (CFindTargetHelp.IsUnitInAtkRange(pUnit.pUnitData.nAtkRange, pUnit.pAtkTarget, pUnit))
                    {
                        GoAtkState();
                    }
                    else
                    {
                        if(pUnit.GetAtkTargetByAtkRange())
                        {
                            GoAtkState();
                        }
                        else
                        {
                            if (pUnit.pUnitData.nMoveSpeed > 0)
                                MoveCheck();
                        }
                    }
                }
            }

        }
    }

    void GoAtkState()
    {
        if (pUnit.pUnitData.nMoveSpeed > 0 && 
            pUnit.IsSkillAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Skill);
        }
        else if (pUnit.IsAtkAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Attack);
        }
    }

    void MoveCheck()
    {
        if (pUnit.GetDirMoveTarget() != null)
        {
            pUnit.SetState(CPlayerUnit.EMState.Move);
        }
    }

}

public class FSMUnitRole_Move : FSMUnitBase
{
    Fix64 f64TotalMoveTime;
    Fix64 f64CurMoveTime;
    Fix64 f64TimeScale;

    //List<MapSlot> pathSlots;
    //int nMoveIdx = 0;
    MapSlot pNextMove;
    
    FixVector3 v64Start = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    FixVector3 v64End = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    FixVector3 v64Distance = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    FixVector3 v64elpaseDis;
    FixVector3 v64CurPos;

    bool bMoveOver = false;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Move;
        bMoveOver = false;
        GetMoveTargetByHor(); 
    }

    public override void OnUpdate(object obj, float delta)
    {
        if (pNextMove == null)
        {
            pUnit.SetState(CPlayerUnit.EMState.Idle);
            return;
        }

        f64CurMoveTime += CLockStepData.g_fixFrameLen;
        RefreshPos();

        if (bMoveOver)
        {
            //pUnit.GetAtkTargetByAlertRange();
            
            if (pUnit.pAtkTarget == null)
            {
                GetMoveTargetByHor();
                if (pNextMove == null)
                {
                    //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                    //         $"索引【{pUnit.nUniqueIdx}】【下个移动目标为空 原地发呆】  ");
                    pUnit.SetState(CPlayerUnit.EMState.Idle);
                    return;
                }
            }
            else
            {
                if (pUnit.pAtkTarget.IsDead())
                {
                    pUnit.pAtkTarget = null;
                    pUnit.pMoveTarget = null;
                    pUnit.RefreshSearch();

                    //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                    //        $"索引【{pUnit.nUniqueIdx}】【攻击目标已死 原地发呆】  ");
                    pUnit.SetState(CPlayerUnit.EMState.Idle);
                }
                else
                {
                    if (CFindTargetHelp.IsUnitInAtkRange(pUnit.pUnitData.nAtkRange, pUnit.pAtkTarget, pUnit))
                    {
                        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                        //          $"索引【{pUnit.nUniqueIdx}】【目标在攻击范围内 打他】  ");
                        GoAtkState();
                    }
                    else
                    {
                        if (pUnit.GetAtkTargetByAtkRange())
                        {
                            //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                            //      $"索引【{pUnit.nUniqueIdx}】【目标不在攻击范围内但是攻击范围内有其他可攻击目标 打他】  ");
                            GoAtkState();
                        }
                        else
                        {
                            GetMoveTargetByHor();
                            if (pNextMove == null)
                            {
                                //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                                //$"索引【{pUnit.nUniqueIdx}】【目标不在攻击范围内且下个移动目标为空 原地发呆】  ");
                                pUnit.SetState(CPlayerUnit.EMState.Idle);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    void RefreshPos(bool refresh = true)
    {
        f64TimeScale = f64CurMoveTime / f64TotalMoveTime;
        //大于1表示当前移动结束
        if (f64TimeScale >= (Fix64)1)
        {
            f64TimeScale = (Fix64)1;

            if(refresh)
            {
                f64CurMoveTime -= f64TotalMoveTime;
                bMoveOver = true;
            } 
        }

        v64elpaseDis = new FixVector3(v64Distance.x * f64TimeScale, v64Distance.y * f64TimeScale, v64Distance.z * f64TimeScale);
        v64CurPos = new FixVector3(v64Start.x + v64elpaseDis.x, v64Start.y + v64elpaseDis.y, v64Start.z + v64elpaseDis.z);
        pUnit.m_fixv3LogicPosition = v64CurPos;
    }

    void GoAtkState()
    {
        if (pUnit.IsSkillAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Skill);
        }
        else if (pUnit.IsAtkAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Attack);
        }
        else
        {
            pUnit.SetState(CPlayerUnit.EMState.Idle);
        }
    }


    void GetMoveTargetByHor()
    {

        pNextMove = AStarFindPath.Ins.GetNextSlotByComputePath(pUnit, pUnit.pStayMapSlot, pUnit.pMoveTarget); // pUnit.GetDirMoveTarget();

        if (pNextMove != null)
        {
            //Debug.LogError(pUnit.pStayMapSlot.vecPos + "====" + pNextMove.vecPos);
            AStarFindPath.Ins.GetNextMoveDir(ref pUnit.emMoveDir, pUnit.pStayMapSlot, pNextMove);
            //计算移动所需要的时间
            f64TotalMoveTime = CBattleMgr.Ins.SlotLerp / pUnit.pUnitData.MoveSpd;
            //f64CurMoveTime = Fix64.Zero;

            v64Start = pUnit.m_fixv3LogicPosition;
            v64End = pNextMove.GetV64Pos();
            v64Distance = v64End - v64Start;

            pUnit.SetMapSlot(pNextMove);
            pUnit.SetRenderLayer(pNextMove.nCurSetRenderLayer);
            pUnit.PlayAnima(EMUnitAnimeState.Move, pUnit.emMoveDir);
            bMoveOver = false;

            //补偿一下多余的时间
            RefreshPos(false);
        }
        else
        {

        }
    }

    //public void GetMovePath()
    //{
    //    pathSlots = pUnit.GetMovePath();
    //    nMoveIdx = 0;
    //}

    //void GetMoveTargetByAStar()
    //{
    //    if(pathSlots == null)
    //    {
    //        GetMovePath();
    //    }
    //    if (pathSlots.Count <= 0 ||
    //        nMoveIdx >= pathSlots.Count)
    //    {
    //        //pathSlots = pUnit.GetMovePath();
    //        //nMoveIdx = 0;
    //    }
    //    else
    //    {
    //        bool bCanMove = true;
    //        ///判断格子是否有人
    //        if (pathSlots[nMoveIdx].pStayUnit != null &&
    //            pathSlots[nMoveIdx].pStayUnit.uid != pUnit.uid)
    //        {
    //            bCanMove = false;
    //        }
    //        ///判断是否为占领多格的类型 
    //        else if (pathSlots[nMoveIdx] != null &&
    //            !pUnit.CheckMapSlotCanMove(pathSlots[nMoveIdx].vecPos))
    //        {
    //            bCanMove = false;
    //        }
    //        //else if (pUnit.emStayRange == CPlayerUnit.EMStayRange.Around &&
    //        //    AStarFindPath.Ins.GetMapSlotByRange(pathSlots[nMoveIdx].vecPos, 1, EMSkillRangeType.Around, pUnit).Count < 6)
    //        //{
    //        //    bCanMove = false;
    //        //}
    //        //else if (pUnit.emStayRange == CPlayerUnit.EMStayRange.AroundTwo &&
    //        //    AStarFindPath.Ins.GetMapSlotByRange(pathSlots[nMoveIdx].vecPos, 2, EMSkillRangeType.Around, pUnit).Count < 18)
    //        //{
    //        //    bCanMove = false;
    //        //}
    //        //else if (pUnit.emStayRange == CPlayerUnit.EMStayRange.AroundThree &&
    //        //    AStarFindPath.Ins.GetMapSlotByRange(pathSlots[nMoveIdx].vecPos, 3, EMSkillRangeType.Around, pUnit).Count < 36)
    //        //{
    //        //    bCanMove = false;
    //        //}
    //        if (!bCanMove)
    //        {
    //            pNextMove = null;
    //            return;
    //            //pathSlots = pUnit.GetMovePath();
    //            //nMoveIdx = 0;
    //        }
    //    }
    //    if (pathSlots.Count <= 0 ||
    //        nMoveIdx >= pathSlots.Count)
    //    {
    //        pNextMove = null;
    //    }
    //    else if (pathSlots[nMoveIdx].vecPos == pUnit.pStayMapSlot.vecPos)
    //    {
    //        pNextMove = null;
    //    }
    //    else
    //    {
    //        pNextMove = pathSlots[nMoveIdx];
    //        //Debug.LogError(pUnit.pMoveTarget.vecPos + "===Move==" + pNextMove.vecPos + "====" + pathSlots.Count + "====" + pathSlots[pathSlots.Count -1].vecPos);
    //        pUnit.emMoveDir = AStarFindPath.Ins.GetNextMoveDir(pUnit.pStayMapSlot, pNextMove);
    //        //计算移动所需要的时间
    //        f64TotalMoveTime = CGameMgr.Ins.SlotLerp / pUnit.pUnitData.MoveSpd;
    //        f64CurMoveTime = Fix64.Zero;

    //        v64Start = pUnit.m_fixv3LogicPosition;
    //        v64End = pNextMove.GetV64Pos();
    //        v64Distance = v64End - v64Start; //new FixVector3(v64End.x - v64Start.x, v64End.y - v64Start.y, v64End.z - v64Start.z);

    //        pUnit.SetMapSlot(pNextMove);
    //        pUnit.SetRenderLayer(pNextMove.nCurSetRenderLayer);

    //        pUnit.PlayAnima(EMUnitAnimeState.Move, pUnit.emMoveDir);

    //        bMoveOver = false;
    //        nMoveIdx++;
    //    }
    //}
}

public class FSMUnitRole_Jump : FSMUnitBase
{
    Fix64 f64TotalJumpTime;
    Fix64 f64CurJumpTime;
    Fix64 f64TimeScale;

    FixVector3 v64Start = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    FixVector3 v64Center = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    FixVector3 v64End = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);
    FixVector3 v64Distance = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    FixVector3 v64elpaseDis;
    FixVector3 v64CurPos;

    bool bJumpOver = false;

    public override void OnBegin(object obj)
    {
        if(pUnit.pStayMapSlot == null ||
            pUnit.pJumpTarget == null)
        {
            pUnit.SetState(CPlayerUnit.EMState.Idle);
            return;
        }
        pUnit.emCurState = CPlayerUnit.EMState.Jump;
        AStarFindPath.Ins.GetNextMoveDir(ref pUnit.emMoveDir, pUnit.pStayMapSlot, pUnit.pJumpTarget);
        //计算跳跃所需要的时间
        f64TotalJumpTime = Mathf.Abs(pUnit.pJumpTarget.vecPos.x - pUnit.pStayMapSlot.vecPos.x) * CBattleMgr.Ins.SlotLerp / (pUnit.pUnitData.MoveSpd * Fix64.Four);

        v64Start = pUnit.m_fixv3LogicPosition;
        v64End = pUnit.pJumpTarget.v64SlotPos;
        v64Center = new FixVector3((v64Start.x + v64End.x) * Fix64.HALFOne,
                                   (v64Start.y + v64End.y) * Fix64.HALFOne + Fix64.Two,
                                   (v64Start.z + v64End.z) * Fix64.HALFOne);
        v64Distance = v64End - v64Start;

        pUnit.SetMapSlot(pUnit.pJumpTarget);
        pUnit.SetRenderLayer(pUnit.pJumpTarget.nCurSetRenderLayer);
        pUnit.PlayAnima(EMUnitAnimeState.Idle, pUnit.emMoveDir);
        bJumpOver = false;
        
    }

    public override void OnUpdate(object obj, float delta)
    {
        f64CurJumpTime += CLockStepData.g_fixFrameLen;
        RefreshPos();

        if (bJumpOver)
        {
            pUnit.m_fixv3LogicPosition = v64End;
            //pUnit.GetAtkTargetByAlertRange();
            
            if (pUnit.pAtkTarget == null)
            {
                pUnit.SetState(CPlayerUnit.EMState.Idle,null,true);
            }
            else
            {
                if (pUnit.pAtkTarget.IsDead())
                {
                    pUnit.pAtkTarget = null;
                    pUnit.pMoveTarget = null;
                    pUnit.RefreshSearch();

                    //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                    //        $"索引【{pUnit.nUniqueIdx}】【攻击目标已死 原地发呆】  ");
                    pUnit.SetState(CPlayerUnit.EMState.Idle, null, true);
                }
                else
                {
                    if (CFindTargetHelp.IsUnitInAtkRange(pUnit.pUnitData.nAtkRange, pUnit.pAtkTarget, pUnit))
                    {
                        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                        //          $"索引【{pUnit.nUniqueIdx}】【目标在攻击范围内 打他】  ");
                        GoAtkState();
                    }
                    else
                    {
                        if (pUnit.GetAtkTargetByAtkRange())
                        {
                            //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                            //      $"索引【{pUnit.nUniqueIdx}】【目标不在攻击范围内但是攻击范围内有其他可攻击目标 打他】  ");
                            GoAtkState();
                        }
                        else
                        {
                            pUnit.SetState(CPlayerUnit.EMState.Idle, null, true);
                        }
                    }
                }
            }
        }
    }

    void RefreshPos(bool refresh = true)
    {
        f64TimeScale = f64CurJumpTime / f64TotalJumpTime;
        //大于1表示当前移动结束
        if (f64TimeScale >= (Fix64)1)
        {
            f64TimeScale = (Fix64)1;

            if (refresh)
            {
                f64CurJumpTime -= f64TotalJumpTime;
                bJumpOver = true;
            }
        }

        //v64elpaseDis = new FixVector3(v64Distance.x * f64TimeScale, v64Distance.y * f64TimeScale, v64Distance.z * f64TimeScale);
        //v64CurPos = new FixVector3(v64Start.x + v64elpaseDis.x, v64Start.y + v64elpaseDis.y, v64Start.z + v64elpaseDis.z);
        v64CurPos = CHelpTools.GetCurvePointFix64(v64Start, v64Center, v64End, f64TimeScale);
        pUnit.m_fixv3LogicPosition = v64CurPos;
    }

    void GoAtkState()
    {
        if (pUnit.IsSkillAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Skill, null, true);
        }
        else if (pUnit.IsAtkAble())
        {
            pUnit.SetState(CPlayerUnit.EMState.Attack, null, true);
        }
        else
        {
            pUnit.SetState(CPlayerUnit.EMState.Idle, null, true);
        }
    }
}


public class FSMUnitRole_Atk : FSMUnitBase
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
        if (pUnit.bFirstDash)
        {
            pUnit.bFirstDash = false;
        }
        DoAtk();
    }

    public override void OnUpdate(object obj, float delta)
    {
        if ((pUnit.pAtkTarget == null ||
            pUnit.pAtkTarget.IsDead()))
        {
            pUnit.pAtkTarget = null;
            pUnit.pMoveTarget = null;
            pUnit.RefreshSearch();
            //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
            //           $"索引【{pUnit.nUniqueIdx}】【要打的人死了 发呆】  ");
            pUnit.SetState(CPlayerUnit.EMState.Idle);
            return;
        }

        if (!bAtkActiveOver)
        {
            //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
            //           $"索引【{pUnit.nUniqueIdx}】【{f64CurAtkActiveTime}】===【{f64TotalAtkActiveTime}】 Add Time == {CLockStepData.g_fixFrameLen} " + $"A【{((pUnit.pAtkTarget == null) ? -1 : pUnit.pAtkTarget.nUniqueIdx)}】 ");
            f64CurAtkActiveTime += CLockStepData.g_fixFrameLen;
            if (f64CurAtkActiveTime >= f64TotalAtkActiveTime)
            {
                //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                //          $"索引 【{pUnit.nUniqueIdx}】【{f64CurAtkActiveTime}】===【{f64TotalAtkActiveTime}】造成了伤害  " + $"A【{((pUnit.pAtkTarget == null) ? -1 : pUnit.pAtkTarget.nUniqueIdx)}】 " +
                //            $"目标状态【HP：{((pUnit.pAtkTarget == null) ? 0 : pUnit.pAtkTarget.pUnitData.nCurHP)}】" +
                //            $"【MAXHP：{((pUnit.pAtkTarget == null) ? 0 : pUnit.pAtkTarget.pUnitData.MaxHP)}】" +
                //            $"【M：{((pUnit.pAtkTarget == null) ? "" : (pUnit.pStayMapSlot.vecPos.x + ":" + pUnit.pStayMapSlot.vecPos.y))}】" +
                //            $"【ATK：{((pUnit.pAtkTarget == null) ? 0 : pUnit.pAtkTarget.pUnitData.AtkDmg)}】");
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
                //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
                //       $"索引【{pUnit.nUniqueIdx}】【攻击动作放完了 发呆】  " + $"A【{((pUnit.pAtkTarget == null) ? -1 : pUnit.pAtkTarget.nUniqueIdx)}】 ");
                pUnit.SetState(CPlayerUnit.EMState.Idle);
                //DoAtk();
            }
        }
    }

    void DoAtk()
    {
        if(pUnit.pAtkTarget == null ||
           pUnit.pAtkTarget.IsDead())
        {
            pUnit.pAtkTarget = null;
            pUnit.pMoveTarget = null;
            pUnit.RefreshSearch();
            //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
            //           $"索引【{pUnit.nUniqueIdx}】【要打的人死了 发呆】  ");
            pUnit.SetState(CPlayerUnit.EMState.Idle);

        }
        else
        {
            pUnit.RefreshAtkCD();
            if (pUnit.pUnitData.nMoveSpeed > 0)
            {
                AStarFindPath.Ins.GetNextMoveDir(ref pUnit.emMoveDir, pUnit.pStayMapSlot, pUnit.pAtkTarget.pStayMapSlot);
                pUnit.PlayAnima(EMUnitAnimeState.Atk, pUnit.emMoveDir, true);
            }
            f64TotalAtkTime = pUnit.pUnitData.AtkTime;
            f64TotalAtkActiveTime = pUnit.pUnitData.AtkActiveTime;
            f64CurAtkTime = Fix64.Zero;
            f64CurAtkActiveTime = Fix64.Zero;
            bAtkOver = false;
            bAtkActiveOver = false;
        }
    }

}

public class FSMUnitRole_Dead : FSMUnitBase
{
    Fix64 f64TotalDeadTime;
    Fix64 f64CurDeadTime;

    protected Fix64 f64MoveTime;
    protected Fix64 f64CurMoveTime;

    protected Fix64 f64MoveSpd;
    float fMoveSpd = 8f;
    float fHeight = 6f;
    Vector3 vecLerp = new Vector3(4f, 3f, 0);

    protected Vector3 vStartPos;
    protected Vector3 vEndPos;
    protected Vector3 vCenterPos;
    protected Vector3 vNextPos;

    protected float fMoveTime;
    protected float fCurMoveTime;
    protected float fCurBeizierTimeScale;
    protected float fCurUnitScaleTimeScale;


    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Dead;
        pUnit.PlayAnima(EMUnitAnimeState.Dead, pUnit.emMoveDir);

        pUnit.pColUnit.Recycle();

        if (pUnit.emDeadType == CPlayerUnit.EMDeadType.Jump)
        {
            ///获取对应的位移差值
            Vector3 vLerp = Vector3.zero;
            ///X轴
            if (pUnit.emMoveDir == EMUnitAnimeDir.UpL ||
               pUnit.emMoveDir == EMUnitAnimeDir.DownL)
            {
                vLerp.x = -vecLerp.y;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.UpR ||
                     pUnit.emMoveDir == EMUnitAnimeDir.DownR)
            {
                vLerp.x = vecLerp.y;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Left)
            {
                vLerp.x = -vecLerp.x;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Right)
            {
                vLerp.x = vecLerp.x;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Up ||
                    pUnit.emMoveDir == EMUnitAnimeDir.Down)
            {
                vLerp.x = 0;
            }
            ///Y轴
            if (pUnit.emMoveDir == EMUnitAnimeDir.DownR ||
               pUnit.emMoveDir == EMUnitAnimeDir.DownL)
            {
                vLerp.y = -vecLerp.y;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.UpR ||
                     pUnit.emMoveDir == EMUnitAnimeDir.UpL)
            {
                vLerp.y = vecLerp.y;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Down)
            {
                vLerp.y = -vecLerp.x;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Up)
            {
                vLerp.y = vecLerp.x;
            }
            else if (pUnit.emMoveDir == EMUnitAnimeDir.Left ||
                    pUnit.emMoveDir == EMUnitAnimeDir.Right)
            {
                vLerp.y = 0;
            }
            vStartPos = pUnit.tranSelf.position;
            vEndPos = pUnit.tranSelf.position - vLerp;
            FixVector3 startPos = new FixVector3((Fix64)vStartPos.x, (Fix64)vStartPos.y, (Fix64)0f);
            FixVector3 endPos = new FixVector3((Fix64)vEndPos.x, (Fix64)vEndPos.y, (Fix64)0f);
            
            vCenterPos = vStartPos + (vEndPos - vStartPos) * 0.5f + Vector3.up * fHeight;

            f64MoveSpd = (Fix64)fMoveSpd;

            //计算移动参数
            f64MoveTime = (Fix64)0.4f; //(FixVector3.Distance(startPos, endPos) / f64MoveSpd);
            fMoveTime = (float)f64MoveTime;

            f64CurMoveTime = Fix64.Zero;
            fCurMoveTime = 0f;

            fCurBeizierTimeScale = 0f;
        }
        else
        {
            f64TotalDeadTime = pUnit.pUnitData.DeadTime;
            f64CurDeadTime = Fix64.Zero;
        }
    }

    public override void OnUpdate(object obj, float delta)
    {
        if (pUnit.emDeadType == CPlayerUnit.EMDeadType.Jump)
        {
            f64CurMoveTime += CLockStepData.g_fixFrameLen;

            //大于1表示当前移动结束
            if (f64CurMoveTime >= f64MoveTime)
            {
                pUnit.Recycle();
            }
        }
        else
        {
            f64CurDeadTime += CLockStepData.g_fixFrameLen;
            if (f64CurDeadTime >= f64TotalDeadTime)
            {
                f64CurDeadTime -= f64TotalDeadTime;
                pUnit.Recycle();
            }
        }

    }

    public override void OnFixedUpdate(object obj, float delta)
    {
        if (pUnit.emDeadType == CPlayerUnit.EMDeadType.Jump)
        {
            //Debug.LogError(vStartPos + "====FixUpdate====" + vEndPos + "=====" + fMoveTime);
            fCurMoveTime += CTimeMgr.FixedDeltaTime;
            fCurBeizierTimeScale = Mathf.Min(fCurMoveTime / fMoveTime, 1f);

            if (fCurBeizierTimeScale >= 0.75f)
            {
                fCurUnitScaleTimeScale = 1 - (fCurBeizierTimeScale - 0.75f) / 0.25f;
                pUnit.transform.localScale = Vector3.one * fCurUnitScaleTimeScale;
            }

            vNextPos = CHelpTools.GetCurvePoint(vStartPos, vCenterPos, vEndPos, fCurBeizierTimeScale);

            pUnit.tranSelf.position = vNextPos;
        }
    }

}


public class FSMUnitRole_Skill : FSMUnitBase
{

    Vector3 vStartPos;
    Vector3 vEndPos;
    public enum EMSkillState
    {
        FirstWait,
        Show,
        Wait,
        Back
    }

    EMSkillState emSkillState;

    #region 技能1
    Vector3 vCenterPos;
    Vector3 vNextPos;

    Fix64 f64MoveTime;
    Fix64 f64CurMoveTime;
    Fix64 f64CurTimeScale;

    float fMoveTime = 0.2f;
    float fCurMoveTime;
    float fCurBeizierTimeScale;

    //float fBeizerSpeed = 2.5f;
    float fFirstWaitTime = 0.5f;
    float fHeight = 8f;
    float fWaitTime = 1.8f;
    float fActiveDmgTime = 1f;
    float fEffectShowTime = 0.6f;

    #endregion

    #region 技能2

    int nCurAtkIdx;

    float fWaitTime2 = 0.5f;
    float fAtkTime = 0.15f;
    float fAtkActiveTime = 0.08f;

    Vector3 vecLerp = new Vector3(6f, 4.5f, 0);

    List<EMUnitAnimeDir> listGetRandomDir = new List<EMUnitAnimeDir>();

    #endregion

    #region 技能3

    
    float fWaitTime3 = 1.8f;

    #endregion

    Fix64 f64TotalSkillTime;
    Fix64 f64CurSkillTime;
    Fix64 f64TimeScale;
    bool bSkillOver;

    Fix64 f64TotalSkillActiveTime;
    Fix64 f64CurSkillActiveTime;
    Fix64 f64TimeActiveScale;
    bool bSkillActiveOver;

    public override void OnBegin(object obj)
    {
        pUnit.emCurState = CPlayerUnit.EMState.Skill;
        DoSkill();
    }

    public override void OnUpdate(object obj, float delta)
    {
        UpdateSkillAction(delta);
        
    }

    public override void OnFixedUpdate(object obj, float delta)
    {
        FixUpdateSkillAction(delta);
    }

    void DoSkill()
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
            pUnit.RefreshSkillCD();
            AStarFindPath.Ins.GetNextMoveDir(ref pUnit.emMoveDir,pUnit.pStayMapSlot, pUnit.pAtkTarget.pStayMapSlot);
            StartSkillAction();
        }
    }

    void StartSkillAction()
    {
        CPlayerBaseInfo baseInfo = CPlayerMgr.Ins.GetPlayer(pUnit.szUserUid);
        //if(baseInfo == null)
        //{
        //}
        //else
        //{
        //    if (pUnit.emCamp == EMUnitCamp.Red)
        //    {
        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左放英雄技能, new IIHeroShowSkill(baseInfo));
        //    }
        //    else if (pUnit.emCamp == EMUnitCamp.Blue)
        //    {
        //        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右放英雄技能, new IIHeroShowSkill(baseInfo));
        //    }
            
        //}
        
        if (pUnit.pUnitData.nSkillID == 1)
        {
            pUnit.PlayAnima(EMUnitAnimeState.Idle, pUnit.emMoveDir, true);
            vStartPos = pUnit.tranSelf.position;
            List<MapSlot> mapSlots = new List<MapSlot>();
            AStarFindPath.Ins.GetLineSlotsByDir(ref mapSlots,6, pUnit.pStayMapSlot.vecPos, pUnit.emMoveDir);
            pUnit.vJumpTarget = mapSlots[mapSlots.Count - 1].vecPos;
            vEndPos = mapSlots[mapSlots.Count - 1].tranSelf.position;
            vCenterPos = vStartPos + (vEndPos - vStartPos) * 0.5f + Vector3.up * fHeight;
           
            f64MoveTime = (Fix64)fFirstWaitTime;
            f64CurMoveTime = Fix64.Zero;
            fCurMoveTime = 0f;

            pUnit.DoSkillEffect(vEndPos,delegate(GameObject obj)
            {
                CEffectFrameGroup eff = obj.GetComponent<CEffectFrameGroup>();
                if(eff != null)
                {
                    eff.SetFrameLayer(mapSlots[mapSlots.Count - 1].nCurSetRenderLayer - 1);
                    //SpriteRenderer spriteRenderer = eff.arrFrameEff[0].GetComponent<SpriteRenderer>();
                    //if(spriteRenderer != null)
                    //{
                    //    spriteRenderer.sortingOrder = -5 + mapSlots[mapSlots.Count - 1].nCurSetRenderLayer / 10 - 1;
                    //}
                }
            });
            pUnit.SetRenderLayer(mapSlots[mapSlots.Count - 1].nCurSetRenderLayer);
            f64CurTimeScale = Fix64.Zero;
            fCurBeizierTimeScale = 0f;
            emSkillState = EMSkillState.FirstWait;
            bSkillOver = false;
        }
        else if (pUnit.pUnitData.nSkillID == 3)
        {
            pUnit.PlayAnima(EMUnitAnimeState.Idle, pUnit.emMoveDir, true);
            f64TotalSkillTime = (Fix64)fWaitTime2;
            f64CurSkillTime = Fix64.Zero;
            bSkillOver = false;
            pUnit.vAtkTarget = pUnit.pAtkTarget.pStayMapSlot;
            pUnit.curAtkAnimeDir = EMUnitAnimeDir.Max;
            pUnit.DoSkillEffect(pUnit.pAtkTarget.pStayMapSlot.tranSelf.position, delegate (GameObject obj)
            {
                CEffectFrameGroup eff = obj.GetComponent<CEffectFrameGroup>();
                if (eff != null)
                {
                    eff.SetFrameLayer(pUnit.pAtkTarget.pStayMapSlot.nCurSetRenderLayer - 1);
                    //SpriteRenderer spriteRenderer = eff.arrFrameEff[0].GetComponent<SpriteRenderer>();
                    //if (spriteRenderer != null)
                    //{
                    //    spriteRenderer.sortingOrder = -5 + pUnit.pAtkTarget.pStayMapSlot.nCurSetRenderLayer / 10 - 1;
                    //}
                }
            });
            emSkillState = EMSkillState.FirstWait;
        }
        else if (pUnit.pUnitData.nSkillID == 2)
        {
            pUnit.PlayAnima(EMUnitAnimeState.Skill, pUnit.emMoveDir, true);
            f64TotalSkillTime = (Fix64)fWaitTime3;
            f64CurSkillTime = Fix64.Zero;
            bSkillOver = false;
            List<MapSlot> mapSlots = new List<MapSlot>();
            AStarFindPath.Ins.GetLineSlotsByDir(ref mapSlots,6, pUnit.pStayMapSlot.vecPos, pUnit.emMoveDir);
            pUnit.vAtkTarget = mapSlots[mapSlots.Count - 1];
            pUnit.DoSkillEffect(pUnit.vAtkTarget.tranSelf.position);
            if (!CHelpTools.IsStringEmptyOrNone(pUnit.szSkillEffect))
            {
                CEffectMgr.Instance.CreateEffSync(pUnit.szSkillEffect, pUnit.vAtkTarget.tranSelf.position, Quaternion.identity, 0);
            }
            emSkillState = EMSkillState.FirstWait;
        }
        else if (pUnit.pUnitData.nSkillID == 4)
        {
            pUnit.PlayAnima(EMUnitAnimeState.Skill, pUnit.emMoveDir, true);
        }
    }

    void UpdateSkillAction(float delta)
    {
        if (pUnit.pUnitData.nSkillID == 1)
        {
            if (emSkillState == EMSkillState.FirstWait)
            {
                f64CurMoveTime += CLockStepData.g_fixFrameLen;
                f64CurTimeScale = f64CurMoveTime / f64MoveTime;

                //大于1表示当前移动结束
                if (f64CurTimeScale >= (Fix64)1)
                {
                    f64CurTimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
                if (bSkillOver)
                {
                    pUnit.PlayAnima(EMUnitAnimeState.Skill, pUnit.emMoveDir, true);
                    //计算移动参数
                    f64MoveTime = (Fix64)fMoveTime;
                    f64CurMoveTime = Fix64.Zero;
                    f64CurTimeScale = Fix64.Zero;
                    fCurMoveTime = 0f;
                    bSkillOver = false;
                    emSkillState = EMSkillState.Show;
                }
            }
            else if (emSkillState == EMSkillState.Show)
            {
                f64CurMoveTime += CLockStepData.g_fixFrameLen;
                f64CurTimeScale = f64CurMoveTime / f64MoveTime;

                //大于1表示当前移动结束
                if (f64CurTimeScale >= (Fix64)1)
                {
                    f64CurTimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
                if (bSkillOver)
                {
                    fCurMoveTime = 0f;
                    if (!CHelpTools.IsStringEmptyOrNone(pUnit.szSkillEffect))
                    {
                        CEffectMgr.Instance.CreateEffSync(pUnit.szSkillEffect, pUnit.tranSelf.position, Quaternion.identity, 0);
                    }
                    pUnit.tranSelf.position = vEndPos;

                    f64MoveTime = (Fix64)fWaitTime;
                    f64CurMoveTime = Fix64.Zero;
                    f64CurTimeScale = Fix64.Zero;
                    f64TotalSkillActiveTime = (Fix64)fActiveDmgTime;
                    f64CurSkillActiveTime = Fix64.Zero;
                    f64TimeActiveScale = Fix64.Zero;
                    bSkillOver = false;
                    bSkillActiveOver = false;
                    emSkillState = EMSkillState.Wait;
                }
            }
            else if (emSkillState == EMSkillState.Wait)
            {
                if (!bSkillActiveOver)
                {
                    f64CurSkillActiveTime += CLockStepData.g_fixFrameLen;
                    f64TimeActiveScale = f64CurSkillActiveTime / f64TotalSkillActiveTime;
                    if (f64TimeActiveScale >= (Fix64)1)
                    {
                        f64TimeActiveScale = (Fix64)1;
                        f64CurSkillActiveTime -= f64TotalSkillActiveTime;
                        pUnit.DoSkill();
                        bSkillActiveOver = true;
                    }
                }

                f64CurMoveTime += CLockStepData.g_fixFrameLen;
                f64CurTimeScale = f64CurMoveTime / f64MoveTime;
                //大于1表示当前移动结束
                if (f64CurTimeScale >= (Fix64)1)
                {
                    f64CurTimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
                if (bSkillOver)
                {
                    vStartPos = pUnit.tranSelf.position;
                    vEndPos = pUnit.pStayMapSlot.tranSelf.position;
                    vCenterPos = vStartPos + (vEndPos - vStartPos) * 0.5f + Vector3.up * fHeight;
                    f64MoveTime = (Fix64)fMoveTime;
                    f64CurMoveTime = Fix64.Zero;
                    fCurMoveTime = 0f;
                    f64CurTimeScale = Fix64.Zero;
                    fCurBeizierTimeScale = 0f;
                    pUnit.SetRenderLayer(pUnit.pStayMapSlot.nCurSetRenderLayer);
                    bSkillOver = false;
                    emSkillState = EMSkillState.Back;
                }
            }
            else if (emSkillState == EMSkillState.Back)
            {
                f64CurMoveTime += CLockStepData.g_fixFrameLen;
                f64CurTimeScale = f64CurMoveTime / f64MoveTime;
                //大于1表示当前移动结束
                if (f64CurTimeScale >= (Fix64)1)
                {
                    f64CurTimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
                if (bSkillOver)
                {
                    bSkillOver = false;
                    pUnit.SetState(CPlayerUnit.EMState.Idle);
                }
            }
        }
        else if (pUnit.pUnitData.nSkillID == 3)
        {
            if (emSkillState == EMSkillState.FirstWait)
            {
                f64CurSkillTime += CLockStepData.g_fixFrameLen;
                f64TimeScale = f64CurSkillTime / f64TotalSkillTime;

                //大于1表示当前移动结束
                if (f64TimeScale >= (Fix64)1)
                {
                    f64TimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                //m_fixv3LogicPosition = CHelpTools.GetCurvePointFix64(v64StartPos, v64CenterPos, v64EndPos, f64CurTimeScale);
                if (bSkillOver)
                {
                    nCurAtkIdx = 0;
                    f64TotalSkillTime = (Fix64)fAtkTime;
                    f64TotalSkillActiveTime = (Fix64)fAtkActiveTime;
                    f64CurSkillTime = Fix64.Zero;
                    f64CurSkillActiveTime = Fix64.Zero;
                    bSkillOver = false;
                    DoSkillActionBySkill3();
                    emSkillState = EMSkillState.Show;
                }
            }
            else
            {
                if (!bSkillActiveOver)
                {
                    f64CurSkillActiveTime += CLockStepData.g_fixFrameLen;
                    f64TimeActiveScale = f64CurSkillActiveTime / f64TotalSkillActiveTime;
                    if (f64TimeActiveScale >= (Fix64)1)
                    {
                        f64TimeActiveScale = (Fix64)1;
                        f64CurSkillActiveTime -= f64TotalSkillActiveTime;
                        pUnit.DoSkill();
                        bSkillActiveOver = true;
                    }
                }
                if (!bSkillOver)
                {
                    f64CurSkillTime += CLockStepData.g_fixFrameLen;
                    f64TimeScale = f64CurSkillTime / f64TotalSkillTime;
                    if (f64TimeScale >= (Fix64)1)
                    {
                        f64TimeScale = (Fix64)1;
                        f64CurSkillTime -= f64TotalSkillTime;
                        bSkillOver = true;
                        fCurMoveTime = 0f;
                        nCurAtkIdx++;
                        if (!CHelpTools.IsStringEmptyOrNone(pUnit.szSkillEffect))
                        {
                            CEffectMgr.Instance.CreateEffSync(pUnit.szSkillEffect, pUnit.tranSelf.position, Quaternion.identity, 0);
                        }
                        if (nCurAtkIdx < pUnit.pUnitData.pSkillInfo.nValue)
                        {
                            DoSkillActionBySkill3();
                        }
                        else
                        {
                            pUnit.StopEffect();
                            pUnit.SetState(CPlayerUnit.EMState.Idle);
                        }
                    }
                }
            }
        }
        else if (pUnit.pUnitData.nSkillID == 2)
        {
            if (emSkillState == EMSkillState.FirstWait)
            {
                f64CurSkillTime += CLockStepData.g_fixFrameLen;
                f64TimeScale = f64CurSkillTime / f64TotalSkillTime;

                //大于1表示当前移动结束
                if (f64TimeScale >= (Fix64)1)
                {
                    f64TimeScale = (Fix64)1;
                    bSkillOver = true;
                }
                if (bSkillOver)
                {
                    pUnit.PlayAnima(EMUnitAnimeState.Skill2, pUnit.emMoveDir, true);
                    f64TotalSkillTime = pUnit.pUnitData.pSkillInfo.f64SkillTime;
                    f64TotalSkillActiveTime = pUnit.pUnitData.pSkillInfo.f64SkillActiveTime;
                    f64CurSkillTime = Fix64.Zero;
                    f64CurSkillActiveTime = Fix64.Zero;
                    bSkillOver = false;
                    bSkillActiveOver = false;
                    emSkillState = EMSkillState.Show;
                }
            }
            else
            {
                if (!bSkillActiveOver)
                {
                    f64CurSkillActiveTime += CLockStepData.g_fixFrameLen;
                    f64TimeActiveScale = f64CurSkillActiveTime / f64TotalSkillActiveTime;
                    if (f64TimeActiveScale >= (Fix64)1)
                    {
                        f64TimeActiveScale = (Fix64)1;
                        f64CurSkillActiveTime -= f64TotalSkillActiveTime;
                        pUnit.DoSkill();
                        bSkillActiveOver = true;
                    }
                }
                if (!bSkillOver)
                {
                    f64CurSkillTime += CLockStepData.g_fixFrameLen;
                    f64TimeScale = f64CurSkillTime / f64TotalSkillTime;
                    if (f64TimeScale >= (Fix64)1)
                    {
                        f64TimeScale = (Fix64)1;
                        f64CurSkillTime -= f64TotalSkillTime;
                        bSkillOver = true;
                        pUnit.SetState(CPlayerUnit.EMState.Idle);
                    }
                }
            }
        }
        else if (pUnit.pUnitData.nSkillID == 4)
        {

        }
    }

    void FixUpdateSkillAction(float delta)
    {
        if (pUnit.pUnitData.nSkillID == 1)
        {
            if(emSkillState == EMSkillState.Back)
            {
                fCurMoveTime += CTimeMgr.FixedDeltaTime;
                fCurBeizierTimeScale = Mathf.Min(fCurMoveTime / fMoveTime, 1f);
                vNextPos = CHelpTools.GetCurvePoint(vStartPos, vCenterPos, vEndPos, fCurBeizierTimeScale);
                if (fCurBeizierTimeScale < 1f)
                {
                    //if (emBulletDir == EMDir.Up)
                    //{
                    //    tranSelf.up = (vNextPos - tranSelf.position).normalized;
                    //}
                    //else if (emBulletDir == EMDir.Right)
                    //{
                    //    tranSelf.right = (vNextPos - tranSelf.position).normalized;
                    //}
                }
                pUnit.tranSelf.position = vNextPos;
                //Debug.LogError(pUnit.tranSelf.position + "====" + vEndPos + "===" + fCurBeizierTimeScale);
            }

        }
        else if (pUnit.pUnitData.nSkillID == 3)
        {
            if (emSkillState == EMSkillState.FirstWait)
            {

            }
            else
            {
                if (!bSkillOver)
                {
                    fCurMoveTime += CTimeMgr.FixedDeltaTime;
                    fCurBeizierTimeScale = Mathf.Min(fCurMoveTime / fMoveTime, 1f);
                    vNextPos = Vector3.Lerp(vStartPos, vEndPos, fCurBeizierTimeScale);
                    pUnit.tranSelf.position = vNextPos;
                }
            }
        }
        else if (pUnit.pUnitData.nSkillID == 2)
        {

        }
        else if (pUnit.pUnitData.nSkillID == 4)
        {

        }
    }

    void DoSkillActionBySkill3()
    {
        ///获取随机的攻击方向
        Vector3 vLerp = Vector3.zero;
        int nRandomIdx = 0;
        if (pUnit.curAtkAnimeDir != EMUnitAnimeDir.Max)
        {
            nRandomIdx = CLockStepMgr.Ins.GetRandomInt(1, 67);
        }
        listGetRandomDir.Clear();
        switch (pUnit.curAtkAnimeDir)
        {
            case EMUnitAnimeDir.Max:
                {
                    nRandomIdx = CLockStepMgr.Ins.GetRandomInt(0, (int)EMUnitAnimeDir.Max);
                }
                break;
            case EMUnitAnimeDir.UpL:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.Down);
                    listGetRandomDir.Add(EMUnitAnimeDir.DownL);
                    listGetRandomDir.Add(EMUnitAnimeDir.Right);
                }
                break;
            case EMUnitAnimeDir.UpR:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.Down);
                    listGetRandomDir.Add(EMUnitAnimeDir.DownR);
                    listGetRandomDir.Add(EMUnitAnimeDir.Right);
                }
                break;
            case EMUnitAnimeDir.DownL:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.Up);
                    listGetRandomDir.Add(EMUnitAnimeDir.UpL);
                    listGetRandomDir.Add(EMUnitAnimeDir.Left);
                }
                break;
            case EMUnitAnimeDir.DownR:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.Up);
                    listGetRandomDir.Add(EMUnitAnimeDir.UpR);
                    listGetRandomDir.Add(EMUnitAnimeDir.Left);
                }
                break;
            case EMUnitAnimeDir.Left:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.DownR);
                    listGetRandomDir.Add(EMUnitAnimeDir.UpR);
                    listGetRandomDir.Add(EMUnitAnimeDir.Down);
                }
                break;
            case EMUnitAnimeDir.Right:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.DownL);
                    listGetRandomDir.Add(EMUnitAnimeDir.UpL);
                    listGetRandomDir.Add(EMUnitAnimeDir.Up);
                }
                break;
            case EMUnitAnimeDir.Up:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.DownR);
                    listGetRandomDir.Add(EMUnitAnimeDir.DownL);
                    listGetRandomDir.Add(EMUnitAnimeDir.Left);
                }
                break;
            case EMUnitAnimeDir.Down:
                {
                    listGetRandomDir.Add(EMUnitAnimeDir.UpL);
                    listGetRandomDir.Add(EMUnitAnimeDir.UpR);
                    listGetRandomDir.Add(EMUnitAnimeDir.Right);
                }
                break;
        }
        if (listGetRandomDir.Count > 0)
        {
            if (nRandomIdx < 22)
            {
                pUnit.curAtkAnimeDir = listGetRandomDir[0];
            }
            else if (nRandomIdx < 44)
            {
                pUnit.curAtkAnimeDir = listGetRandomDir[1];
            }
            else
            {
                pUnit.curAtkAnimeDir = listGetRandomDir[2];
            }
        }
        else
        {
            pUnit.curAtkAnimeDir = (EMUnitAnimeDir)nRandomIdx;
        }
        ///获取对应的位移差值
        ///X轴
        if(pUnit.curAtkAnimeDir == EMUnitAnimeDir.UpL ||
           pUnit.curAtkAnimeDir == EMUnitAnimeDir.DownL)
        {
            vLerp.x = -vecLerp.y;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.UpR ||
                 pUnit.curAtkAnimeDir == EMUnitAnimeDir.DownR)
        {
            vLerp.x = vecLerp.y;
        }
        else if(pUnit.curAtkAnimeDir == EMUnitAnimeDir.Left)
        {
            vLerp.x = -vecLerp.x;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.Right)
        {
            vLerp.x = vecLerp.x;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.Up ||
                pUnit.curAtkAnimeDir == EMUnitAnimeDir.Down)
        {
            vLerp.x = 0;
        }
        ///Y轴
        if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.DownR ||
           pUnit.curAtkAnimeDir == EMUnitAnimeDir.DownL)
        {
            vLerp.y = -vecLerp.y;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.UpR ||
                 pUnit.curAtkAnimeDir == EMUnitAnimeDir.UpL)
        {
            vLerp.y = vecLerp.y;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.Down)
        {
            vLerp.y = -vecLerp.x;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.Up)
        {
            vLerp.y = vecLerp.x;
        }
        else if (pUnit.curAtkAnimeDir == EMUnitAnimeDir.Left ||
                pUnit.curAtkAnimeDir == EMUnitAnimeDir.Right)
        {
            vLerp.y = 0;
        }

        vStartPos = pUnit.vAtkTarget.tranSelf.position - vLerp;
        vEndPos = pUnit.vAtkTarget.tranSelf.position + vLerp;

        pUnit.PlayEffect(pUnit.curAtkAnimeDir, pUnit.vAtkTarget.nCurSetRenderLayer + 1);
        pUnit.PlayAnima(EMUnitAnimeState.Skill, pUnit.curAtkAnimeDir, true);
        f64CurSkillTime = Fix64.Zero;
        f64CurSkillActiveTime = Fix64.Zero;
        bSkillOver = false;
        bSkillActiveOver = false;

    }

}
