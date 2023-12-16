using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CTestRecord : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if(GUILayout.Button("Õ≥º∆≤‚ ‘"))
        {
            CNetConfigMgr.Ins.Init();
            CHttpMgr.Instance.Init();
            //CHttpMgr.Instance.szUrl = "http://api.jh.biggersun.com";
            //CHttpMgr.Instance.nPort = 0;

            CHttpParam pParams = new CHttpParam();
            pParams.AddSlot(new CHttpParamSlot("game", "battleAnt"));
            pParams.AddSlot(new CHttpParamSlot("version", Application.version));
            pParams.AddSlot(new CHttpParamSlot("platform", "douyin"));
            pParams.AddSlot(new CHttpParamSlot("room", "789456123"));
            pParams.AddSlot(new CHttpParamSlot("price", 10.ToString()));
            pParams.AddSlot(new CHttpParamSlot("nickname", System.Net.WebUtility.UrlEncode("≤‚ ‘‘±")));

            CHttpMgr.Instance.SendHttpMsgWWWForms("http://api.jh.biggersun.com", 0, CHttpRecordConst.AddGift, pParams, 10);
        }
    }
}
