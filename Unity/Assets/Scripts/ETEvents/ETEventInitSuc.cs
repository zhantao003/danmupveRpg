using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[Event(EventIdType.ETFrameWorkInitFinish)]
public class ETEventInitSuc : AEvent
{
    public override void Run()
    {
        //SceneManager.LoadScene("Login");
    }
}
