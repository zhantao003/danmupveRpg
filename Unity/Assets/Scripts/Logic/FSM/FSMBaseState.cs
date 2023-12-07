using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class FSMBaseState 
{
    //状态类型(只能有一个主状态执行Update）
    public enum EM_FSM_Type
    { 
        EM_FSM_Type_NULL = 0,
        EM_FSM_Type_Main,  //主状态 
        EM_FSM_Type_AddOn, //附加状态  
        EM_FSM_TYPE_Dead,
    };

    public virtual void OnReady(object obj) { }

    public virtual void OnBegin(object obj) { }

    public virtual void OnUpdate(object obj, float delta) { }

    public virtual void OnFixedUpdate(object obj, float delta) { }

    public virtual void OnEnd(object obj) { ClearAllAddi(obj, true); }

    //与下一个状态做一次额外交互
    public virtual void SendData(FSMBaseState state) { } 

    public EM_FSM_Type GetStateType() { return m_emType; }

    public void SetActive(bool bActive) { m_bActive = bActive; }

    public bool GetActive() { return m_bActive; }

    //设置状态变量
    public virtual void SetMsgParam(CLocalNetMsg pMsg)
    {
        pMsgParam = pMsg;
    }

    protected EM_FSM_Type m_emType = EM_FSM_Type.EM_FSM_Type_NULL;

    protected bool m_bActive = true; //状该态是否处于激活状态，false则从状态机销毁该状态

    protected object m_data = null;

    protected CLocalNetMsg pMsgParam = null;

    #region 附加状态

    public List<FSMBaseState> listAddiState = new List<FSMBaseState>(); //附加状态

    /// <summary>
    /// 添加附加状态
    /// </summary>
    /// <param name="fsm"></param>
    public void AddAddiState(object obj, FSMBaseState fsm)
    {
        fsm.SetActive(true);
        fsm.OnReady(obj);
        fsm.OnBegin(obj);
        listAddiState.Add(fsm);
    }

    /// <summary>
    /// 清除所有附加状态
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="bEnd"></param>
    public void ClearAllAddi(object obj, bool bEnd = true)
    {
        if(bEnd)
        {
            for(int i=0; i<listAddiState.Count; i++)
            {
                listAddiState[i].OnEnd(obj);
            }
        }

        listAddiState.Clear();
    }

    public void OnUpdateAddiState(object obj, float dt)
    {
        for (int nIdx = 0; nIdx < listAddiState.Count;)
        {
            FSMBaseState ele = listAddiState[nIdx];
            if (ele.GetActive())
            {
                //ele.OnCheckInput(m_objTarget);
                ele.OnUpdate(obj, dt);
                nIdx++;
            }
            else
            {
                ele.OnEnd(obj);
                listAddiState.Remove(ele);
            }
        }
    }

    public void OnFixedUpdateAddiState(object obj, float dt)
    {
        for (int nIdx = 0; nIdx < listAddiState.Count;)
        {
            FSMBaseState ele = listAddiState[nIdx];
            if (ele.GetActive())
            {
                //ele.OnCheckInput(m_objTarget);
                ele.OnFixedUpdate(obj, dt);
                nIdx++;
            }
            else
            {
                ele.OnEnd(obj);
                listAddiState.Remove(ele);
            }
        }
    }

    #endregion
}