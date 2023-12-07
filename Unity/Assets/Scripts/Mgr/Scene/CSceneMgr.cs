using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class CSceneMgr : MonoBehaviour {
    #region Instance

    static private CSceneMgr m_Instance;

    public static CSceneMgr Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<CSceneMgr>();
                if (m_Instance == null)
                {
                    GameObject objScene = new GameObject();
                    m_Instance = objScene.AddComponent<CSceneMgr>();
                    m_Instance.gameObject.name = "[Game - SceneManager]";
                }

                GameObject.DontDestroyOnLoad(m_Instance.gameObject);
            }
            return m_Instance;
        }
    }

    static public void Destroy()
    {
        m_Instance = null;
    }

    #endregion

    #region Const

    protected const float SCENELOAD_MAXPROGRESS = 1F;

    #endregion

    #region 场景信息数据结构

    //场景信息
    public class ST_SceneInfo
    {
        public string szSceneName = "";
        public int nScriptType = 0;

        public ST_SceneInfo(int type, string name)
        {
            nScriptType = type;
            szSceneName = name;
        }
    }

    //场景加载任务
    public class ST_TaskSceneLoad
    {
        public enum EM_SceneState
        {
            EM_SceneState_Null = 0,
            EM_SceneState_ReadyLoad,
            EM_SceneState_Loading,
            EM_SceneState_LoadComplete,
            EM_SceneState_LoadPref, //预加载资源
            EM_SceneState_Failed,
            EM_SceneState_Max,
        }

        public int m_nSceneID;
        public EM_SceneState m_emState = EM_SceneState.EM_SceneState_Null;
        public string m_szSceneName;
        public int m_nScriptType;
        public AsyncOperation m_AsyncInfo;
    }

    #endregion
  
    //当前场景对象
    public CSceneBase m_objCurScene = new CSceneBase();

    //上一个场景
    public CSceneFactory.EMSceneType emPreScene = CSceneFactory.EMSceneType.Max;

    //场景信息
    public Dictionary<CSceneFactory.EMSceneType, ST_SceneInfo> m_dicSceneInfo = new Dictionary<CSceneFactory.EMSceneType, ST_SceneInfo>();

    //场景加载列表
    public List<ST_TaskSceneLoad> m_listSceneTask = new List<ST_TaskSceneLoad>();

    //加载方式
    private LoadSceneMode m_LoadMode = LoadSceneMode.Single;

    bool bInit = false;

    #region 场景加载相关

    public void Init()
    {
        //m_dicSceneInfo.Add(CSceneFactory.EMSceneType.Menu,
        //                   new ST_SceneInfo((int)CSceneFactory.EMSceneType.Menu, "Menu"));

        //m_dicSceneInfo.Add(CSceneFactory.EMSceneType.Adventure,
        //                 new ST_SceneInfo((int)CSceneFactory.EMSceneType.Adventure, "Scene_Adventure"));

        //m_dicSceneInfo.Add(CSceneFactory.EMSceneType.Battle,
        //                 new ST_SceneInfo((int)CSceneFactory.EMSceneType.Battle, "Scene_Battle"));
        if (!bInit)
        {
            CSceneFactory.Instance.Init();
            bInit = true;
        }
    }

    /// <summary>
    /// 加载场景
    /// </summary>
    /// <param name="szSceneName"></param>
    /// <param name="bNeedLoading"></param>
    /// <param name="mode"></param>
    public void LoadScene(CSceneFactory.EMSceneType emScene, bool bNeedLoading = true, LoadSceneMode mode = LoadSceneMode.Single)
    {
        Debug.Log("Load Scene: " + emScene.ToString());
        //Loading
        m_LoadMode = mode;
        if (bNeedLoading)
        {
            //TODO:打开加载UI
            //UIManager.Instance.OpenUI(UIResType.Loading);
        }

        //ST_SceneInfo pSceneInfo = GetSceneInfo(emScene);
        //if(pSceneInfo==null)
        //{
        //    Debug.LogError("None Scene Info: " + emScene); ;
        //    return;
        //}

        CSceneFactory.CSceneInfo pSceneInfo = CSceneFactory.Instance.GetSceneScriptObj((int)emScene);
        
        ST_TaskSceneLoad task = new ST_TaskSceneLoad();
        
        task.m_nSceneID = pSceneInfo.szName.GetHashCode();
        task.m_szSceneName = pSceneInfo.szName;
        task.m_nScriptType = (int)emScene;
        task.m_AsyncInfo = null;
        task.m_emState = ST_TaskSceneLoad.EM_SceneState.EM_SceneState_ReadyLoad;
        m_listSceneTask.Add(task);
    }

    public void Update()
    {
        if (m_listSceneTask.Count == 0)
        {
            m_objCurScene.OnSceneUpdate();
        }
        else //有待加载的场景,暂定每次最多只能有一个场景在加载
        {
            //当前加载场景的TASK
            ST_TaskSceneLoad task = m_listSceneTask[0];
            switch (task.m_emState)
            {
                case ST_TaskSceneLoad.EM_SceneState.EM_SceneState_ReadyLoad:
                    {
                        if (task.m_AsyncInfo == null)
                        {
                            task.m_AsyncInfo = SceneManager.LoadSceneAsync(task.m_szSceneName, m_LoadMode);
                            
                            if (task.m_AsyncInfo != null)
                            {
                               // task.m_AsyncInfo.allowSceneActivation = false;
                                SceneLeave(); //离开当前场景
                                task.m_emState = ST_TaskSceneLoad.EM_SceneState.EM_SceneState_Loading;
                            }
                            else
                            {
                                task.m_emState = ST_TaskSceneLoad.EM_SceneState.EM_SceneState_Failed;
                            }
                        }
                    }
                    break;
                case ST_TaskSceneLoad.EM_SceneState.EM_SceneState_Loading:
                    {
                        //判断是否加载完成
                        if (task.m_AsyncInfo.progress >= SCENELOAD_MAXPROGRESS &&
                            task.m_AsyncInfo.isDone)
                        {                            
                            task.m_emState = ST_TaskSceneLoad.EM_SceneState.EM_SceneState_LoadComplete;
                        }
                    }
                    break;
                case ST_TaskSceneLoad.EM_SceneState.EM_SceneState_LoadComplete:
                    {
                        m_objCurScene = CSceneFactory.Instance.GetSceneScriptObj(task.m_nScriptType).pScene;
                        m_objCurScene.szSceneName = task.m_szSceneName;
                        m_objCurScene.emSceneType = (CSceneFactory.EMSceneType)task.m_nScriptType;

                        m_objCurScene.OnSceneAwake();
                        m_objCurScene.OnScenePrefLoad(); //进行资源预加载
                        task.m_emState = ST_TaskSceneLoad.EM_SceneState.EM_SceneState_LoadPref;
                    }
                    break;
                case ST_TaskSceneLoad.EM_SceneState.EM_SceneState_LoadPref:
                    {
                        if (m_objCurScene.IsPrefLoadComplete())
                        {
                            Debug.LogWarning("Scene 【" + m_objCurScene.szSceneName + "】 Load Complete.");
                            m_objCurScene.OnSceneStart();
                            m_listSceneTask.Remove(task);
                        }
                    }
                    break;
                default:
                    {
                        Debug.LogError("Scene 【" + task.m_szSceneName + "】 Load Failed.");
                        m_listSceneTask.Remove(task);
                    }
                    break;
            }
        }
    }

    public void LastUpdate()
    {
        if (m_listSceneTask.Count == 0)
        {
            m_objCurScene.OnSceneLateUpdate();
        }
    }

    public void FixedUpdate()
    {
        if (m_listSceneTask.Count == 0)
        {
            m_objCurScene.OnSceneFixedUpdate();
        }
    }

    //开始加载新场景时离开当前场景执行函数
    public void SceneLeave()
    {
        Debug.Log("Leave Scene: " + m_objCurScene.szSceneName);
        m_objCurScene.OnSceneLeave();
        emPreScene = m_objCurScene.emSceneType;
        m_objCurScene = null;
    }

    public ST_SceneInfo GetSceneInfo(CSceneFactory.EMSceneType emScene)
    {
        ST_SceneInfo info = null;
        if (m_dicSceneInfo.TryGetValue(emScene, out info))
        {
        }
        return info;
    }

    #endregion
}
