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

    //�ۼ����е�ʱ��
    float m_fAccumilatedTime = 0;

    //��һ���߼�֡��ʱ��
    float m_fNextGameTime = 0;

    //Ԥ����ÿ֡��ʱ�䳤��
    float m_fFrameLen;

    ////���ص��߼�����
    //BattleLogic m_callUnit = null;

    //��֮֡���ʱ���
    float m_fInterpolation = 0;

    //С������
    public List<CLockUnityObject> listUnits = new List<CLockUnityObject>();

    //�ӵ�����
    public List<CLockUnityObject> listBullets = new List<CLockUnityObject>();

    public CLockUnityObject pGameMgr;

    public CLockUnityObject pWaitCreatMgr;

    //���������
    public CLockPhysicMgr pPhysicMgr;

    //����ָ��
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
    //        /**************������֡ͬ���ĺ����߼�*********************/
    //        m_fAccumilatedTime = m_fAccumilatedTime + CTimeMgr.FixedDeltaTime;

    //        //�����ʵ�ۼƵ�ʱ�䳬����Ϸ֡�߼�ԭ��Ӧ�е�ʱ��,��ѭ��ִ���߼�,ȷ�������߼������㲻����Ϊ֡���ʱ��Ĳ������������ͬ�Ľ��
    //        while (m_fAccumilatedTime > m_fNextGameTime)
    //        {
    //            CheckEvent();

    //            //��������Ϸ��صľ����߼�
    //            OnUpdateLogic();

    //            //������һ���߼�֡Ӧ�е�ʱ��
    //            m_fNextGameTime += m_fFrameLen;

    //            //��Ϸ�߼�֡����
    //            CLockStepData.g_uGameLogicFrame += 1;
    //        }

    //        //������֡��ʱ���,�������в��䶯��
    //        m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

    //        //���»���λ��
    //        OnUpdateRender(m_fInterpolation);
    //        /**************֡ͬ���ĺ����߼����*********************/
    //    }
    //    else if (emType == EMType.Net)
    //    {
    //        bNetFrameAble = CLockStepData.g_uServerLogicFrame - CLockStepData.g_uGameLogicFrame >= CLockStepData.g_uServerWaitFrame;
    //        if (bNetFrameAble)
    //        {
    //            /**************������֡ͬ���ĺ����߼�*********************/
    //            m_fAccumilatedTime = m_fAccumilatedTime + CTimeMgr.FixedDeltaTime;

    //            //�����ʵ�ۼƵ�ʱ�䳬����Ϸ֡�߼�ԭ��Ӧ�е�ʱ��,��ѭ��ִ���߼�,ȷ�������߼������㲻����Ϊ֡���ʱ��Ĳ������������ͬ�Ľ��
    //            while (m_fAccumilatedTime > m_fNextGameTime)
    //            {
    //                CheckEvent();

    //                //��������Ϸ��صľ����߼�
    //                OnUpdateLogic();

    //                //������һ���߼�֡Ӧ�е�ʱ��
    //                m_fNextGameTime += m_fFrameLen;

    //                //��Ϸ�߼�֡����
    //                CLockStepData.g_uGameLogicFrame += 1;

    //                //Debug.Log("�����߼�֡��" + CLockStepData.g_uGameLogicFrame + "    ������֡��" + CLockStepData.g_uServerLogicFrame);
    //            }

    //            //������֡��ʱ���,�������в��䶯��
    //            m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

    //            //���»���λ��
    //            OnUpdateRender(m_fInterpolation);
    //            /**************֡ͬ���ĺ����߼����*********************/
    //        }
    //    }
    //}

    public void OnLockUpdate(float dt)
    {
        if (!bActive) return;

        if (emType == EMType.Local)
        {
            /**************������֡ͬ���ĺ����߼�*********************/
            m_fAccumilatedTime = m_fAccumilatedTime + dt;

            //�����ʵ�ۼƵ�ʱ�䳬����Ϸ֡�߼�ԭ��Ӧ�е�ʱ��,��ѭ��ִ���߼�,ȷ�������߼������㲻����Ϊ֡���ʱ��Ĳ������������ͬ�Ľ��
            while (m_fAccumilatedTime > m_fNextGameTime)
            {
                CheckEvent();

                //��������Ϸ��صľ����߼�
                OnUpdateLogic();

                //�����������
                OnUpdatePhysic();

                //������һ���߼�֡Ӧ�е�ʱ��
                m_fNextGameTime += m_fFrameLen;

                //��Ϸ�߼�֡����
                CLockStepData.g_uGameLogicFrame += 1;
            }

            //������֡��ʱ���,�������в��䶯��
            m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

            //���»���λ��
            OnUpdateRender(m_fInterpolation);
            /**************֡ͬ���ĺ����߼����*********************/
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
                /**************������֡ͬ���ĺ����߼�*********************/
                m_fAccumilatedTime = m_fAccumilatedTime + dt;

                //�����ʵ�ۼƵ�ʱ�䳬����Ϸ֡�߼�ԭ��Ӧ�е�ʱ��,��ѭ��ִ���߼�,ȷ�������߼������㲻����Ϊ֡���ʱ��Ĳ������������ͬ�Ľ��
                while (m_fAccumilatedTime > m_fNextGameTime)
                {
                    CheckEvent();

                    //��������Ϸ��صľ����߼�
                    OnUpdateLogic();

                    //�����������
                    OnUpdatePhysic();

                    //������һ���߼�֡Ӧ�е�ʱ��
                    m_fNextGameTime += m_fFrameLen;

                    //��Ϸ�߼�֡����
                    CLockStepData.g_uGameLogicFrame += 1;

                    //��ʱ��������
                    if(CLockStepData.g_uGameLogicFrame > CLockStepData.g_uServerLogicFrame - CLockStepData.g_uServerWaitFrame)
                    {
                        while(m_fAccumilatedTime > m_fNextGameTime)
                        {
                            m_fAccumilatedTime -= m_fFrameLen;
                        }

                        break;
                    }

                    //Debug.Log("�����߼�֡��" + CLockStepData.g_uGameLogicFrame + "    ������֡��" + CLockStepData.g_uServerLogicFrame);
                }

                //������֡��ʱ���,�������в��䶯��
                m_fInterpolation = (m_fAccumilatedTime + m_fFrameLen - m_fNextGameTime) / m_fFrameLen;

                //���»���λ��
                OnUpdateRender(m_fInterpolation);
                /**************֡ͬ���ĺ����߼����*********************/
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
        //Debug.Log("ʱ�䣺" + delta);
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

    #region ��֡�¼�

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

    #region ���¶��в���

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

        //Debug.Log("��ǰ֡��" + CLockStepData.g_uGameLogicFrame + "  �������" + nRes + "    ���������" + CLockStepData.pRand.count + "  ��Сֵ:" + min + "  ���ֵ:" + max);
        
        return nRes;
    }

    public long GetRandomInt()
    {
        uint nRes = CLockStepData.pRand.Next();

        //Debug.Log("��ǰ֡��" + CLockStepData.g_uGameLogicFrame + "  �������" + nRes + "    ���������" + CLockStepData.pRand.count);

        return (long)nRes;
    }
}
