using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "QQConfig", menuName = "DanmuConfig/QQ")]
public class QQDanmuConfig : ScriptableObject
{
    public string szAppId;
    public string szAppSecret;
    public string szRoomId;
}
