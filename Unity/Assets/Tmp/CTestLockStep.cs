using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestLockStep : MonoBehaviour
{
    float fCurDelta = 0f;
    public bool bPause = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fCurDelta += CTimeMgr.DeltaTime;

        if (!bPause)
        {
            CLockStepMgr.Ins.OnLockUpdate(fCurDelta);
            fCurDelta = 0f;
        }

        //OnUpdateInput();
    }
    public bool bUpSpeed;
    public float fUpValue;
    float fCurSpeed = 1;
    void OnUpdateInput()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            bPause = !bPause;
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            CLockStepMgr.Ins.OnLockUpdate(CTimeMgr.DeltaTimeUnScale);
        }

        if (Input.GetKey(KeyCode.M))
        {
            CLockStepMgr.Ins.OnLockUpdate(CTimeMgr.DeltaTimeUnScale);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            bUpSpeed = !bUpSpeed;
            if (bUpSpeed)
            {
                CTimeMgr.TimeScale = 1f;
            }
            else
            {
                CTimeMgr.TimeScale = 0.1f;
            }

        }
    }
}
