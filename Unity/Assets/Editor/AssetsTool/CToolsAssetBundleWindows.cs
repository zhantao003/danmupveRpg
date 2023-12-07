using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

public class BundleInfo
{
    public List<string> ParentPaths = new List<string>();
}

public enum PlatformType
{
    None,
    Android,
    IOS,
    PC,
    MacOS,
}

public enum BuildType
{
    Development,
    Release,
}

public class CToolsAssetBundleWindows : EditorWindow
{
    private readonly Dictionary<string, BundleInfo> dictionary = new Dictionary<string, BundleInfo>();

    private PlatformType platformType;
    private bool isBuildExe;
    private bool isContainAB;
    private bool isEngryt;  //是否加密
    private BuildType buildType = BuildType.Release;
    private BuildOptions buildOptions = BuildOptions.AllowDebugging | BuildOptions.Development;
    private BuildAssetBundleOptions buildAssetBundleOptions = BuildAssetBundleOptions.None;

    [MenuItem("Tools/打包工具")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CToolsAssetBundleWindows));
    }

    void OnEnable()
    {
        Debug.Log("开始窗口");

#if UNITY_STANDALONE
        this.platformType = PlatformType.PC;
#elif UNITY_ANDROID
            this.platformType = PlatformType.Android;
#elif UNITY_IPHONE
            this.platformType = PlatformType.IOS;
#endif

        this.isContainAB = true;
        this.isEngryt = true;
        this.buildAssetBundleOptions = BuildAssetBundleOptions.ChunkBasedCompression;
    }


    private void OnGUI()
    {
        GUILayout.BeginHorizontal();
        this.platformType = (PlatformType)EditorGUILayout.EnumPopup(platformType);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        this.isBuildExe = EditorGUILayout.Toggle("是否打包EXE: ", this.isBuildExe);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        this.isContainAB = EditorGUILayout.Toggle("是否同将资源打进EXE: ", this.isContainAB);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        this.isEngryt = EditorGUILayout.Toggle("是否加密: ", this.isEngryt);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        this.buildType = (BuildType)EditorGUILayout.EnumPopup("BuildType: ", this.buildType);
        GUILayout.EndHorizontal();

        switch (buildType)
        {
            case BuildType.Development:
                this.buildOptions = BuildOptions.Development | BuildOptions.AutoRunPlayer | BuildOptions.ConnectWithProfiler | BuildOptions.AllowDebugging;
                break;
            case BuildType.Release:
                this.buildOptions = BuildOptions.None;
                break;
        }

        GUILayout.BeginHorizontal();
        this.buildAssetBundleOptions = (BuildAssetBundleOptions)EditorGUILayout.EnumFlagsField("BuildAssetBundleOptions(可多选): ", this.buildAssetBundleOptions);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("开始打包"))
        {
            //if (this.platformType == PlatformType.None)
            //{
            //    Debug.LogError("请选择打包平台!");
            //    return;
            //}

            string AssetBundlesPath = GetABPath();
            DateTime pStartTime = DateTime.Now;

            Debug.Log("打包平台：" + EditorUserBuildSettings.activeBuildTarget);

            if(isEngryt)
            {
                BuildPipeline.SetAssetBundleEncryptKey(CEncryptHelper.ASSETKEY);
                buildAssetBundleOptions = buildAssetBundleOptions | BuildAssetBundleOptions.EnableProtection;
            }
            else
            {
                BuildPipeline.SetAssetBundleEncryptKey(null);
            }

            BuildPipeline.BuildAssetBundles(AssetBundlesPath, buildAssetBundleOptions, EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打包耗时：" + (DateTime.Now - pStartTime).TotalSeconds + "s");

            //BuildHelper.Build(this.platformType, this.buildAssetBundleOptions, this.buildOptions, this.isBuildExe, this.isContainAB);
        }
    }

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
}
