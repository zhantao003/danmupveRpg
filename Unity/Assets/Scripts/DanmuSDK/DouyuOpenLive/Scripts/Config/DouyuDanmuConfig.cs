using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyuDanmu
{
    [UnityEngine.CreateAssetMenu(fileName = "DouyuConfig", menuName = "DanmuConfig/Douyu")]
    public class DouyuDanmuConfig : ScriptableObject
    {
        public string appId;
        public string appKey;
        public string roomId;
        public string version;
    }
}

