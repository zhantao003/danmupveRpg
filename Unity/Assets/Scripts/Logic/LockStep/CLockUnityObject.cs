using FixMath.NET;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockUnityObject : MonoBehaviour
{
    [ReadOnly]
    public long nUniqueIdx; //Ψһ����

    public Transform tranSelf;

    //����λ��
    public FixVector3 m_fixv3LastPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    //�߼�λ��
    public FixVector3 m_fixv3LogicPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    /// <summary>
    /// ��ʼ��Ψһ����
    /// </summary>
    public virtual void InitUniqueIdx()
    {
        if (CBattleMgr.Ins == null) return;

        nUniqueIdx = CBattleMgr.Ins.nCurUnitCount++;
        gameObject.name = "U_" + nUniqueIdx;
    }

    public virtual void OnUpdateLogic()
    {
        RecordLastPos();
    }

    public virtual void OnUpdateRender(float dt)
    {
        UpdatePos(dt);
    }

    #region �������

    public virtual void UpdatePos(float interpolation)
    {
        tranSelf.localPosition = Vector3.Lerp(m_fixv3LastPosition.ToVector3(), m_fixv3LogicPosition.ToVector3(), interpolation);
    }

    public virtual void ForceRefreshPos()
    {
        tranSelf.localPosition = m_fixv3LogicPosition.ToVector3();
        //Debug.Log(tranSelf.localPosition);
    }

    //��¼����λ��
    public void RecordLastPos()
    {
        m_fixv3LastPosition = m_fixv3LogicPosition;
    }

    #endregion

    protected virtual void OnRecycleLockStepUnit()
    {
        CLockStepMgr.Ins.RemoveLockUnit(this); 
    }

    protected virtual void OnRecycleLockStepBullet()
    {
        CLockStepMgr.Ins.RemoveLockBullet(this);
    }
}
