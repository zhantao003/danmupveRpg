using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Reflection;

//TBL委托方法
public delegate void DelegateLoadTBL(CTBLLoader loader);

public class CTBLInfo
{
    #region Instance

    private CTBLInfo() { }
    private static CTBLInfo m_Instance = null;
    public static CTBLInfo Inst
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CTBLInfo();
            }
            return m_Instance;
        }
    }
    #endregion

    public bool m_bInited = false;

    // ab 配置
    public AssetBundle pTBLBundle = null;

    #region 配置信息句柄

    Dictionary<string, CTBLConfigBase> dicTBLHandlers = new Dictionary<string, CTBLConfigBase>();
    Dictionary<string, CTBLConfigBase> dicTBLAbsolutHandlers = new Dictionary<string, CTBLConfigBase>();

    //public CTBLHandlerGlobalValueInfo pGlobalConfig = new CTBLHandlerGlobalValueInfo();  //全局变量    
    //public CTBLHanderRandomNameInfo pHanderRandomNameInfo = new CTBLHanderRandomNameInfo();  //随机名字

    #endregion

    public void Init()
    {
        if (!m_bInited)
        {
            // 加载 AB
            string p = Path.Combine(CAppPathMgr.AssetBundleHotfixDir, ABConst.TBL);
            if (File.Exists(p))
            {
                pTBLBundle = AssetBundle.LoadFromFile(p);
            }
            else
            {
                try
                {
                    p = Path.Combine(CAppPathMgr.AssetBundleLocalDir, ABConst.TBL);
                    pTBLBundle = AssetBundle.LoadFromFile(p);
                }
                catch (Exception e)
                {
                    Debug.LogError($"加载资源报错，路径：{p}");
                }
            }



            CheckAllHandlers();

            foreach (string tbls in dicTBLHandlers.Keys)
            {
                Debug.Log("加载配表:" + tbls);
#if TBL_LOCAL
                LoadTBL($"TBL/{tbls}", dicTBLHandlers[tbls].LoadInfo);
#else
                if (pTBLBundle == null)
                {
                    Debug.LogError($"没有取到TBL配置表资源，路径：{p}");
                    return;
                }

                LoadTBLByBundle(pTBLBundle, tbls, dicTBLHandlers[tbls].LoadInfo);
#endif
            }

            Debug.LogWarning("TBL加载完成");

            //// 释放掉AB
            //pTBLBundle.Unload(false);
            //pTBLBundle = null;

            m_bInited = true;
        }
    }

    public void InitByLocal()
    {
        CheckAllHandlers();

        foreach (string tbls in dicTBLAbsolutHandlers.Keys)
        {
            LoadTBLAbsolutePath(tbls, dicTBLAbsolutHandlers[tbls].LoadInfo);
        }

        //LoadTBLAbsolutePath(Application.dataPath + "/Bundle/TBL/RandomSlotType.txt", pHandlerRandomSlotType.LoadInfo);
        //LoadTBLAbsolutePath(Application.dataPath + "/Bundle/TBL/Stage.txt", pHandlerStageInfo.LoadInfo);
        //LoadTBLAbsolutePath(Application.dataPath + "/Bundle/TBL/Common/RandomName.txt", pHanderRandomNameInfo.LoadInfo);

        //LoadTBLAbsolutePath(Application.dataPath + "/Bundle/TBL/Common/GlobalValueInt.txt", pGlobalConfig.LoadIntInfo);
        //LoadTBLAbsolutePath(Application.dataPath + "/Bundle/TBL/Common/GlobalValueString.txt", pGlobalConfig.LoadStringInfo);

        //CTBLLanguageInfo.Inst.LoadLanguageByTxt(Application.dataPath + "/Bundle/TBL/Language/");
    }

    public void CheckAllHandlers()
    {
        dicTBLHandlers.Clear();
        dicTBLAbsolutHandlers.Clear();

        Assembly assembly = typeof(CTBLInfo).Assembly;
        foreach (Type type in assembly.GetTypes())
        {
            object[] objects = type.GetCustomAttributes(typeof(CTBLConfigAttri), false);
            if (objects.Length == 0)
            {
                continue;
            }

            CTBLConfigAttri httAttri = (CTBLConfigAttri)objects[0];

            CTBLConfigBase iHandler = Activator.CreateInstance(type) as CTBLConfigBase;
            if (iHandler == null)
            {
                Debug.LogError("None Handler:" + httAttri.tblName);
                continue;
            }

            Debug.Log("配表：" + httAttri.tblName);
            dicTBLHandlers.Add(httAttri.tblName, iHandler);
            dicTBLAbsolutHandlers.Add(Application.dataPath + "/Bundle/TBL/" + httAttri.tblFilePath + "/" + httAttri.tblName + ".txt", iHandler);
        }

    }

    public void LoadTBL(string szPath, DelegateLoadTBL dlg)
    {
        CTBLLoader loader = new CTBLLoader();
        loader.LoadFromFile(szPath);

        if (dlg != null)
        {
            dlg(loader);
        }
    }

    public void LoadTBLAbsolutePath(string szPath, DelegateLoadTBL dlg)
    {
        CTBLLoader loader = new CTBLLoader();
        loader.LoadFromFileabAolutePath(szPath);

        if (dlg != null)
        {
            dlg(loader);
        }
    }

    public void LoadTBLByBundle(AssetBundle pBundle, string szConfig, DelegateLoadTBL dlg)
    {
        TextAsset textasset = pBundle.LoadAsset<TextAsset>(szConfig);

        if (textasset == null)
        {
            Debug.LogError("没有配表资源：" + szConfig);
            return;
        }

        //Debug.Log(szConfig);
        string szContent = textasset.text; //Encoding.Unicode.GetString(textasset.bytes);

#if TBL_ENGRYPT
        szContent = CEncryptHelper.AesDecrypt(szContent);
#endif

        CTBLLoader loader = new CTBLLoader();
        loader.LoadFromFileContent(szContent);

        if (dlg != null)
        {
            dlg(loader);
        }
    }

    public void LoadTBLByContent(string szContent, DelegateLoadTBL dlg)
    {
        CTBLLoader loader = new CTBLLoader();
        loader.LoadFromFileContent(szContent);

        if (dlg != null)
        {
            dlg(loader);
        }
    }

    public string LoadTextByBundle(AssetBundle pBundle, string szConfig)
    {
        TextAsset textasset = pBundle.LoadAsset<TextAsset>(szConfig);
        string szContent = textasset.text;

        return szContent;
    }

    #region DATA

    #endregion
}