using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Event(EventIdType.SessionDisconnect)]
public class ETEventSessionDisconnect : AEvent
{
    public override void Run()
    {
        Log.Warning("ET.Session Disconnected");
    }
}
