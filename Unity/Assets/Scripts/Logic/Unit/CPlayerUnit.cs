using BEPUphysics.BroadPhaseEntries;
using FixMath.NET;
using System.Collections.Generic;
using UnityEngine;



public class CPlayerUnit : CLockUnityObject
{
    /// <summary>
    /// 单位类型
    /// </summary>
    public enum EMUnitType
    {
        Unit,           //单位
        Base,           //基地
        Barracks,       //兵营
        Tower,          //塔
        Build,          //普通建筑
    }
    /// <summary>
    /// 占领范围的类型
    /// </summary>
    public enum EMStayRange
    {
        Normal,         //一人一格(1个格子)
        Around,         //周边范围(6个格子)
        AroundTwo,      //周边范围(18个格子)
        AroundThree,    //周边范围(36个格子)
        AroundFour,     //周边范围(??个格子)
    }

    //状态
    public enum EMState
    {
        None = 0,

        Idle,       //待机
        Move,       //移动
        Attack,     //攻击
        Dead,       //死亡
        Skill,      //攻击
        Show,       //表演
        Jump,       //跳
    }

    public enum EMDeadType
    {
        Normal,
        Jump
    }

    /// <summary>
    /// 移动类型
    /// </summary>
    public enum EMMoveType
    {
        Ground,         //步行
        Fly,            //飞行
    }

    public enum EMAtkRange
    {
        Normal,         //普通
        HengSao,        //横扫
        Line,           //直线攻击
        AtkAround,      //攻击周围
    }

    public EMDeadType emDeadType;

    public int nSoliderIdx;

    public string szUserUid;

    public string szSelfUid;

    public Transform tranFirePos;   //发射点

    public SpriteRenderer pRenderer;

    public EMAtkRange emAtkRange;

    public EMUnitType emUnitType;

    public EMState emCurState;

    public EMUnitAnimeDir emMoveDir;

    public EMStayRange emStayRange;

    public EMMoveType emMoveType;

    /// <summary>
    /// Buff包
    /// </summary>
    CBuffPack buffPack = new CBuffPack();
    /// <summary>
    /// 判断是否为玩家单位
    /// </summary>
    public EMUnitCamp emCamp;
    /// <summary>
    /// 状态机
    /// </summary>
    protected FSMManager pFSM = null;
    /// <summary>
    /// 动画控制器
    /// </summary>
    public CUnitAnimeCtrl pAnimeCtrl;
    /// <summary>
    /// 移动目标
    /// </summary>
    public MapSlot pMoveTarget;
    /// <summary>
    /// 当前停留的格子
    /// </summary>
    public MapSlot pStayMapSlot;
    /// <summary>
    /// 攻击目标 
    /// </summary>
    public CPlayerUnit pAtkTarget;
    /// <summary>
    /// 当前格子上设置的渲染层级
    /// </summary>
    public int nCurSetRenderLayer;
    /// <summary>
    /// 攻击CD用
    /// </summary>
    public bool bAtkAble = false;
    protected CPropertyTimerFix64 pTickerAtkCD = new CPropertyTimerFix64();
    /// <summary>
    /// 技能CD用
    /// </summary>
    public bool bSkillAble = false;
    protected CPropertyTimerFix64 pTickerSkillCD = new CPropertyTimerFix64();
    /// <summary>
    /// 检测敌人用
    /// </summary>
    public float fCheckCD;
    protected Fix64 fCheckCDF64;
    protected CPropertyTimerFix64 pTickerCheck = new CPropertyTimerFix64();

    public CUnitData pUnitData = new CUnitData();

    public DelegateNFuncCall dlgRecycle;                //回收事件

    public CEffectFramePlay[] pChgLayerEffect;

    public CEffectFramePlay pSkillEffect;
    public SpriteRenderer pSkillRender;
    public string szSkillEffect;

    public LayerMask pRedCheckMask;
    public LayerMask pBlueCheckMask;

    public LayerMask pRedHeroCheckMask;
    public LayerMask pBlueHeroCheckMask;

    public bool bCheckTarget;

    #region 路点相关

    List<MapSlot> listStayMapSlots = new List<MapSlot>();
    /// <summary>
    /// 所属的路线类型 
    /// </summary>
    public EMStayPathType emPathType;

    #endregion

    /// <summary>
    /// 出生时进入冲刺状态
    /// </summary>
    public bool bFirstDash;

    [Header("减伤信息")]
    public CMitigationInfo[] cMitigationInfos;
    /// <summary>
    /// 当前减伤信息
    /// </summary>
    protected CMitigationInfo curMitigationInfos;

    //碰撞对象
    public CColBindUnit pColUnit;

    /// <summary>
    /// 仇恨目标
    /// </summary>
    public CPlayerUnit pHeatTarget;

    public bool bYuanCheng;

    [Header("最大检测距离")]
    public int nMaxCheckRange;
    [Header("检测目标点与跳跃点的距离")]
    public int nCheckTargetDis;

    public virtual void Init(EMUnitCamp camp = EMUnitCamp.Blue)
    {
        nRefreshDir = 0;
        bSkillAble = true;
        bAtkAble = true;

        fCheckCDF64 = (Fix64)fCheckCD;

        preDir = EMUnitAnimeDir.Max;
        enabled = true;
        transform.localScale = Vector3.one;
        szSelfUid = CHelpTools.GenerateIdFix64().ToString();
        pMoveTarget = null;
        pAtkTarget = null;
        emCamp = camp;
        buffPack.Init(this);
        bFirstDash = true;
        pUnitData.Init(this);
        if (emCamp == EMUnitCamp.Blue)
        {
            if (pUnitData.emUnitLev == EMUnitLev.Lv5 ||
                pUnitData.emUnitLev == EMUnitLev.Lv4)
            {
                gameObject.layer = LayerMask.NameToLayer("BlueHero");
                pColUnit.gameObject.layer = LayerMask.NameToLayer("BlueHero");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Blue");
                pColUnit.gameObject.layer = LayerMask.NameToLayer("Blue");
            }
        }
        else if (emCamp == EMUnitCamp.Red)
        {
            if (pUnitData.emUnitLev == EMUnitLev.Lv5 ||
                pUnitData.emUnitLev == EMUnitLev.Lv4)
            {
                gameObject.layer = LayerMask.NameToLayer("RedHero");
                pColUnit.gameObject.layer = LayerMask.NameToLayer("RedHero");
            }
            else
            {
                gameObject.layer = LayerMask.NameToLayer("Red");
                pColUnit.gameObject.layer = LayerMask.NameToLayer("Red");
            }
        }
        //初始化时记录一下坐标
        StopEffect();
        RecordLastPos();
        ForceRefreshPos();
       
        ///投矛手红色子弹 特殊处理
        if (pUnitData.nTBLID == 203)
        {
            if (emCamp == EMUnitCamp.Blue)
            {
                if (CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp == EMCamp.Camp1)
                {
                    pUnitData.szBullet = "Bullet002Red";
                }
            }
            else if (emCamp == EMUnitCamp.Red)
            {
                if (CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp == EMCamp.Camp1)
                {
                    pUnitData.szBullet = "Bullet002Red";
                }
            }
        }
        InitFSM();

        if (pColUnit != null)
        {
            pColUnit.Init();
        }

        SetState(CPlayerUnit.EMState.Move);

        RefreshSearch();
    }

    /// <summary>
    /// 刷新阵营加成Buff
    /// </summary>
    public void RefreshCampInfo()
    {
        if (emCamp == EMUnitCamp.Blue)
        {
            pUnitData.InitAddInfo(CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo, CBattleMgr.Ins.mapMgr.pBlueBase.nLev, nSoliderIdx);
        }
        else if (emCamp == EMUnitCamp.Red)
        {
            pUnitData.InitAddInfo(CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo, CBattleMgr.Ins.mapMgr.pRedBase.nLev, nSoliderIdx);
        }
    }

    /// <summary>
    /// 检测场上已经有的buff
    /// </summary>
    void CheckBuffByBattle()
    {
        if (emUnitType != CPlayerUnit.EMUnitType.Unit)
            return;
        Dictionary<int, int> dicBuffInfos = null;
        if (emCamp == EMUnitCamp.Blue)
        {
            dicBuffInfos = CBattleMgr.Ins.mapMgr.pBlueBase.dicBuffAddByLayer;
        }
        else if (emCamp == EMUnitCamp.Red)
        {
            dicBuffInfos = CBattleMgr.Ins.mapMgr.pRedBase.dicBuffAddByLayer;
        }
        foreach (var buffInfo in dicBuffInfos)
        {
            if (buffInfo.Value > 0)
            {
                buffPack.AddBuff(buffInfo.Key, buffInfo.Value);
            }
        }
    }

    /// <summary>
    /// 检测减伤信息
    /// </summary>
    protected void CheckMitigation()
    {
        if (cMitigationInfos == null ||
            cMitigationInfos.Length <= 0) return;
        bool bChgInfo = false;
        float fCurHPRate = (float)pUnitData.nCurHP / (float)pUnitData.nMaxHP;
        for (int i = 0; i < cMitigationInfos.Length; i++)
        {
            if (cMitigationInfos[i] == null) return;
            if (cMitigationInfos[i].fValue > fCurHPRate)
            {
                if (curMitigationInfos == null)
                {
                    bChgInfo = true;
                    curMitigationInfos = cMitigationInfos[i];
                }
                else if (cMitigationInfos[i].fValue < curMitigationInfos.fValue)
                {
                    bChgInfo = true;
                    curMitigationInfos = cMitigationInfos[i];
                }
            }
        }
        if (bChgInfo)
        {
            ///Debug.LogError(curMitigationInfos.fValue + "==Cur Chg Info===" + fCurHPRate);
            curMitigationInfos.StartTick();
        }
    }


    public void PlayEffect(EMUnitAnimeDir dir, int nLayer)
    {
        if (pSkillEffect == null)
            return;
        switch (dir)
        {
            case EMUnitAnimeDir.UpL:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 135));
                }
                break;
            case EMUnitAnimeDir.UpR:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 45));
                }
                break;
            case EMUnitAnimeDir.DownL:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -135));
                }
                break;
            case EMUnitAnimeDir.DownR:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -45));
                }
                break;
            case EMUnitAnimeDir.Left:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -180));
                }
                break;
            case EMUnitAnimeDir.Right:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 0));
                }
                break;
            case EMUnitAnimeDir.Up:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, 90));
                }
                break;
            case EMUnitAnimeDir.Down:
                {
                    pSkillEffect.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
                }
                break;
        }
        pSkillEffect.PlayAnime();
        pSkillRender.sortingOrder = nLayer;
    }

    public void StopEffect()
    {
        if (pSkillEffect == null)
            return;
        pSkillEffect.StopAnime();
    }


    public void BuffChg(bool add, int nBuffID, int num)
    {
        if (add)
        {

            buffPack.AddBuff(nBuffID, num);
        }
        else
        {
            CBuffBase buffBase = buffPack.GetBuffByID(nBuffID);
            if (buffBase != null)
            {
                buffBase.RemoveLayer(num);
            }
        }
    }
    /// <summary>
    /// 获取最近的移动目标
    /// </summary>
    public void GetBestMoveTarget(FixVector3 f64CheckPos, ref MapSlot mapSlot)
    {
        mapSlot = null;
        if (emStayRange == EMStayRange.Normal)
        {
            mapSlot = pStayMapSlot;
        }
        else
        {
            List<MapSlot> listSlot = null;
            AStarFindPath.Ins.GetOutAroundSlot((int)emStayRange, pStayMapSlot.vecPos, ref listSlot);
            Fix64 f64Dis = Fix64.Zero;
            Fix64 f64CurDis = (Fix64)99999;
            for (int i = 0; i < listSlot.Count; i++)
            {
                f64Dis = FixVector3.SqrMagnitude(listSlot[i].v64SlotPos - f64CheckPos);
                //nDis = (listSlot[i].vecPos - vPos).sqrMagnitude;
                if (f64CurDis > f64Dis)
                {
                    f64CurDis = f64Dis;
                    mapSlot = listSlot[i];
                }
            }
        }
    }

    /// <summary>
    /// 判断周围是否还有可移动的点位
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    public bool CheckCanMovePos(CPlayerUnit unit)
    {
        bool bHavePos = false;

        if (emStayRange == EMStayRange.Normal)
        {
            
        }
        else
        {
            List<MapSlot> listSlot = null;
            AStarFindPath.Ins.GetOutAroundSlot(((int)emStayRange + 1 + (int)unit.emStayRange), pStayMapSlot.vecPos, ref listSlot);
            for (int i = 0; i < listSlot.Count; i++)
            {
                if(listSlot[i].bCanMove(unit))
                {
                    bHavePos = true;
                    break;
                }
            }
        }

        return bHavePos;
    }

    public MapSlot vHitTargetSlot;
    /// <summary>
    /// 获取最近的移动目标
    /// </summary>
    public MapSlot GetHitTarget()
    {
        return vHitTargetSlot;
    }

    /// <summary>
    /// 移向目标点
    /// </summary>
    /// <param name="mapSlot"></param>
    public void MoveToTarget(MapSlot mapSlot)
    {
        pMoveTarget = mapSlot;
        if (emCurState != EMState.Move)
        {
            SetState(EMState.Move);
        }
        else
        {

        }
    }


    /// <summary>
    /// 是否可以攻击
    /// </summary>
    /// <returns></returns>
    public bool IsAtkAble()
    {
        //重置攻击对象
        if (pAtkTarget != null &&
           pAtkTarget.szUserUid.Equals(szUserUid))
        {
            pAtkTarget = null;
            RefreshSearch();
            return false;
        }

        return bAtkAble;
    }

    /// <summary>
    /// 刷新攻击CD
    /// </summary>
    public void RefreshAtkCD()
    {
        if (pUnitData.nAtkCD <= 0) return;
        bAtkAble = false;

        if (pTickerAtkCD.Value <= Fix64.Zero)
        {
            pTickerAtkCD.Value = pUnitData.AtkCD;
        }

        pTickerAtkCD.FillTime();
    }

    /// <summary>
    /// 是否可以放技能
    /// </summary>
    /// <returns></returns>
    public bool IsSkillAble()
    {
        //重置攻击对象
        if (pAtkTarget != null &&
           pAtkTarget.szUserUid.Equals(szUserUid))
        {
            pAtkTarget = null;
            RefreshSearch();
            return false;
        }
        if (pUnitData.nSkillID <= 0)
        {
            return false;
        }
        return bSkillAble;
    }


    /// <summary>
    /// 刷新技能CD
    /// </summary>
    public void RefreshSkillCD()
    {
        if (pUnitData.pSkillInfo == null ||
            pUnitData.pSkillInfo.nSkillCD <= 0) return;

        bSkillAble = false;
        if (pTickerSkillCD.Value <= Fix64.Zero)
        {
            pTickerSkillCD.Value = pUnitData.pSkillInfo.f64SkillCD;
        }
        pTickerSkillCD.FillTime();
    }

    public void RefreshSearch()
    {
        if (pAtkTarget != null &&
            !pAtkTarget.IsDead())
            return;

       
        if (pUnitData.nTBLID == 102 ||
            pUnitData.nTBLID == 203 ||
            pUnitData.nTBLID == 204 ||
            pUnitData.nTBLID == 301 ||
            pUnitData.nTBLID == 303 ||
            pUnitData.nTBLID == 1001 ||
            pUnitData.nTBLID == 1003)
        {
            if(GetAtkTargetByAtkRange())
            {
                return;
            }
        }

        bCheckTarget = true;
        if (pTickerCheck.Value <= Fix64.Zero)
        {
            pTickerCheck.Value = fCheckCDF64;
        }
        pTickerCheck.FillTime();
    }


    public virtual void SetRenderLayer(int nLayer)
    {
        nCurSetRenderLayer = nLayer;
        if (pRenderer != null)
            pRenderer.sortingOrder = nCurSetRenderLayer;
        if (pChgLayerEffect != null)
        {
            for (int i = 0; i < pChgLayerEffect.Length; i++)
            {
                pChgLayerEffect[i].SetFrameLayer(nCurSetRenderLayer - i - 2);
            }

        }
    }

    Vector3 vRecyclePos = new Vector3(10000f, 10000f, 10000f);
    public void SetMapSlot(MapSlot slot)
    {
        if (slot == null)
        {
            pColUnit.tranSelf.position = vRecyclePos;
            return;
        }

        if (pStayMapSlot != null)
        {
            List<MapSlot> listSlotPre = GetSelfSlot();
            for (int i = 0; i < listSlotPre.Count; i++)
            {
                if (emMoveType == EMMoveType.Ground)
                {
                    if (listSlotPre[i].pStayGroundUnit != null &&
                        listSlotPre[i].pStayGroundUnit.szSelfUid != szSelfUid) continue;
                    listSlotPre[i].pStayGroundUnit = null;
                }
                else if (emMoveType == EMMoveType.Fly)
                {
                    if (listSlotPre[i].pStayFlyUnit != null &&
                        listSlotPre[i].pStayFlyUnit.szSelfUid != szSelfUid) continue;
                    listSlotPre[i].pStayFlyUnit = null;
                }
                listSlotPre[i].OnExit();
            }
        }

        pStayMapSlot = slot;

        if (pStayMapSlot != null)
        {
            pColUnit.SetPos(pStayMapSlot);
        }

        List<MapSlot> listSlotCur = GetSelfSlot();
        for (int i = 0; i < listSlotCur.Count; i++)
        {
            if (listSlotCur[i] == null) continue;
            if (emMoveType == EMMoveType.Ground)
            {
                listSlotCur[i].pStayGroundUnit = this;
            }
            else if (emMoveType == EMMoveType.Fly)
            {
                listSlotCur[i].pStayFlyUnit = this;
            }
            listSlotCur[i].OnEnter();
        }
    }

    protected virtual void InitFSM()
    {
        pFSM = new FSMManager(this);
        pFSM.AddState((int)EMState.Idle, new FSMUnitRole_Idle());
        pFSM.AddState((int)EMState.Move, new FSMUnitRole_Move());
        pFSM.AddState((int)EMState.Attack, new FSMUnitRole_Atk());
        pFSM.AddState((int)EMState.Dead, new FSMUnitRole_Dead());
        pFSM.AddState((int)EMState.Skill, new FSMUnitRole_Skill());
        pFSM.AddState((int)EMState.Jump, new FSMUnitRole_Jump());
    }

    public virtual void PlayAnima(EMUnitAnimeState state, EMUnitAnimeDir dir, bool force = false)
    {
        if (pAnimeCtrl == null) return;
        pAnimeCtrl.PlayAnime(state, dir, force);
    }

    /// <summary>
    /// 设置状态机状态
    /// </summary>
    /// <param name="state"></param>
    /// <param name="msg"></param>
    public virtual void SetState(EMState state, CLocalNetMsg msg = null,bool bJumpChg =false)
    {
        if (pFSM == null) return;
        if(emCurState == EMState.Jump && !bJumpChg)
        {
            if(state ==  EMState.Dead)
            {

            }
            else
            {
                return;
            }
        }
        pFSM.ChangeMainState((int)state, msg);
    }

    /// <summary>
    /// 获取当前的状态
    /// </summary>
    /// <returns></returns>
    public virtual FSMBaseState GetState()
    {
        if (pFSM == null) return null;

        return pFSM.GetCurState();
    }

    /// <summary>
    /// 受到攻击
    /// </summary>
    /// <param name="szuid"></param>
    /// <param name="nDmg"></param>
    public virtual void OnHit(CPlayerUnit atkunit, int nDmg, MapSlot pHitSlot = null)
    {
        ///技能状态不挨打
        if (emCurState == EMState.Skill) return;
        ///自己不能打自己
        if (atkunit.szUserUid == szUserUid) return;

        if (IsDead()) return;

        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  "
        //           + $"攻击者索引【{atkunit.nUniqueIdx}】"
        //           + $"受击伤害【{nDmg}】"
        //           + $"受击者位置【{pStayMapSlot.vecPos.x},{pStayMapSlot.vecPos.y}】"
        //           + $"攻击者伤害【{atkunit.pStayMapSlot.vecPos.x},{atkunit.pStayMapSlot.vecPos.y}】"
        //           + $"攻击目标索引【{nUniqueIdx}】【受击事件】  ");

        if (emUnitType == EMUnitType.Unit &&
            pAtkTarget != null &&
            pAtkTarget.emUnitType != EMUnitType.Unit)
        {
            try
            {
                if (CFindTargetHelp.IsUnitInAtkRange(pUnitData.nAtkRange, atkunit, this))
                {
                    pAtkTarget = atkunit;
                }
            }
            catch (System.Exception e)
            {
                //Debug.LogError("Atk Hate Find None!");
                Debug.LogError(e.Message);
            }
        }
        //Debug.LogError(pUnitData.nCurHP + "==OnHit==" + nDmg + "==UnitID==" + pUnitData.nTBLID + "===OnlyID===" + szSelfUid);
        //if (pHeatTarget != null)
        //{
        //    pHeatTarget = atkunit;
        //    pAtkTarget = atkunit;
        //}
        pUnitData.nCurHP -= nDmg;
        if (pUnitData.nCurHP <= 0)
        {
            pUnitData.nCurHP = 0;
            SetState(EMState.Dead);
            ///发送单位被击杀的监听事件
            if(atkunit != null)
            {
                CLocalNetMsg msg = new CLocalNetMsg();
                msg.SetInt("camp", (int)emCamp);
                msg.SetInt("pathtype", (int)emPathType);
                msg.SetInt("unittype", (int)emUnitType);
                msg.SetLong("gold", pUnitData.nlGold);
                msg.SetInt("buffid", pUnitData.nBuffID);
                msg.SetInt("atkunittype", (int)atkunit.emUnitType);
                msg.SetString("atkuid", atkunit.szUserUid);
                msg.SetString("atkunituid", atkunit.szSelfUid);
                msg.SetString("unitname", pUnitData.szName);
                msg.SetString("szprefabname", atkunit.pUnitData.szPrefabName);
                msg.SetInt("unitlev", (int)pUnitData.emUnitLev);
                CGameObserverMgr.SendMsg(CGameObserverConst.UnitDead, msg);
            }
        }
    }

    /// <summary>
    /// 受到治疗
    /// </summary>
    /// <param name="szuid"></param>
    /// <param name="nCureValue"></param>
    public virtual void OnCure(string szuid, int nCureValue)
    {
        pUnitData.nCurHP += nCureValue;
        if (pUnitData.nCurHP > pUnitData.MaxHP)
        {
            pUnitData.nCurHP = pUnitData.MaxHP;
        }
    }

    /// <summary>
    /// 跳向的目标点
    /// </summary>
    public Vector2Int vJumpTarget;
    public MapSlot vAtkTarget;
    public EMUnitAnimeDir curAtkAnimeDir;


    /// <summary>
    /// 释放技能
    /// </summary>
    public void DoSkill()
    {
        if (pUnitData.pSkillInfo.nID == 1)
        {
            List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
            List<MapSlot> slots = new List<MapSlot>();
            AStarFindPath.Ins.GetAroundSlot(ref slots, pUnitData.pSkillInfo.nSkillRange, vJumpTarget);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                if (slots[i].pStayGroundUnit != null &&
                   !slots[i].pStayGroundUnit.IsDead() &&
                   slots[i].pStayGroundUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayGroundUnit))
                {
                    listTarget.Add(slots[i].pStayGroundUnit);
                }
                if (pUnitData.bCanAtkFly &&
                   slots[i].pStayFlyUnit != null &&
                   !slots[i].pStayFlyUnit.IsDead() &&
                   slots[i].pStayFlyUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayFlyUnit))
                {
                    listTarget.Add(slots[i].pStayFlyUnit);
                }
                //slots[i].ActiveRenderColor(true);
            }
            for (int i = 0; i < listTarget.Count; i++)
            {
                if (listTarget[i] == null) continue;
                listTarget[i].OnHit(this, (pUnitData.pSkillInfo.nSkillDmg + (listTarget[i].emUnitType == EMUnitType.Unit ? 0 : pUnitData.pSkillInfo.nAtkBuildDmg)), pStayMapSlot);
            }
        }
        else if (pUnitData.pSkillInfo.nID == 2)
        {
            List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
            List<MapSlot> slots = new List<MapSlot>();
            AStarFindPath.Ins.GetAroundSlot(ref slots, pUnitData.pSkillInfo.nSkillRange, vAtkTarget.vecPos);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                if (slots[i].pStayGroundUnit != null &&
                   !slots[i].pStayGroundUnit.IsDead() &&
                   slots[i].pStayGroundUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayGroundUnit))
                {
                    listTarget.Add(slots[i].pStayGroundUnit);
                }
                if (pUnitData.bCanAtkFly &&
                   slots[i].pStayFlyUnit != null &&
                   !slots[i].pStayFlyUnit.IsDead() &&
                   slots[i].pStayFlyUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayFlyUnit))
                {
                    listTarget.Add(slots[i].pStayFlyUnit);
                }
                //slots[i].ActiveRenderColor(true);
            }
            for (int i = 0; i < listTarget.Count; i++)
            {
                if (listTarget[i] == null) continue;
                listTarget[i].OnHit(this, (pUnitData.pSkillInfo.nSkillDmg + (listTarget[i].emUnitType == EMUnitType.Unit ? 0 : pUnitData.pSkillInfo.nAtkBuildDmg)), pStayMapSlot);
            }
        }
        else if (pUnitData.pSkillInfo.nID == 3)
        {
            //获取攻击目标
            List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
            List<MapSlot> slots = new List<MapSlot>();
            AStarFindPath.Ins.GetLineSlotsByDir(ref slots, pUnitData.pSkillInfo.nSkillRange, vAtkTarget.vecPos, curAtkAnimeDir, true);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                if (slots[i].pStayGroundUnit != null &&
                   !slots[i].pStayGroundUnit.IsDead() &&
                   slots[i].pStayGroundUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayGroundUnit))
                {
                    listTarget.Add(slots[i].pStayGroundUnit);
                }
                else if (pUnitData.bCanAtkFly &&
                   slots[i].pStayFlyUnit != null &&
                   !slots[i].pStayFlyUnit.IsDead() &&
                   slots[i].pStayFlyUnit.emCamp != emCamp &&
                   !listTarget.Contains(slots[i].pStayFlyUnit))
                {
                    listTarget.Add(slots[i].pStayFlyUnit);
                }
                //slots[i].ActiveRenderColor(true);
            }
            for (int i = 0; i < listTarget.Count; i++)
            {
                if (listTarget[i] == null) continue;
                listTarget[i].OnHit(this, (pUnitData.pSkillInfo.nSkillDmg + (listTarget[i].emUnitType == EMUnitType.Unit ? 0 : pUnitData.pSkillInfo.nAtkBuildDmg)), pStayMapSlot);
            }
        }
        else if (pUnitData.pSkillInfo.nID == 4)
        {
            ///对自己周围的盟友进行治疗
            List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
            List<MapSlot> slots = new List<MapSlot>();
            AStarFindPath.Ins.GetAroundSlot(ref slots, pUnitData.nDmgRange, pStayMapSlot.vecPos);
            for (int i = 0; i < slots.Count; i++)
            {
                if (slots[i] == null) continue;
                if (slots[i].pStayGroundUnit != null &&
                   !slots[i].pStayGroundUnit.IsDead() &&
                   slots[i].pStayGroundUnit.emCamp == emCamp &&
                   !listTarget.Contains(slots[i].pStayGroundUnit))
                {
                    listTarget.Add(slots[i].pStayGroundUnit);
                }
                if (pUnitData.bCanAtkFly &&
                   slots[i].pStayFlyUnit != null &&
                   !slots[i].pStayFlyUnit.IsDead() &&
                   slots[i].pStayFlyUnit.emCamp == emCamp &&
                   !listTarget.Contains(slots[i].pStayFlyUnit))
                {
                    listTarget.Add(slots[i].pStayFlyUnit);
                }
            }
            for (int i = 0; i < listTarget.Count; i++)
            {
                if (listTarget[i] == null) continue;
                //治疗
                listTarget[i].OnCure(szUserUid, pUnitData.pSkillInfo.nSkillDmg);
            }
        }
    }

    public void DoSkillEffect(Vector3 vPos = default(Vector3), DelegateGOFuncCall callSuc = null)
    {
        if (pUnitData.pSkillInfo == null ||
            CHelpTools.IsStringEmptyOrNone(pUnitData.pSkillInfo.szSkillEffect)) return;
        CEffectMgr.Instance.CreateEffSync(pUnitData.pSkillInfo.szSkillEffect, vPos, Quaternion.identity, 0, callSuc);
    }

    /// <summary>
    /// 攻击目标
    /// </summary>
    public void AtkTarget()
    {
        if (pAtkTarget == null ||
            pAtkTarget.IsDead())
        {
            return;
        }
        if (emAtkRange == EMAtkRange.Normal)
        {
            ///判断是否需要发射子弹
            if (string.IsNullOrEmpty(pUnitData.szBullet))
            {
                ///判断伤害范围是否＞1
                if (pUnitData.nDmgRange > 1)
                {
                    List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
                    List<MapSlot> slots = new List<MapSlot>();
                    AStarFindPath.Ins.GetAroundSlot(ref slots, pUnitData.nDmgRange - 1, pStayMapSlot.vecPos);
                    slots.Add(pAtkTarget.pStayMapSlot);
                    for (int i = 0; i < slots.Count; i++)
                    {
                        if (slots[i] == null) continue;
                        if (slots[i].pStayGroundUnit != null &&
                           !slots[i].pStayGroundUnit.IsDead() &&
                            slots[i].pStayGroundUnit.emCamp != emCamp &&
                            !listTarget.Contains(slots[i].pStayGroundUnit))
                        {
                            listTarget.Add(slots[i].pStayGroundUnit);
                        }
                        if (pUnitData.bCanAtkFly &&
                           slots[i].pStayFlyUnit != null &&
                           !slots[i].pStayFlyUnit.IsDead() &&
                            slots[i].pStayFlyUnit.emCamp != emCamp &&
                            !listTarget.Contains(slots[i].pStayFlyUnit))
                        {
                            listTarget.Add(slots[i].pStayFlyUnit);
                        }
                    }
                    for (int i = 0; i < listTarget.Count; i++)
                    {
                        if (listTarget[i] == null) continue;
                        if (listTarget[i].emUnitType == EMUnitType.Unit)
                        {
                            listTarget[i].OnHit(this, pUnitData.AtkDmg, pStayMapSlot);
                        }
                        else
                        {
                            listTarget[i].OnHit(this, pUnitData.nAtkBuildDmg, pStayMapSlot);
                        }
                    }
                }
                else
                {
                    if (pAtkTarget.emUnitType == EMUnitType.Unit)
                    {
                        pAtkTarget.OnHit(this, pUnitData.AtkDmg, pStayMapSlot);
                    }
                    else
                    {
                        pAtkTarget.OnHit(this, pUnitData.nAtkBuildDmg, pStayMapSlot);
                    }
                }
            }
            else
            {
                CreatBullet();
            }
        }
        else if (emAtkRange == EMAtkRange.HengSao)
        {
            AtkHengSao();
        }
        else if (emAtkRange == EMAtkRange.Line)
        {
            AtkLine();
        }
        else if (emAtkRange == EMAtkRange.AtkAround)
        {
            AtkAround();
        }
    }

    public Vector3 vBulletLerp = new Vector3(0, 0.54f, 0f);

    /// <summary>
    /// 生成子弹
    /// </summary>
    protected virtual void CreatBullet()
    {
        CBulletBeizierUnit bulletUnit = CBulletMgr.Ins.PopBullet(pUnitData.szBullet);
        if (bulletUnit == null)
        {
            GameObject objNewPlayer = GameObject.Instantiate(Resources.Load("Bullet/" + pUnitData.szBullet) as GameObject);
            Transform tranNewPlayer = objNewPlayer.GetComponent<Transform>();
            bulletUnit = objNewPlayer.GetComponent<CBulletBeizierUnit>();
            bulletUnit.InitUniqueIdx();
        }

        FixVector3 startPos = new FixVector3((Fix64)(tranFirePos.position.x + vBulletLerp.x), (Fix64)(tranFirePos.position.y + vBulletLerp.y), (Fix64)(tranFirePos.position.z + vBulletLerp.z));

        bulletUnit.Init(startPos, pAtkTarget.pStayMapSlot.v64SlotPos, pAtkTarget, this);
        CBulletMgr.Ins.AddBulletUnit(bulletUnit);
        CLockStepMgr.Ins.AddLockBullet(bulletUnit);

    }

    /// <summary>
    /// 横扫攻击
    /// </summary>
    void AtkHengSao()
    {
        List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
        List<MapSlot> slots = new List<MapSlot>();
        AStarFindPath.Ins.GetHengSaoSlotsByDir(ref slots, pAtkTarget.pStayMapSlot.vecPos, emMoveDir);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].pStayGroundUnit != null &&
               !slots[i].pStayGroundUnit.IsDead() &&
               slots[i].pStayGroundUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayGroundUnit))
            {
                listTarget.Add(slots[i].pStayGroundUnit);
            }
            if (pUnitData.bCanAtkFly &&
               slots[i].pStayFlyUnit != null &&
               !slots[i].pStayFlyUnit.IsDead() &&
               slots[i].pStayFlyUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayFlyUnit))
            {
                listTarget.Add(slots[i].pStayFlyUnit);
            }
            //slots[i].ActiveRenderColor(true);
        }
        for (int i = 0; i < listTarget.Count; i++)
        {
            if (listTarget[i] == null) continue;
            if (listTarget[i].emUnitType == EMUnitType.Unit)
            {
                listTarget[i].OnHit(this, pUnitData.AtkDmg, pStayMapSlot);
            }
            else
            {
                listTarget[i].OnHit(this, pUnitData.nAtkBuildDmg, pStayMapSlot);
            }
        }
    }

    /// <summary>
    /// 直线攻击
    /// </summary>
    void AtkLine()
    {
        List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
        List<MapSlot> slots = new List<MapSlot>();
        AStarFindPath.Ins.GetLineSlotsByDir(ref slots, pUnitData.nDmgRange, pAtkTarget.pStayMapSlot.vecPos, emMoveDir);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].pStayGroundUnit != null &&
               !slots[i].pStayGroundUnit.IsDead() &&
               slots[i].pStayGroundUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayGroundUnit))
            {
                listTarget.Add(slots[i].pStayGroundUnit);
            }
            if (pUnitData.bCanAtkFly &&
               slots[i].pStayFlyUnit != null &&
               !slots[i].pStayFlyUnit.IsDead() &&
               slots[i].pStayFlyUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayFlyUnit))
            {
                listTarget.Add(slots[i].pStayFlyUnit);
            }
            //slots[i].ActiveRenderColor(true);
        }
        for (int i = 0; i < listTarget.Count; i++)
        {
            if (listTarget[i] == null) continue;
            if (listTarget[i].emUnitType == EMUnitType.Unit)
            {
                listTarget[i].OnHit(this, pUnitData.AtkDmg, pStayMapSlot);
            }
            else
            {
                listTarget[i].OnHit(this, pUnitData.nAtkBuildDmg, pStayMapSlot);
            }
        }
    }

    /// <summary>
    /// 攻击周围
    /// </summary>
    void AtkAround()
    {
        List<CPlayerUnit> listTarget = new List<CPlayerUnit>();
        List<MapSlot> slots = new List<MapSlot>();
        AStarFindPath.Ins.GetAroundSlot(ref slots, pUnitData.nDmgRange, pAtkTarget.pStayMapSlot.vecPos);
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null) continue;
            if (slots[i].pStayGroundUnit != null &&
               !slots[i].pStayGroundUnit.IsDead() &&
               slots[i].pStayGroundUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayGroundUnit))
            {
                listTarget.Add(slots[i].pStayGroundUnit);
            }
            if (pUnitData.bCanAtkFly &&
               slots[i].pStayFlyUnit != null &&
               !slots[i].pStayFlyUnit.IsDead() &&
               slots[i].pStayFlyUnit.emCamp != emCamp &&
               !listTarget.Contains(slots[i].pStayFlyUnit))
            {
                listTarget.Add(slots[i].pStayFlyUnit);
            }
        }
        for (int i = 0; i < listTarget.Count; i++)
        {
            if (listTarget[i] == null) continue;
            if (listTarget[i].emUnitType == EMUnitType.Unit)
            {
                if (emUnitType == CPlayerUnit.EMUnitType.Unit)
                {
                    listTarget[i].OnHit(this, pUnitData.AtkDmg, pStayMapSlot);
                }
                else
                {
                    if (emUnitType == CPlayerUnit.EMUnitType.Tower)
                    {
                        listTarget[i].OnHit(this, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetTowerDmg(listTarget[i].pUnitData.nTBLID), pStayMapSlot);
                    }
                    else if (emUnitType == CPlayerUnit.EMUnitType.Base)
                    {
                        listTarget[i].OnHit(this, CTBLHandlerModeValue.Ins.GetInfo(CGameAntGlobalMgr.Ins.nHPLev + 1).GetBaseDmg(listTarget[i].pUnitData.nTBLID), pStayMapSlot);
                    }
                }
            }
            else
            {
                listTarget[i].OnHit(this, pUnitData.nAtkBuildDmg, pStayMapSlot);
            }
        }
    }

    Collider2D[] arrCheckCol = new Collider2D[128];
    List<BroadPhaseEntry> listCheckEntry = new List<BroadPhaseEntry>();

    /// <summary>
    /// 获取警戒范围内的攻击目标
    /// </summary>
    public void GetAtkTargetByAlertRange()
    {
        if (pStayMapSlot == null) return;

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (pUnitData.emUnitLev == EMUnitLev.Lv5)
            {
                CFindTargetHelp.SearchTargetInRange(pUnitData.nAlertRange, EMSkillSearchCamp.Enemy, ref pAtkTarget, this, emCamp == EMUnitCamp.Blue ? pBlueHeroCheckMask : pRedHeroCheckMask, arrCheckCol);
                if (pAtkTarget == null)
                {
                    CFindTargetHelp.SearchTargetInRange(pUnitData.nAlertRange, EMSkillSearchCamp.Enemy, ref pAtkTarget, this, emCamp == EMUnitCamp.Blue ? pBlueCheckMask : pRedCheckMask, arrCheckCol);
                }
            }
            else
            {
                CFindTargetHelp.SearchTargetInRange(pUnitData.nAlertRange, EMSkillSearchCamp.Enemy, ref pAtkTarget, this, emCamp == EMUnitCamp.Blue ? pBlueCheckMask : pRedCheckMask, arrCheckCol);
            }
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            CFindTargetHelp.SearchTargetInRangeBySlot(pUnitData.nAlertRange, pStayMapSlot.vecPos, EMSkillSearchCamp.Enemy, ref pAtkTarget, this,false);
        }

        
    }

    Collider2D[] arrCheckCol2 = new Collider2D[64];
    List<BroadPhaseEntry> listCheckEntry2 = new List<BroadPhaseEntry>();

    /// <summary>
    /// 获取攻击范围内的攻击目标
    /// </summary>
    public bool GetAtkTargetByAtkRange()
    {
        bool bGetTarget = false;
        if (pStayMapSlot == null) return bGetTarget;
        CPlayerUnit pGetTarget = null;

        if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            if (pUnitData.emUnitLev == EMUnitLev.Lv5)
            {
                CFindTargetHelp.SearchTargetInRange(pUnitData.nAtkRange, EMSkillSearchCamp.Enemy, ref pGetTarget, this, emCamp == EMUnitCamp.Blue ? pBlueHeroCheckMask : pRedHeroCheckMask, arrCheckCol2);
                if (pAtkTarget == null)
                {
                    CFindTargetHelp.SearchTargetInRange(pUnitData.nAtkRange, EMSkillSearchCamp.Enemy, ref pGetTarget, this, emCamp == EMUnitCamp.Blue ? pBlueCheckMask : pRedCheckMask, arrCheckCol2);
                }
            }
            else
            {
                CFindTargetHelp.SearchTargetInRange(pUnitData.nAtkRange, EMSkillSearchCamp.Enemy, ref pGetTarget, this, emCamp == EMUnitCamp.Blue ? pBlueCheckMask : pRedCheckMask, arrCheckCol2);
            }
        }
        else if (CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP)
        {
            CFindTargetHelp.SearchTargetInRangeBySlot(pUnitData.nAtkRange, pStayMapSlot.vecPos, EMSkillSearchCamp.Enemy, ref pGetTarget, this, true);
        }

        if (pGetTarget != null)
        {
            //Debug.LogError(pGetTarget.pStayMapSlot.vecPos + "====" + pUnitData.nAtkRange + "=====" + pStayMapSlot.vecPos);
            bGetTarget = true;
            pAtkTarget = pGetTarget;
        }
        
        return bGetTarget;

    }

    public EMUnitAnimeDir preDir;
    int nRefreshDir = 0;
    /// <summary>
    /// 获取横向移动目标
    /// </summary>
    /// <returns></returns>
    public MapSlot GetDirMoveTarget()
    {
        EMUnitAnimeDir dir = EMUnitAnimeDir.Down;
        if (pStayMapSlot != null &&
            pMoveTarget != null &&
            pStayMapSlot.vecPos == pMoveTarget.vecPos)
        {
            pMoveTarget = null;
        }
        if (pMoveTarget != null)
        {
            if (pAtkTarget == null)
            {
                pMoveTarget = null;
            }
            else
            {
                if (pAtkTarget.IsDead())
                {
                    pAtkTarget = null;
                    pMoveTarget = null;
                }
                else
                {
                    pAtkTarget.GetBestMoveTarget(pStayMapSlot.v64SlotPos, ref pMoveTarget);
                }
                //if (pMoveTarget.pStayGroundUnit != pAtkTarget)
                //{
                //    if (pUnitData.bCanAtkFly &&
                //        pMoveTarget.pStayFlyUnit != pAtkTarget)
                //    {
                //        pMoveTarget = null;
                //        pAtkTarget.GetBestMoveTarget(pStayMapSlot.v64SlotPos,ref pMoveTarget);
                //    }
                //    else
                //    {
                //        pMoveTarget = null;
                //    }
                //}
            }
        }
        
        //EMUnitAnimeDir emGetDir = preDir;
        MapSlot mapSlot = null;
        //if (pMoveTarget == null)
        //{
        //    AStarFindPath.Ins.pGetNextMovePosByDir(ref mapSlot, ref preDir,ref nCheckCount, pStayMapSlot, this, dir, null);
        //}
        //else
        //{
        if (pMoveTarget != null)
        {
            AStarFindPath.Ins.pGetNextMovePosByDir(ref mapSlot, ref preDir, pStayMapSlot, this, dir, pMoveTarget.pStayGroundUnit);
        }
        //}
        //nRefreshDir++;
        //if(preDir == EMUnitAnimeDir.Max ||
        //   nRefreshDir >=3)
        //{
        //    nRefreshDir = 0;
        //    preDir = emGetDir;
        //}
        return mapSlot;
    }

    public MapSlot pJumpTarget;
    List<MapSlot> listCheckJumpSlots = new List<MapSlot>();
    MapSlot pNextMove;
    
    /// <summary>
    /// 判断该格子是否可以走
    /// </summary>
    /// <param name="vecPos"></param>
    /// <returns></returns>
    public bool CheckMapSlotCanMove(Vector2Int vecPos)
    {
        bool bCanMove = true;

        MapSlot pGoSlot = null;
        AStarFindPath.Ins.GetMapSlot(ref pGoSlot, vecPos);
        if (pGoSlot != null)
        {
            if (emMoveType == EMMoveType.Ground &&
                pGoSlot.pStayGroundUnit != null &&
                pGoSlot.pStayGroundUnit.szSelfUid != szSelfUid)
            {
                bCanMove = false;
            }
            else if (emMoveType == EMMoveType.Fly)
            {
                if (pGoSlot.pStayGroundUnit != null &&
                    pGoSlot.pStayGroundUnit.szSelfUid != szSelfUid &&
                    pGoSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                {
                    bCanMove = false;
                }
                if (pGoSlot.pStayFlyUnit != null &&
                   pGoSlot.pStayFlyUnit.szSelfUid != szSelfUid)
                {
                    bCanMove = false;
                }
            }
        }
        if (bCanMove)
        {
            if (pStayMapSlot != null &&
                vecPos == pStayMapSlot.vecPos)
            {
                bCanMove = false;
            }
            else
            {

                if (emStayRange == CPlayerUnit.EMStayRange.Around &&
                    !AStarFindPath.Ins.bGetAllAroundSlot(vecPos, 1, this))
                {
                    bCanMove = false;
                }
                else if (emStayRange == CPlayerUnit.EMStayRange.AroundTwo &&
                    !AStarFindPath.Ins.bGetAllAroundSlot(vecPos, 2, this))
                {
                    bCanMove = false;
                }
                else if (emStayRange == CPlayerUnit.EMStayRange.AroundThree &&
                    !AStarFindPath.Ins.bGetAllAroundSlot(vecPos, 3, this))
                {
                    bCanMove = false;
                }
                else if (emStayRange == CPlayerUnit.EMStayRange.AroundFour &&
                   !AStarFindPath.Ins.bGetAllAroundSlot(vecPos, 4, this))
                {
                    bCanMove = false;
                }
            }
        }
        return bCanMove;
    }

    /// <summary>
    /// 获取自己占领的所有格子
    /// </summary>
    /// <returns></returns>
    public List<MapSlot> GetSelfSlot()
    {
        if (listStayMapSlots == null)
        {
            listStayMapSlots = new List<MapSlot>();
        }
        listStayMapSlots.Clear();
        if (pStayMapSlot != null)
        {
            switch (emStayRange)
            {
                case EMStayRange.Normal:            //单格
                    {

                    }
                    break;
                case EMStayRange.Around:            //周围
                    {
                        AStarFindPath.Ins.GetMapSlotByRange(ref listStayMapSlots, pStayMapSlot.vecPos, 1, EMSkillRangeType.Around);
                    }
                    break;
                case EMStayRange.AroundTwo:            //周围
                    {
                        AStarFindPath.Ins.GetMapSlotByRange(ref listStayMapSlots, pStayMapSlot.vecPos, 2, EMSkillRangeType.Around);
                    }
                    break;
                case EMStayRange.AroundThree:            //周围
                    {
                        AStarFindPath.Ins.GetMapSlotByRange(ref listStayMapSlots, pStayMapSlot.vecPos, 3, EMSkillRangeType.Around);
                    }
                    break;
                case EMStayRange.AroundFour:            //周围
                    {
                        AStarFindPath.Ins.GetMapSlotByRange(ref listStayMapSlots, pStayMapSlot.vecPos, 4, EMSkillRangeType.Around);
                    }
                    break;
            }
        }
        listStayMapSlots.Add(pStayMapSlot);
        return listStayMapSlots;
    }

    public virtual bool IsDead()
    {
        bool bDead = (emCurState == EMState.Dead);

        return bDead;
    }

    public override void OnUpdateLogic()
    {
        //Debug.Log($"帧:{CLockStepData.g_uGameLogicFrame}  " +
        //    $"索引【{nUniqueIdx}】【{emCurState.ToString()}】  " +
        //    $"M【{pStayMapSlot.vecPos.x} : {pStayMapSlot.vecPos.y}】 " +
        //    $"A【{((pAtkTarget == null)? -1 : pAtkTarget.nUniqueIdx)}】 " +
        //    $"检时:{pTickerCheck.CurValue}  攻击CD:{pTickerAtkCD.CurValue}\r\n" +
        //    $"目标状态【HP：{((pAtkTarget == null) ? 0 : pAtkTarget.pUnitData.nCurHP)}】" +
        //    $"【MAXHP：{((pAtkTarget == null) ? 0 : pAtkTarget.pUnitData.MaxHP)}】" +
        //    $"【M：{((pAtkTarget == null) ? "" : (pStayMapSlot.vecPos.x + ":" + pStayMapSlot.vecPos.y))}】" +
        //    $"【ATK：{((pAtkTarget == null) ? 0 : pAtkTarget.pUnitData.AtkDmg)}】" + 
        //    emMoveDir);

        RecordLastPos();

        if (pFSM != null)
            pFSM.Update(0);

        if (emCurState != EMState.Attack &&
            emCurState != EMState.Dead)
        {
            if (!bAtkAble &&
                 pTickerAtkCD.Value > Fix64.Zero &&
                 pTickerAtkCD.Tick(CLockStepData.g_fixFrameLen))
            {
                bAtkAble = true;
                pTickerAtkCD.Value = pUnitData.AtkCD;
            }
        }
        if (emCurState != EMState.Skill &&
            emCurState != EMState.Dead)
        {
            if (!bSkillAble &&
                 pTickerSkillCD.Value > Fix64.Zero &&
                 pTickerSkillCD.Tick(CLockStepData.g_fixFrameLen))
            {
                bSkillAble = true;
                pTickerSkillCD.Value = Fix64.MinusOne;
            }
        }
        if (emCurState != EMState.Skill &&
            emCurState != EMState.Attack &&
            emCurState != EMState.Dead)
        {
            if (pTickerCheck.Value > Fix64.Zero &&
                pTickerCheck.Tick(CLockStepData.g_fixFrameLen))
            {
                GetAtkTargetByAlertRange();
                if (pAtkTarget != null &&
                    !pAtkTarget.IsDead())
                {
                    bCheckTarget = false;
                    pTickerCheck.Value = Fix64.MinusOne;
                }
                else
                {
                    RefreshSearch();
                }
            }
        }
    }

    public override void OnUpdateRender(float dt)
    {
        if (emCurState == EMState.Skill ||
            emCurState == EMState.Dead) return;
        UpdatePos(dt);
    }

    private void FixedUpdate()
    {
        if (pFSM != null)
            pFSM.FixedUpdate(CTimeMgr.FixedDeltaTime);

        if (pAnimeCtrl != null)
        {
            pAnimeCtrl.UpdateFrame(CTimeMgr.FixedDeltaTime);
        }
    }

    public virtual void Recycle()
    {
        if (pStayMapSlot != null)
        {
            List<MapSlot> listSlotPre = GetSelfSlot();
            for (int i = 0; i < listSlotPre.Count; i++)
            {
                if (emMoveType == EMMoveType.Ground)
                {
                    if (listSlotPre[i].pStayGroundUnit != null &&
                        listSlotPre[i].pStayGroundUnit.szSelfUid != szSelfUid) continue;
                    listSlotPre[i].pStayGroundUnit = null;
                }
                else if (emMoveType == EMMoveType.Fly)
                {
                    if (listSlotPre[i].pStayFlyUnit != null &&
                        listSlotPre[i].pStayFlyUnit.szSelfUid != szSelfUid) continue;
                    listSlotPre[i].pStayFlyUnit = null;
                }
                //listSlotPre[i].ActiveRenderColor(false);
            }
        }
        pStayMapSlot = null;

        pMoveTarget = null;
        pAtkTarget = null;

        pTickerCheck.Value = Fix64.MinusOne;
        pTickerAtkCD.Value = Fix64.MinusOne;
        pTickerSkillCD.Value = Fix64.MinusOne;

        buffPack.Clear();
        OnRecycleLockStepUnit();
        CPlayerMgr.Ins.RemovePlayerUnit(this);
        dlgRecycle?.Invoke();

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            MapSlot slot = MapMgr.Ins.GetRandomIdleSlot();
            MoveToTarget(slot);
        }
    }

}
