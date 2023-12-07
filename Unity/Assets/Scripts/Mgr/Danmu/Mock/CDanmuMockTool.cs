using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CDanmuMockTool
{
    public static CDanmuEventHandler handler;

    public static void Mock(this CDanmuChat dm)
    {
        if (handler == null) return;
        Debug.Log($"�û�uid:{dm.uid}" +
                  $"�ǳƣ�{dm.nickName}" +
                  $"��Ļ��{dm.content}");
        handler.OnDanmuChatInfo(dm);
    }

    public static void Mock(this CDanmuGift dm)
    {
        if (handler == null) return;
        Debug.Log($"�û�uid:{dm.uid}" +
                  $"�ǳƣ�{dm.nickName}" +
                  $"���{dm.giftName}" +
                  $"������{dm.giftNum}");
        handler.OnDanmuSendGift(dm);
    }

    public static void Mock(this CDanmuVipInfo dm)
    {
        if (handler == null) return;
        Debug.Log($"�û�uid:{dm.uid}" +
                  $"�ǳƣ�{dm.nickName}" +
                  $"����VIP��{dm.vipLv}");
        handler.OnDanmuVipBuy(dm);
    }

    public static void Mock(this CDanmuLike dm)
    {
        if (handler == null) return;
        Debug.Log($"�û�uid:{dm.uid}" +
                  $"�ǳƣ�{dm.nickName}" +
                  $"���ޣ�{dm.likeNum}");
        handler.OnDanmuLikeInfo(dm);
    }
}
