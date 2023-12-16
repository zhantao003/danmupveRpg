using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

[System.Serializable]
public class CMitigationInfo
{
    public float fValue;
    public int nDelValue;
    public bool bActive;
    public float fStayCheckTime;
    Fix64 f64StayTotalTime;
    Fix64 f64StayCurTime;

    public void StartTick()
    {
        f64StayTotalTime = (Fix64)fStayCheckTime;
        f64StayCurTime = Fix64.Zero;
        bActive = true;
    }

    public bool Tick(Fix64 dt)
    {
        if (bActive)
        {
            f64StayCurTime += dt;
            //大于1表示当前移动结束
            if (f64StayCurTime >= f64StayTotalTime)
            {
                f64StayCurTime = Fix64.Zero;
                bActive = false;
                return true;
            }
        }
        return false;
    }

}

public class CBaseUnit : CPlayerUnit
{
    /// <summary>
    /// 阵营信息
    /// </summary>
    public CampInfo pCampInfo;
    /// <summary>
    /// 当前的基地等级
    /// </summary>
    public int nLev;
    /// <summary>
    /// 升级所需的经验
    /// </summary>
    public int[] nNeedExp;
    /// <summary>
    /// 当前积累的经验
    /// </summary>
    public int nCurExp;
    /// <summary>
    /// 出生点位
    /// </summary>
    public Vector2Int vCreatPos;

    public Sprite[] pBuildTex;

    public CTowerDmgInfo[] pDmgInfo;

    /// <summary>
    /// 受击判定
    /// </summary>
    public bool bHit;
    public float fHitCheckTime;
    Fix64 f64HitTotalTime;
    Fix64 f64HitCurTime;

    public UITweenBase[] arrTweenBeHit;

    public string szBoomEffect;

    public string[] szBaseBullets;

    public Dictionary<int, int> dicBuffAddByLayer = new Dictionary<int, int>();

    long nlTotalBattleValue;

    /// <summary>
    /// 总战斗力
    /// </summary>
    public long TotalBattleValue
    {
        get
        {
            return nlTotalBattleValue;
        }
        set
        {
            nlTotalBattleValue = value;
            delBattleChg?.Invoke(nlTotalBattleValue, emCamp);
        }
    }
    /// <summary>
    /// 战斗力变化事件
    /// </summary>
    public DelegateCampBattleChg delBattleChg;

    public override void Init(EMUnitCamp camp = EMUnitCamp.Blue)
    {
        bSkillAble = true;
        bAtkAble = true;
        pRenderer.enabled = true;

        fCheckCDF64 = (Fix64)fCheckCD;

        //pCreatTick = new CPropertyTimer();
        //pCreatTick.Value = fCreatTime;
        //pCreatTick.FillTime();

        szSelfUid = CHelpTools.GenerateIdFix64().ToString();
        pMoveTarget = null;
        pAtkTarget = null;
        MapSlot slot2 = null;
        AStarFindPath.Ins.GetMapSlot(ref slot2,vCreatPos);
        SetMapSlot(slot2);
        SetRenderLayer(slot2.nCurSetRenderLayer);
        tranSelf.position = slot2.tranSelf.position;
        InitCampInfo(emCamp == EMUnitCamp.Blue ? CBattleMgr.Ins.pBlueCamp : CBattleMgr.Ins.pRedCamp);
        
        nLev = 1;
        szUserUid = CHelpTools.GenerateId().ToString();
        pUnitData.Init(this);
        InitFSM();
        SetState(EMState.Idle);
        m_fixv3LogicPosition = new FixVector3((Fix64)transform.position.x, (Fix64)transform.position.y, Fix64.Zero);
        CLockStepMgr.Ins.AddLockUnit(this);
        List<ST_BaseInfo> baseInfos = CTBLHandlerBaseInfo.Ins.GetInfos();
        nNeedExp = new int[baseInfos.Count];
        for(int i = 0;i < baseInfos.Count;i++)
        {
            nNeedExp[i] = baseInfos[i].nExp;
        }
        int nIdx = 0;
        if (emCamp == EMUnitCamp.Blue)
        {
            nIdx = (int)CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp;
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            nIdx = (int)CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp;
            transform.localScale = Vector3.one;
        }
        pUnitData.szBullet = szBaseBullets[nIdx];

        if (pColUnit != null)
        {
            pColUnit.Init();
        }

        RefreshSearch();

    }

    public int GetDmg(int nID)
    {
        int nDmg = 0;
        for (int i = 0; i < pDmgInfo.Length; i++)
        {
            if (pDmgInfo[i].nID == nID)
            {
                nDmg = pDmgInfo[i].nDmg;
            }
        }
        return nDmg;
    }

    public void SetWorldUI()
    {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.SetBuildHp(this);
    }

    public void ChgBattle(long value,bool bAdd)
    {
        if(bAdd)
        {
            TotalBattleValue += value;// System.Convert.ToInt64((float)value * 1.3f);
        }
        else
        {
            TotalBattleValue -= value;// System.Convert.ToInt64((float)value * 1.3f);
        }
    }

    public void ChgBuff(int nID,int nCount,bool bAdd)
    {
        if(!dicBuffAddByLayer.ContainsKey(nID))
        {
            dicBuffAddByLayer.Add(nID, 0);
        }
        if(bAdd)
        {
            dicBuffAddByLayer[nID] += nCount;
        }
        else
        {
            dicBuffAddByLayer[nID] -= nCount;
            if(dicBuffAddByLayer[nID] < 0)
            {
                dicBuffAddByLayer[nID] = 0;
            }
        }
    }

    public float GetBuffNumPer(int nID)
    {
        ST_BuffInfo pBuffTBLInfo = CTBLHandlerBuffInfo.Ins.GetInfo(nID);
        CLocalNetMsg msg = new CLocalNetMsg(pBuffTBLInfo.arrAddPro[0]);
        if (dicBuffAddByLayer.ContainsKey(nID))
        {
            int layerCount = GetBuffLayer(nID);
            return layerCount * msg.GetInt("data");
        }
        return 0;
    }

    public int GetBuffLayer(int nID) {
        ST_BuffInfo pBuffTBLInfo = CTBLHandlerBuffInfo.Ins.GetInfo(nID);
        if (dicBuffAddByLayer.ContainsKey(nID))
        {
            int layerCount = dicBuffAddByLayer[nID] > pBuffTBLInfo.nMaxLayer ? pBuffTBLInfo.nMaxLayer : dicBuffAddByLayer[nID];
            return layerCount;
        }
        return 0;
    }

    /// <summary>
    /// 对应路线的兵营被摧毁
    /// </summary>
    /// <param name="pathType"></param>
    public void OnBarracksDestroy(EMStayPathType pathType)
    {
        switch(pathType)
        {
            case EMStayPathType.Up:
                {
                    pCampInfo.bUpDead = true;
                }
                break;
            case EMStayPathType.Center:
                {
                    pCampInfo.bCenterDead = true;
                }
                break;
            case EMStayPathType.Down:
                {
                    pCampInfo.bDownDead = true;
                }
                break;
        }
        if(pCampInfo.bUpDead &&
            pCampInfo.bDownDead)
        {
            pCampInfo.bCenterDead = true;
        }
    }

    
    public void InitCampInfo(CampInfo campInfo)
    {
        pCampInfo = new CampInfo(campInfo);
        pRenderer.sprite = pBuildTex[(int)pCampInfo.emCamp];
    }

    protected override void InitFSM()
    {
        pFSM = new FSMManager(this);
        //pFSM.AddState((int)EMState.Idle, new FSMUnitRole_Idle());
        pFSM.AddState((int)EMState.Idle, new FSMUnitRole_Idle());
        pFSM.AddState((int)EMState.Attack, new FSMUnitRole_Atk());
        pFSM.AddState((int)EMState.Dead, new FSMUnitRole_Dead());
    }

    public void ResetHitState()
    {
        bHit = true;
        f64HitTotalTime = (Fix64)fHitCheckTime;
        f64HitCurTime = Fix64.Zero;
    }

    public override void OnHit(CPlayerUnit atkunit, int nDmg, MapSlot pHitSlot = null)
    {
        ///技能状态不挨打
        if (emCurState == EMState.Skill) return;
        ///自己不能打自己
        if (atkunit.szUserUid == szUserUid) return;
        if (IsDead()) return;
        ResetHitState();
        vHitTargetSlot = pHitSlot;
        if(curMitigationInfos != null &&
            curMitigationInfos.bActive)
        {
            pUnitData.nCurHP -= nDmg * (100 - curMitigationInfos.nDelValue) / 100;// System.Convert.ToInt32((float)nDmg * (1f - (float)curMitigationInfos.nDelValue * 0.01f));
        }
        else
        {
            pUnitData.nCurHP -= nDmg;
        }
        CheckMitigation();
        ///发送建筑掉血的监听事件
        CLocalNetMsg msg2 = new CLocalNetMsg();
        msg2.SetInt("camp", (int)emCamp);
        msg2.SetInt("unittype", (int)emUnitType);
        msg2.SetInt("pathtype", (int)emPathType);
        msg2.SetLong("curhp", pUnitData.nCurHP);
        msg2.SetInt("maxhp", pUnitData.MaxHP);
        msg2.SetString("atkuid", atkunit.szUserUid);
        CGameObserverMgr.SendMsg(CGameObserverConst.BuildHPChg, msg2);

        if (pUnitData.nCurHP <= 0)
        {
            pRenderer.enabled = false;
            pUnitData.nCurHP = 0;
            if(!CHelpTools.IsStringEmptyOrNone(szBoomEffect))
            {
                CEffectMgr.Instance.CreateEffSync(szBoomEffect, tranSelf.position, Quaternion.identity, 0);
            }
            CCameraController.Ins.LookToTarget(tranSelf.position);
            SetState(EMState.Dead);
            ///发送单位被击杀的监听事件
            CLocalNetMsg msg = new CLocalNetMsg();
            msg.SetInt("camp", (int)emCamp);
            msg.SetInt("pathtype", (int)emPathType);
            msg.SetInt("unittype", (int)emUnitType);
            msg.SetLong("gold", pUnitData.nlGold);
            msg.SetInt("buffid", pUnitData.nBuffID);
            msg.SetString("atkuid", atkunit.szUserUid);
            CGameObserverMgr.SendMsg(CGameObserverConst.UnitDead, msg);
        }
        else
        {
            //播放受击动画
            if (arrTweenBeHit[0] != null &&
                !arrTweenBeHit[0].enabled)
            {
                for (int i = 0; i < arrTweenBeHit.Length; i++)
                {
                    arrTweenBeHit[i].Play();
                }
            }
        }
    }

    int nGetLev = 0;
    /// <summary>
    /// 增加基地经验
    /// </summary>
    /// <param name="nExp"></param>
    public bool AddExp(int nExp)
    {
        nGetLev = -1;
        nCurExp += nExp;
        for (int i = 0; i < nNeedExp.Length; i++)
        {
            //if (nNeedExp[i] == 0) break;
            if(nNeedExp[i] < nCurExp)
            {
                nGetLev = i+1;
            }
            else
            {
                break;
            }
        }
        if(nGetLev >= 1)
        {
            bool bRefreshUI = nLev != nGetLev;
            nLev = nGetLev;
            if (bRefreshUI)
            {
                UIGameInfo gameInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
                if (gameInfo != null)
                {
                    gameInfo.RefreshBaseInfo(emCamp == EMUnitCamp.Red);
                }
                return bRefreshUI;
            }
        }
        return false;
    }

    public override void OnUpdateLogic()
    {
        if (pFSM != null)
        {
            pFSM.Update(0);
        }
        ///受击计时器
        if (bHit)
        {
            f64HitCurTime += CLockStepData.g_fixFrameLen;
            //大于1表示当前移动结束
            if (f64HitCurTime >= f64HitTotalTime)
            {
                f64HitCurTime = Fix64.Zero;
                bHit = false;
            }
        }
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
        if (curMitigationInfos != null &&
           curMitigationInfos.bActive)
        {
            if (curMitigationInfos.Tick(CLockStepData.g_fixFrameLen))
            {
                curMitigationInfos.bActive = false;
            }
        }
        //pAnimeCtrl.UpdateFrame((float)CLockStepData.g_fixFrameLen);
    }

    public override void OnUpdateRender(float dt)
    {

    }

    private void FixedUpdate()
    {
        if (pFSM != null)
            pFSM.FixedUpdate(CTimeMgr.FixedDeltaTime);
    }

    public override void Recycle()
    {
        if (pStayMapSlot != null)
        {
            List<MapSlot> listSlotPre = GetSelfSlot();
            for (int i = 0; i < listSlotPre.Count; i++)
            {
                if (listSlotPre[i].pStayGroundUnit != null &&
                    listSlotPre[i].pStayGroundUnit.szSelfUid != szSelfUid) continue;
                listSlotPre[i].pStayGroundUnit = null;
                //listSlotPre[i].ActiveRenderColor(false);
            }
        }
        pStayMapSlot = null;
        pMoveTarget = null;
        pAtkTarget = null;
        CLockStepMgr.Ins.RemoveLockUnit(this);
        ///发送基地被摧毁的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", (int)emCamp);
        CGameObserverMgr.SendMsg(CGameObserverConst.BaseDestroy, msg);

        dlgRecycle?.Invoke();
        enabled = false;
    }

    public string GetAddProByCamp(bool needBuffPer=true)
    {
        string szAddPro = string.Empty;

        switch (pCampInfo.emAddType)
        {
            case AddTypeByCamp.Hp:
                {
                    szAddPro = "血量上限";
                }
                break;
            case AddTypeByCamp.Atk:
                {
                    szAddPro = "攻击伤害";
                }
                break;
            case AddTypeByCamp.MoveSpeed:
                {
                    szAddPro = "移动速度";
                }
                break;
            case AddTypeByCamp.AtkSpeed:
                {
                    szAddPro = "攻击速度";
                }
                break;
        }
        //Debug.Log("nLev====" + nLev);
        if(needBuffPer)
            szAddPro += "+" + pCampInfo.nValues[nLev-1] + "%";

        return szAddPro;
    }

    public int GetAddValueByCamp()
    {
        return pCampInfo.nValues[nLev - 1];
    }

}
