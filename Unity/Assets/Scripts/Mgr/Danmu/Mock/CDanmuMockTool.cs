using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CDanmuMockTool
{
    public static CDanmuEventHandler handler;

    public static void Mock(this CDanmuChat dm)
    {
        if (handler == null) return;
        Debug.Log($"用户uid:{dm.uid}" +
                  $"昵称：{dm.nickName}" +
                  $"弹幕：{dm.content}");
        handler.OnDanmuChatInfo(dm);
    }

    public static void Mock(this CDanmuGift dm)
    {
        if (handler == null) return;
        Debug.Log($"用户uid:{dm.uid}" +
                  $"昵称：{dm.nickName}" +
                  $"礼物：{dm.giftName}" +
                  $"数量：{dm.giftNum}");
        handler.OnDanmuSendGift(dm);
    }

    public static void Mock(this CDanmuVipInfo dm)
    {
        if (handler == null) return;
        Debug.Log($"用户uid:{dm.uid}" +
                  $"昵称：{dm.nickName}" +
                  $"购买VIP：{dm.vipLv}");
        handler.OnDanmuVipBuy(dm);
    }

    public static void Mock(this CDanmuLike dm)
    {
        if (handler == null) return;
        Debug.Log($"用户uid:{dm.uid}" +
                  $"昵称：{dm.nickName}" +
                  $"点赞：{dm.likeNum}");
        handler.OnDanmuLikeInfo(dm);
    }
}
