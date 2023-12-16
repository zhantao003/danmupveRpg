using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DouyuDanmu
{
    public static class DouyuApi
    {
        public static string szAppId;
        public static string szAppSecret;
        public static string szVersion;

        /// <summary>
        /// 开放平台域名
        /// </summary>
        private static string szReqUrl = "livelink.douyucdn.cn";

        private const string szReqMethod = "POST";

        public static async Task<string> GetToken(string roomId)
        {
            string postUrl = szReqUrl + DouyuConst.req_getToken;
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            //公共传参
            dicParams.Add("appId", szAppId);
            dicParams.Add("timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString("f0"));
            dicParams.Add("v", szVersion);

            //业务传参
            dicParams.Add("rid", roomId);

            //签名
            dicParams = SignTools.SignParams(dicParams, szAppSecret);

            //发起请求
            var result = await RequestWebUTF8Post(postUrl, szReqMethod, dicParams);

            return result;
        }

        public static async Task<string> StartGame(string token)
        {
            string postUrl = szReqUrl + DouyuConst.req_startGame;
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            //公共传参
            dicParams.Add("appId", szAppId);
            dicParams.Add("timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString("f0"));
            dicParams.Add("v", szVersion);

            //业务传参
            dicParams.Add("token", token);

            //签名
            dicParams = SignTools.SignParams(dicParams, szAppSecret);

            //发起请求
            var result = await RequestWebUTF8Post(postUrl, szReqMethod, dicParams);

            return result;
        }

        public static async Task<string> EndGame(string token)
        {
            string postUrl = szReqUrl + DouyuConst.req_endGame;
            Dictionary<string, string> dicParams = new Dictionary<string, string>();

            //公共传参
            dicParams.Add("appId", szAppId);
            dicParams.Add("timestamp", DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds.ToString("f0"));
            dicParams.Add("v", szVersion);

            //业务传参
            dicParams.Add("token", token);

            //签名
            dicParams = SignTools.SignParams(dicParams, szAppSecret);

            //发起请求
            var result = await RequestWebUTF8Post(postUrl, szReqMethod, dicParams);

            return result;
        }

        private static async Task<string> RequestWebUTF8(string url, string method, Dictionary<string, string> dicParam)
        {
            UnityWebRequest webRequest = new UnityWebRequest(url);
            webRequest.method = method;
            if (dicParam != null)
            {
                foreach (var item in dicParam)
                {
                    Debug.Log("参数：" + item.Key + "=" + item.Value);
                    webRequest.SetRequestHeader(item.Key, item.Value);
                }
            }

            //webRequest.SetRequestHeader("Authorization", auth);
            //webRequest.SetRequestHeader("Accept", "application/json");

            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");

            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            await webRequest.SendWebRequest();
            var text = webRequest.downloadHandler.text;

            webRequest.Dispose();
            return text;
        }

        private static async Task<string> RequestWebUTF8Post(string url, string method, Dictionary<string, string> dicParam)
        {
            UnityWebRequest webRequest = UnityWebRequest.Post(url, dicParam);
            webRequest.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.disposeUploadHandlerOnDispose = true;
            webRequest.disposeDownloadHandlerOnDispose = true;
            await webRequest.SendWebRequest();
            var text = webRequest.downloadHandler.text;

            webRequest.Dispose();
            return text;
        }

        private static TaskAwaiter GetAwaiter(this UnityEngine.AsyncOperation asyncOp)
        {
            var tcs = new TaskCompletionSource<object>();
            asyncOp.completed += _ => { tcs.SetResult(null); };
            return ((Task)tcs.Task).GetAwaiter();
        }
    }
}

