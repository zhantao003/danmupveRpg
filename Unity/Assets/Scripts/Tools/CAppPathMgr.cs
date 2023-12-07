using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 路径统一管理对象
/// </summary>
public class CAppPathMgr
{
    //AB包内部路径
    public static string AssetBundleLocalDir
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Application.streamingAssetsPath + "/appres/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return Application.streamingAssetsPath + "/appres/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.streamingAssetsPath + "/appres/";
            }

            return Application.streamingAssetsPath + "/appres/";
        }
    }

    //AB包外部路径
    public static string AssetBundleHotfixDir
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Application.dataPath + "/../appres/";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                return Application.persistentDataPath + "/appres/";
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                return Application.persistentDataPath + "/appres/";
            }

            return Application.dataPath + "/../appres/";
        }
    }

    //日志文件路径
    public static string LOG_DIR
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
            || Application.platform == RuntimePlatform.WindowsPlayer)
                return Application.dataPath + "/../Log/";
            else if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer)
                return Application.temporaryCachePath + "/Log/";
            else
                return Application.dataPath + "/../Log/";
        }
    }

    //本地存档文件路径
    public static string LOCALSAVEDATA_DIR
    {
        get
        {
            if (Application.platform == RuntimePlatform.WindowsEditor
            ||  Application.platform == RuntimePlatform.WindowsPlayer)
                return Application.dataPath + "/../SaveData/";
            else if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer)
                return Application.persistentDataPath + "/SaveData/";
            else
                return Application.persistentDataPath + "/SaveData/";
        }
    }

    public static string LOCALSAVEDATA_SYS
    {
        get
        {
            return Application.persistentDataPath + "/GameData/";
        } 
    }

    //系统设置存档文件
    public static string SaveFile_SystemConfig = "SystemConfig.dat";

    //房间设置存档文件
    public static string SaveFile_RoomConfig = "RoomConfig.dat";

    //主播任务配置存档文件
    public static string SaveFile_VQuestConfig = "VQuestConfig.dat";

    //加入条件配置存档文件
    public static string SaveFile_JoinInfoConfig = "JoinInfoConfig.dat";

    //房间设置存档文件
    public static string SaveFile_NetConfig = "NetConfig.dat";
}

/// <summary>
/// Assetbundle全局常量管理器
/// </summary>
public class ABConst
{
    public const string TBL = "tbl";
    public const string Role = "role";
    public const string Skill = "skill";
    public const string ShopInfo = "shopinfo";
    public const string Icon = "icon";

    public const string CommonEffect = "effect/common";
}

/// <summary>
/// AB包加载时的外部状态常量
/// </summary>
public enum EMAssetBundleLoadState
{
    Loading = 0,
    Suc =1,
    Fail = 2,
}
