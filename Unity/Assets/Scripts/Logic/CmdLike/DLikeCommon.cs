using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuLikeAttrite(CDanmuLikeConst.Like)]
public class DLikeCommon : CDanmuLikeAction
{
    public override void DoAction(CDanmuLike dm)
    {
        Debug.Log("�յ����ޣ�" + dm.uid + "  ������" + dm.likeNum);
    }
}
