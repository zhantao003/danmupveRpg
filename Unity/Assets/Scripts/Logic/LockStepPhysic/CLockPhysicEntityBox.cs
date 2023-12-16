using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicEntityBox : CLockPhysicEntityBase
{
    public BoxCollider pCol;

    public override void Init()
    {
        base.Init();

        float width = pCol.size.x;
        float height = pCol.size.y;
        float length = pCol.size.z;

        vOriginCenter = pCol.center;
        v64ColliderCenter = new FixVector3((Fix64)pCol.center.x, (Fix64)pCol.center.y, (Fix64)pCol.center.z);

        pPhysicMat = pCol.material;
        isTrigger = pCol.isTrigger;

        if(pEntity == null)
        {
            if (bIsStatic)
            {
                pEntity = new BEPUphysics.Entities.Prefabs.Box(BEPUutilities.Vector3.Zero, (FixMath.NET.Fix64)width, (FixMath.NET.Fix64)height, (FixMath.NET.Fix64)length);
                //pEntity.BecomeKinematic();
            }
            else
            {
                pEntity = new BEPUphysics.Entities.Prefabs.Box(BEPUutilities.Vector3.Zero, (FixMath.NET.Fix64)width, (FixMath.NET.Fix64)height, (FixMath.NET.Fix64)length, (FixMath.NET.Fix64)fMass);
            }
        }
        
        AddToSpace();
        SyncEntityTransFromGameObject(v64UnitPos, FixVector4.Zero);
    }
}
