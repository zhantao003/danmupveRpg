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
    [Header("是否可以通行")]
    public bool canMove;
    [Header("是否可以放置角色")]
    public bool canPlace;
    [Header("当前格子的类型")]
    public EMSlotType emSlotType;
    [Header("地图所在的坐标")]
    public Vector2Int vecPos;
    [Header("当前格子上所停留的角色（地面）")]
    public CPlayerUnit pStayGroundUnit;
    [Header("当前格子上所停留的角色（飞行）")]
    public CPlayerUnit pStayFlyUnit;
    /// <summary>
    /// 当前格子上设置的渲染层级
    /// </summary>
    public int nCurSetRenderLayer;
    
    public MapSlot parent;
    //通往起点的花费值 
    public int gCost;
    //通往终点的花费值
    public int hCost;

    /// <summary>
    /// 总花费值
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
    /// 判断该玩家是否可以走在该格子上面
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
    /// 进入悬停事件
    /// </summary>
    public void OnEnter()
    {
        //ActiveRenderColor(true);
    }

    /// <summary>
    /// 离开事件
    /// </summary>
    public void OnExit()
    {
        //ActiveRenderColor(false);
    }

    /// <summary>
    /// 设置初始状态
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
