using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageStaticImageComponent : MonoBehaviour
{
    public Image img;
    public Sprite[] pImgsByLanguage;
    private void Start()
    {
        if (img == null) img = GetComponent<Image>();
        if (img != null)
        {
            int nType = (int)CTBLLanguageInfo.Inst.curLanguageType;
            if (nType < pImgsByLanguage.Length)
            {
                img.sprite = pImgsByLanguage[nType];
            }
        }
    }
}
