using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    public enum EMType
    {
        Default,
        Uppon,
    }

    public EMType emType = EMType.Default;

    public Camera uiCam;
}
