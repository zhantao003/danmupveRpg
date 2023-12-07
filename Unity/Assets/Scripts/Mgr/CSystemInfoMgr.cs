using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSystemInfoConst
{
    public const string ALLSOUND = "allsound";  // 主音量
    public const string AUDIO = "audio";        // 音效
    public const string BGM = "bgm";            // 音乐
    public const string FULLSCREEN = "fullscreen";  //全屏
    public const string RESOLUTIONX = "resolutionx";    //分辨率X
    public const string RESOLUTIONY = "resolutiony";    //分辨率Y
    public const string FPS = "fps";            //帧率
    public const string VUTBERCODE = "vcode";    //主播身份码
}

public class CSystemInfoMgr
{
    #region Instance

    private CSystemInfoMgr() { }
    private static CSystemInfoMgr m_Instance = null;
    public static CSystemInfoMgr Inst
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CSystemInfoMgr();
            }

            return m_Instance;
        }
    }
    
    #endregion

    protected bool m_bInited = false;

    CLocalNetMsg pSaveData = null;
    
    public void Init()
    {
        if (!m_bInited)
        {
            ///读取语言类型
            if (!LoadSystem(CAppPathMgr.LOCALSAVEDATA_DIR + CAppPathMgr.SaveFile_SystemConfig))
            {
                CreatNewSystemFile();
            }

            Debug.LogWarning("SystemInfo 加载完成");
            
            m_bInited = true;
        }
    }
    
    /// <summary>
    /// 创建一个新的系统文件
    /// </summary>
    void CreatNewSystemFile()
    {
        if (pSaveData == null)
            pSaveData = new CLocalNetMsg();

        ///默认打开音频和背景音乐
        SaveAllSoundSet(100);
        SaveAudioSet(100);
        SaveBgmSet(100);

        if(Application.platform == RuntimePlatform.WindowsEditor ||
           Application.platform == RuntimePlatform.WindowsPlayer)
        {
            //Resolution pCurResolution = Screen.resolutions[Screen.resolutions.Length - 1];
            SetResolution(1600, 900, false);
        }

        SetFPS(60);

        ///没找到该文件 创建一个新的该文件
        SaveFile();
    }

    public int GetInt(string key)
    {
        int nRes = pSaveData.GetInt(key);

        return nRes;
    }

    public bool GetBool(string key)
    {
        bool bRes = pSaveData.GetBool(key);

        return bRes;
    }

    public string GetString(string key)
    {
        string szRes = pSaveData.GetString(key);

        return szRes;
    }

    /// <summary>
    /// 开关总音量
    /// </summary>
    /// <param name="nOpen"></param>
    public void SaveAllSoundSet(int value)
    {
        pSaveData.SetInt(CSystemInfoConst.ALLSOUND, value);

        CAudioMgr.Ins.MainVolum = (float)value * 0.01F;
    }

    /// <summary>
    /// 开关音频
    /// </summary>
    /// <param name="nOpen"></param>
    public void SaveAudioSet(int value)
    {
        pSaveData.SetInt(CSystemInfoConst.AUDIO, value);

        CAudioMgr.Ins.VolumSound = (float)value * 0.01F;
    }

    /// <summary>
    /// 开关背景音乐
    /// </summary>
    /// <param name="nOpen"></param>
    public void SaveBgmSet(int value)
    {
        pSaveData.SetInt(CSystemInfoConst.BGM, value);

        CAudioMgr.Ins.VolumMusic = (float)value * 0.01F;
    }

    /// <summary>
    /// 设置屏幕的分辨率
    /// </summary>
    /// <param name="nX"></param>
    /// <param name="nY"></param>
    public void SetResolution(int nX,int nY, bool fullscreen = true)
    {
        pSaveData.SetBool(CSystemInfoConst.FULLSCREEN, fullscreen);
        pSaveData.SetInt(CSystemInfoConst.RESOLUTIONX, nX);
        pSaveData.SetInt(CSystemInfoConst.RESOLUTIONY, nY);

        Screen.fullScreen = fullscreen;
        Screen.SetResolution(nX, nY, fullscreen);
    }

    /// <summary>
    /// 设置游戏帧率
    /// </summary>
    /// <param name="nFPS"></param>
    public void SetFPS(int nFPS)
    {
        pSaveData.SetInt(CSystemInfoConst.FPS, nFPS);
        Application.targetFrameRate = nFPS;
    }

    public void SetVCode(string code)
    {
        pSaveData.SetString(CSystemInfoConst.VUTBERCODE, code);
    }

    public void SaveFile()
    {
        LocalFileManage.Ins.SaveFileAsyc(CAppPathMgr.SaveFile_SystemConfig, pSaveData.GetData(), CAppPathMgr.LOCALSAVEDATA_DIR);
    }

    /// <summary>
    /// 读取语言类型信息
    /// </summary>
    public bool LoadSystem(string strPath)
    {
        if (!File.Exists(strPath))
        {
            return false;
        }

        try
        {
            string strText = LocalFileManage.Ins.LoadFileInfo(strPath);
            pSaveData = new CLocalNetMsg(strText);

            //音频相关
            CAudioMgr.Ins.MainVolum = (float)(pSaveData.GetInt(CSystemInfoConst.ALLSOUND)) * 0.01F;
            CAudioMgr.Ins.VolumSound = (float)(pSaveData.GetInt(CSystemInfoConst.AUDIO)) * 0.01F;
            CAudioMgr.Ins.VolumMusic = (float)(pSaveData.GetInt(CSystemInfoConst.BGM)) * 0.01F;

            //设置屏幕相关信息
            bool bFullScreen = pSaveData.GetBool(CSystemInfoConst.FULLSCREEN);
            Screen.fullScreen = bFullScreen;

            Screen.SetResolution(pSaveData.GetInt(CSystemInfoConst.RESOLUTIONX),
                                 pSaveData.GetInt(CSystemInfoConst.RESOLUTIONY),
                                 bFullScreen);

            Application.targetFrameRate = pSaveData.GetInt(CSystemInfoConst.FPS);

            return true;
        }
        catch(System.Exception e)
        {
            return false;
        }
    }
}
