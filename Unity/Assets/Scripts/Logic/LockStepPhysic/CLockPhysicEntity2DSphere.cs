using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicEntity2DSphere : CLockPhysicEntityBase
{
    public CircleCollider2D pCol; 
    protected float fRadius;

    public override void Init()
    {
        base.Init();

        fRadius = pCol.radius;

        vOriginCenter = pCol.offset;
        v64ColliderCenter = new FixVector3((Fix64)vOriginCenter.x, (Fix64)vOriginCenter.y, (Fix64)vOriginCenter.z);

        //pPhysicMat = pCol.sharedMaterial;
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
