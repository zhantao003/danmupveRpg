using FixMath.NET;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AddTypeByCamp
{
    Hp,
    Atk,
    MoveSpeed,
    AtkSpeed,
}

/// <summary>
/// 每个兵种增加的数值
/// </summary>
public class CPreSolderAddValue
{
    public Fix64[] fAddValue;
    public Fix64[] fAddValue2;
    public Fix64[] fAddValue3; 
    public Fix64[] fAddValue4;
}

[System.Serializable]
public class CampInfo
{
    public EMCamp emCamp;

    public string szCampName;
    public string szCampCNName;
    public string szBuffName;

    public AddTypeByCamp emAddType;

    public int[] nValues;

    public CPreSolderAddValue[] nSolderAddValue;

    public Color pColor;

    public Sprite pHpSprite;
    public Sprite pHpSpriteSpe;
    public Sprite pPlayerScoreBG;
    public Sprite pCampHeadBG;
    public Sprite pCampHead;
    public Sprite pStartFightBG;
    public Sprite pStartFightNameBG;

    public int nBuffSolderTBLID;            //光环兵ID

    public int nHeroSolderTBLID;            //英雄ID

    public bool bUpDead;                    //上路兵营是否被摧毁

    public bool bCenterDead;                //中路兵营是否被摧毁

    public bool bDownDead;                  //下路兵营是否被摧毁
    
    public CampInfo()
    {
        bUpDead = false;
        bCenterDead = false;
        bDownDead = false;
    }

    public CampInfo(CampInfo info)
    {
        nBuffSolderTBLID = info.nBuffSolderTBLID;
        nHeroSolderTBLID = info.nHeroSolderTBLID;
        emCamp = info.emCamp;
        szCampName = info.szCampName;
        szCampCNName = info.szCampCNName;
        szBuffName = info.szBuffName;
        emAddType = info.emAddType;
        nValues = info.nValues;
        nSolderAddValue = info.nSolderAddValue;
        pColor = info.pColor;
        pHpSprite = info.pHpSprite;
        pHpSpriteSpe = info.pHpSpriteSpe;
        pPlayerScoreBG = info.pPlayerScoreBG;
        pCampHeadBG = info.pCampHeadBG;
        pCampHead = info.pCampHead;
        pStartFightBG = info.pStartFightBG;
        pStartFightNameBG = info.pStartFightNameBG;
        bUpDead = false;
        bCenterDead = false;
        bDownDead = false;
    }

}
