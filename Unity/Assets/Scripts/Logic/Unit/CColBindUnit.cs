using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CColBindUnit : MonoBehaviour
{
    public Transform tranSelf;

    public CPlayerUnit pBindUnit;

    public CLockPhysicEntityBase pBEPUEntity;

    public void Init()
    {
        if(CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.LocalPvP)
        {
            Destroy(pBEPUEntity);
            pBEPUEntity = null;
            //pBEPUEntity.Init();
        }
        else
        {
            if (pBEPUEntity != null)
            {
                pBEPUEntity.Init();
            }
        }
    }

    public void Recycle()
    {
        if(pBEPUEntity!=null)
        {
            pBEPUEntity.RemoveSpace();
        }
    }

    public void SetPos(MapSlot slot)
    {
        tranSelf.position = slot.transform.position;
        if(pBEPUEntity!=null)
        {
            pBEPUEntity.SyncEntityTransFromGameObject(slot.v64SlotPos, FixVector4.Zero);
        }
    }
}
