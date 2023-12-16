using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitCheckTarget : MonoBehaviour
{
    public int nRange;
    public bool bCheck;
    public GameObject objSphere;
    public MapSlot mapSlot;

    Collider2D[] cols;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            objSphere.SetActive(true);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            objSphere.SetActive(false);
        }
        if (!bCheck)
            return;
        if(Input.GetKeyDown(KeyCode.N))
        {
            float fRadius = 0.25f + nRange * 0.6f;
            if (cols != null)
            {
                for (int i = 0; i < cols.Length; i++)
                {
                    if (cols[i].GetComponent<UnitCheckTarget>() != null)
                    {
                        cols[i].GetComponent<UnitCheckTarget>().mapSlot.objColor.SetActive(false);
                    }
                }
            }
            cols = Physics2D.OverlapCircleAll(transform.position, fRadius);
            for (int i = 0;i < cols.Length;i++)
            {
                if(cols[i].GetComponent<UnitCheckTarget>() != null)
                {
                    cols[i].GetComponent<UnitCheckTarget>().mapSlot.objColor.SetActive(true);
                }
            }
            Debug.LogError(cols.Length);
        }
    }
}
