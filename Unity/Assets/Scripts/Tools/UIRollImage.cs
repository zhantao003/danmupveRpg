using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIRollImage : MonoBehaviour
{
    public Image uiImg;

    public float fCurValue;

    public float fRollSpeed;

    public void FixedUpdate()
    {
        fCurValue += CTimeMgr.FixedDeltaTime * fRollSpeed;
        uiImg.material.SetTextureOffset("_MainTex", new Vector2(0, fCurValue));
        if(fCurValue >= 15)
        {
            fCurValue = 0;
        }
    }

}
