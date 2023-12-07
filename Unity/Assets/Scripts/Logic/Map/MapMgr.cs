using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public Dictionary<Vector2Int, MapSlot> dicMapSlots = new Dictionary<Vector2Int, MapSlot>();

    public Vector2Int vMapSize;

    public GameObject objMapSlot;

    public Transform tranParent;

    public void InitMap()
    {
        for(int i = 0;i < vMapSize.x;i++)
        {
            for(int j = 0; j < vMapSize.y;j++)
            {
                Vector2Int vPos = new Vector2Int(i, j);
                GameObject objNewSlot = GameObject.Instantiate(objMapSlot);
                objNewSlot.gameObject.name = $"map_{vPos.x}:{vPos.y}";
                Transform tranNewSlot = objNewSlot.GetComponent<Transform>();
                tranNewSlot.SetParent(tranParent);

                //计算地块的坐标
                Vector3 v64SlotPos = HexMetrics.GetPosFromHexGrid(vPos.x, vPos.y);
                tranNewSlot.localPosition = v64SlotPos;

                MapSlot mapSlot = objNewSlot.GetComponent<MapSlot>();
                //mapSlot.v64SlotPos = v64SlotPos;
                //mapSlot.vecPos = vPos;
                //mapSlot.SetText();
                //mapSlot.SetRenderLayer((nYCount - y - 1) * nLayerLerp + x);
                //listMapSlots.Add(mapSlot);
                dicMapSlots.Add(vPos, mapSlot);
                objNewSlot.SetActive(true);
            }
        }
    }


}
