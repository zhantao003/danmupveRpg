using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLockPhysicMgr : CSingleCompBase<CLockPhysicMgr>
{
    public BEPUphysics.Space pBEPUSpace;

    public void Init()
    {
        Physics.autoSimulation = false; // �ر�ԭ�������������;
        pBEPUSpace = new BEPUphysics.Space(); // ������������
        pBEPUSpace.ForceUpdater.gravity = new BEPUutilities.Vector3(0, (Fix64)(-9.8f), 0); // ��������
        pBEPUSpace.TimeStepSettings.TimeStepDuration = CLockStepData.g_fixFrameLen; // ���õ���ʱ����
    }

    public void OnUpdate(Fix64 dt)
    {
        if(pBEPUSpace!=null)
        pBEPUSpace.Update();
    }
}
