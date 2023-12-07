using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CLogTools
{
    public static void DebugLogReg()
    {
        Application.logMessageReceived += ProcessException;
    }

    //Log日志输出
    protected static void Log(string szLog)
    {
        //获取当前系统时间
        DateTime now = DateTime.Now;
        string strPath = Path.Combine(CAppPathMgr.LOG_DIR, now.Year + "-" + now.Month + "-" + now.Day);

        if (!Directory.Exists(strPath))
        {
            Directory.CreateDirectory(strPath);
        }

        //Debug.Log(strPath);

        StreamWriter sw = new StreamWriter(Path.Combine(strPath, "Log.txt"), true, Encoding.Unicode);

        if (sw == null)
        {
            Debug.LogWarning("Log" + " Write failed");
            return;
        }

        szLog += "--" + now + "\r\n";
        sw.Write(szLog.ToCharArray(), 0, szLog.Length);
        sw.Close();
    }

    //运行报错Log输出委托
    protected static void ProcessException(string condition, string stackTrace, LogType type)
    {
        if (type == LogType.Warning || 
            type == LogType.Error || 
            type == LogType.Exception)
        {
            Log("[" + type.ToString() + "]:" + condition);
            Log(stackTrace);
        }
    }
}
