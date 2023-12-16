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
            if (GUILayout.Button("野生接口连接服务器"))
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

            if (GUILayout.Button("鉴权"))
            {
                DouyinOpenClient.Ins.StartConnect(DouyinOpenClient.Ins.pStaticConfig.szDevID, delegate (int value)
                {
                    if (value != 0)
                    {
                        Debug.LogError("连接平台失败");
                        return;
                    }

                    Debug.Log("连接平台成功");
                });
            }

            if (GUILayout.Button("关闭任务"))
            {
                DouyinOpenClient.Ins.CloseConnect();
            }
        }

        /// <summary>
        /// 客户端连接成功
        /// </summary>
        void ClientConnectSuc()
        {
            Debug.Log("开始游戏成功");
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

