using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CDanmuCmdAttrite(CDanmuEventConst.JoinQueue)]
public class DCmdJoinGame : CDanmuCmdAction
{
    public override void DoAction(CDanmuChat dm, string addInfo)
    {
        //if (CGameColorFishMgr.Ins.pMap == null) return;

        Debug.Log("��ң�" + dm.nickName + " �������Ϸ");

        CPlayerBaseInfo pPlayerInfo = CPlayerMgr.Ins.GetPlayer(dm.uid.ToString());
        if (pPlayerInfo == null)
        {
            //�ȵ�¼
            CPlayerNetHelper.Login(dm.uid, CDanmuSDKCenter.Ins.szRoomId, dm.nickName, dm.headIcon, CPlayerBaseInfo.EMUserType.Guanzhong,
                                   dm.fanLv, dm.fanName, dm.fanEquip, dm.vipLv,
                                   new HHandlerJoinGame(dm.nickName, dm.headIcon, 
                                   "", null,
                                   delegate(CPlayerBaseInfo value) {
                                       //CGameColorPetMgr.Ins.JoinPlayer(value);
                                   }));
        }
        else
        {
            //CGameColorPetMgr.Ins.JoinPlayer(pPlayerInfo);
        }
    }

}
