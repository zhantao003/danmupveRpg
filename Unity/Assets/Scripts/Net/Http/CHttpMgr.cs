using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using SharedLibrary;
using Newtonsoft.Json;
using UnityEngine.Networking;
using Sirenix.Utilities;
using System.Xml;

public class CHttpMgr : MonoBehaviour {
    #region Instance

    private static CHttpMgr __instatnc__ = null;
    public static CHttpMgr Instance
    {
        get
        {
            //__instatnc__ = FindObjectOfType<CHttpMgr>();

            if (__instatnc__ == null)
            {
                GameObject gameobject = new GameObject("[Game - HttpMgr]");
                DontDestroyOnLoad(gameobject);
                __instatnc__ = gameobject.AddComponent<CHttpMgr>();
            }

            return __instatnc__;
        }

    }
    #endregion

    public bool bDebug = false;

    public string szUrl = "127.0.0.1";
    public int nPort = 12347;

    public int nReconnectTimes = 10;

    public string szUserId = "";
    public string szToken = "";
    public DateTime serverUTCBase;
    public DateTime localTimeBase;

    protected CHttpEventHandler pHandlerMgr = new CHttpEventHandler();

    public void Init()
    {
        bDebug = true;
        szUrl = CNetConfigMgr.Ins.GetHttpServerIP();
        nPort = CNetConfigMgr.Ins.GetHttpServerPort();
    }

	// Use this for initialization
	void Awake () {
#if UNITY_EDITOR
        bDebug = true;
#endif
    }

	// Update is called once per frame
	void Update () {

    }

    /// <summary>
    /// 发送HTTP消息
    /// </summary>
    /// <param name="szRequestType"></param>
    /// <param name="requestObject"></param>
    public void SendHttpMsg(string szRequestType, string requestStr , bool needToken = true, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (requestStr .IsNullOrWhitespace())
        {
            SendDirectRequest(
                szFinalUrl + szRequestType,
                  needToken,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect
                );
        }
        else
        {
            SendRequest(
                szFinalUrl + szRequestType,
                requestStr,
                  needToken,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect,
                engryt);
        }
    }

    public void SendHttpMsg(string szRequestType, string requestStr, INetEventHandler handler, bool needToken = true, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (requestStr.IsNullOrWhitespace())
        {
            SendDirectRequest(
                szFinalUrl + szRequestType,
                needToken,
                handler,
                reconnect
                );
        }
        else
        {
            SendRequest(
                szFinalUrl + szRequestType,
                requestStr,
                needToken,
                handler,
                reconnect,
                engryt);
        }
    }

    protected void SendRequest(string url,string requestStr, bool needToken = true, INetEventHandler handler = null,int reconnect = 0, bool engryt = false)
    {
        StartCoroutine(POST(url, requestStr, needToken, handler, reconnect,engryt));
    }

    protected void SendDirectRequest(string url, bool needToken = true, INetEventHandler handler = null, int reconnect = 0)
    {
        StartCoroutine(SimPlePOST(url, handler, reconnect));
    }

    private IEnumerator POST(string url,string requestStr, bool needToken = true, INetEventHandler handler = null, int reconnect = 0, bool engryt = false)
    {
        UnityWebRequest www = new UnityWebRequest(url, RequestType.POST.ToString());
        www.timeout = 8;
        www.SetRequestHeader("Content-Type", "application/json");
        string token = "Bearer " + szToken;
        if (needToken && !szToken.IsNullOrWhitespace())
        {
            www.SetRequestHeader("Authorization", token);
        }
        if (engryt)
        {
            long timeStamp = CHelpTools.GetServerTimeStamp(serverUTCBase.AddSeconds((DateTime.Now - localTimeBase).TotalSeconds));
            //Debug.Log(requestStr + timeStamp);
            string nonceStr = CEncryptHelper.MD5Encrypt(requestStr + timeStamp);
            www.SetRequestHeader("NonceStr", nonceStr);
            www.SetRequestHeader("Timestamp", timeStamp.ToString());
            www.SetRequestHeader("Signature", CEncryptHelper.SHA256Encrypt(CEncryptHelper.AesEncrypt(token + requestStr + nonceStr + timeStamp)));

            SecureRequest req = new SecureRequest();
            req.Data = requestStr;
            var bodyRaw = Encoding.UTF8.GetBytes(req.GetJsonMsg().GetData());
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }
        else {
            if (!requestStr.IsNullOrWhitespace())
            {
                var bodyRaw = Encoding.UTF8.GetBytes(requestStr);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            }
        }
      
        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError ||
           www.result == UnityWebRequest.Result.ProtocolError ||
           www.result == UnityWebRequest.Result.DataProcessingError)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Request Failed:\n" + (url.IndexOf('?') == -1 ? url + "?" : url + "&") + requestStr + "\nServer Error:\n" + www.error);
#endif
            if (reconnect > 0)
            {
                Debug.Log("网络请求重发");
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netgenxin"));
                CTimeTickMgr.Inst.PushTicker(1.8F, delegate (object[] values)
                {
                    StartCoroutine(POST(url, requestStr, needToken, handler,  reconnect - 1,engryt));
                });
            }

            yield break;
        }

        //处理接受到的
        string response = www.downloadHandler.text;
#if UNITY_EDITOR
        Debug.Log("Request Suc:\n" + (url.IndexOf('?') == -1 ? url + "?" : url + "&") + requestStr + "\nServer response:\n" + response);
#endif
        string failReason = "json 解析错误";
        JSONNode jsonNode = JSON.Parse(response);
        if (jsonNode != null && jsonNode.HasKey("FailReason"))
        {
            failReason = jsonNode["FailReason"];
        }
        //IBaseResponse responsData = (IBaseResponse)CHelpTools.DeserializeObject(response ,typeof(IBaseResponse));
        if (handler != null)
        {
#if UNITY_EDITOR
            Debug.Log("【Receive Msg】" + response);
#endif
            //处理通用的errorCode
            string errCode = failReason;
            if (!errCode.IsNullOrWhitespace())
            {
                handler.OnErrorCode(errCode);
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }
            //response 为http原生返回的json，为什么属性首字母是小写
            handler.OnMsgHandler(response);
        }

        yield break;
    }

    private IEnumerator SimPlePOST(string url, INetEventHandler handler = null, int reconnect = 0)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 8;

        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        if (szToken != string.Empty)
        {
            www.SetRequestHeader("Authorization", "Bearer " + szToken);
        }

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError ||
            www.result == UnityWebRequest.Result.DataProcessingError)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Request Failed:\n" + url + "\nServer Error:\n" + www.error);
#endif

            if (reconnect > 0)
            {
                Debug.Log("网络请求重发");
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netgenxin"));
                CTimeTickMgr.Inst.PushTicker(1.8F, delegate (object[] values)
                {
                    StartCoroutine(SimPlePOST(url, handler, reconnect - 1));
                });
            }

            yield break;
        }

        //处理接受到的
        string response = www.downloadHandler.text;
#if UNITY_EDITOR
        Debug.Log("Request Suc:\n" + url + "\nServer response:\n" + response);
#endif
        JSONNode jsonNode = JSON.Parse(response);
        string failReason = jsonNode["FailReason"];
        if (handler != null)
        {
#if UNITY_EDITOR
            Debug.Log("【Receive Msg】" + response);
#endif
            //处理通用的errorCode
            string errCode = failReason;
            if (!errCode.IsNullOrWhitespace())
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netconnecterrot"));
                handler.OnErrorCode(errCode);
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }

            handler.OnMsgHandler(response);
        }
        yield break;
    }
    private enum RequestType
    {
        GET = 0,
        POST = 1,
        PUT = 2
    }

    #region HttpParams
    /// <summary>
    /// 发送HTTP消息
    /// </summary>
    /// <param name="szRequestType"></param>
    /// <param name="pParams"></param>
    public void SendHttpMsgWWWForms(string url, int port, string szRequestType, CHttpParam pParams = null, int reconnect = 0)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = url + ((port == 0) ? "" : (":" + port)) + "/";

        if (pParams == null)
        {
            SendDirectRequestWWWForms(
                szFinalUrl + szRequestType,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect);
        }
        else
        {
            SendRequestWWWForms(
                szFinalUrl + szRequestType,
                pParams,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect);
        }
    }

    public void SendHttpMsgWWWForms(string szRequestType, INetEventHandler handler, CHttpParam pParams = null, int reconnect = 0)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (pParams == null)
        {
            SendDirectRequestWWWForms(
                szFinalUrl + szRequestType,
                handler,
                reconnect);
        }
        else
        {
            SendRequestWWWForms(
                szFinalUrl + szRequestType,
                pParams,
                handler,
                reconnect);
        }
    }

    protected void SendRequestWWWForms(string url, CHttpParam data, INetEventHandler handler = null, int reconnect = 0)
    {
        WWWForm dataForm = new WWWForm();

        if (data != null)
        {
            for (int i = 0; i < data.listParams.Count; i++)
            {
                dataForm.AddField(data.listParams[i].szKey, data.listParams[i].szValue);
            }

            for (int i = 0; i < data.listParamsInt.Count; i++)
            {
                dataForm.AddField(data.listParamsInt[i].szKey, data.listParamsInt[i].nValue.ToString());
            }
        }

        StartCoroutine(POSTForms(url, dataForm, handler, reconnect));
    }

    protected void SendDirectRequestWWWForms(string url, INetEventHandler handler = null, int reconnect = 0)
    {
        StartCoroutine(SimPlePOSTForms(url, handler, reconnect));
    }

    private IEnumerator POSTForms(string url, WWWForm data, INetEventHandler handler = null, int reconnect = 0)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, data);
        www.timeout = 8;
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
           www.result == UnityWebRequest.Result.ProtocolError ||
           www.result == UnityWebRequest.Result.DataProcessingError)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Request Failed:\n" + (url.IndexOf('?') == -1 ? url + "?" : url + "&") + Encoding.UTF8.GetString(data.data) + "\nServer Error:\n" + www.error);
#endif

            if (reconnect > 0)
            {
                Debug.Log("网络请求重发");
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netgenxin"));
                CTimeTickMgr.Inst.PushTicker(1.8F, delegate (object[] values)
                {
                    StartCoroutine(POSTForms(url, data, handler, reconnect - 1));
                });
            }

            yield break;
        }

        //处理接受到的
        string response = www.downloadHandler.text;
#if UNITY_EDITOR
        Debug.Log("Request Suc:\n" + (url.IndexOf('?') == -1 ? url + "?" : url + "&") + Encoding.UTF8.GetString(data.data) + "\nServer response:\n" + response);
#endif

        response = response.Trim();
        CLocalNetMsg pMsg = new CLocalNetMsg(response);
        if (handler != null)
        {
#if UNITY_EDITOR
            Debug.Log("【Receive Msg】" + pMsg.GetData());
#endif

            //处理通用的errorCode
            string errCode = pMsg.GetString("state");
            if (!errCode.Equals("ok"))
            {
                handler.OnErrorCode("errCode");
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }
            //if (pHandlerMgr.OnCommonError(errCode))    
            //{
            //    UIToast.Show("网络连接错误");
            //    UIManager.Instance.CloseUI(UIResType.NetWait);
            //    yield break;
            //}

            handler.OnMsgHandler(pMsg.GetData());
        }

        //UIManager.Instance.CloseUI(UIResType.WaitNet);
        yield break;
    }

    private IEnumerator SimPlePOSTForms(string url, INetEventHandler handler = null, int reconnect = 0)
    {
        UnityWebRequest www = UnityWebRequest.Get(url);
        www.timeout = 8;
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError ||
            www.result == UnityWebRequest.Result.DataProcessingError)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Request Failed:\n" + url + "\nServer Error:\n" + www.error);
#endif

            if (reconnect > 0)
            {
                Debug.Log("网络请求重发");
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netgenxin"));
                CTimeTickMgr.Inst.PushTicker(1.8F, delegate (object[] values)
                {
                    StartCoroutine(SimPlePOST(url, handler, reconnect - 1));
                });
            }

            yield break;
        }

        //处理接受到的
        string response = www.downloadHandler.text;
#if UNITY_EDITOR
        Debug.Log("Request Suc:\n" + url + "\nServer response:\n" + response);
#endif

        response = response.Trim();
        CLocalNetMsg pMsg = new CLocalNetMsg(response);
        if (handler != null)
        {
#if UNITY_EDITOR
            Debug.Log("【Receive Msg】" + pMsg.GetData());
#endif

            //处理通用的errorCode
            //int errCode = pMsg.GetInt("errorCode");
            //if (pHandlerMgr.OnCommonError(errCode))    //登录Token过期
            //{
            //    UIToast.Show("网络连接错误");
            //    UIManager.Instance.CloseUI(UIResType.NetWait);
            //    yield break;
            //}

            string errCode = pMsg.GetString("state");
            if (!errCode.Equals("ok"))
            {
                UIToast.Show(CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "netconnecterrot"));
                handler.OnErrorCode("errCode");
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }

            handler.OnMsgHandler(pMsg.GetData());
        }

        //UIManager.Instance.CloseUI(UIResType.WaitNet);
        yield break;
    }

    #endregion
}
