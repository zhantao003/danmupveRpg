using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIconLoad : MonoBehaviour
{
    public Image pImg;



    public void IconLoad(string szIcon)
    {
        Sprite sprite = Resources.Load<Sprite>("Fish/" + szIcon);
        if (sprite == null) return;

        pImg.sprite = sprite;
    }


}
