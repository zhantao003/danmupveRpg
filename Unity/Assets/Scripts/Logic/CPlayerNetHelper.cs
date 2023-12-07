using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerNetHelper
{
    public static void Login(string uid, string roomId, string userName, string headIcon, CPlayerBaseInfo.EMUserType userType, long fanLv, string fanName, bool fanWear, long guarLv, INetEventHandler handler)
    {
        CHttpParam pParamLogin = new CHttpParam
        (
            new CHttpParamSlot("uid", uid),
            new CHttpParamSlot("roomId", roomId),
            new CHttpParamSlot("nickName", System.Net.WebUtility.UrlEncode(userName)),
            new CHttpParamSlot("headIcon", System.Net.WebUtility.UrlEncode(headIcon)),
            new CHttpParamSlot("userType", ((int)userType).ToString()),
            new CHttpParamSlot("fansMedalLevel", fanLv.ToString()),
            new CHttpParamSlot("fansMedalName", fanName),
            new CHttpParamSlot("fansMedalWearingStatus", fanWear.ToString()),
            new CHttpParamSlot("guardLevel", guarLv.ToString())
        );

        CHttpMgr.Instance.SendHttpMsg(
               CHttpConst.LoginViewer,
               handler,
               pParamLogin);
    }
}
