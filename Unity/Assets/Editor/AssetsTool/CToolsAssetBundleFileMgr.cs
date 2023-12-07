using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

/// <summary>
/// 资源文件工具
/// </summary>
public class CToolsAssetBundleFileMgr
{
    /// <summary>
    /// 菜单：拷贝所有外部资源和热更代码库到StreamingAssets
    /// </summary>
    [MenuItem(CToolsAssetBundleBuilder.szABToolBar + "/File/【1】拷贝所有外部资源", false, 0)]
    static void MenuCopyAllFiles()
    {
        DateTime pStartTime = DateTime.Now;

        //拷贝AB文件
        //CopyAssetbundleFiles();

        AssetDatabase.Refresh();

        Debug.Log("拷贝文件耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");
    }

    /// <summary>
    /// 菜单：拷贝Assetbundle文件
    /// </summary>
    [MenuItem(CToolsAssetBundleBuilder.szABToolBar + "/File/【2】拷贝AssetBundle文件", false, 0)]
    static void MenuCopyAB()
    {
        DateTime pStartTime = DateTime.Now;

        //CopyAssetbundleFiles();

        AssetDatabase.Refresh();

        Debug.Log("拷贝文件耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");
    }
    
    /// <summary>
    /// 拷贝对应平台的AB文件
    /// </summary>
    static void CopyAssetbundleFiles()
    {
        //获取当前平台下AB包的打包路径
        string szAllABPath = CToolsAssetBundleBuilder.GetABPath();
        DirectoryInfo pRoot = new DirectoryInfo(szAllABPath);
        if (!pRoot.Exists)
        {
            Debug.LogWarning("还没有打包该平台下的AB资源包");
            return;
        }

        //生成拷贝的根目录
        string szCopyRootPath = CToolsAssetBundleBuilder.GetABPath();
        if(Directory.Exists(szCopyRootPath))
        {
            Directory.Delete(szCopyRootPath, true);
        }
        Directory.CreateDirectory(szCopyRootPath);

        //获取所有文件
        List<FileInfo> listFiles = GetAllFilesInPath(pRoot, new string[] { ".manifest" });
        for (int i = 0; i < listFiles.Count; i++)
        {
            string szFilePath = Path.Combine(listFiles[i].DirectoryName, listFiles[i].Name);
            szFilePath = szFilePath.Replace("\\", "/");
            //拆分路径和AB文件名
            string[] szFileInfo = listFiles[i].DirectoryName
                .Replace("\\", "/")
                .Split(new string[] { CResLoadMgr.PATH_PLATFORM }, StringSplitOptions.RemoveEmptyEntries);

            string szABPathName = "";
            if (szFileInfo.Length == 2)
            {
                szABPathName = szFileInfo[1];
            }

            string szABFileName = listFiles[i].Name;

            //生成拷贝到的路径
            string szCopyFilePath = szCopyRootPath + szABPathName;
            if(!Directory.Exists(szCopyFilePath))
            {
                Directory.CreateDirectory(szCopyFilePath);
            }

            string szFinalFilePath = Path.Combine(szCopyFilePath, szABFileName);
            Debug.Log("拷贝AB到：" + szFinalFilePath);
            File.Copy(szFilePath, szFinalFilePath, true);
        }
    }

    /// <summary>
    /// 获取指定路劲下的所有文件
    /// </summary>
    /// <param name="pRoot"></param>
    /// <returns></returns>
    public static List<FileInfo> GetAllFilesInPath(DirectoryInfo pRoot, string [] szExceptExtension = null)
    {
        List<FileInfo> listFileInfo = new List<FileInfo>();

        string szPlatformAB = CResLoadMgr.PATH_PLATFORM.Replace("/", "");

        //检索该目录下的所有文件
        FileInfo[] arrFiles = pRoot.GetFiles();
        if(arrFiles!=null)
        {
            for (int i = 0; i < arrFiles.Length; i++)
            {
                bool bAdd = true;

                //检查是否有排除后缀
                if(szExceptExtension!=null)
                {
                    for(int ex =0; ex<szExceptExtension.Length; ex++)
                    {
                        if(arrFiles[i].Name.Contains(szExceptExtension[ex]))
                        {
                            bAdd = false;
                            break;
                        }
                    }
                }

                //目录AB文件不用拷贝
                if(arrFiles[i].Name.Equals(szPlatformAB))
                {
                    bAdd = false;
                }

                if(!bAdd)
                {
                    continue;
                }

                listFileInfo.Add(arrFiles[i]);
            }
        }

        DirectoryInfo[] pChildFolder = pRoot.GetDirectories();
        for (int i = 0; i < pChildFolder.Length; i++)
        {
            listFileInfo.AddRange(GetAllFilesInPath(pChildFolder[i], szExceptExtension));
        }

        return listFileInfo;
    }
}
