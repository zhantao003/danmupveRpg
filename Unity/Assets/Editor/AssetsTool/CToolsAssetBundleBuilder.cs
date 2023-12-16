using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System;

/// <summary>
/// AssetBundle打包器
/// </summary>
public class CToolsAssetBundleBuilder : EditorWindow
{
    public const string szABToolBar = "ABTool";

    public const string PATHTMP = "tmp/";

    private BuildType buildType;
    private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;

    /// <summary>
    /// 获取打包AB包的输出路径
    /// </summary>
    /// <returns></returns>
    public static string GetABPath()
    {
        string szRes = CAppPathMgr.AssetBundleLocalDir;
        if (!Directory.Exists(szRes))
        {
            Debug.Log("创建路径:" + szRes);
            Directory.CreateDirectory(szRes);
        }

        return szRes;
    }

    /// <summary>
    /// 临时文件夹
    /// </summary>
    /// <returns></returns>
    static string GetTmpPath()
    {
        string szRes = CAppPathMgr.AssetBundleLocalDir + PATHTMP;
        if (!Directory.Exists(szRes))
        {
            Debug.Log("创建路径:" + szRes);
            Directory.CreateDirectory(szRes);
        }

        return szRes;
    }

    /// <summary>
    /// 打包所有AB包
    /// </summary>
    [MenuItem(szABToolBar + "/Build/【1】生成所有 AssetBundles 文件", false, 0)]
    static void BuildAll()
    {
        //DateTime pStartTime = DateTime.Now;

        //string AssetBundlesPath = GetABPath();

        //Debug.Log("打包平台：" + EditorUserBuildSettings.activeBuildTarget);
        //BuildPipeline.BuildAssetBundles(AssetBundlesPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        //AssetDatabase.Refresh();
        //Debug.Log("打包耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");

        GetWindow(typeof(CToolsAssetBundleWindows));
    }

    private void OnGUI()
    {
        string AssetBundlesPath = GetABPath();
        this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumPopup("BuildAssetBundleOptions: ", this.buildAssetBundleOptions);

        if (GUILayout.Button("开始打包"))
        {
            DateTime pStartTime = DateTime.Now;
            Debug.Log("打包平台：" + EditorUserBuildSettings.activeBuildTarget);
            BuildPipeline.BuildAssetBundles(AssetBundlesPath, buildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打包耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");
        }
    }

    /// <summary>
    /// 打包选择项
    /// </summary>
    [MenuItem(szABToolBar + "/Build/【2】生成当前选择 AssetBundles 文件", false, 0)]
    static void BuildSelection()
    {
        //获取当前选择项
        string[] assetGUIDs = Selection.assetGUIDs;
        if (assetGUIDs == null || assetGUIDs.Length <= 0) return;

        //查询所有用到的AssetBundle标签
        List<string> listABName = new List<string>();
        for (int i = 0; i < assetGUIDs.Length; i++)
        {
            string szPath = AssetDatabase.GUIDToAssetPath(assetGUIDs[i]);

            string szAssetBundleName = AssetDatabase.GetImplicitAssetBundleName(szPath);
            if (string.IsNullOrEmpty(szAssetBundleName))
            {
                continue;
            }

            if (!listABName.Contains(szAssetBundleName))
            {
                listABName.Add(szAssetBundleName);
            }
        }

        //添加打包任务集合
        List<AssetBundleBuild> listBundleRequest = new List<AssetBundleBuild>();
        for (int i = 0; i < listABName.Count; i++)
        {
            //Debug.Log("AB Name:" + listABName[i]);
            string[] arrAssetsInAB = AssetDatabase.GetAssetPathsFromAssetBundle(listABName[i]);
            if (arrAssetsInAB != null && arrAssetsInAB.Length > 0)
            {
                AssetBundleBuild pBuildRequest = new AssetBundleBuild();
                pBuildRequest.assetBundleName = listABName[i];
                pBuildRequest.assetNames = arrAssetsInAB;
                if (pBuildRequest.assetNames != null && pBuildRequest.assetNames.Length > 0)
                {
                    listBundleRequest.Add(pBuildRequest);
                }
            }
        }

        DateTime pStartTime = DateTime.Now;

        //打包
        string AssetBundlesPath = GetABPath();
        if(listBundleRequest.Count > 0)
        {
            AssetBundleManifest pMainfest = BuildPipeline.BuildAssetBundles(AssetBundlesPath, listBundleRequest.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
             
            //CopyABtoPath(pMainfest, GetABPath());
        }

        Debug.Log("打包耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");
    }

    /// <summary>
    /// 拷贝所有的AB到指定路径
    /// </summary>
    /// <param name="pManifest"></param>
    /// <param name="szPath"></param>
    static void CopyABtoPath(AssetBundleManifest pMainfest, string szPath)
    {
        string[] szTargetABs = pMainfest.GetAllAssetBundles();
        if (szTargetABs == null || szTargetABs.Length <= 0) return;
        
        for(int i=0; i<szTargetABs.Length; i++)
        {
            Debug.Log("生成的AB包: " + szTargetABs[i]);
        }
    }
}
