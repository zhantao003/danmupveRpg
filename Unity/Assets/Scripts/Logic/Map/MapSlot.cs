using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EMSlotStateType
{
    CanMove,
    NoMove,
}

public enum EMSlotType
{
    Normal,
    Event,
    Item,
}

public class MapSlot : MonoBehaviour
{
    public GameObject objSelf;
    public Transform tranSelf;
    [Header("�Ƿ����ͨ��")]
    public bool canMove;
    [Header("�Ƿ���Է��ý�ɫ")]
    public bool canPlace;
    [Header("��ǰ���ӵ�����")]
    public EMSlotType emSlotType;
    [Header("��ͼ���ڵ�����")]
    public Vector2Int vecPos;
    [Header("��ǰ��������ͣ���Ľ�ɫ�����棩")]
    public CPlayerUnit pStayGroundUnit;
    [Header("��ǰ��������ͣ���Ľ�ɫ�����У�")]
    public CPlayerUnit pStayFlyUnit;
    /// <summary>
    /// ��ǰ���������õ���Ⱦ�㼶
    /// </summary>
    public int nCurSetRenderLayer;
    
    public MapSlot parent;
    //ͨ�����Ļ���ֵ 
    public int gCost;
    //ͨ���յ�Ļ���ֵ
    public int hCost;

    /// <summary>
    /// �ܻ���ֵ
    /// </summary>
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public Renderer pRenderer;

    bool bActive;

    public GameObject objColor;

    public TextMesh pText;

    public FixVector3 v64SlotPos;

    public void Init(bool bCanMove)
    {
        canMove = bCanMove;
        SetOriState();
    }

    public void SetText()
    {
        pText.text = "(" + vecPos.x + "," + vecPos.y + ")";
    }

    public void ActiveRenderColor(bool bActive)
    {
        objColor.SetActive(bActive);
    }

    public void SetRenderLayer(int nLayer)
    {
        nCurSetRenderLayer = nLayer;
        if (pRenderer != null)
            pRenderer.sortingOrder = nCurSetRenderLayer;
    }

    public void SetActive(bool active)
    {
        bActive = active;
        if (objSelf != null)
            objSelf.SetActive(bActive);
    }

    /// <summary>
    /// �жϸ�����Ƿ�������ڸø�������
    /// </summary>
    /// <param name="pUnit"></param>
    /// <returns></returns>
    public bool bCanMove(CPlayerUnit pUnit)
    {
        bool bCanMove = true;
        if (pUnit.emMoveType == CPlayerUnit.EMMoveType.Ground &&
            pStayGroundUnit != null)
        {
            if (pUnit.emStayRange == CPlayerUnit.EMStayRange.Normal)
            {
                bCanMove = false;
            }
            else
            {
                if (!pUnit.CheckMapSlotCanMove(vecPos))
                {
                    bCanMove = false;
                }
            }
        }
        else if (pUnit.emMoveType == CPlayerUnit.EMMoveType.Fly)
            //&& pStayFlyUnit != null)
        {
            if (pUnit.emStayRange == CPlayerUnit.EMStayRange.Normal &&
                pStayFlyUnit != null)
            {
                bCanMove = false;
            }
            else
            {
                if (!pUnit.CheckMapSlotCanMove(vecPos))
                {
                    bCanMove = false;
                }
            }
        }
        else
        {
            if (!pUnit.CheckMapSlotCanMove(vecPos))
            {
                bCanMove = false;
            }
        }


        return bCanMove;
    }

    /// <summary>
    /// ������ͣ�¼�
    /// </summary>
    public void OnEnter()
    {
        //ActiveRenderColor(true);
    }

    /// <summary>
    /// �뿪�¼�
    /// </summary>
    public void OnExit()
    {
        //ActiveRenderColor(false);
    }

    /// <summary>
    /// ���ó�ʼ״̬
    /// </summary>
    void SetOriState()
    {
        
    }

    public FixVector3 GetV64Pos()
    {
        return v64SlotPos; 
    }

    public CLocalNetMsg ToNetMsg()
    {
        CLocalNetMsg msg = new CLocalNetMsg();

        msg.SetInt("X", vecPos.x);
        msg.SetInt("Y", vecPos.y);
        msg.SetInt("BindUnit", pStayGroundUnit == null ? 0 : 1);
        

        return msg;
    }

}
