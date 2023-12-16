using BEPUphysics.Entities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicEntitySphere : CLockPhysicEntityBase
{
    public SphereCollider pCol;
    protected float fRadius;

    public override void Init()
    {
        base.Init();

        fRadius = pCol.radius;

        vOriginCenter = pCol.center;
        v64ColliderCenter = new FixVector3((Fix64)pCol.center.x, (Fix64)pCol.center.y, (Fix64)pCol.center.z);

        pPhysicMat = pCol.material;
        isTrigger = pCol.isTrigger;

        if (bIsStatic)
        {
            pEntity = new BEPUphysics.Entities.Prefabs.Sphere(BEPUutilities.Vector3.Zero, (FixMath.NET.Fix64)fRadius);
            pEntity.BecomeKinematic();
        }
        else
        {
            pEntity = new BEPUphysics.Entities.Prefabs.Sphere(BEPUutilities.Vector3.Zero, (FixMath.NET.Fix64)fRadius, (FixMath.NET.Fix64)this.fMass);
        }

        AddToSpace();
        SyncEntityTransFromGameObject(v64UnitPos, FixVector4.Zero);
    }
}
