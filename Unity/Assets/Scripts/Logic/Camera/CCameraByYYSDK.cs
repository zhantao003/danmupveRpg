using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class CCameraByYYSDK : MonoBehaviour
{
    public Camera pCam;

    [DllImport("nativePlugin")]
    public static extern void UpdataSharedD3D11Texture2D(IntPtr UnityTexture);

    private void Update()
    {
        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            RenderTexture renderTexture = pCam.targetTexture;
            if (renderTexture)
            {
                UpdataSharedD3D11Texture2D(renderTexture.GetNativeTexturePtr());
            }
        }
    }

}
