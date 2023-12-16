using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class MovePanelComponent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public OperatingPanelComponent op;
    public RectTransform opPanel;

    [Header("Move按钮边界设置")]
    public float xMax;
    public float xMin;
    public float yMax;
    public float yMin;

    public Transform tranZiRoot;
    public void OnBeginDrag(PointerEventData eventData)
    {

    }

    public void OnDrag(PointerEventData eventData)
    {
        if(eventData.dragging)
        opPanel.anchoredPosition += eventData.delta*2;
        if (opPanel.anchoredPosition.x > xMax)
        {
            opPanel.anchoredPosition = new Vector2(xMax, opPanel.anchoredPosition.y);
        }
        if (opPanel.anchoredPosition.x < xMin)
        {
            opPanel.anchoredPosition = new Vector2(xMin, opPanel.anchoredPosition.y);
        }
        if (opPanel.anchoredPosition.y > yMax)
        {
            opPanel.anchoredPosition = new Vector2(opPanel.anchoredPosition.x, yMax);
        }
        if (opPanel.anchoredPosition.y < yMin)
        {
            opPanel.anchoredPosition = new Vector2(opPanel.anchoredPosition.x, yMin);
        }
        tranZiRoot.position = opPanel.position;
        op.isPointEnter = true;
        CCameraController.Ins.bDragMove = false;
        op.pointStayTime = 1f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        PlayerPrefs.SetFloat("OpPosX", opPanel.anchoredPosition.x);
        PlayerPrefs.SetFloat("OpPosY", opPanel.anchoredPosition.y);

        op.isPointEnter = false;
        CCameraController.Ins.bDragMove = true;
    }

    public void InitBoxPos() {
        float setX = PlayerPrefs.GetFloat("OpPosX", opPanel.anchoredPosition.x);
        float setY = PlayerPrefs.GetFloat("OpPosY", opPanel.anchoredPosition.y);
        if (setX < xMin)
            setX = xMin;
        if (setX > xMax)
            setX = xMax;
        opPanel.anchoredPosition = new Vector2(setX, setY);
    }
    // Update is called once per frame
    void Update()
    {

    }
}