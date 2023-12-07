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
    /// ���Gate��Session
    /// </summary>
    public void EventClearSession()
    {
        SessionComponent.Instance.Session.Dispose();
        SessionComponent.Instance.Session = null;
    }

    /// <summary>
    /// �ǳ�
    /// </summary>
    public void EventLoginOut()
    {
        SessionComponent.Instance.Session.callDispose = OnLoginOut;
        SessionComponent.Instance.Session.Dispose();
    }

    /// <summary>
    /// Session�Ͽ�ʱ�Ĳ���
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
