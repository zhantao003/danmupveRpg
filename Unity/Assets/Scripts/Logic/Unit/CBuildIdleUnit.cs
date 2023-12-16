using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuildIdleUnit : MonoBehaviour
{
    public SpriteRenderer pRenderer;

    public Sprite[] pBuildTex;

    public void Init(EMUnitCamp camp = EMUnitCamp.Blue)
    {
        if (camp == EMUnitCamp.Blue)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            pRenderer.sprite = pBuildTex[(int)CBattleMgr.Ins.mapMgr.pBlueBase.pCampInfo.emCamp];
        }
        else if (camp == EMUnitCamp.Red)
        {
            transform.localScale = Vector3.one;
            pRenderer.sprite = pBuildTex[(int)CBattleMgr.Ins.mapMgr.pRedBase.pCampInfo.emCamp];
        }
    }
}
