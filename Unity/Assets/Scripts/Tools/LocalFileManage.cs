using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//文件保存信息
public class ISaveFileInfo
{
    public long lGuid;
    public string szFileName;
    public string szFilePath;
    public string szContent;
    public bool bReplace;

    public DelegateNFuncCall callSuc = null;
}

public class ILoadFileInfo
{
    public long lGuid;
    public string szFilePath;
    public string szContent;
    public DelegateSFuncCall callSuc = null;
}

public class LocalFileManage : CSingleCompBase<LocalFileManage>
{
    bool bIsSavingFile = false;
    bool bIsLoadingFile = false;

    Queue<ISaveFileInfo> listSaveQuest = new Queue<ISaveFileInfo>();

    Queue<ILoadFileInfo> listLoadQuest = new Queue<ILoadFileInfo>();

    /// <summary>
    /// 保存信息到对应文本里面
    /// </summary>
    /// <param name="strSavePath"></param>
    /// <param name="strFileName"></param>
    /// <param name="strContent"></param>
    /// <param name="bReplace"></param>
    public void SaveFile(string strFileName,string strContent, string strSavePath, bool bReplace = true)
    {
        if (!Directory.Exists(strSavePath))
        {
            Directory.CreateDirectory(strSavePath);
        }
        string fullPath = strSavePath + strFileName;

        try
        {
            StreamWriter sWrite = new StreamWriter(fullPath, !bReplace, Encoding.UTF8);
            sWrite.Write(strContent);
            sWrite.Close();
            sWrite.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError("[error]" + e);
        }
    }

    /// <summary>
    /// 读取文本信息
    /// </summary>
    /// <param name="strPath"></param>
    /// <returns></returns>
    public string LoadFileInfo(string strPath)
    {
        if (!File.Exists(strPath))
        {
            return "";
        }
        string strInfo = "";
        try
        {
            //打开文件
            StreamReader sReader = new StreamReader(strPath, Encoding.UTF8);
            strInfo = sReader.ReadToEnd();
            sReader.Close();
            sReader.Dispose();
        }
        catch (Exception e)
        {
            Debug.LogError("[error]" + e);
        }
        return strInfo;
    }

    /// <summary>
    /// 异步写文件
    /// </summary>
    /// <param name="strFileName"></param>
    /// <param name="strContent"></param>
    /// <param name="strSavePath"></param>
    /// <param name="bReplace"></param>
    public void SaveFileAsyc(string strFileName, string strContent, string strSavePath, bool bReplace = true, DelegateNFuncCall call = null)
    {
        ISaveFileInfo pSaveInfo = new ISaveFileInfo();
        pSaveInfo.lGuid = CHelpTools.GenerateId();
        pSaveInfo.szFileName = strFileName;
        pSaveInfo.szFilePath = strSavePath;
        pSaveInfo.szContent = strContent;
        pSaveInfo.bReplace = bReplace;
        pSaveInfo.callSuc = call;

        listSaveQuest.Enqueue(pSaveInfo);

        if(!bIsSavingFile)
        {
            bIsSavingFile = true;
            StartCoroutine(OnSaveFile());
        }
    }

    IEnumerator OnSaveFile()
    {
        while(listSaveQuest.Count > 0)
        {
            ISaveFileInfo curSaveInfo = listSaveQuest.Dequeue();

            Debug.Log("开始写文件任务：" + curSaveInfo.lGuid);
            DateTime now = DateTime.Now;

            //构造存储地址
            if (!Directory.Exists(curSaveInfo.szFilePath))
            {
                Directory.CreateDirectory(curSaveInfo.szFilePath);
            }
            string fullPath = curSaveInfo.szFilePath + curSaveInfo.szFileName;

            StreamWriter sWrite = new StreamWriter(fullPath, !curSaveInfo.bReplace, Encoding.UTF8);
            Task curTask = sWrite.WriteAsync(curSaveInfo.szContent);

            while(!curTask.IsCompleted)
            {
                yield return 0;
            }

            sWrite.Close();
            sWrite.Dispose();

            curSaveInfo.callSuc?.Invoke();

            Debug.Log("任务：" + curSaveInfo.lGuid + "   完成");
            Debug.Log("耗时：" + (DateTime.Now - now).TotalMilliseconds + " MS");
        }

        bIsSavingFile = false;

        yield break;
    }

    /// <summary>
    /// 异步加载文件
    /// </summary>
    /// <param name="strPath"></param>
    /// <param name="call"></param>
    public void LoadFileASync(string strPath, DelegateSFuncCall call = null)
    {
        ILoadFileInfo pLoadInfo = new ILoadFileInfo();
        pLoadInfo.lGuid = CHelpTools.GenerateId();
        pLoadInfo.szFilePath = strPath;
        pLoadInfo.callSuc = call;

        listLoadQuest.Enqueue(pLoadInfo);

        if (!bIsLoadingFile)
        {
            bIsLoadingFile = true;
            StartCoroutine(OnLoadFile());
        }
    }

    IEnumerator OnLoadFile()
    {
        while (listLoadQuest.Count > 0)
        {
            ILoadFileInfo curLoadInfo = listLoadQuest.Dequeue();

            Debug.Log("开始读文件任务：" + curLoadInfo.lGuid);
            DateTime now = DateTime.Now;

            if (!File.Exists(curLoadInfo.szFilePath))
            {
                continue;
            }

            //打开文件
            StreamReader sReader = new StreamReader(curLoadInfo.szFilePath, Encoding.UTF8);
            Task<string> curTask = sReader.ReadToEndAsync();

            while (!curTask.IsCompleted)
            {
                yield return 0;
            }

            curLoadInfo.szContent = curTask.Result;

            sReader.Close();
            sReader.Dispose();

            curLoadInfo.callSuc?.Invoke(curLoadInfo.szContent);

            Debug.Log("任务：" + curLoadInfo.lGuid + "   完成");
            Debug.Log("耗时：" + (DateTime.Now - now).TotalMilliseconds + " MS");
        }

        bIsLoadingFile = false;

        yield break;
    }

    /// <summary>
    /// 获取指定路劲下的所有文件
    /// </summary>
    /// <param name="pRoot"></param>
    /// <returns></returns>
    public static List<FileInfo> GetAllFilesInPath(DirectoryInfo pRoot, string[] szExceptAdd = null)
    {
        List<FileInfo> listFileInfo = new List<FileInfo>();

        //检索该目录下的所有文件
        FileInfo[] arrFiles = pRoot.GetFiles();
        if (arrFiles != null)
        {
            for (int i = 0; i < arrFiles.Length; i++)
            {
                bool bAdd = false;

                //检查是否有排除后缀
                if (szExceptAdd != null)
                {
                    for (int ex = 0; ex < szExceptAdd.Length; ex++)
                    {
                        if (arrFiles[i].Name.Contains(szExceptAdd[ex]))
                        {
                            bAdd = true;
                            break;
                        }
                    }
                }

                if (!bAdd)
                {
                    continue;
                }

                listFileInfo.Add(arrFiles[i]);
            }
        }

        DirectoryInfo[] pChildFolder = pRoot.GetDirectories();
        for (int i = 0; i < pChildFolder.Length; i++)
        {
            listFileInfo.AddRange(GetAllFilesInPath(pChildFolder[i], szExceptAdd));
        }

        return listFileInfo;
    }
}
