using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBarracksUnit : CPlayerUnit
{
    /// <summary>
    /// 出生点位
    /// </summary>
    public Vector2Int vCreatPos;

    public Sprite[] pBuildTex;

    public GameObject objNormalRoot;
    public GameObject objDestroyRoot;

    public UITweenBase[] arrTweenBeHit;

    public string szBoomEffect;

    public override void Init(EMUnitCamp camp = EMUnitCamp.Blue)
    {
        pRenderer.enabled = true;
        szSelfUid = CHelpTools.GenerateIdFix64().ToString();
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
        
        if(camp == EMUnitCamp.Blue)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            pRenderer.sprite = pBuildTex[(int)CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp];
        }
        else if(camp == EMUnitCamp.Red)
        {
            transform.localScale = Vector3.one;
            pRenderer.sprite = pBuildTex[(int)CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp];
        }

        if (pColUnit != null)
        {
            pColUnit.Init();
        }

        ActiveBuildObj(false);
    }

    public void SetWorldUI()
    {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.SetBuildHp(this);
    }

    public void ActiveBuildObj(bool bDestroy)
    {
        objNormalRoot.SetActive(!bDestroy);
        objDestroyRoot.SetActive(bDestroy);
    }

    protected override void InitFSM()
    {
        pFSM = new FSMManager(this);
        pFSM.AddState((int)EMState.Idle, new FSMUnitRole_Idle());
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
        msg2.SetString("atkuid", atkunit.szSelfUid);
        CGameObserverMgr.SendMsg(CGameObserverConst.BuildHPChg, msg2);

        if (pUnitData.nCurHP <= 0)
        {
            pRenderer.enabled = false;
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
            msg.SetString("atkuid", szUserUid);
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

    public override void OnUpdateLogic()
    {
        if (pFSM != null)
        {
            pFSM.Update(0);
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

        ///发送兵营被摧毁的监听事件
        CLocalNetMsg msg = new CLocalNetMsg();
        msg.SetInt("camp", (int)emCamp);
        msg.SetInt("pathtype", (int)emPathType);
        CGameObserverMgr.SendMsg(CGameObserverConst.BarracksDestroy, msg);

        CLockStepMgr.Ins.RemoveLockUnit(this);
        ActiveBuildObj(true);
        //gameObject.SetActive(false);
        dlgRecycle?.Invoke();
        enabled = false;
    }

}
