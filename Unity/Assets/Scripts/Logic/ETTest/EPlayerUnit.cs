using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EPlayerUnit : MonoBehaviour
{
    public long nUserId;
    public string szPlatformId;
    
    public Transform tranNameRoot;
    public Transform tranRoot;
    public Renderer pRenderEye;

    public bool bIsSelf;

    /// <summary>
    /// ≥ı ºªØ
    /// </summary>
    public void SetPlayerInfo(EUserInfo user, bool self)
    {
        nUserId = user.nUserId;
        szPlatformId = user.szPlatformId;
        bIsSelf = self;

        pRenderEye.material.color = (bIsSelf ? Color.blue : Color.red);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
