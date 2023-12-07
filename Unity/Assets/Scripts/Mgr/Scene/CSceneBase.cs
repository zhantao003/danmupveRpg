using UnityEngine;
using System.Collections;

public class CSceneBase {
#region CONST

    protected const float MAX_PREFLOADPROGRESS = 1F;

#endregion

    //场景名字
    public string szSceneName{ get; set; }

    public CSceneFactory.EMSceneType emSceneType = CSceneFactory.EMSceneType.Max;

    //预加载结束标记
    public bool bPrefLoadComplete { get; set; }

    //预加载进度条
    public float fPrefLoadProgess { get; set; }

    public CSceneBase()
    {
        OnInitPrefLoad();
    }

    public virtual void OnSceneAwake() {  }//Debug.Log("Base SceneAwake"); }
    public virtual void OnScenePrefLoad() { SetPrefLoadComplelte(); }
    public virtual void OnSceneStart() { }//Debug.Log("Base SceneStart"); }
    public virtual void OnSceneUpdate() { }//Debug.Log("Base SceneUpdate"); }
    public virtual void OnSceneFixedUpdate() { }
    public virtual void OnSceneLateUpdate() { }
    public virtual void OnSceneLeave()
    {
        //CTCameraInst.Destroy(); CUIManager.Instance.ChgLevel(); XTLoadMgr.Inst.ChgLevel();
        //Debug.Log("Base SceneLeave");
    }

    //初始化资源预加载
    public virtual void OnInitPrefLoad()
    {
        bPrefLoadComplete = false;
        fPrefLoadProgess = 0F;
    }

    //设置资源预加载结束标记
    public virtual void SetPrefLoadComplelte()
    {
        bPrefLoadComplete = true;
        fPrefLoadProgess = MAX_PREFLOADPROGRESS;
    }

    //判断资源预加载是否结束
    public virtual bool IsPrefLoadComplete()
    {
        return bPrefLoadComplete && (fPrefLoadProgess >= MAX_PREFLOADPROGRESS);
    }
}
