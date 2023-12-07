using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
    /// <param name="pParams"></param>
    public void SendHttpMsg(string szRequestType, CHttpParam pParams = null, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (pParams == null)
        {
            SendDirectRequest(
                szFinalUrl + szRequestType,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect,
                engryt);
        }
        else
        {
            SendRequest(
                szFinalUrl + szRequestType,
                pParams,
                pHandlerMgr.GetHandler(szRequestType),
                reconnect,
                engryt);
        }
    }

    public void SendHttpMsg(string szRequestType, INetEventHandler handler, CHttpParam pParams = null, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (pParams == null)
        {
            SendDirectRequest(
                szFinalUrl + szRequestType,
                handler,
                reconnect,
                engryt);
        }
        else
        {
            SendRequest(
                szFinalUrl + szRequestType,
                pParams, 
                handler,
                reconnect,
                engryt);
        }
    }

    /// <summary>
    /// 发送请求（带用户的登录token)
    /// </summary>
    public void SendHttpMsgWithToken(string szRequestType, CHttpParam pParams = null, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (pParams == null)
        {
            pParams = new CHttpParam(new CHttpParamSlot("roomId", szUserId),
                                     new CHttpParamSlot("token", szToken));

            SendRequest(szFinalUrl +
                szRequestType,
                pParams, pHandlerMgr.GetHandler(szRequestType), reconnect, engryt);
        }
        else
        {
            pParams.AddSlot(new CHttpParamSlot("roomId", szUserId));
            pParams.AddSlot(new CHttpParamSlot("token", szToken));

            SendRequest(szFinalUrl +
                szRequestType,
                pParams, pHandlerMgr.GetHandler(szRequestType), reconnect, engryt);
        }
    }

    public void SendHttpMsgWithToken(string szRequestType, INetEventHandler handler, CHttpParam pParams = null, int reconnect = 0, bool engryt = false)
    {
        if (Instance == null)
        {
            Debug.LogError("None Http GameObject");
            return;
        }

        string szFinalUrl = szUrl + ((nPort == 0) ? "" : (":" + nPort)) + "/";

        if (pParams == null)
        {
            pParams = new CHttpParam(new CHttpParamSlot("roomId", szUserId),
                                     new CHttpParamSlot("token", szToken));

            SendRequest(
                szFinalUrl + szRequestType,
                pParams,
                handler, 
                reconnect,
                engryt);
        }
        else
        {
            pParams.AddSlot(new CHttpParamSlot("roomId", szUserId));
            pParams.AddSlot(new CHttpParamSlot("token", szToken));

            SendRequest(
                szFinalUrl + szRequestType,
                pParams,
                handler,
                reconnect,
                engryt);
        }
    }

    protected void SendRequest(string url, CHttpParam data, INetEventHandler handler = null, int reconnect = 0, bool engryt = false)
    {
        WWWForm dataForm = new WWWForm();
       
        if(engryt)
        {
            dataForm.AddField("data", CEncryptHelper.AesEncrypt(data.ToJsonMsg().GetData()));
        }
        else
        {
            if (data != null)
            {
                for (int i = 0; i < data.listParams.Count; i++)
                {
                    dataForm.AddField(data.listParams[i].szKey, data.listParams[i].szValue);
                }
            }
        }

        StartCoroutine(POST(url, dataForm, handler, reconnect, engryt));
    }

    protected void SendDirectRequest(string url, INetEventHandler handler = null, int reconnect = 0, bool engryt = false)
    {
        StartCoroutine(SimPlePOST(url, handler, reconnect));
    }

    private IEnumerator POST(string url, WWWForm data, INetEventHandler handler = null, int reconnect = 0, bool engryt = false)
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
                UIToast.Show("服务器更新中，请稍候");
                CTimeTickMgr.Inst.PushTicker(1.8F, delegate (object[] values)
                {
                    StartCoroutine(POST(url, data, handler, reconnect - 1));
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
            string errCode = pMsg.GetString("status");
            if (!errCode.Equals("ok"))
            {
                handler.OnErrorCode(pMsg);
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }

            handler.OnMsgHandler(pMsg);
        }

        yield break;
    }

    private IEnumerator SimPlePOST(string url, INetEventHandler handler = null, int reconnect = 0)
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
                UIToast.Show("服务器更新中，请稍候");
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
            string errCode = pMsg.GetString("status");
            if (!errCode.Equals("ok"))
            {
                UIToast.Show("网络连接错误");
                UIManager.Instance.CloseUI(UIResType.NetWait);
                yield break;
            }

            handler.OnMsgHandler(pMsg);
        }

        //UIManager.Instance.CloseUI(UIResType.WaitNet);
        yield break;
    }
}
