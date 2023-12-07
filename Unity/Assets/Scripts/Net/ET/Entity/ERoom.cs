using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ObjectSystem]
public class ERoomAwakeSystem : AwakeSystem<ERoom, string>
{
    public override void Awake(ERoom self, string id)
    {
        self.Awake(id);
    }
}

public class ERoom : Entity
{
    public string szRoomID;    //房间唯一ID

    public int nMapId;      //地图ID
    public int nMaxPlayer;  //最大游玩人数

    public class RoomSlot
    {
        public int nSeatIdx;
        public long userId;
        public bool isReady;
        public EUserInfo player;
    }

    // 房间座位信息
    protected List<RoomSlot> listRoomSlot = new List<RoomSlot>();

    public void Awake(string id)
    {
        szRoomID = id;
    }

    public void AddPlayer(RoomSlot slot)
    {
        if (slot == null) return;

        listRoomSlot.Add(slot);
    }

    public void RemovePlayer(long id)
    {
        for (int i = 0; i < listRoomSlot.Count;)
        {
            if (listRoomSlot[i].userId == id)
            {
                listRoomSlot.RemoveAt(i);
                return;
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    /// 获取玩家自己的信息
    /// </summary>
    public RoomSlot GetSelfSlot()
    {
        RoomSlot pRes = null;

        pRes = GetPlayerByID(EUserInfoMgr.Ins.pSelf.nUserId);

        return pRes;
    }

    /// <summary>
    /// 根据List的索引获取RoomSlot对象
    /// </summary>
    public RoomSlot GetPlayerByIdx(int idx)
    {
        if (idx < 0 || idx >= listRoomSlot.Count) return null;

        return listRoomSlot[idx];
    }

    /// <summary>
    /// 根据座位号获取RoomSlot对象
    /// </summary>
    public RoomSlot GetPlayerBySeat(int seat)
    {
        if (seat < 0 || seat >= listRoomSlot.Count) return null;

        for (int i = 0; i < listRoomSlot.Count; i++)
        {
            if (listRoomSlot[i].nSeatIdx == seat)
            {
                return listRoomSlot[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 根据玩家ID获取RoomSlot对象
    /// </summary>
    /// <param name="playerID"></param>
    public RoomSlot GetPlayerByID(long playerID)
    {
        for (int i = 0; i < listRoomSlot.Count; i++)
        {
            if (listRoomSlot[i].userId == playerID)
            {
                return listRoomSlot[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 获取房间内玩家数量
    /// </summary>
    public int GetPlayerCount()
    {
        return listRoomSlot.Count;
    }

    /// <summary>
    /// 是否都准备了
    /// </summary>
    public bool IsAllReady()
    {
        if (nMaxPlayer != listRoomSlot.Count) return false;

        for (int i = 0; i < listRoomSlot.Count; i++)
        {
            if (!listRoomSlot[i].isReady)
                return false;
        }

        return true;
    }

    public override void Dispose()
    {
        szRoomID = "";
        listRoomSlot.Clear();

        base.Dispose();
    }
}
