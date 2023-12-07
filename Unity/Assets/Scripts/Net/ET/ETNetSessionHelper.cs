using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETNetSessionHelper 
{
    #region Instance

    private ETNetSessionHelper() { }
    private static ETNetSessionHelper m_Instance = null;
    public static ETNetSessionHelper Inst
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new ETNetSessionHelper();
            }

            return m_Instance;
        }
    }

    #endregion

    /// <summary>
    /// 清除Gate的Session
    /// </summary>
    public void EventClearSession()
    {
        SessionComponent.Instance.Session.Dispose();
        SessionComponent.Instance.Session = null;
    }

    /// <summary>
    /// 登出
    /// </summary>
    public void EventLoginOut()
    {
        SessionComponent.Instance.Session.callDispose = OnLoginOut;
        SessionComponent.Instance.Session.Dispose();
    }

    /// <summary>
    /// Session断开时的操作
    /// </summary>
    public void OnSessionDispose()
    {
        ETGame.EventSystem.Run(EventIdType.SessionDisconnect);
    }

    public void OnSessionCallError()
    {
        ETGame.EventSystem.Run(EventIdType.SessionCallError);
    }

    public void OnLoginOut()
    {
        ETGame.EventSystem.Run(EventIdType.LoginOut);
    }
}
