using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerJoinComp : MonoBehaviour
{
    //public Text lvName;
    //public Image lvIcon;
    public Image bgImg;
    public Text playerName;
    private float deadTime = 2;
    private float deadCounter = 0;
    private bool fromLeftOrRight = false;
    private float offset = 280;
    public Transform subTs;
    //public CanvasGroup cg;
    public Animator anim;
    
    public void Init(/*string lvName, Sprite lvIcon,*/ Color color, string playerName, bool fromLeftOrRight)
    {
        //this.lvName.text = lvName;
        //this.lvIcon.sprite = lvIcon;
        bgImg.color = new Color( color.r,color.g,color.b,93f/255);
        this.playerName.text = playerName;
        this.fromLeftOrRight = fromLeftOrRight;
        //this.subTs.localPosition = fromLeftOrRight ? Vector3.left * offset : Vector3.right * offset;
        //cg.alpha = 0;

        StartCoroutine(Play());
    }

    IEnumerator Play() {
        float passTime = 0;
        if (fromLeftOrRight)
        {
            anim.CrossFadeInFixedTime("RedPlayerJoinPrefab_01_appear", 0);
            passTime = CGameEffMgr.GetAnimatorLength(anim, "RedPlayerJoinPrefab_01_appear");

        }
        else {
            anim.CrossFadeInFixedTime("BluePlayerJoinPrefab_01_appear", 0);
            passTime = CGameEffMgr.GetAnimatorLength(anim, "BluePlayerJoinPrefab_01_appear");
        }
        yield return new WaitForSeconds(passTime);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        //subTs.transform.localPosition = Vector3.MoveTowards(subTs.transform.localPosition, Vector3.zero, offset * 2 * Time.deltaTime);
        //deadCounter += Time.deltaTime;
        //if (deadCounter >= deadTime) Destroy(gameObject);
        //if (deadCounter < deadTime * 0.4f)
        //{
        //    cg.alpha = Mathf.MoveTowards(cg.alpha, 1, 1 / (deadTime * 0.2f) * Time.deltaTime);
        //}
        //if (deadCounter > deadTime * 0.6f)
        //{
        //    cg.alpha = Mathf.MoveTowards(cg.alpha, 0, 1 / (deadTime * 0.2f) * Time.deltaTime);
        //}
    }

}