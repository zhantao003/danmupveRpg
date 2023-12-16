using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicMgr : CSingleCompBase<CLockPhysicMgr>
{
    public BEPUphysics.Space pBEPUSpace;

    public void Init()
    {
        Physics.autoSimulation = false; // 关闭原来物理引擎迭代;
        pBEPUSpace = new BEPUphysics.Space(); // 创建物理世界
        pBEPUSpace.ForceUpdater.gravity = new BEPUutilities.Vector3(0, (Fix64)(-9.8f), 0); // 配置重力
        pBEPUSpace.TimeStepSettings.TimeStepDuration = CLockStepData.g_fixFrameLen; // 设置迭代时间间隔
    }

    public void OnUpdate(Fix64 dt)
    {
        if(pBEPUSpace!=null)
        pBEPUSpace.Update();
    }
}
