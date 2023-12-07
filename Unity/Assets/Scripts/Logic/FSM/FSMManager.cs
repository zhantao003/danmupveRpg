using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class FSMManager
{
    public FSMManager(object obj)
    {
        m_objTarget = obj;
    }

    public bool bInited = false;

    public void Update(float delta)
    {
        //m_fsmCurState.OnCheckInput(m_objTarget);
        m_fsmCurState.OnUpdate(m_objTarget, delta);
        m_fsmCurState.OnUpdateAddiState(m_objTarget, delta);

        //����״̬����
        for (int nIdx = 0; nIdx < m_listFsmAddiState.Count;)
        {
            FSMBaseState ele = m_listFsmAddiState[nIdx];
            if (ele.GetActive())
            {
                //ele.OnCheckInput(m_objTarget);
                ele.OnUpdate(m_objTarget, delta);
                nIdx++;
            }
            else
            {
                ele.OnEnd(m_objTarget);
                m_listFsmAddiState.Remove(ele);
            }
        }
    }

    public void FixedUpdate(float delta)
    {
        m_fsmCurState.OnFixedUpdate(m_objTarget, delta);
        m_fsmCurState.OnFixedUpdateAddiState(m_objTarget, delta);

        //����״̬����
        for (int nIdx = 0; nIdx < m_listFsmAddiState.Count;)
        {
            FSMBaseState ele = m_listFsmAddiState[nIdx];
            if (ele.GetActive())
            {
                ele.OnFixedUpdate(m_objTarget, delta);
                nIdx++;
            }
            else
            {
                ele.OnEnd(m_objTarget);
                m_listFsmAddiState.Remove(ele);
            }
        }
    }

    public bool ChangeMainState(int nID, CLocalNetMsg pParams = null, Action pDlgSuc = null)
    {
        bool bTrans = true;

        if (bTrans)
        {
            //��ȡ��״̬����
            m_fsmNewState = GetState(nID);

            //if (!m_fsmCurState.GetType().Equals(m_fsmNewState.GetType()))
            //{

            //��ֹ��ǰ״̬
            m_fsmCurState.OnEnd(m_objTarget);
            //����ǰ״̬����
            m_fsmPerState = m_fsmCurState;
            //����״̬�л�Ϊ��ǰ״̬
            m_fsmCurState = m_fsmNewState;
            if (m_fsmCurState == null)
            {
                Debug.LogWarning("null target:" + nID);
                return false;
            }
                
            m_fsmCurState.ClearAllAddi(m_objTarget, false);
            m_fsmCurState.SetMsgParam(pParams);

            if (pDlgSuc != null)
            {
                pDlgSuc();
            }

            m_fsmPerState.SendData(m_fsmCurState);
            m_fsmCurState.OnReady(m_objTarget);
            m_fsmCurState.OnBegin(m_objTarget);

            if (m_dlg != null)
            {
                m_dlg();
            }
        }

        return bTrans;
    }

    public void SetTarget(object obj)
    {
        m_objTarget = obj;
    }

    public void AddAddiState(FSMBaseState state, CLocalNetMsg pParams = null)
    {
        state.SetMsgParam(pParams);
        state.OnReady(m_objTarget);
        state.OnBegin(m_objTarget);
        m_listFsmAddiState.Add(state);
    }

    public void ClearAllAddiState()
    {
        for (int nIdx = 0; nIdx < m_listFsmAddiState.Count; nIdx++)
        {
            FSMBaseState ele = m_listFsmAddiState[nIdx];
            ele.SetActive(false);
        }
    }

    public FSMBaseState GetCurState()
    {
        return m_fsmCurState;
    }

    public FSMBaseState GetNewState()
    {
        return m_fsmNewState;
    }

    public void RegisterStateChgDlg(OnStateChg dlg)
    {
        m_dlg += dlg;
    }

    public void UnRegisterStateChgDlg(OnStateChg dlg)
    {
        m_dlg -= dlg;
    }

    public void AddState(int nID, FSMBaseState fsm)
    {
        dicFSMStates.Add(nID, fsm);
    }

    protected FSMBaseState GetState(int nID)
    {
        FSMBaseState pRes = null;
        if (dicFSMStates.TryGetValue(nID, out pRes))
        {

        }

        return pRes;
    }

    //����״̬������״̬
    public void AddCurFSMAdditive(FSMBaseState fsm)
    {
        m_fsmCurState.AddAddiState(m_objTarget, fsm);
    }

    //�����״̬�ĸ�����״̬
    public void ClearCurFSMAdditive()
    {
        m_fsmCurState.ClearAllAddi(m_objTarget);
    }

    public void ClearAllStates()
    {
        dicFSMStates.Clear();
    }

    //����״̬
    protected FSMBaseState m_fsmPerState = new FSMBaseState(); //��һ��״̬
    protected FSMBaseState m_fsmCurState = new FSMBaseState(); //��ǰ״̬
    protected FSMBaseState m_fsmNewState = new FSMBaseState(); //��״̬

    //����״̬
    protected List<FSMBaseState> m_listFsmAddiState = new List<FSMBaseState>();
    public List<FSMBaseState> ListFSMAddiState
    {
        get
        {
            return m_listFsmAddiState;
        }
    }


    protected object m_objTarget = null; //����Ŀ���״̬

    //״̬�ı�ʱ��ί��
    public delegate void OnStateChg();
    protected OnStateChg m_dlg = null;

    //��״̬��������״̬
    protected Dictionary<int, FSMBaseState> dicFSMStates = new Dictionary<int, FSMBaseState>();
}
