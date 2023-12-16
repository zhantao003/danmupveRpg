using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    public Text uiFPS;
    public Text uiCount;

    public float fpsMeasuringDelta = 2.0f;

    private float timePassed;
    private int m_FrameCount = 0;
    private float m_FPS = 0.0f;

    private void Start()
    {
        timePassed = 0.0f;
    }

    private void Update()
    {
        m_FrameCount = m_FrameCount + 1;
        timePassed = timePassed + Time.deltaTime;

        if (timePassed > fpsMeasuringDelta)
        {
            m_FPS = m_FrameCount / timePassed;
            timePassed = 0.0f;
            m_FrameCount = 0;
        }
        uiFPS.text = "FPS:" + m_FPS.ToString("f0");
        if (CPlayerMgr.Ins == null) return;
        uiCount.text = "������:" + CPlayerMgr.Ins.GetAllAliveCount();
    }
}
