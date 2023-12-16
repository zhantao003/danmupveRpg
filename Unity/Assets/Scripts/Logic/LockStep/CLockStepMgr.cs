using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CLockStepMgr : CSingleCompBase<CLockStepMgr>
{
    public enum EMType
    {
        Local,
        Net,
    }

    public EMType emType = EMType.Local;

    //累计运行的时间
    float m_fAccumilatedTime = 0;

    //下一个逻辑帧的时间
    float m_fNextGameTime = 0;

    //预定的每帧的时间长度
    float m_fFrameLen;

    ////挂载的逻辑对象
    //BattleLogic m_callUnit = null;

    //两帧之间的时间差
    float m_fInterpolation = 0;

    //小兵队列
    public List<CLockUnityObject> listUnits = new List<CLockUnityObject>();

    //子弹队列
    public List<CLockUnityObject> listBullets = new List<CLockUnityObject>();

    public CLockUnityObject pGameMgr;

    public CLockUnityObject pWaitCreatMgr;

    //物理管理器
    public CLockPhysicMgr pPhysicMgr;

    //操作指令
    public Dictionary<long, List<CLockStepEvent>> dicLockStepEvents = new Dictionary<long, List<CLockStepEvent>>();

    public bool bAutoUpdate = true;

    public bool bActive = false;

    bool bNetFrameAble = false;

    public void Init(EMType lockType = EMType.Local)
    {
        emType = lockType;

        m_fFrameLen = (float)CLockStepData.g_fixFrameLen;

        m_fAccumilatedTime = 0;

        m_fNextGameTime = 0;

        m_fInterpolation = 0;

        if (emType == EMType.Local)
        {
            bActive = true;

            if(pPhysicMgr!=null)
                pPhysicMgr.Init();
        }
        else
        {
            //if (pPhysicMgr != null)
            //    pPhysicMgr.Init();
        }
    }

    public void InitPhysicOnly()
    {
        if (pPhysicMgr != null)
            pPhysicMgr.Init();
    }

    private void Start()
    {
        //Init();
    }

    void Update()
    {
        if(bAutoUpdate)
        {
            OnLockUpdate(CTimeMgr.DeltaTime);
        }
    }

    //void FixedUpdate()
    //{
    //    if (!bActive) return;

    //    if (emType == EMType.Local)
    //    {
    //        /**************以下是帧同步的核心逻辑*********************/
    //        m_fAccumilatedTime = m_fAccumilatedTime + CTimeMgr.FixedDeltaTime;

    //        //如果真实累计的时间超过游戏帧逻辑原本应有的时间,则循环执行逻辑,确保整个逻辑的运算不会因为帧间隔时间的波动而计算出不同的结果
    //        while (m_fAccumilatedTime > m_fNextGameTime)
    //        {
    //            CheckEvent();

    //            //运行与游戏相关的具体逻辑
    //            OnUpdateLogic();

    //            //计算下一个逻辑帧应有的时间
    //            m_fNextGameTime += m_fFrameLen;

    //            //游戏逻辑帧自增
    //            CLockStepData.g_uGameLogicFrame += 1;
    //        }

    //        //计算两帧的时间差,用于运行补间动画
    //        m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

    //        //更新绘制位置
    //        OnUpdateRender(m_fInterpolation);
    //        /**************帧同步的核心逻辑完毕*********************/
    //    }
    //    else if (emType == EMType.Net)
    //    {
    //        bNetFrameAble = CLockStepData.g_uServerLogicFrame - CLockStepData.g_uGameLogicFrame >= CLockStepData.g_uServerWaitFrame;
    //        if (bNetFrameAble)
    //        {
    //            /**************以下是帧同步的核心逻辑*********************/
    //            m_fAccumilatedTime = m_fAccumilatedTime + CTimeMgr.FixedDeltaTime;

    //            //如果真实累计的时间超过游戏帧逻辑原本应有的时间,则循环执行逻辑,确保整个逻辑的运算不会因为帧间隔时间的波动而计算出不同的结果
    //            while (m_fAccumilatedTime > m_fNextGameTime)
    //            {
    //                CheckEvent();

    //                //运行与游戏相关的具体逻辑
    //                OnUpdateLogic();

    //                //计算下一个逻辑帧应有的时间
    //                m_fNextGameTime += m_fFrameLen;

    //                //游戏逻辑帧自增
    //                CLockStepData.g_uGameLogicFrame += 1;

    //                //Debug.Log("本地逻辑帧：" + CLockStepData.g_uGameLogicFrame + "    服务器帧：" + CLockStepData.g_uServerLogicFrame);
    //            }

    //            //计算两帧的时间差,用于运行补间动画
    //            m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

    //            //更新绘制位置
    //            OnUpdateRender(m_fInterpolation);
    //            /**************帧同步的核心逻辑完毕*********************/
    //        }
    //    }
    //}

    public void OnLockUpdate(float dt)
    {
        if (!bActive) return;

        if (emType == EMType.Local)
        {
            /**************以下是帧同步的核心逻辑*********************/
            m_fAccumilatedTime = m_fAccumilatedTime + dt;

            //如果真实累计的时间超过游戏帧逻辑原本应有的时间,则循环执行逻辑,确保整个逻辑的运算不会因为帧间隔时间的波动而计算出不同的结果
            while (m_fAccumilatedTime > m_fNextGameTime)
            {
                CheckEvent();

                //运行与游戏相关的具体逻辑
                OnUpdateLogic();

                //物理引擎更新
                OnUpdatePhysic();

                //计算下一个逻辑帧应有的时间
                m_fNextGameTime += m_fFrameLen;

                //游戏逻辑帧自增
                CLockStepData.g_uGameLogicFrame += 1;
            }

            //计算两帧的时间差,用于运行补间动画
            m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

            //更新绘制位置
            OnUpdateRender(m_fInterpolation);
            /**************帧同步的核心逻辑完毕*********************/
        }
        else if (emType == EMType.Net)
        {
            //if (CBattleMgr.Ins == null)
            //    return;
            //if (CBattleMgr.Ins != null &&
            //    (CBattleMgr.Ins.emGameState == CBattleMgr.EMGameState.End))
            //{
            //    return;
            //}
            bNetFrameAble = CLockStepData.g_uServerLogicFrame - CLockStepData.g_uGameLogicFrame >= CLockStepData.g_uServerWaitFrame;
            if (bNetFrameAble)
            {
                /**************以下是帧同步的核心逻辑*********************/
                m_fAccumilatedTime = m_fAccumilatedTime + dt;

                //如果真实累计的时间超过游戏帧逻辑原本应有的时间,则循环执行逻辑,确保整个逻辑的运算不会因为帧间隔时间的波动而计算出不同的结果
                while (m_fAccumilatedTime > m_fNextGameTime)
                {
                    CheckEvent();

                    //运行与游戏相关的具体逻辑
                    OnUpdateLogic();

                    //物理引擎更新
                    OnUpdatePhysic();

                    //计算下一个逻辑帧应有的时间
                    m_fNextGameTime += m_fFrameLen;

                    //游戏逻辑帧自增
                    CLockStepData.g_uGameLogicFrame += 1;

                    //暂时不更新了
                    if(CLockStepData.g_uGameLogicFrame > CLockStepData.g_uServerLogicFrame - CLockStepData.g_uServerWaitFrame)
                    {
                        while(m_fAccumilatedTime > m_fNextGameTime)
                        {
                            m_fAccumilatedTime -= m_fFrameLen;
                        }

                        break;
                    }

                    //Debug.Log("本地逻辑帧：" + CLockStepData.g_uGameLogicFrame + "    服务器帧：" + CLockStepData.g_uServerLogicFrame);
                }

                //计算两帧的时间差,用于运行补间动画
                m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

                //更新绘制位置
                OnUpdateRender(m_fInterpolation);
                /**************帧同步的核心逻辑完毕*********************/
            }
        }
    }

    void OnUpdateLogic()
    {
        if(pGameMgr != null)
        {
            pGameMgr.OnUpdateLogic();
        }
        if(pWaitCreatMgr != null)
        {
            pWaitCreatMgr.OnUpdateLogic();
        }
        for(int i=0; i<listUnits.Count; i++)
        {
            listUnits[i].OnUpdateLogic();
        }
        for (int i = 0; i < listBullets.Count; i++)
        {
            listBullets[i].OnUpdateLogic();
        }
    }

    void OnUpdateRender(float delta)
    {
        //Debug.Log("时间：" + delta);
        for (int i = 0; i < listUnits.Count; i++)
        {
            listUnits[i].OnUpdateRender(delta);
        }
        for (int i = 0; i < listBullets.Count; i++)
        {
            listBullets[i].OnUpdateRender(delta);
        }
    }

    void OnUpdatePhysic()
    {
        if (pPhysicMgr == null) return;

        pPhysicMgr.OnUpdate(CLockStepData.g_fixFrameLen);
    }

    #region 锁帧事件

    public void AddLSEvent(CLockStepEvent lsEvent)
    {
        List<CLockStepEvent> listEvents = null;
        if (!dicLockStepEvents.TryGetValue(CLockStepData.g_uGameLogicFrame + 1, out listEvents))
        {
            listEvents = new List<CLockStepEvent>();
            listEvents.Add(lsEvent);

            dicLockStepEvents.Add(CLockStepData.g_uGameLogicFrame + 1, listEvents);
        }
        else
        {
            listEvents.Add(lsEvent);
        }
    }

    public void AddLSEvent(long frame, CLockStepEvent lsEvent)
    {
        List<CLockStepEvent> listEvents = null;
        if (!dicLockStepEvents.TryGetValue(frame, out listEvents))
        {
            listEvents = new List<CLockStepEvent>();
            listEvents.Add(lsEvent);

            dicLockStepEvents.Add(frame, listEvents);
        }
        else
        {
            listEvents.Add(lsEvent);
        }
    }

    void CheckEvent()
    {
        List<CLockStepEvent> listEvents = null;
        if (!dicLockStepEvents.TryGetValue(CLockStepData.g_uGameLogicFrame, out listEvents))
        {
            return;
        }

        for (int i = 0; i < listEvents.Count; i++)
        {
            listEvents[i].DoEvent();
        }
    }

    #endregion

    #region 更新队列操作

    public void AddLockUnit(CLockUnityObject unit)
    {
        listUnits.Add(unit);
    }

    public void RemoveLockUnit(CLockUnityObject unit)
    {
        listUnits.Remove(unit);
    }

    public void AddLockBullet(CLockUnityObject bullet)
    {
        listBullets.Add(bullet);
    }

    public void RemoveLockBullet(CLockUnityObject bullet)
    {
        listBullets.Remove(bullet);
    }

    #endregion

    public void ClearAllList()
    {
        listUnits.Clear();
        listBullets.Clear();
        dicLockStepEvents.Clear();
        pGameMgr = null;
        pWaitCreatMgr = null;
        CLockStepData.g_uGameLogicFrame = 0;

        bActive = false;
    }

    public int GetRandomInt(int min, int max)
    {
        int nRes = CLockStepData.pRand.Range(min, max);

        //Debug.Log("当前帧：" + CLockStepData.g_uGameLogicFrame + "  随机数：" + nRes + "    随机总数：" + CLockStepData.pRand.count + "  最小值:" + min + "  最大值:" + max);
        
        return nRes;
    }

    public long GetRandomInt()
    {
        uint nRes = CLockStepData.pRand.Next();

        //Debug.Log("当前帧：" + CLockStepData.g_uGameLogicFrame + "  随机数：" + nRes + "    随机总数：" + CLockStepData.pRand.count);

        return (long)nRes;
    }
}
