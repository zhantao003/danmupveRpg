using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuLikeAttrite(CDanmuLikeConst.Like)]
public class DLikeCommon : CDanmuLikeAction
{
    public override void DoAction(CDanmuLike dm)
    {
        Debug.Log("收到点赞：" + dm.uid + "  次数：" + dm.likeNum);
    }
}
