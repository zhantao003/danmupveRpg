using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KsDanmu
{
    public class KsOpenClient : MonoBehaviour
    {
        static KsOpenClient ins = null;
        public static KsOpenClient Ins
        {
            get
            {
                if (ins == null)
                {
                    ins = FindObjectOfType<KsOpenClient>();
                }

                return ins;
            }
        }

        public KSDanmuConfig pStaticConfig;     //平台相关配置
        public KsOpenEventHandler pEventHandler;    //事件管理器

        [ReadOnly]
        public string szToken;  //鉴权用的Token
        [ReadOnly]
        public string szUid;    //主播uid
        [ReadOnly]
        public string szRoomID; //房间ID
        [ReadOnly]
        public string szNickName = "";
        [ReadOnly]
        public string szHeadIcon = "";

        KsInteractSdkDll pClient = null;
        KsOpenCallback pCallBack = null;

        //调试模式
        public bool bDebug;

        void Start()
        {
            ins = this;
        }

        /// <summary>
        /// 启动连接
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callSuc"></param>
        public async void StartConnect(string code, System.Action<int> callSuc = null)
        {
            pCallBack = new KsOpenCallback();
            pCallBack.dlgConnectSuc = callSuc;
            pCallBack.pEventHandler = pEventHandler;
            pCallBack.pClient = this;

            Debug.Log("快手AppId：" + pStaticConfig.szAppId);
            pClient = new KsInteractSdkDll(pStaticConfig.szAppId, pCallBack);

            string szExtra = "{\"playId\":" + pStaticConfig.szPlayId + "}";
            //string szExtra = "";

            bool res = false;
            Debug.Log("快手房间码：" + code);
            res = await pClient.Connect(code, szExtra);

            if (!res)
            {
                Debug.LogError("连接失败");
                callSuc?.Invoke(-1);
                return;
            }

            callSuc.Invoke(0);
        }

        public async void CloseConnect(System.Action call = null)
        {
            Debug.Log("关闭快手弹幕连接");

            if (!string.IsNullOrEmpty(szRoomID))
            {
                szRoomID = "";
            }

            if (!string.IsNullOrEmpty(szUid))
            {
                szUid = "";
            }

            if (!string.IsNullOrEmpty(szToken))
            {
                szToken = "";
            }

            if(pClient!=null)
            {
                bool res = await pClient.Disconnect();
                pClient = null;
            }

            call?.Invoke();
        }

        public bool IsGaming()
        {
            return string.IsNullOrEmpty(szToken);
        }

        void OnDestroy()
        {
            CloseConnect();
        }
    }
}