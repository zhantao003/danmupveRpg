using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;

//资源加载管理器
public class CResLoadMgr : MonoBehaviour
{
    #region Instance

    private CResLoadMgr() { }
    private static CResLoadMgr m_Instance = null;
    public static CResLoadMgr Inst
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject gameobject = new GameObject("[Game - ResourceManager]");
                m_Instance = gameobject.AddComponent<CResLoadMgr>();
                DontDestroyOnLoad(gameobject);
            }

            return m_Instance;
        }
    }

    #endregion

    public void Init()
    {

    }

    void Update()
    {
        OnASynUpdate();
    }

    //资源类型
    public enum EM_ResLoadType
    {
        ContinuousAssetbundle, //永续存在的资源包
        CanbeUnloadAssetbundle,//会被销毁的资源包
        AssetbundleConfig,     //资源包配置表

        //特殊指定类型资源
        Map,    //地图资源
        Effect,  //通用特效资源
        Role,   //角色资源
        Audio,  //声音

        OtherAsset, //其他资源
    }

    //同步加载结束委托
    public delegate void OnSyncLoadOverDlg(Object res, object data, bool bSuc);

    //异步加载结束委托
    public delegate void OnASyncLoadOverDlg(CAsynResAtom res, object data, bool bSuc);

    //同步加载结束委托
    public delegate void OnASyncLoadAssetsOverDlg(Object pRes);

    #region 同步加载

    //同步加载资源原子对象
    public class CSynResAtom
    {
        public int nID;
        public Object pAsset;   //加载完成的资源
        public EM_ResLoadType emLoadType = EM_ResLoadType.CanbeUnloadAssetbundle;
        public bool bDes = false;
        public object data; //传入数据
        public string szProtol; //协议前缀
        public string szPath;   //资源路径   

        public OnSyncLoadOverDlg syncDlgOver = null;

        public void OnLoadOver()
        {
            syncDlgOver = null;
        }
    }

    //同步加载已存资源
    protected Dictionary<int, CSynResAtom> m_dicSynAssets = new Dictionary<int, CSynResAtom>();

    //同步加载资源
    public object SynLoad(string szPath, EM_ResLoadType emLoadType = EM_ResLoadType.CanbeUnloadAssetbundle, OnSyncLoadOverDlg dlg = null, object data = null)
    {
        //判断是否已经加载过的资源
        int nPathHashCode = szPath.GetHashCode();
        CSynResAtom pRes = null;
        if (m_dicSynAssets.TryGetValue(nPathHashCode, out pRes))
        {
            if (dlg != null)
            {
                dlg(pRes.pAsset, data, true);
            }
        }
        else
        {
            //新加载资源
            pRes = new CSynResAtom();
            pRes.nID = szPath.GetHashCode();
            pRes.szPath = szPath;
            pRes.pAsset = Resources.Load(szPath);
            pRes.emLoadType = emLoadType;
            if (pRes.pAsset != null)
            {
                if (dlg != null)
                {
                    dlg(pRes.pAsset, data, true);
                }
                m_dicSynAssets.Add(pRes.nID, pRes);
            }
            else
            {
                Debug.LogError("Load Asset Error: " + szPath + "  【None Asset】");
            }
        }

        return pRes.pAsset;
    }

    //public void AddRes(int nHashCode, object pRes)
    //{
    //    if (!m_dicSynAssets.ContainsKey(nHashCode))
    //        m_dicSynAssets.Add(nHashCode, pRes);
    //}

    //public bool HasRes(int nHashCode)
    //{
    //    return m_dicSynAssets.ContainsKey(nHashCode);
    //}

    /// <summary>
    /// 清除同步加载的资源
    /// </summary>
    public void UnloadAllSycnAssets()
    {
        List<int> listTmpDes = new List<int>();
        foreach (CSynResAtom pAtom in m_dicSynAssets.Values)
        {
            if (pAtom.emLoadType == EM_ResLoadType.ContinuousAssetbundle)
            {
                continue;
            }

            listTmpDes.Add(pAtom.nID);

            if (pAtom.pAsset.GetType() == typeof(GameObject))
            {
                pAtom.pAsset = null;
            }
            else
            {
                Resources.UnloadAsset(pAtom.pAsset);
            }

            Resources.UnloadUnusedAssets();
        }

        for (int i = 0; i < listTmpDes.Count; i++)
        {
            m_dicSynAssets.Remove(listTmpDes[i]);
        }
    }

    #endregion

    #region 异步加载

    #region 资源打包路径

    public static string ASSETBUNDLECONFIG = ".bundleconfig";

    public static string PATH_PLATFORM =
#if UNITY_STANDALONE_WIN
    "win/";
#elif UNITY_ANDROID
    "android/";
#elif UNITY_IPHONE
    "ios/";
#elif UNITY_PS4
    "ps4/";
#endif

    #endregion

    //异步加载已存资源
    protected Dictionary<EM_ResLoadType, List<CAsynResAtom>> m_dicASynAssets = new Dictionary<EM_ResLoadType, List<CAsynResAtom>>();

    //异步加载原子列表
    protected List<CAsynResAtom> m_listASynAtoms = new List<CAsynResAtom>();
    protected List<CAsynResAtom> m_listDelAtoms = new List<CAsynResAtom>();

    //异步加载资源原子对象
    public class CAsynResAtom
    {
        public enum EM_STATE
        {
            READY,
            LOADING,
            COMPLETE,
            FAILED,
            LOADINGMANIFEST,    //加载AssetBundle配置文件
            WAITDEPENDENCES,    //等待依赖项目加载
        }

        public int nID;
        public object pAsset;   //加载完成的资源
        public EM_STATE emState;//状态
        public EM_ResLoadType emLoadType = EM_ResLoadType.CanbeUnloadAssetbundle;
        public bool bDes = false;
        public object data; //传入数据
        public string szProtol; //协议前缀
        public string szPath;   //资源路径   
        public UnityWebRequest pWWW;        //WWW加载资源对象
        //public UnityWebRequest pWebReq;

        public OnSyncLoadOverDlg syncDlgOver = null;
        public OnASyncLoadOverDlg asyncDlgOver = null;
        public CAsynResAtom pAtomBundleConfig;  //Assetbundle配置文件任务
        public CAsynResAtom[] pAsynResAtomDependences;  //依赖项任务列表

        public void OnLoadOver()
        {
            pWWW.Dispose();
            syncDlgOver = null;
            asyncDlgOver = null;
        }

        /// <summary>
        /// 检查依赖项是否加载完成
        /// </summary>
        /// <returns></returns>
        public bool IsDependencesOver()
        {
            bool bComplete = true;

            if (pAsynResAtomDependences == null ||
                pAsynResAtomDependences.Length <= 0)
                return bComplete;

            for (int i = 0; i < pAsynResAtomDependences.Length; i++)
            {
                if (pAsynResAtomDependences[i].emState != EM_STATE.COMPLETE &&
                   pAsynResAtomDependences[i].emState != EM_STATE.FAILED)
                {
                    return false;
                }
            }

            return bComplete;
        }
    }

    //异步加载资源协议
    public enum Protol
    {
        LocalFile,
        WebStream,
    }

    //异步加载资源(暂时只支持加载AssetBundle)
    public CAsynResAtom ASynLoad(string szPath, EM_ResLoadType emResType = EM_ResLoadType.CanbeUnloadAssetbundle, OnASyncLoadOverDlg dlg = null, object data = null, Protol protol = Protol.LocalFile)
    {
        //根据协议指定加载路径
        string szFinalPath = szPath;
        switch (protol)
        {
            case Protol.LocalFile:
                {
                    // 加载 AB
                    szFinalPath = Path.Combine(CAppPathMgr.AssetBundleHotfixDir, GetStr(szPath));
                    if (!File.Exists(szFinalPath))
                    {
                        szFinalPath = Path.Combine(CAppPathMgr.AssetBundleLocalDir, GetStr(szPath));
                    }
                }
                break;
            case Protol.WebStream:
                {

                }
                break;
        }

        //Debug.Log($"资源加载路径：{szFinalPath}");

        //判断是否已经有该资源
        int nPathHashCode = szFinalPath.GetHashCode();
        CAsynResAtom pAtom = GetAsynResAtomByID(nPathHashCode, emResType);
        if (pAtom != null)
        {
            if (dlg != null)
            {
                dlg(pAtom, data, true);
            }
        }
        else
        {
            //判断当前是否有相同任务
            pAtom = IsSameResLoading(nPathHashCode);
            if (pAtom == null)
            {
                pAtom = new CAsynResAtom();
                pAtom.nID = nPathHashCode;
                pAtom.szProtol = "";
                pAtom.szPath = szFinalPath;
                pAtom.syncDlgOver = null;
                pAtom.asyncDlgOver = dlg;
                pAtom.data = data;
                pAtom.emState = CAsynResAtom.EM_STATE.READY;
                pAtom.emLoadType = emResType;
                pAtom.pAtomBundleConfig = null;
                pAtom.pAsynResAtomDependences = null;
                m_listASynAtoms.Add(pAtom);
            }
            else
            {
                //Debug.Log("重复加载:" + szPath + "  状态：" + pAtom.emState);

                if (pAtom.emState != CAsynResAtom.EM_STATE.FAILED)
                {
                    pAtom.asyncDlgOver += dlg;
                }
            }
        }

        return pAtom;
    }

    public void OnASynUpdate()
    {
        if (m_listASynAtoms.Count > 0)
        {
            for (int nIdx = 0; nIdx < m_listASynAtoms.Count;)
            {
                bool bNext = true;
                CAsynResAtom pAtom = m_listASynAtoms[nIdx];

                try
                {
                    switch (pAtom.emState)
                    {
                        case CAsynResAtom.EM_STATE.READY:
                            {
                                string path = pAtom.szProtol + pAtom.szPath;
                                //Debug.LogWarning("[ResLoadMgr] path: " + path);
                                pAtom.pWWW = UnityWebRequestAssetBundle.GetAssetBundle(path);
                                //pAtom.pWWW = UnityWebRequest.Get(path);

                                pAtom.pWWW.SendWebRequest();

                                pAtom.emState = CAsynResAtom.EM_STATE.LOADING;
                            }
                            break;
                        case CAsynResAtom.EM_STATE.LOADING:
                            {
                                if (!System.String.IsNullOrEmpty(pAtom.pWWW.error))
                                {
                                    //Debug.LogError(pAtom.szPath + "  " + pAtom.pWWW.error);
                                    pAtom.emState = CAsynResAtom.EM_STATE.FAILED;
                                }
                                else
                                {
                                    if (pAtom.pWWW.isDone)
                                    {
                                        OnWWWLoadOver(pAtom);
                                    }
                                }
                            }
                            break;
                        case CAsynResAtom.EM_STATE.COMPLETE:
                            {
                                AddAsynResAtom(pAtom);

                                if (pAtom.asyncDlgOver != null)
                                {
                                    pAtom.asyncDlgOver(pAtom, pAtom.data, true);
                                    pAtom.asyncDlgOver = null;
                                }

                                //Debug.Log("加载完成：" + pAtom.szPath);
                                pAtom.OnLoadOver();
                                m_listASynAtoms.Remove(pAtom);
                                m_listDelAtoms.Add(pAtom);
                                bNext = false;
                            }
                            break;
                        case CAsynResAtom.EM_STATE.FAILED:
                            {
                                if (pAtom.asyncDlgOver != null)
                                {
                                    pAtom.asyncDlgOver(pAtom, pAtom.data, false);
                                }

                                pAtom.OnLoadOver();
                                m_listASynAtoms.Remove(pAtom);
                                m_listDelAtoms.Add(pAtom);
                                bNext = false;
                            }
                            break;
                        case CAsynResAtom.EM_STATE.LOADINGMANIFEST:
                            {
                                if (pAtom.pAtomBundleConfig == null ||
                                   pAtom.pAtomBundleConfig.emState == CAsynResAtom.EM_STATE.FAILED)
                                {
                                    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;
                                }
                                else if (pAtom.pAtomBundleConfig.emState == CAsynResAtom.EM_STATE.COMPLETE)
                                {
                                    pAtom.emState = CAsynResAtom.EM_STATE.WAITDEPENDENCES;
                                }
                            }
                            break;
                        case CAsynResAtom.EM_STATE.WAITDEPENDENCES:
                            {
                                if (pAtom.IsDependencesOver())
                                    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;
                            }
                            break;
                    }

                    if (bNext)
                    {
                        nIdx++;
                    }
                }
                catch (System.Exception e)
                {
                    m_listASynAtoms.Remove(pAtom);
                    m_listDelAtoms.Add(pAtom);
                    Debug.LogError("资源管理器加载问题Exception:" + e.Message);
                }
            }
        }

        if (m_listDelAtoms.Count > 0)
        {
            m_listDelAtoms.Clear();
        }

        CheckAysnCreateAssetAtom();
    }

    protected void OnWWWLoadOver(CAsynResAtom pAtom)
    {
        //Debug.Log(pAtom.emLoadType);

        switch (pAtom.emLoadType)
        {
            case EM_ResLoadType.CanbeUnloadAssetbundle:
            case EM_ResLoadType.ContinuousAssetbundle:
            case EM_ResLoadType.Audio:
            case EM_ResLoadType.Map:
            case EM_ResLoadType.Role:
            case EM_ResLoadType.Effect:
                {
                    AssetBundle pBundle = (pAtom.pWWW.downloadHandler as DownloadHandlerAssetBundle).assetBundle;

                    pAtom.pAsset = pBundle;

                    //判断是否有依赖项目
                    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;

                    //if (File.Exists(pAtom.szPath + ASSETBUNDLECONFIG))
                    //{
                    //    pAtom.pAtomBundleConfig = ASynLoad(pBundle.name + ASSETBUNDLECONFIG, EM_ResLoadType.AssetbundleConfig, null, pAtom);
                    //    pAtom.emState = CAsynResAtom.EM_STATE.LOADINGMANIFEST;
                    //}
                    //else
                    //{
                    //    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;
                    //}
                }
                break;
            case EM_ResLoadType.AssetbundleConfig:
                {
                    //解析assetbundle中的依赖项信息
                    byte[] bytes = pAtom.pWWW.downloadHandler.data;

                    string szContent = System.Text.Encoding.Unicode.GetString(bytes);
                    JSONObject m_pJsonData = new JSONObject();
                    m_pJsonData = JSONNode.Parse(szContent) as JSONObject;
                    if (m_pJsonData == null)
                    {
                        Debug.LogError("Error JSON Data : " + szContent);
                        pAtom.emState = CAsynResAtom.EM_STATE.FAILED;
                    }

                    //对比是否与原assetbundle一致
                    CAsynResAtom pOriginAtom = pAtom.data as CAsynResAtom;
                    if (pOriginAtom == null) pAtom.emState = CAsynResAtom.EM_STATE.FAILED;

                    AssetBundle pOriginAssetBundle = pOriginAtom.pAsset as AssetBundle;
                    if (pOriginAssetBundle == null) pAtom.emState = CAsynResAtom.EM_STATE.FAILED;

                    string szMainAsset = m_pJsonData["main"].Value;
                    if (!szMainAsset.Equals(pOriginAssetBundle.name)) pAtom.emState = CAsynResAtom.EM_STATE.FAILED;

                    //开启依赖项的加载任务
                    JSONArray pJArr = m_pJsonData["dependences"].AsArray;
                    pOriginAtom.pAsynResAtomDependences = new CAsynResAtom[pJArr.Count];
                    for (int i = 0; i < pJArr.Count; i++)
                    {
                        string szDependenceAssetbundleName = pJArr[i].Value;
                        Debug.Log("资源本体名：" + pOriginAssetBundle.name + "  依赖项目资源包名：" + szDependenceAssetbundleName);
                        pOriginAtom.pAsynResAtomDependences[i] = ASynLoad(szDependenceAssetbundleName, pOriginAtom.emLoadType);
                    }

                    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;
                }
                break;
            case EM_ResLoadType.OtherAsset:
                {
                    pAtom.pAsset = pAtom.pWWW;
                    pAtom.emState = CAsynResAtom.EM_STATE.COMPLETE;
                }
                break;
        }
    }

    /// <summary>
    /// 添加新的异步资源原子
    /// </summary>
    /// <param name="pAtom"></param>
    protected void AddAsynResAtom(CAsynResAtom pAtom)
    {
        List<CAsynResAtom> listAtom = null;
        if (m_dicASynAssets.TryGetValue(pAtom.emLoadType, out listAtom))
        {
            listAtom.Add(pAtom);
        }
        else
        {
            listAtom = new List<CAsynResAtom>();
            listAtom.Add(pAtom);
            m_dicASynAssets.Add(pAtom.emLoadType, listAtom);
        }
    }

    /// <summary>
    /// 通过资源索引ID获取资源包
    /// </summary>
    /// <returns></returns>
    public CAsynResAtom GetAsynResAtomByID(int nID, EM_ResLoadType emType)
    {
        CAsynResAtom pRes = null;
        List<CAsynResAtom> listAtom = null;
        if (m_dicASynAssets.TryGetValue(emType, out listAtom))
        {
            for (int i = 0; i < listAtom.Count; i++)
            {
                if (listAtom[i].nID == nID)
                {
                    pRes = listAtom[i];
                    break;
                }
            }
        }
        return pRes;
    }

    /// <summary>
    /// 是否有相同资源正在加载
    /// </summary>
    /// <param name="nID"></param>
    /// <returns></returns>
    protected CAsynResAtom IsSameResLoading(int nID)
    {
        CAsynResAtom pRes = null;
        for (int i = 0; i < m_listASynAtoms.Count; i++)
        {
            if (m_listASynAtoms[i].nID == nID)
            {
                pRes = m_listASynAtoms[i];
                break;
            }
        }

        return pRes;
    }

    /// <summary>
    /// 回收资源
    /// </summary>
    public void UnloadAllASycnAssets(bool unloadAllLoadedObjects)
    {
        //List<int> listTmpDes = new List<int>();
        foreach (KeyValuePair<EM_ResLoadType, List<CAsynResAtom>> ele in m_dicASynAssets)
        {
            if (ele.Key == EM_ResLoadType.ContinuousAssetbundle)
            {
                continue;
            }

            List<CAsynResAtom> listRes = ele.Value;
            if (listRes == null)
            {
                continue;
            }

            Debug.Log("资源类型:" + ele.Key + "卸载数:" + listRes.Count);
            for (int i = 0; i < listRes.Count; i++)
            {
                CAsynResAtom pAtom = listRes[i];
                AssetBundle pBundle = pAtom.pAsset as AssetBundle;
                if (pBundle != null)
                {
                    pBundle.Unload(unloadAllLoadedObjects);
                }
                pAtom.pWWW.Dispose();
            }

            listRes.Clear();
        }
        //m_dicASynAssets.Clear();
    }

    #endregion

    #region 异步创建资源

    public class CSAynAssetLoadAtom
    {
        public AssetBundleRequest pAssetLoader;
        public OnASyncLoadAssetsOverDlg pLoadOverDlg;
    }

    List<CSAynAssetLoadAtom> listAssetBundleReqs = new List<CSAynAssetLoadAtom>();

    /// <summary>
    /// 异步创建Prefab资源
    /// </summary>
    /// <param name="szAssetBundle"></param>
    /// <param name="szPrefabName"></param>
    /// <param name="dlg"></param>
    public void ACreateAssetByType(string szAssetBundle, string szPrefabName, System.Type pType, OnASyncLoadAssetsOverDlg dlg, EM_ResLoadType emResType = EM_ResLoadType.CanbeUnloadAssetbundle)
    {
        ASynLoad(szAssetBundle, emResType, delegate (CAsynResAtom res, object data, bool bSuc)
        {
            AssetBundle pBundle = res.pAsset as AssetBundle;
            if (pBundle == null)
            {
                Debug.LogError("Error AssetBundle : " + szAssetBundle);
                return;
            }

            ASyncCreateAsset(szPrefabName, pBundle, pType, delegate (UnityEngine.Object objRes)
            {
                if (objRes != null)
                {
                    if (dlg != null)
                    {
                        dlg(objRes);
                    }
                }
                else
                {
                    if (dlg != null)
                    {
                        dlg(null);
                    }
                }
            });
        });
    }

    /// <summary>
    /// 创建异步加载资源的任务
    /// </summary>
    /// <param name="szPrefabName"></param>
    /// <param name="pBundle"></param>
    /// <param name="pType"></param>
    /// <param name="pDlg"></param>
    protected void ASyncCreateAsset(string szPrefabName, AssetBundle pBundle, System.Type pType, OnASyncLoadAssetsOverDlg pDlg)
    {
        CSAynAssetLoadAtom pAtom = new CSAynAssetLoadAtom();
        if (!pBundle.Contains(szPrefabName))
        {
            Debug.LogWarning("None Prefab Asset:" + szPrefabName);

            if (pDlg != null)
            {
                pDlg(null);
            }

            return;
        }
        pAtom.pAssetLoader = pBundle.LoadAssetAsync(szPrefabName, pType);
        pAtom.pLoadOverDlg = pDlg;
        listAssetBundleReqs.Add(pAtom);
    }

    public void CheckAysnCreateAssetAtom()
    {
        for (int i = 0; i < listAssetBundleReqs.Count;)
        {
            if (listAssetBundleReqs[i].pAssetLoader.isDone)
            {
                listAssetBundleReqs[i].pLoadOverDlg(listAssetBundleReqs[i].pAssetLoader.asset);

                listAssetBundleReqs.RemoveAt(i);
            }
            else
            {
                i++;
            }
        }
    }

    #endregion

    public void UnloadAllAssetsByType(EM_ResLoadType[] emUnloadTypes, bool unloadAllLoadedObjects)
    {
        //同步资源卸载
        List<int> listTmpDes = new List<int>();
        foreach (CSynResAtom pAtom in m_dicSynAssets.Values)
        {
            if (pAtom.emLoadType == EM_ResLoadType.ContinuousAssetbundle)
            {
                continue;
            }

            //判断是否允许卸载的资源
            bool bUnloadAble = false;
            for (int i = 0; i < emUnloadTypes.Length; i++)
            {
                if (pAtom.emLoadType == emUnloadTypes[i])
                {
                    bUnloadAble = true;
                    break;
                }
            }

            if (!bUnloadAble)
            {
                continue;
            }

            listTmpDes.Add(pAtom.nID);

            if (pAtom.pAsset.GetType() == typeof(GameObject))
            {
                pAtom.pAsset = null;
            }
            else
            {
                Resources.UnloadAsset(pAtom.pAsset);
            }

            Resources.UnloadUnusedAssets();
        }

        for (int i = 0; i < listTmpDes.Count; i++)
        {
            m_dicSynAssets.Remove(listTmpDes[i]);
        }

        //异步资源卸载
        foreach (KeyValuePair<EM_ResLoadType, List<CAsynResAtom>> ele in m_dicASynAssets)
        {
            if (ele.Key == EM_ResLoadType.ContinuousAssetbundle)
            {
                continue;
            }

            List<CAsynResAtom> listRes = ele.Value;
            if (listRes == null)
            {
                continue;
            }

            //判断是否允许卸载的资源
            bool bUnloadAble = false;
            for (int i = 0; i < emUnloadTypes.Length; i++)
            {
                if (ele.Key == emUnloadTypes[i])
                {
                    bUnloadAble = true;
                    break;
                }
            }

            if (!bUnloadAble)
            {
                continue;
            }

            Debug.Log("资源类型:" + ele.Key + "卸载数:" + listRes.Count);

            for (int i = 0; i < listRes.Count; i++)
            {
                CAsynResAtom pAtom = listRes[i];
                AssetBundle pBundle = pAtom.pAsset as AssetBundle;
                if (pBundle != null)
                {
                    pBundle.Unload(unloadAllLoadedObjects);
                }
                pAtom.pWWW.Dispose();
            }

            listRes.Clear();
        }
    }

    //
    //
    // 文件名称 路径转换
    string GetStr(string str)
    {
        //if(Application.platform == RuntimePlatform.Android)
        //{
        //    return str.Replace("/", "_");
        //}
        //else
        //{
        //    return str;
        //}
        return str;
    }
}
