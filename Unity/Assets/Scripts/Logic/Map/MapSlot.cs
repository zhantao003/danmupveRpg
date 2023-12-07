using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSlot : MonoBehaviour
{
    public GameObject objSlot;

    public Vector2Int vPos;



    public Vector2Int GetPos()
    {
        return vPos;
    }

    public bool bUse()
    {
        return false;
    }

    public void SetUnit()
    {

    }

}
