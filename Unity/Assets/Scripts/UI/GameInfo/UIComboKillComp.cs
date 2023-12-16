using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIComboKillComp : MonoBehaviour
{
    public Text txt;
    private Vector3 size;
    private Vector3 refV3;
    public float deadTime = 5;
    private float deadCounter;
    public float upSpeed = 100;
    private float deadTimeMult = 1;
    /// <summary>
    /// 玩家连杀记录
    /// </summary>
    /// <param name="size">整体大小</param>
    /// <param name="text">信息内容</param>
    /// <param name="color">字颜色</param>
    /// <param name="deadTimeMult">向上飘屏时间</param>
    public void Init(float size, string text, Color color, float deadTimeMult)
    {
        this.txt.transform.localScale = Vector3.one * size * 3;
        this.size = Vector3.one * size;
        this.txt.text = text;
        color.a = 0;
        this.txt.color = color;
        this.deadTimeMult = deadTimeMult;
    }

    private void Update()
    {
        txt.transform.localScale = Vector3.SmoothDamp(txt.transform.localScale, size, ref refV3, 0.3f);
        Color c = txt.color;
        c.a = Mathf.MoveTowards(c.a, 1, 3f * Time.deltaTime);
        txt.color = c;
        txt.rectTransform.localPosition += Vector3.up * upSpeed * Time.deltaTime / deadTimeMult;
        deadCounter += Time.deltaTime;
        if (deadCounter > deadTime * deadTimeMult) Destroy(gameObject);
    }

}
