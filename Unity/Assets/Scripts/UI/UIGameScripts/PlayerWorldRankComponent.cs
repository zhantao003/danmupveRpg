using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWorldRankComponent : MonoBehaviour
{
    public CanvasGroup alphaGroup;
    public Text worldRank;
    public Text lvTxt;
    public RawImage playerFace;
    public GameObject BG;
    public Text playerName;
    public Text playerPoint;

    public Slider expSlider;
    private float fillStartAt;
    private float fillEndAt;
    private float fillSpeed;

    private bool inited = false;

    public bool show = true;
    //LocalLvData newLvData;

    //public void Init(int rank, PlayerInfoContent playerInfo) {
    //    MainGameController.Instance.StartCoroutine(GetImgSprite(playerInfo.FaceUrl));
    //    playerName.text = playerInfo.CName;
    //    playerPoint.text = playerInfo.Exp.ToString("F0");
    //    alphaGroup.alpha = 0;
    //    expSlider = GetComponentInChildren<Slider>();
    //    if (rank > 3)
    //    {
    //        worldRank.text = rank.ToString();
    //        if (rank % 2 == 1) {
    //            BG.gameObject.SetActive(false);
    //        }
    //    }
    //    else
    //    {
    //        BG.gameObject.SetActive(false);
    //        worldRank.text = "";
    //    }
    //    (int a, int b, int c, LocalLvData lld) = SailMath.GetLevel((int)playerInfo.Exp);
    //    newLvData = lld;
    //    lvTxt.text = lld.lvName;
    //    fillStartAt = 0;
    //    fillEndAt = (float)((int)playerInfo.Exp - b) / (float)(c - b);

    //    inited = true;
    //    expSlider.value = fillStartAt;
    //    fillSpeed = (fillEndAt - fillStartAt) / 5f;
    //}

    //IEnumerator GetImgSprite(string faceUrl)
    //{
    //    yield return ServerQuery.GetImgSprite(faceUrl, (Texture2D faceSprite) =>
    //    {
    //        if(playerFace!=null)
    //            playerFace.texture = faceSprite;
    //    });
    //}
    //// Update is called once per frame
    //void Update()
    //{
    //    if (!inited )
    //    {
    //        alphaGroup.alpha = 0f;
    //        return;
    //    }
    //    alphaGroup.alpha = Mathf.MoveTowards(alphaGroup.alpha, show ? 1 : 0, 2f * Time.deltaTime);
    //    fillStartAt = Mathf.MoveTowards(fillStartAt, fillEndAt, fillSpeed * Time.deltaTime);
    //    expSlider.value = fillStartAt % 1f;
    //}

    //public void Release()
    //{
    //    show = false;
    //}
}
