using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CTBLConfigSlot 
{
    public int nID;

    public abstract void InitByLoader(CTBLLoader loader);
}
