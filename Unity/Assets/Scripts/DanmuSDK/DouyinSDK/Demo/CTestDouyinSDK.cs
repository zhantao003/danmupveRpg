using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DouyinDanmu;
using System;
using NativeWebSocket;

namespace DouyinDanmu
{
    public class CTestDouyinSDK : MonoBehaviour
    {
        DouyinYSWebSocket pClient;

        private void OnGUI()
        {
            if (GUILayout.Button("Ұ���ӿ����ӷ�����"))
            {
                if (pClient != null)
                {
                    pClient.Dispose();
                    pClient = null;
                }

                pClient = new DouyinYSWebSocket("ws://127.0.0.1:8888");
                pClient.dlgConnectSuc += ClientConnectSuc;
                pClient.Connect("", TimeSpan.FromSeconds(2), 10);
            }

            if (GUILayout.Button("��Ȩ"))
            {
                DouyinOpenClient.Ins.StartConnect(DouyinOpenClient.Ins.pStaticConfig.szDevID, delegate (int value)
                {
                    if (value != 0)
                    {
                        Debug.LogError("����ƽ̨ʧ��");
                        return;
                    }

                    Debug.Log("����ƽ̨�ɹ�");
                });
            }

            if (GUILayout.Button("�ر�����"))
            {
                DouyinOpenClient.Ins.CloseConnect();
            }
        }

        /// <summary>
        /// �ͻ������ӳɹ�
        /// </summary>
        void ClientConnectSuc()
        {
            Debug.Log("��ʼ��Ϸ�ɹ�");
        }

        private void Update()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            if (pClient is { ws: { State: WebSocketState.Open } })
            {
                pClient.ws.DispatchMessageQueue();
            }
#endif
        }
    }
}

