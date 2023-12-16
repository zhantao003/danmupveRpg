using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIBindCamera : UICamera
{
    public Camera bindCamera;
    // Start is called before the first frame update

    private float originSize;
    public float zoneVal = 1;
    void Start()
    {
        if (bindCamera != null)
            originSize = bindCamera.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        uiCam.orthographicSize = bindCamera.orthographicSize;
        zoneVal = uiCam.orthographicSize / originSize;
    }
}
