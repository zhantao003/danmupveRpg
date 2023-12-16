using BEPUphysics.Entities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicEntityBase : MonoBehaviour
{
    public CLockUnityObject pUnit;
    public float fMass = 1f;
    public bool bIsStatic = false;

    protected Entity pEntity = null;
    protected Vector3 vOriginCenter = Vector3.zero;
    protected bool isTrigger = false;
    protected PhysicMaterial pPhysicMat = null;

    protected FixVector3 v64UnitPos;
    protected FixVector4 v64UnitRot;
    protected FixVector3 v64ColliderCenter;

    public virtual void Init()
    {
        v64UnitPos = pUnit.m_fixv3LogicPosition;
        v64UnitRot = new FixVector4((Fix64)pUnit.tranSelf.rotation.x,
                                    (Fix64)pUnit.tranSelf.rotation.y,
                                    (Fix64)pUnit.tranSelf.rotation.z,
                                    (Fix64)pUnit.tranSelf.rotation.w);
    }

    public void AddToSpace()
    {
        if (pEntity == null ||
            CLockPhysicMgr.Ins == null)
        {
            return;
        }

        pEntity.pLockUnit = pUnit;
        CLockPhysicMgr.Ins.pBEPUSpace.Add(pEntity);
    }

    /// <summary>
    /// 同步物理对象到GameoObject
    /// </summary>
    public void SyncEntityTransToGameObject()
    {
        if (pEntity == null)
        {
            return;
        }

        // 位置
        BEPUutilities.Vector3 pos = pEntity.Position;
        Vector3 unityPosition = (new FixVector3(pos.X, pos.Y,pos.Z)).ToVector3();
        unityPosition -= vOriginCenter;
        pUnit.tranSelf.position = unityPosition;

        Debug.Log("Entity:" + pEntity.position + "   GameObject:" + pUnit.transform.position);

        // 旋转
        BEPUutilities.Quaternion rot = pEntity.Orientation;
        Quaternion r = new Quaternion((float)rot.X, (float)rot.Y, (float)rot.Z, (float)rot.W);
        pUnit.tranSelf.rotation = r;
    }

    public virtual void SyncEntityTransFromGameObject(FixVector3 pos, FixVector4 rot)
    {
        if (pEntity == null)
        {
            return;
        }

        // 位置
        pos += v64ColliderCenter;
        pEntity.Position = new BEPUutilities.Vector3(pos.x, pos.y, pos.z);

        // 旋转
        pEntity.Orientation = new BEPUutilities.Quaternion(rot.x, rot.y, rot.z, rot.w);
    }

    public void RemoveSpace()
    {
        if (pEntity == null ||
            CLockPhysicMgr.Ins == null)
        {
            return;
        }

        CLockPhysicMgr.Ins.pBEPUSpace.Remove(pEntity);
    }
}
