using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EMUnitCamp
{
    Blue = 0,         //玩家
    Red = 1,          //敌军

    Max,        
}

public enum EMSkillSearchCamp
{
    Friend = 0, //友军
    Enemy = 1,  //敌军
    Stand = 2,  //中立
}

public class CFindTargetHelp : MonoBehaviour
{
    /// <summary>
    /// 获取要找的指定阵营
    /// </summary>
    /// <param name="search"></param>
    /// <param name="selfCamp"></param>
    /// <returns></returns>
    public static EMUnitCamp GetTargetCamp(EMSkillSearchCamp search, EMUnitCamp selfCamp)
    {
        EMUnitCamp emTarget = selfCamp;
        if (search == EMSkillSearchCamp.Enemy)
        {
            if (selfCamp == EMUnitCamp.Blue)
                emTarget = EMUnitCamp.Red;
            else if (selfCamp == EMUnitCamp.Red)
                emTarget = EMUnitCamp.Blue;
        }

        return emTarget;
    }


    #region 仇恨对象寻找逻辑

    public static void SearchTargetInRangeBySlot(int nRange,Vector2Int vecCheckPos, EMSkillSearchCamp searchCamp, ref CPlayerUnit pTargetUnit, CPlayerUnit checkUnit,bool bOutIn)
    {
        EMUnitCamp emTargetCamp = GetTargetCamp(searchCamp, checkUnit.emCamp);
        //GetOutAroundSlot
        CPlayerUnit colUnit = null;
        List <MapSlot> listSlots = null;
        if (bOutIn)
        {
            for (int i = nRange; i >= 1; i--)
            {
                colUnit = null;
                AStarFindPath.Ins.GetOutAroundSlot(i, vecCheckPos, ref listSlots);

                for (int j = 0; j < listSlots.Count; j++)
                {
                    if (listSlots[j] == null) continue;
                    colUnit = listSlots[j].pStayGroundUnit;
                    if (colUnit == null)
                    {
                        if (checkUnit.pUnitData.bCanAtkFly)
                        {
                            colUnit = listSlots[j].pStayFlyUnit;
                        }
                    }
                    if (colUnit == null ||
                        colUnit.IsDead() ||
                        colUnit == checkUnit ||
                        colUnit.emCamp != emTargetCamp)
                    {
                        continue;
                    }

                    pTargetUnit = colUnit;
                    break;
                }
                if (pTargetUnit != null)
                    break;
            }
        }
        else
        {
            for (int i = 1; i <= nRange; i++)
            {
                colUnit = null;
                AStarFindPath.Ins.GetOutAroundSlot(i, vecCheckPos, ref listSlots);

                for (int j = 0; j < listSlots.Count; j++)
                {
                    if (listSlots[j] == null) continue;
                    colUnit = listSlots[j].pStayGroundUnit;
                    if (colUnit == null)
                    {
                        if (checkUnit.pUnitData.bCanAtkFly)
                        {
                            colUnit = listSlots[j].pStayFlyUnit;
                        }
                    }
                    if (colUnit == null ||
                        colUnit.IsDead() ||
                        colUnit == checkUnit ||
                        colUnit.emCamp != emTargetCamp)
                    {
                        continue;
                    }

                    pTargetUnit = colUnit;
                    break;
                }
                if (pTargetUnit != null)
                    break;
            }
        }

        //List<MapSlot> listSlots = AStarFindPath.Ins.GetAroundSlot(checkUnit.pUnitData.nAlertRange, vecCheckPos);
        //CPlayerUnit colUnit = null;
        //int nDis = 0;
        //int nCurDis = 99999;
        //for (int i = 0;i < listSlots.Count;i++)
        //{
        //    if (listSlots[i] == null) continue;
        //    colUnit = listSlots[i].pStayGroundUnit;
        //    if(colUnit == null)
        //    {
        //        if(checkUnit.pUnitData.bCanAtkFly)
        //        {
        //            colUnit = listSlots[i].pStayFlyUnit;
        //        }
        //    }
        //    if (colUnit == null)
        //    {
        //        continue;
        //    }
        //    if (colUnit.IsDead()) continue;
        //    if (colUnit == checkUnit) continue;
        //    if (colUnit.emCamp != emTargetCamp) continue;
        //if (colUnit.pStayMapSlot != null &&
        //   checkUnit.pStayMapSlot != null)
        //{
        //    nDis = (colUnit.pStayMapSlot.vecPos - checkUnit.pStayMapSlot.vecPos).sqrMagnitude;
        //}
        //else
        //{
        //    nDis = 0;
        //}
        //    if(nDis > 0 && nCurDis > nDis)
        //    {
        //        nCurDis = nDis;
        //        pTargetUnit = colUnit;
        //    }
        //}

        if (pTargetUnit != null)
        {
            //Debug.Log($"定位帧：{CLockStepData.g_uGameLogicFrame}  【索引{checkUnit.nUniqueIdx}】 定位：【{pTargetUnit.nUniqueIdx}】");

            pTargetUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos,ref checkUnit.pMoveTarget);
        }
    }

    static Fix64 f64BaseRange = (Fix64)0.25f;
    static Fix64 f64SlotRangeLerp = (Fix64)0.6f;
    static float fBaseRange = 0.25f;
    static float fSlotRangeLerp = 0.6f;

    /// <summary>
    /// 用碰撞盒子检测
    /// </summary>
    /// <param name="searchCamp"></param>
    /// <param name="checkUnit"></param>
    /// <param name="checkMask"></param>
    /// <param name="arrCols"></param>
    /// <returns></returns>
    //public static void SearchTargetInRange(int nRange, EMSkillSearchCamp searchCamp, ref CPlayerUnit pTargetUnit, CPlayerUnit checkUnit, LayerMask checkMask, Collider2D[] arrCols)
    //{
    //    pTargetUnit = null;
    //    EMUnitCamp emTargetCamp = GetTargetCamp(searchCamp, checkUnit.emCamp);

    //    float fRadius = fBaseRange + (float)(nRange) * fSlotRangeLerp;
    //    Fix64 fCheckDis = Fix64.Pow2(f64BaseRange + (Fix64)nRange * f64SlotRangeLerp);

    //    int nCheckCount = Physics2D.OverlapCircleNonAlloc(checkUnit.pStayMapSlot.tranSelf.position, fRadius, arrCols, checkMask.value);
    //    if (nCheckCount > 0)
    //    {
    //        Fix64 f64Dis = Fix64.Zero;
    //        Fix64 f64CurDis = (Fix64)99999;

    //        CPlayerUnit colUnit = null;
    //        CColBindUnit colBind = null;
    //        MapSlot pCheckSlot = null;

    //        for (int i = 0; i < arrCols.Length; i++)
    //        {
    //            if (arrCols[i] == null) continue;

    //            colBind = arrCols[i].GetComponent<CColBindUnit>();
    //            if (colBind == null)
    //                continue;

    //            colUnit = colBind.pBindUnit;
    //            if (colUnit == null)
    //                continue;

    //            if (colUnit.emMoveType == CPlayerUnit.EMMoveType.Fly &&
    //                !checkUnit.pUnitData.bCanAtkFly)
    //                continue;

    //            if (colUnit.IsDead() ||
    //                colUnit.szSelfUid == checkUnit.szSelfUid ||
    //                colUnit.emCamp != emTargetCamp) 
    //                continue;

    //            if (colUnit.pStayMapSlot != null &&
    //                checkUnit.pStayMapSlot != null)
    //            {
    //                if(colUnit.emStayRange == CPlayerUnit.EMStayRange.Normal)
    //                {
    //                    pCheckSlot = colUnit.pStayMapSlot;
    //                }
    //                else
    //                {
    //                    colUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref pCheckSlot);
    //                }
    //                //if (nRange == 1)
    //                //{
    //                //    Debug.LogError(checkUnit.pStayMapSlot.vecPos + "=====" + fRadius + "====AAAA ====" + nRange + "===="+ colUnit.pStayMapSlot.vecPos + "====" + arrCols.Length);
    //                //}
    //                f64Dis = FixVector3.SqrMagnitude(pCheckSlot.v64SlotPos - checkUnit.pStayMapSlot.v64SlotPos);
    //                //if (checkUnit.emStayRange > CPlayerUnit.EMStayRange.Around)
    //                //{
    //                //    Debug.LogError(f64Dis + "===" + pCheckSlot.vecPos + "====" + colUnit.pStayMapSlot.vecPos);
    //                //}
    //            }
    //            else
    //            {
    //                f64Dis = Fix64.Zero;
    //            }

    //            if (f64Dis > Fix64.Zero &&
    //                f64Dis < fCheckDis)
    //            {
    //                if(f64CurDis > f64Dis)
    //                {
    //                    f64CurDis = f64Dis;
    //                    pTargetUnit = colUnit;
    //                }
    //                else if(f64CurDis == f64Dis)
    //                {
    //                    if(pTargetUnit == null)
    //                    {
    //                        pTargetUnit = colUnit;
    //                    }
    //                    else
    //                    {
    //                        if(pTargetUnit.nUniqueIdx > colUnit.nUniqueIdx)
    //                        {
    //                            pTargetUnit = colUnit;
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //    }
       
    //    if (pTargetUnit != null)
    //    {
    //        Debug.Log(checkUnit.nUniqueIdx + " 定位：" + pTargetUnit.nUniqueIdx);

    //        pTargetUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos,ref checkUnit.pMoveTarget);
    //    }
    //    else
    //    {
    //        //Debug.Log(checkUnit.nUniqueIdx + " 没找到人");
    //    }
    //    //return pTargetUnit;
    //}

    public static void SearchTargetInRange(int nRange, EMSkillSearchCamp searchCamp, ref CPlayerUnit pTargetUnit, CPlayerUnit checkUnit, LayerMask checkMask, Collider2D[] arrCols)
    {
        pTargetUnit = null;
        EMUnitCamp emTargetCamp = GetTargetCamp(searchCamp, checkUnit.emCamp);

        float fRadius = fBaseRange + (float)(nRange) * fSlotRangeLerp;
        Fix64 fCheckDis = Fix64.Pow2(f64BaseRange + (Fix64)nRange * f64SlotRangeLerp);

        Collider2D[] arrResCols = Physics2D.OverlapCircleAll(checkUnit.pStayMapSlot.tranSelf.position, fRadius, checkMask.value);
        if (arrResCols != null && 
            arrResCols.Length > 0)
        {
            Fix64 f64Dis = Fix64.Zero;
            Fix64 f64CurDis = (Fix64)99999;

            CPlayerUnit colUnit = null;
            CColBindUnit colBind = null;
            MapSlot pCheckSlot = null;

            for (int i = 0; i < arrResCols.Length; i++)
            {
                if (arrResCols[i] == null) continue;

                colBind = arrResCols[i].GetComponent<CColBindUnit>();
                if (colBind == null)
                    continue;

                colUnit = colBind.pBindUnit;
                if (colUnit == null)
                    continue;

                if (colUnit.emMoveType == CPlayerUnit.EMMoveType.Fly &&
                    !checkUnit.pUnitData.bCanAtkFly)
                    continue;

                if (colUnit.IsDead() ||
                    colUnit.szSelfUid == checkUnit.szSelfUid ||
                    colUnit.emCamp != emTargetCamp)
                    continue;

                if (colUnit.pStayMapSlot != null &&
                    checkUnit.pStayMapSlot != null)
                {
                    if (colUnit.emStayRange == CPlayerUnit.EMStayRange.Normal)
                    {
                        pCheckSlot = colUnit.pStayMapSlot;
                    }
                    else
                    {
                        colUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref pCheckSlot);
                    }
                    //if (nRange == 1)
                    //{
                    //    Debug.LogError(checkUnit.pStayMapSlot.vecPos + "=====" + fRadius + "====AAAA ====" + nRange + "===="+ colUnit.pStayMapSlot.vecPos + "====" + arrCols.Length);
                    //}
                    f64Dis = FixVector3.SqrMagnitude(pCheckSlot.v64SlotPos - checkUnit.pStayMapSlot.v64SlotPos);
                    //if (checkUnit.emStayRange > CPlayerUnit.EMStayRange.Around)
                    //{
                    //    Debug.LogError(f64Dis + "===" + pCheckSlot.vecPos + "====" + colUnit.pStayMapSlot.vecPos);
                    //}
                }
                else
                {
                    f64Dis = Fix64.Zero;
                }

                if (f64Dis > Fix64.Zero &&
                    f64Dis < fCheckDis)
                {
                    if (f64CurDis > f64Dis)
                    {
                        f64CurDis = f64Dis;
                        pTargetUnit = colUnit;
                    }
                    else if (f64CurDis == f64Dis)
                    {
                        if (pTargetUnit == null)
                        {
                            pTargetUnit = colUnit;
                        }
                        else
                        {
                            if (pTargetUnit.nUniqueIdx > colUnit.nUniqueIdx)
                            {
                                pTargetUnit = colUnit;
                            }
                        }
                    }
                }
            }
        }

        if (pTargetUnit != null)
        {
            //Debug.Log($"定位帧：{CLockStepData.g_uGameLogicFrame}  【索引{checkUnit.nUniqueIdx}】 索敌数：【{arrResCols.Length}】 定位：【{pTargetUnit.nUniqueIdx}】");
            pTargetUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref checkUnit.pMoveTarget);
        }
        else
        {
            //Debug.Log(checkUnit.nUniqueIdx + " 没找到人");
        }
        //return pTargetUnit;
    }

    public static void SearchTargetInRangeNet(int nRange, EMSkillSearchCamp searchCamp, ref CPlayerUnit pTargetUnit, CPlayerUnit checkUnit, List<BroadPhaseEntry> listCheckEntry)
    {
        pTargetUnit = null;
        EMUnitCamp emTargetCamp = GetTargetCamp(searchCamp, checkUnit.emCamp);

        BEPUutilities.Vector3 vCheckPos = new BEPUutilities.Vector3(checkUnit.pStayMapSlot.v64SlotPos.x,
                                                                    checkUnit.pStayMapSlot.v64SlotPos.y,
                                                                    checkUnit.pStayMapSlot.v64SlotPos.z);
        
        Fix64 f64Radius = f64BaseRange + (Fix64)nRange * f64SlotRangeLerp;
        Fix64 f64CheckDis = Fix64.Pow2(f64BaseRange + (Fix64)nRange * f64SlotRangeLerp);

        CLockPhysicMgr.Ins.pBEPUSpace.
                    BroadPhase.QueryAccelerator.
                    GetEntries(new BEPUutilities.BoundingSphere(vCheckPos, f64Radius), listCheckEntry);

        if (listCheckEntry != null &&
            listCheckEntry.Count > 0)
        {
            Fix64 f64Dis = Fix64.Zero;
            Fix64 f64CurDis = (Fix64)99999;

            CPlayerUnit colUnit = null;
            MapSlot pCheckSlot = null;

            for (int i = 0; i < listCheckEntry.Count; i++)
            {
                if (listCheckEntry[i] == null) continue;

                EntityCollidable entityCollision = listCheckEntry[i] as EntityCollidable;
                if (entityCollision == null) continue;

                Entity pEntity = entityCollision.Entity;
                if (pEntity == null) continue;
                if (pEntity.isDynamic) continue;

                CLockUnityObject pLockUnit = pEntity.pLockUnit;
                if (pLockUnit == null) continue;

                colUnit = pLockUnit as CPlayerUnit;
                if (colUnit == null) continue;

                if (colUnit.emMoveType == CPlayerUnit.EMMoveType.Fly &&
                    !checkUnit.pUnitData.bCanAtkFly)
                    continue;

                if (colUnit.IsDead() ||
                    colUnit.szSelfUid == checkUnit.szSelfUid ||
                    colUnit.emCamp != emTargetCamp)
                    continue;

                if (colUnit.pStayMapSlot != null &&
                    checkUnit.pStayMapSlot != null)
                {
                    if (colUnit.emStayRange == CPlayerUnit.EMStayRange.Normal)
                    {
                        pCheckSlot = colUnit.pStayMapSlot;
                    }
                    else
                    {
                        colUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref pCheckSlot);
                    }

                    f64Dis = FixVector3.SqrMagnitude(pCheckSlot.v64SlotPos - checkUnit.pStayMapSlot.v64SlotPos);
                }
                else
                {
                    f64Dis = Fix64.Zero;
                }

                if (f64Dis > Fix64.Zero &&
                    f64Dis < f64CheckDis)
                {
                    if (f64CurDis > f64Dis)
                    {
                        f64CurDis = f64Dis;
                        pTargetUnit = colUnit;
                    }
                    else if (f64CurDis == f64Dis)
                    {
                        if (pTargetUnit == null)
                        {
                            pTargetUnit = colUnit;
                        }
                        else
                        {
                            if (pTargetUnit.nUniqueIdx > colUnit.nUniqueIdx)
                            {
                                pTargetUnit = colUnit;
                            }
                        }
                    }
                }
            }
        }

        if (pTargetUnit != null)
        {
            //Debug.Log($"定位帧：{CLockStepData.g_uGameLogicFrame}  【索引{checkUnit.nUniqueIdx}】 索敌数：【{listCheckEntry.Count}】 定位：【{pTargetUnit.nUniqueIdx}】");

            pTargetUnit.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref checkUnit.pMoveTarget);
        }
        else
        {
            //Debug.Log(checkUnit.nUniqueIdx + " 没找到人");
        }
        //return pTargetUnit;
    }

    ///// <summary>
    ///// 寻找范围内最近的目标(移动目标)
    ///// </summary>
    //public static CPlayerUnit SearchRoleAtkTargetNearest(Vector2Int vecCheckPos, int nRange, EMSkillSearchCamp searchCamp, CPlayerUnit startUnit)
    //{
    //    return null;
    //    if (nRange > AStarFindPath.Ins.vecMapSize.x)
    //        nRange = AStarFindPath.Ins.vecMapSize.x;

    //    CPlayerUnit pNearestUnit = null;
    //    MapSlot pNearSlot = null;
    //    CPlayerUnit colUnit = null;
    //    List <MapSlot> listCheckSlot = new List<MapSlot>();
    //    EMUnitCamp emTargetCamp = GetTargetCamp(searchCamp, startUnit.emCamp);
    //    List<MapSlot> listSlots = AStarFindPath.Ins.GetMapSlotByRange(vecCheckPos, nRange, EMSkillRangeType.NormalAtk);
    //    float nPathLength = 999;
    //    MapSlot pMoveTarget = null;
    //    if (listSlots != null)
    //    {
    //        for (int i = 0; i < listSlots.Count; i++)
    //        {
    //            if (listSlots[i] == null) continue;
    //            if (listSlots[i].pStayUnit == null) continue;
    //            colUnit = listSlots[i].pStayUnit;
    //            if (colUnit == null) continue;
    //            if (colUnit.emCamp != emTargetCamp) continue;

    //            int pathLength = 1000;
    //            float fCheckDis = 1000;
    //            int nDis = (colUnit.pStayMapSlot.vecPos - startUnit.pStayMapSlot.vecPos).sqrMagnitude;
    //            if (IsUnitInAtkRange(startUnit.pStayMapSlot.vecPos, startUnit.pUnitData.nAtkRange, colUnit))
    //            {
    //                fCheckDis = Mathf.Sqrt(nDis);
    //            }
    //            else
    //            {
    //                pathLength = AStarFindPath.Ins.nGetPathLength(startUnit.pStayMapSlot, colUnit.pStayMapSlot);
    //            }
    //            //Debug.LogError(fCheckDis + "====" + pathLength + "====" + colUnit.pStayMapSlot.vecPos + "=====" + startUnit.pStayMapSlot.vecPos);
    //            ///不在攻击范围中的角色，按可攻击区域找最近的点移动
    //            if (pathLength == -1)
    //            {
    //                listCheckSlot = AStarFindPath.Ins.GetMapSlotByRange(colUnit.pStayMapSlot.vecPos, startUnit.pUnitData.nAtkRange, EMSkillRangeType.Around);// (int)startUnit.pData.nAtkRange);
    //                pNearSlot = null;
    //                float fValue = 999;
    //                for (int j = 0; j < listCheckSlot.Count; j++)
    //                {
    //                    if (!startUnit.CheckMapSlotCanMove(listCheckSlot[j].vecPos))
    //                        continue;
    //                    float fDis = (startUnit.pStayMapSlot.vecPos - listCheckSlot[j].vecPos).sqrMagnitude;
    //                    if (fValue > fDis)
    //                    {
    //                        fValue = fDis;
    //                        pNearSlot = listCheckSlot[j];
    //                    }
    //                }
    //                if (pNearSlot != null)
    //                {
    //                    pathLength = AStarFindPath.Ins.nGetPathLength(startUnit.pStayMapSlot, pNearSlot);
    //                    if (!colUnit.IsDead() && pathLength > 0 && nPathLength > pathLength)
    //                    {
    //                        nPathLength = pathLength;
    //                        pNearestUnit = colUnit;
    //                        pMoveTarget = pNearSlot;
    //                    }
    //                }
    //            }
    //            else if (!colUnit.IsDead())// && (pathLength > 0 || fCheckDis > 0))
    //            {
    //                bool bInSteadSlot = false;
    //                ///判断到达目标所需距离是否相等
    //                if ((pathLength < 1000 && nPathLength == pathLength) ||
    //                    (fCheckDis < 1000 && nPathLength == fCheckDis))
    //                {
    //                    if (colUnit.pStayMapSlot.vecPos.x == startUnit.pStayMapSlot.vecPos.x)
    //                    {
    //                        ///X轴相同时，Y轴值越小优先级越高
    //                        if (colUnit.pStayMapSlot.vecPos.y < startUnit.pStayMapSlot.vecPos.y)
    //                        {
    //                            bInSteadSlot = true;
    //                        }
    //                    }
    //                    else if (colUnit.pStayMapSlot.vecPos.y == startUnit.pStayMapSlot.vecPos.y)
    //                    {
    //                        ///Y轴相同时，X轴值越小优先级越高
    //                        if (colUnit.pStayMapSlot.vecPos.x < startUnit.pStayMapSlot.vecPos.x)
    //                        {
    //                            bInSteadSlot = true;
    //                        }
    //                    }
    //                }
    //                else if (nPathLength > pathLength || nPathLength > fCheckDis)
    //                {
    //                    bInSteadSlot = true;
    //                }
    //                if (bInSteadSlot)
    //                {
    //                    //nPathLength = pathLength;
    //                    pNearestUnit = colUnit;
    //                    ///在攻击范围中的角色，找最近点进行移动
    //                    if (pathLength == 1000 && fCheckDis >= 1)
    //                    {
    //                        listCheckSlot = AStarFindPath.Ins.GetMapSlotByRange(colUnit.pStayMapSlot.vecPos, startUnit.pUnitData.nAtkRange, EMSkillRangeType.Around);// (int)startUnit.pData.nAtkRange);
    //                        pNearSlot = null;
    //                        float fValue = 999;
    //                        for (int j = 0; j < listCheckSlot.Count; j++)
    //                        {
    //                            ///判断是否当前角色所待的点
    //                            if (listCheckSlot[j].vecPos == startUnit.pStayMapSlot.vecPos)
    //                            {

    //                            }
    //                            else if (!startUnit.CheckMapSlotCanMove(listCheckSlot[j].vecPos))
    //                            {
    //                                continue;
    //                            }
    //                            float fDis = (startUnit.pStayMapSlot.vecPos - listCheckSlot[j].vecPos).sqrMagnitude;
    //                            if (fValue > fDis)
    //                            {
    //                                fValue = fDis;
    //                                pNearSlot = listCheckSlot[j];
    //                            }
    //                        }
    //                        if (pNearSlot != null && pNearSlot.vecPos != startUnit.pStayMapSlot.vecPos)
    //                        {
    //                            pathLength = AStarFindPath.Ins.nGetPathLength(startUnit.pStayMapSlot, pNearSlot);
    //                            if (!colUnit.IsDead() && pathLength >= 0 && nPathLength > pathLength)
    //                            {
    //                                pMoveTarget = pNearSlot;
    //                            }
    //                        }
    //                        //Debug.Log("In This =====" + startUnit.pData.nAtkRange + "====" + pNearestUnit.mapPos + "=====" + startUnit.pMoveTarget.vecPos + "=====" + startUnit.mapPos);
    //                    }
    //                    else
    //                    {
    //                        pMoveTarget = null;
    //                    }

    //                    if (pathLength > fCheckDis)
    //                    {
    //                        nPathLength = fCheckDis;
    //                    }
    //                    else
    //                    {
    //                        nPathLength = pathLength;
    //                    }
    //                }
    //            }
    //        }
    //        //Debug.Log("LastPath==== " + nPathLength);
    //    }
    //    if (pNearestUnit != null &&
    //        pMoveTarget != null)
    //    {
    //        startUnit.MoveToTarget(pMoveTarget);
    //        startUnit.GetPath();
    //    }

    //    return pNearestUnit;
    //}


    #endregion

    /// <summary>
    /// 检测该目标是否还在攻击范围内
    /// </summary>
    /// <param name="vecCheckPos"></param>
    /// <param name="nRange"></param>
    /// <param name="layerMask"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static bool IsUnitInAtkRange(Vector2Int vecCheckPos, int nRange, CPlayerUnit target,bool bAtkFly)
    {
        bool bInRange = false;
        List<MapSlot> listSlots = new List<MapSlot>();
        AStarFindPath.Ins.GetAroundSlot(ref listSlots,nRange, vecCheckPos);
        if (listSlots != null)
        {
            for (int i = 0; i < listSlots.Count; i++)
            {
                if (listSlots[i] == null) continue;
                CPlayerUnit colUnit = listSlots[i].pStayGroundUnit;
                if (colUnit == null)
                {
                    if (bAtkFly)
                    {
                        colUnit = listSlots[i].pStayFlyUnit;
                    }
                }
                if (colUnit != null &&
                    !colUnit.IsDead() &&
                    colUnit == target)
                {
                    bInRange = true;
                    break;
                }
            }
        }
        return bInRange;
    }

    /// <summary>
    /// 检测该目标是否还在攻击范围内(碰撞盒子检测)
    /// </summary>
    /// <param name="vecCheckPos"></param>
    /// <param name="nRange"></param>
    /// <param name="layerMask"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public static bool IsUnitInAtkRange(int nRange, CPlayerUnit target, CPlayerUnit checkUnit)
    {
        //if(f64BaseRange == Fix64.Zero)
        //{
        //    f64BaseRange = (Fix64)0.5f;
        //}

        //if (f64SlotRangeLerp == Fix64.Zero)
        //{
        //    f64SlotRangeLerp = (Fix64)1.2f;
        //}

        if (target == null ||
            checkUnit == null) return false;

        if (target.pStayMapSlot == null ||
            checkUnit.pStayMapSlot == null) return false;

        bool bInRange = false;
        Fix64 f64Radius = f64BaseRange + (Fix64)nRange * f64SlotRangeLerp;
        Fix64 f64RadiusPow = Fix64.Pow2(f64Radius);
        if (target.emStayRange > CPlayerUnit.EMStayRange.Normal)
        {
            MapSlot mapSlot = null;
            target.GetBestMoveTarget(checkUnit.pStayMapSlot.v64SlotPos, ref mapSlot);
            if (FixVector3.SqrMagnitude(checkUnit.pStayMapSlot.v64SlotPos - mapSlot.v64SlotPos) <= f64RadiusPow)
            {
                bInRange = true;
            }
        }
        else if (FixVector3.SqrMagnitude(target.pStayMapSlot.v64SlotPos - checkUnit.pStayMapSlot.v64SlotPos) <= f64RadiusPow)
        {
            bInRange = true;
        }
        //Debug.LogError(checkUnit.pStayMapSlot.vecPos + "===Range==" + target.pStayMapSlot.vecPos + "====" + nRange + "====" + bInRange);
        return bInRange;
    }

}
