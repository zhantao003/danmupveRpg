using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTowerDmgInfo
{
    public int nID;
    public int nDmg;
}

public class CTowerUnit : CPlayerUnit
{
    /// <summary>
    /// 出生点位
    /// </summary>
    public Vector2Int vCreatPos;

    public Vector3 vTargetLerp = new Vector3(0, 1f, 0);

    public SpriteRenderer pTowerRenderer;

    public Sprite[] pBuildTex;

    public Sprite[] pHeadTex;

    public string[] szTowerBullets;

    public GameObject objNormalRoot;
    public GameObject objDestroyRoot;

    public CTowerDmgInfo[] pDmgInfo;

    public UITweenBase[] arrTweenBeHit;

    public string szBoomEffect;

    public override void Init(EMUnitCamp camp = EMUnitCamp.Blue)
    {
        bSkillAble = true;
        bAtkAble = true;
        szSelfUid = CHelpTools.GenerateIdFix64().ToString();

        fCheckCDF64 = (Fix64)fCheckCD;
        pMoveTarget = null;
        pAtkTarget = null;
        MapSlot slot2 = null;
        AStarFindPath.Ins.GetMapSlot(ref slot2,vCreatPos);
        SetMapSlot(slot2);
        SetRenderLayer(slot2.nCurSetRenderLayer);
        tranSelf.position = slot2.tranSelf.position;

        szUserUid = CHelpTools.GenerateId().ToString();
        pUnitData.Init(this);
        InitFSM();
        SetState(EMState.Idle);
        m_fixv3LogicPosition = new FixVector3((Fix64)transform.position.x, (Fix64)transform.position.y, Fix64.Zero);
        CLockStepMgr.Ins.AddLockUnit(this);
        int nIdx = 0;
        if (camp == EMUnitCamp.Blue)
        {
            nIdx = (int)CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp;
        }
        else if (camp == EMUnitCamp.Red)
        {
            nIdx = (int)CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp;
        }
        pRenderer.sprite = pBuildTex[nIdx];
        pTowerRenderer.sprite = pHeadTex[nIdx];
        pUnitData.szBullet = szTowerBullets[nIdx];
        if (emCamp == EMUnitCamp.Blue)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = Vector3.one;
        }

        if (pColUnit != null)
        {
            pColUnit.Init();
        }

        ActiveBuildObj(false);
        RefreshSearch();
        //List<MapSlot> listSlotCur = AStarFindPath.Ins.GetAroundSlot(pUnitData.nAlertRange, pStayMapSlot.vecPos);
        //for (int i = 0; i < listSlotCur.Count; i++)
        //{
        //    listSlotCur[i].OnEnter();
        //    //listSlotCur[i].ActiveRenderColor(true);
        //}
    }

    public int GetDmg(int nID)
    {
        int nDmg = 0;
        for(int i = 0;i < pDmgInfo.Length;i++)
        {
            if(pDmgInfo[i].nID == nID)
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

    protected override void CreatBullet()
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
        FixVector3 targetPos = pAtkTarget.pStayMapSlot.v64SlotPos + new FixVector3((Fix64)vTargetLerp.x, (Fix64)vTargetLerp.y, (Fix64)vTargetLerp.z);
        bulletUnit.Init(startPos, targetPos, pAtkTarget, this);
        CBulletMgr.Ins.AddBulletUnit(bulletUnit);
        CLockStepMgr.Ins.AddLockBullet(bulletUnit);

    }

    public void ActiveBuildObj(bool bDestroy)
    {
        objNormalRoot.SetActive(!bDestroy);
        objDestroyRoot.SetActive(bDestroy);
    }

    public override void SetRenderLayer(int nLayer)
    {
        nCurSetRenderLayer = nLayer;
        if (pRenderer != null)
            pRenderer.sortingOrder = nCurSetRenderLayer;
        if (pTowerRenderer != null)
        {
            pTowerRenderer.sortingOrder = nCurSetRenderLayer + 1;
        }
    }

    protected override void InitFSM()
    {
        pFSM = new FSMManager(this);
        pFSM.AddState((int)EMState.Idle, new FSMUnitRole_Idle());
        pFSM.AddState((int)EMState.Attack, new FSMUnitRole_Atk());
        pFSM.AddState((int)EMState.Dead, new FSMUnitRole_Dead());
    }

    public override void OnHit(CPlayerUnit atkunit, int nDmg, MapSlot pHitSlot = null)
    {
        ///技能状态不挨打
        if (emCurState == EMState.Skill) return;
        ///自己不能打自己
        if (atkunit.szUserUid == szUserUid) return;
        if (IsDead()) return;
        if (curMitigationInfos != null &&
            curMitigationInfos.bActive)
        {
            pUnitData.nCurHP -= nDmg * (100 - curMitigationInfos.nDelValue) / 100;//  System.Convert.ToInt32((float)nDmg * (1f - (float)curMitigationInfos.nDelValue * 0.01f));
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
        //float
        if (pUnitData.nCurHP <= 0)
        {
            pUnitData.nCurHP = 0;
            if (!CHelpTools.IsStringEmptyOrNone(szBoomEffect))
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
            if (arrTweenBeHit[0]!=null && 
                !arrTweenBeHit[0].enabled)
            {
                for(int i=0; i<arrTweenBeHit.Length; i++)
                {
                    arrTweenBeHit[i].Play();
                }
            }
        }
    }

    public override void OnUpdateLogic()
    {
        if (pFSM != null)
        {
            pFSM.Update(0);
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
        //pTickerSkillCD
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
        ///发送兵营被摧毁的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", (int)emCamp);
        msg.SetInt("pathtype", (int)emPathType);
        CGameObserverMgr.SendMsg(CGameObserverConst.TowerDestroy, msg);

        CLockStepMgr.Ins.RemoveLockUnit(this);
        ActiveBuildObj(true);
        //gameObject.SetActive(false);
        dlgRecycle?.Invoke();
        enabled = false;
    }

}
