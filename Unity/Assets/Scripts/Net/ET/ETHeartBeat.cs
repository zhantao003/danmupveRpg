using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETHeartBeat : MonoBehaviour
{
    public static ETHeartBeat Ins = null;
    public bool bActive = false;
    public CPropertyTimer pTimerTick = new CPropertyTimer();

    long nlSeverTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        Ins = this;
        pTimerTick.FillTime();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bActive) return;

        if(pTimerTick.Tick(Time.deltaTime))
        {
            if (CBattleMgr.Ins != null &&
                CBattleMgr.Ins.emGameState == CBattleMgr.EMGameState.Gaming &&
                CGameAntGlobalMgr.Ins != null &&
                CGameAntGlobalMgr.Ins.emGameType == CGameAntGlobalMgr.EMGameType.NetPvP  &&
                nlSeverTime == CLockStepData.g_uServerLogicFrame)
            {
                UIDisconnect.Show();
            }
            ETHandlerReqHeartBeat.Request().Coroutine();
            pTimerTick.FillTime();
            nlSeverTime = CLockStepData.g_uServerLogicFrame;
        }
    }
}
