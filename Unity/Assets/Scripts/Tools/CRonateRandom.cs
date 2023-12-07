using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CRonateRandom : MonoBehaviour
{
    public Vector2 vRonateRange;
    public Vector3 vRonateLerp;
    Vector3 vRoanteSpd;
    Transform tranSelf;

    // Start is called before the first frame update
    void Start()
    {
        tranSelf = GetComponent<Transform>();
        Refresh();
    }

    private void FixedUpdate()
    {
        tranSelf.Rotate(vRoanteSpd * CTimeMgr.FixedDeltaTime);
    }

    [ContextMenu("Refresh")]
    void Refresh()
    {
        tranSelf.localRotation = Quaternion.identity;
        vRoanteSpd = new Vector3(Random.Range(vRonateRange.x, vRonateRange.y) * vRonateLerp.x,
                                 Random.Range(vRonateRange.x, vRonateRange.y) * vRonateLerp.y,
                                 Random.Range(vRonateRange.x, vRonateRange.y) * vRonateLerp.z);
    }
}
