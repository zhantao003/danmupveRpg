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

        public KSDanmuConfig pStaticConfig;     //ƽ̨�������
        public KsOpenEventHandler pEventHandler;    //�¼�������

        [ReadOnly]
        public string szToken;  //��Ȩ�õ�Token
        [ReadOnly]
        public string szUid;    //����uid
        [ReadOnly]
        public string szRoomID; //����ID
        [ReadOnly]
        public string szNickName = "";
        [ReadOnly]
        public string szHeadIcon = "";

        KsInteractSdkDll pClient = null;
        KsOpenCallback pCallBack = null;

        //����ģʽ
        public bool bDebug;

        void Start()
        {
            ins = this;
        }

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="callSuc"></param>
        public async void StartConnect(string code, System.Action<int> callSuc = null)
        {
            pCallBack = new KsOpenCallback();
            pCallBack.dlgConnectSuc = callSuc;
            pCallBack.pEventHandler = pEventHandler;
            pCallBack.pClient = this;

            Debug.Log("����AppId��" + pStaticConfig.szAppId);
            pClient = new KsInteractSdkDll(pStaticConfig.szAppId, pCallBack);

            string szExtra = "{\"playId\":" + pStaticConfig.szPlayId + "}";
            //string szExtra = "";

            bool res = false;
            Debug.Log("���ַ����룺" + code);
            res = await pClient.Connect(code, szExtra);

            if (!res)
            {
                Debug.LogError("����ʧ��");
                callSuc?.Invoke(-1);
                return;
            }

            callSuc.Invoke(0);
        }

        public async void CloseConnect(System.Action call = null)
        {
            Debug.Log("�رտ��ֵ�Ļ����");

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