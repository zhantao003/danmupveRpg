using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMUnitBase : FSMBaseState
{
    public CPlayerUnit pUnit;

    public override void OnReady(object obj)
    {
        pUnit = obj as CPlayerUnit;
    }
}