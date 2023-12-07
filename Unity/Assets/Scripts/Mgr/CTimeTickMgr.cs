using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 计时器管理器
/// </summary>
public class CTimeTickMgr : MonoBehaviour
{
    #region Instance

    private CTimeTickMgr() { }
    private static CTimeTickMgr m_Instance = null;
    public static CTimeTickMgr Inst
    {
        get
        {
            if (m_Instance == null)
            {
                GameObject gameobject = new GameObject("[Game - TimeTickManager]");
                m_Instance = gameobject.AddComponent<CTimeTickMgr>();
                DontDestroyOnLoad(gameobject);
            }

            return m_Instance;
        }
    }

    #endregion

    /// <summary>
    /// 计时器类型 
    /// </summary>
    public enum EMLoopType
    {
        Once,
        Loop,
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public class CTicker
    {
        public int nGuid;

        public bool bActive = false;

        protected EMLoopType emLoop = EMLoopType.Once;

        protected CPropertyTimer pTimer;

        protected object[] objParams;

        protected DelegateOFuncCall dlgEvent;

        public CTicker(int id, float time, EMLoopType loop, DelegateOFuncCall call, object[] data)
        {
            nGuid = id;

            pTimer = new CPropertyTimer();
            pTimer.Value = time;
            pTimer.FillTime();

            emLoop = loop;

            dlgEvent = call;

            objParams = data;

            bActive = true;
        }

        public void OnUpdate(float dt)
        {
            if (!bActive) return;

            if(pTimer.Tick(dt))
            {
                DoEvent();
            }
        }

        /// <summary>
        /// 执行事件
        /// </summary>
        void DoEvent()
        {
            if (dlgEvent != null)
            {
                dlgEvent(objParams);
            }

            if (emLoop == EMLoopType.Loop)
            {
                pTimer.FillTime();
            }
            else if (emLoop == EMLoopType.Once)
            {
                bActive = false;
            }
        }
    }

    protected int nTickerCreateCount = 0; //计时器生成数量

    protected List<CTicker> listTickers = new List<CTicker>();  //计时器列表

    public void Init()
    {

    }

    /// <summary>
    /// 添加计时器
    /// </summary>
    /// <returns></returns>
    public int PushTicker(float fTime, DelegateOFuncCall call, EMLoopType emLoopType = EMLoopType.Once, params object[] data)
    {
        int nCode = nTickerCreateCount++;

        CTicker pTicker = new CTicker(nCode, fTime, emLoopType, call, data);
        listTickers.Add(pTicker);

        return nCode;
    }

    /// <summary>
    /// 停止计时器
    /// </summary>
    /// <param name="guid"></param>
    public void StopTicker(int guid)
    {
        for(int i=0; i<listTickers.Count; i++)
        {
            if(listTickers[i].nGuid == guid)
            {
                listTickers[i].bActive = false;
                return;
            }
        }
    }

    void Update()
    {
        for(int i=0; i<listTickers.Count;)
        {
            listTickers[i].OnUpdate(CTimeMgr.DeltaTimeUnScale);

            if(listTickers[i].bActive)
            {
                i++;
            }
            else
            {
                listTickers[i] = null;
                listTickers.RemoveAt(i);
            }
        }
    }

    private void OnDestroy()
    {
        listTickers.Clear();
    }
}
