using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneFactory {
    #region Instance

    static private CSceneFactory m_Instance;

    public static CSceneFactory Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CSceneFactory();
            }

            return m_Instance;
        }
    }

    #endregion

    public enum EMSceneType
    {
        MainMenu = 0,       //开始场景

        //游戏场景
        GameMap101 = 101,         
        GameModeSelect102 = 102,
        GameMap101Net = 103,

        Max,
    }

    public class CSceneInfo
    {
        public string szName;
        public CSceneBase pScene;
    }

    //场景脚本对象
    Dictionary<int, CSceneInfo> dicScenes = new Dictionary<int, CSceneInfo>();

    public void Init()
    {
        dicScenes.Add((int)EMSceneType.MainMenu, new CSceneInfo() { szName = "MainMenu", pScene = new CSceneMainMenu() });
        dicScenes.Add((int)EMSceneType.GameMap101, new CSceneInfo() { szName = "Game101", pScene = new CSceneGame() });
        dicScenes.Add((int)EMSceneType.GameMap101Net, new CSceneInfo() { szName = "Game101Net", pScene = new CSceneNetGame() });
        dicScenes.Add((int)EMSceneType.GameModeSelect102, new CSceneInfo() { szName = "ModeSelect102", pScene = new CSceneGameModeSelect() });
    }

    /// <summary>
    /// 获取指定的场景脚本
    /// </summary>
    /// <param name="nType"></param>
    /// <returns></returns>
    public CSceneInfo GetSceneScriptObj(int nType)
    {
        CSceneInfo pRes = null;
        if(dicScenes.TryGetValue(nType, out pRes))
        {

        }

        return pRes;
    }
}
