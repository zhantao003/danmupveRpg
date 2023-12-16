using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [MessageHandler]
    public class ETHandlerResSelfEnterRoom : AMHandler<Actor_EnterRoom_M2C>
    {
        protected override async ETTask Run(Session session, Actor_EnterRoom_M2C message)
        {
            //创建房间对象
            ERoomInfoMgr.Ins.InitRoom(message.RoomInfo);

            Debug.Log($"进入房间{ERoomInfoMgr.Ins.pSelfRoom.szRoomID} 最大游玩人数：{ERoomInfoMgr.Ins.pSelfRoom.nMaxPlayer}");

            RefreshUI();

            await ETTask.CompletedTask;
        }

        static void RefreshUI()
        {
            UINetMatch match = UIManager.Instance.GetUI(UIResType.ETNetMatch) as UINetMatch;
            if (match.IsOpen())
            {
                for (int i = 0; i < ERoomInfoMgr.Ins.pSelfRoom.nMaxPlayer; i++)
                {
                    ERoom.RoomSlot pRoomSlot = ERoomInfoMgr.Ins.pSelfRoom.GetPlayerByIdx(i);
                    if (pRoomSlot == null) continue;

                    //记录自己的所属阵营
                    if (pRoomSlot.userId == EUserInfoMgr.Ins.pSelf.nUserId)
                    {
                        if(pRoomSlot.nSeatIdx == 0) //左边
                        {
                            EUserInfoMgr.Ins.emSelfCamp = EMUnitCamp.Red;
                        }
                        else if(pRoomSlot.nSeatIdx == 1)
                        {
                            EUserInfoMgr.Ins.emSelfCamp = EMUnitCamp.Blue;
                        }
                        
                        continue;
                    }

                    match.MatchSetEnemyInfo(pRoomSlot.player);
                }

                match.StartCoroutine(match.OnStartGame());
            }
        }
    }
}

