using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DouyuDanmu
{
    public class DouyuConst
    {
        /// <summary>
        /// 获取Token
        /// </summary>
        public const string req_getToken = "/api/gw/token";

        /// <summary>
        /// 游戏开始
        /// </summary>
        public const string req_startGame = "/api/gw/game/in";

        /// <summary>
        /// 游戏结束
        /// </summary>
        public const string req_endGame = "/api/gw/game/over";
    }
}

