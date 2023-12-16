using FixMath.NET;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockUnityObject : MonoBehaviour
{
    [ReadOnly]
    public long nUniqueIdx; //唯一索引

    public Transform tranSelf;

    //最后的位置
    public FixVector3 m_fixv3LastPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    //逻辑位置
    public FixVector3 m_fixv3LogicPosition = new FixVector3(Fix64.Zero, Fix64.Zero, Fix64.Zero);

    /// <summary>
    /// 初始化唯一索引
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

    #region 坐标相关

    public virtual void UpdatePos(float interpolation)
    {
        tranSelf.localPosition = Vector3.Lerp(m_fixv3LastPosition.ToVector3(), m_fixv3LogicPosition.ToVector3(), interpolation);
    }

    public virtual void ForceRefreshPos()
    {
        tranSelf.localPosition = m_fixv3LogicPosition.ToVector3();
        //Debug.Log(tranSelf.localPosition);
    }

    //记录最后的位置
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
