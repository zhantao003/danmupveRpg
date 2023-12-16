using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIBuildDestroyComp : MonoBehaviour
{
    public Text txt_Des;
    private Vector2 refV2;
    //public CanvasGroup cg;
    public List<MaskableGraphic> cgs;
    private bool cgOn;
    private bool inited = false;
    private RectTransform rTransform;
    private Vector2 target;
    private bool redOrBlue;

    Color colorPlay;
    public void Init(string txtDes,bool redOrBlue)
    {
        txt_Des.text = txtDes;
        this.rTransform = GetComponent<RectTransform>();
        for (int i = 0; i < cgs.Count; i++) {
            colorPlay = cgs[i].color;
            colorPlay.a = 0;
            cgs[i].color = colorPlay;
        }
        this.redOrBlue = redOrBlue;
        StartCoroutine(Play());
    }

    private void Update()
    {
        if (!inited) return;
        rTransform.anchoredPosition = Vector2.SmoothDamp(rTransform.anchoredPosition, target, ref refV2, 0.2f);
        for (int i = 0; i < cgs.Count; i++)
        {
            float r = Mathf.MoveTowards(cgs[i].color.a, cgOn ? 1 : 1, 3 * Time.deltaTime);
            colorPlay = cgs[i].color;
            colorPlay.a = r;
            cgs[i].color = colorPlay;
        }
    }

    private IEnumerator Play()
    {

        RectTransform rt = GetComponent<RectTransform>();
        float width = rt.rect.width;
        rTransform.anchoredPosition = Vector2.right * (width+50) * (redOrBlue ? -1 : 1);
        target = Vector2.zero;
        yield return new WaitForEndOfFrame();
        this.inited = true;

        cgOn = true;
        yield return new WaitForSeconds(3f);
        cgOn = false;
        target = Vector2.left * (width+50) * (redOrBlue ? 1 : -1);
        while (true)
        {
            if (Vector2.Distance(rt.anchoredPosition, target) < 1) Destroy(gameObject);
            yield return new WaitForEndOfFrame();
        }
    }
}
