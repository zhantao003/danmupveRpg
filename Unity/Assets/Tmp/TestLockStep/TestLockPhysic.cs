using BEPUphysics.BroadPhaseEntries;
using BEPUphysics.BroadPhaseEntries.MobileCollidables;
using BEPUphysics.Entities;
using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class TestLockPhysic : MonoBehaviour
{
    public Camera pCam;
    public GameObject objRoot;
    public Transform tranCheck;

    //平面
    public TestLockUnit[] arrUnits;

    List<BroadPhaseEntry> listCheckEntry = new List<BroadPhaseEntry>();

    // Start is called before the first frame update
    void Start()
    {
        CLockStepData.pRand = new SRandom((uint)Random.Range(0, 99999));
        CLockStepMgr.Ins.Init(CLockStepMgr.EMType.Local);

        for(int i=0; i<arrUnits.Length; i++)
        {
            arrUnits[i].m_fixv3LogicPosition = new FixVector3((Fix64)arrUnits[i].tranSelf.position.x,
                                                              (Fix64)arrUnits[i].tranSelf.position.y,
                                                              (Fix64)arrUnits[i].tranSelf.position.z);
            arrUnits[i].ForceRefreshPos();
            arrUnits[i].RecordLastPos();
            arrUnits[i].Init();

            CLockStepMgr.Ins.AddLockUnit(arrUnits[i]);
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray pRay = pCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit pHit;
            if(Physics.Raycast(pRay.origin, pRay.direction, out pHit))
            {
                tranCheck.position = pHit.point;

                BEPUutilities.Vector3 vCheckPos = new BEPUutilities.Vector3((Fix64)tranCheck.position.x,
                                                                             Fix64.Zero,
                                                                            (Fix64)tranCheck.position.z);

                CLockPhysicMgr.Ins.pBEPUSpace.
                    BroadPhase.QueryAccelerator.
                    GetEntries(new BEPUutilities.BoundingSphere(vCheckPos, (Fix64)1.5f), listCheckEntry);

                for(int i=0; i<listCheckEntry.Count; i++)
                {
                    EntityCollidable entityCollision = listCheckEntry[i] as EntityCollidable;
                    if (entityCollision == null) continue;

                    Entity pEntity = entityCollision.Entity;
                    if (pEntity == null) continue;
                    if (pEntity.isDynamic) continue;

                    CLockUnityObject pLockUnit = pEntity.pLockUnit;
                    if (pLockUnit == null) continue;

                    TestLockUnit pTestUnit = pLockUnit as TestLockUnit;
                    if (pTestUnit == null) continue;

                    if(pTestUnit.pRenderer != null)
                    {
                        pTestUnit.pRenderer.material.color = Color.red;
                    }
                }

                listCheckEntry.Clear();
            }
        }
    }

    private void OnGUI()
    {
        if(GUILayout.Button("创建物体"))
        {
            GameObject objNewObj = GameObject.Instantiate(objRoot) as GameObject;
            objNewObj.SetActive(true);
            
            TestLockUnit objUnit = objNewObj.GetComponent<TestLockUnit>();
            if(objUnit!=null)
            {
                objUnit.m_fixv3LogicPosition = new FixVector3(CLockStepMgr.Ins.GetRandomInt(-10, 10),
                                                              0,
                                                              CLockStepMgr.Ins.GetRandomInt(-10, 10));
                objUnit.ForceRefreshPos();
                objUnit.RecordLastPos();

                objUnit.InitUniqueIdx();
                objUnit.Init();

                CLockStepMgr.Ins.AddLockUnit(objUnit);
            }
        }

        if(GUILayout.Button("换位置"))
        {
            for(int i=0; i<CLockStepMgr.Ins.listUnits.Count; i++)
            {
                CLockUnityObject objUnit = CLockStepMgr.Ins.listUnits[i];
                objUnit.m_fixv3LogicPosition = new FixVector3(CLockStepMgr.Ins.GetRandomInt(-10, 10),
                                                              0,
                                                              CLockStepMgr.Ins.GetRandomInt(-10, 10));
                objUnit.ForceRefreshPos();
                objUnit.RecordLastPos();

                TestLockUnit pPhysicUnit = objUnit as TestLockUnit;
                if (pPhysicUnit == null) continue;

                //pPhysicUnit.pPhysicEntity.RemoveSpace();
                //pPhysicUnit.pPhysicEntity.Init();
                pPhysicUnit.pPhysicEntity.SyncEntityTransFromGameObject(objUnit.m_fixv3LogicPosition, FixVector4.Zero);
            }
        }

        if (GUILayout.Button("清除颜色"))
        {
            for (int i = 0; i < CLockStepMgr.Ins.listUnits.Count; i++)
            {
                CLockUnityObject objUnit = CLockStepMgr.Ins.listUnits[i];
                TestLockUnit pPhysicUnit = objUnit as TestLockUnit;
                if (pPhysicUnit == null) continue;

                pPhysicUnit.pRenderer.material.color = Color.gray;
            }
        }

        if(GUILayout.Button("清除对象"))
        {
            for (int i = 0; i < CLockStepMgr.Ins.listUnits.Count;)
            {
                TestLockUnit pPhysicUnit = CLockStepMgr.Ins.listUnits[i] as TestLockUnit;
                
                if(pPhysicUnit!=null)
                {
                    pPhysicUnit.pPhysicEntity.RemoveSpace();
                    CLockStepMgr.Ins.RemoveLockUnit(pPhysicUnit);

                    GameObject.Destroy(pPhysicUnit.gameObject);
                }
                else
                {
                    i++;
                }
            }

            CLockStepMgr.Ins.listUnits.Clear();
        }

        if(GUILayout.Button("全体去物理"))
        {
            for (int i = 0; i < CLockStepMgr.Ins.listUnits.Count; i++)
            {
                TestLockUnit pPhysicUnit = CLockStepMgr.Ins.listUnits[i] as TestLockUnit;

                if (pPhysicUnit != null)
                {
                    pPhysicUnit.pPhysicEntity.RemoveSpace();
                }
            }
        }

        if (GUILayout.Button("全体加物理"))
        {
            for (int i = 0; i < CLockStepMgr.Ins.listUnits.Count; i++)
            {
                TestLockUnit pPhysicUnit = CLockStepMgr.Ins.listUnits[i] as TestLockUnit;

                if (pPhysicUnit != null)
                {
                    pPhysicUnit.pPhysicEntity.Init();
                }
            }
        }
    }
}
