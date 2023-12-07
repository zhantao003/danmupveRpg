using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuGiftAttrite(CDanmuGiftConst.Test)]
public class DGiftTest : CDanmuGiftAction
{
    public override void DoAction(CDanmuGift dm)
    {
        Debug.Log(" ’µΩ¿ÒŒÔ£∫" + dm.giftName);
    }
}
