using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CTBLInfo.Inst.Init();
        //UIManager.Instance.RefreshUI();
        //CDanmuBilibiliMgr.Ins.StartGame("");
    }

    
}
