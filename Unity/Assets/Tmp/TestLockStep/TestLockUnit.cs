using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLockUnit : CLockUnityObject
{
    public Renderer pRenderer;
    public CLockPhysicEntityBase pPhysicEntity;

    public void Init()
    {
        pPhysicEntity.Init();
    }

    public override void OnUpdateLogic()
    {
        //RecordLastPos();

        //pPhysicEntity.SyncGameObjectTransToEntity();
    }

    public override void UpdatePos(float interpolation)
    {
        if(!pPhysicEntity.bIsStatic)
        {
            pPhysicEntity.SyncEntityTransToGameObject();
        }
    }

    protected override void OnRecycleLockStepUnit()
    {
        if(pPhysicEntity != null)
        {
            pPhysicEntity.RemoveSpace();
        }

        base.OnRecycleLockStepUnit();
    }
}
