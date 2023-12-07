using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSingleMgrBase <T> where T:class
{
    static T ins = null;
    public static T Ins
    {
        get
        {
            if(ins == null)
            {
                ins = (T)Activator.CreateInstance(typeof(T), true);
            }

            return ins;
        }
    }
}

public class CSingleCompBase<T> : MonoBehaviour where T : MonoBehaviour
{
    static T ins = null;

    static readonly object objLock = new object();

    protected static bool bApplicationIsQuiting { get; private set; }

    protected static bool bIsGlobal = true;

    static CSingleCompBase()
    {
        bApplicationIsQuiting = false;
    }

    public static T Ins
    {
        get
        {
            if(bApplicationIsQuiting)
            {
                if(Debug.isDebugBuild)
                {
                    Debug.Log("[Singleton]" + typeof(T) + " already destroyed on application quit.");
                }

                return null;
            }

            lock(objLock)
            {
                if(ins == null)
                {
                    //先寻找场景中现有的Object
                    ins = FindObjectOfType<T>();
                    if(ins!=null)
                    {
                        DontDestroyOnLoad(ins.gameObject);
                    }
                    else
                    {
                        GameObject newInsObj = new GameObject();
                        ins = newInsObj.AddComponent<T>();
                        newInsObj.name = $"[{typeof(T)}]";
                        if (bIsGlobal && Application.isPlaying)
                        {
                            DontDestroyOnLoad(newInsObj);
                        }
                    }
                }

                return ins;
            }
        }
    }

    public void OnApplicationQuit()
    {
        bApplicationIsQuiting = true;
    }
}
