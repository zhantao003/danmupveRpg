using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[CHttpEvent(CHttpConst.HeartBeat)]
public class HHandlerHeartBeat : INetEventHandler
{
    public void OnErrorCode(string failReason)
    {

    }

    public void OnMsgHandler(string resPonseJson)
    {
        HeartBeatResponse resPonse = new HeartBeatResponse();
        resPonse.FillDatas(resPonseJson);
        CHttpMgr.Instance.szToken = resPonse.Token;
    }
}

