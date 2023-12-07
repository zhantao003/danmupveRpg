using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "BilibiliConfig", menuName = "DanmuConfig/Bilibili")]
public class CDanmuBiliBiliConfig : ScriptableObject
{
    public string accessKeySecret;
    public string accessKeyId;
    public string roomId;
    public string appId;
    public string code;
}
