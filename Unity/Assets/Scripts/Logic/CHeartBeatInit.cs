using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHeartBeatInit : CSingleCompBase<CHeartBeatInit>
{
    public bool isLogin;
    private float heartBeatCounter = 0;
    public float heartBeatGap = 120;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!isLogin)
            return;
        heartBeatCounter += Time.deltaTime;
        if (heartBeatCounter > heartBeatGap)
        {
            heartBeatCounter = 0;
            HeartBeatRequest heartBeatRequest = new HeartBeatRequest(CDanmuSDKCenter.Ins.szNickName, CDanmuSDKCenter.Ins.szHeadIcon, Application.version, CDanmuSDKCenter.Ins.szUid);
            //Debug.Log("heartbeat!");
            CHttpMgr.Instance.SendHttpMsg(CHttpConst.HeartBeat, heartBeatRequest.GetJsonMsg().GetData(), true);
        }
    }
}
