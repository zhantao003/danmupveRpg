using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLocalNetConst
{
    public const string MSGTYPE = "msg";

    public class MsgLocalType
    {
        public const int MapUnitUnbind = 10001;   //地图事件物体解绑
    }

    public static void ProcMsg(ILocaNetInterface pTarget, CLocalNetMsg pMsg)
    {
        pTarget.OnProLocalMsg(pMsg);
    }
}
