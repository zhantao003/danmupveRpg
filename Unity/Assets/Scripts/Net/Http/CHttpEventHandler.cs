using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CHttpEventHandler
{
    public Dictionary<string, INetEventHandler> dicHandler = new Dictionary<string, INetEventHandler>();

    public CHttpEventHandler()
    {
        Init();
    }

    public INetEventHandler GetHandler(string szHandler)
    {
        INetEventHandler pHandler = null;
        if (dicHandler.TryGetValue(szHandler, out pHandler))
        {

        }

        return pHandler;
    }

    public bool OnCommonError(int code)
    {
        if(code != 0)
        {
            return true;
        }

        //if(code == 3)
        //{
        //    return true;
        //}
        //if (code == 1)
        //{
        //    return true;
        //}
        //if (code == 2)
        //{
        //    return true;
        //}

        return false;
    }

    public void Init()
    {
        dicHandler.Clear();

        Assembly assembly = typeof(CHttpEventHandler).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            object[] objects = type.GetCustomAttributes(typeof(CHttpEventAttribute), false);
            if (objects.Length == 0)
            {
                continue;
            }

            CHttpEventAttribute httAttri = (CHttpEventAttribute)objects[0];

            INetEventHandler iHandler = Activator.CreateInstance(type) as INetEventHandler;
            if (iHandler == null)
            {
                Debug.LogError("None Handler:" + httAttri.eventKey);
                continue;
            }

            dicHandler.Add(httAttri.eventKey, iHandler);
        }
    }
}
