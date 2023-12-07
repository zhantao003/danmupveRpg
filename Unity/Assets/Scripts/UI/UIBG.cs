using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBG : UIBase
{
    public GameObject objBG;

    public void SetActiveBG(bool bActive)
    {
        objBG.SetActive(bActive);
    }

}
