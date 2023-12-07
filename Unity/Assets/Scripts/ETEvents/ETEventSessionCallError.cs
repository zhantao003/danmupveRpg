using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Event(EventIdType.SessionCallError)]
public class ETEventSessionCallError : AEvent
{
    public override void Run()
    {
        Log.Warning("ET Log Out");

        SessionComponent.Instance.Session.callDispose = null;
        //CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.Login);
    }
}
