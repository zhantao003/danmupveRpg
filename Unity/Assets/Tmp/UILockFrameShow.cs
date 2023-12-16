using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILockFrameShow : MonoBehaviour
{
    public Text uiLabelLocal;
    public Text uiLabelServer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LateUpdate()
    {
        uiLabelLocal.text = "L£º" + CLockStepData.g_uGameLogicFrame.ToString();
        //Debug.LogError("Cur Trame ====" + CLockStepData.g_uServerLogicFrame);
        uiLabelServer.text = "S£º" + CLockStepData.g_uServerLogicFrame.ToString();
    }
}
