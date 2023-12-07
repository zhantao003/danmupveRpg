using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ERoomSimpleInfo
{
    public string szRoomId;
    public int nMaxPlayer;
    public int nMapId;
}

public class ERoomInfoMgr :CSingleMgrBase<ERoomInfoMgr>
{
    public ERoom pSelfRoom; //��ǰ���ڵķ���

    public List<ERoomSimpleInfo> listPublicRooms = new List<ERoomSimpleInfo>();

    public void InitRoom(DRoomInfo RoomInfo)
    {
        //�����������
        ERoom pRoom = ComponentFactory.Create<ERoom, string>(RoomInfo.RoomId);
        pRoom.nMapId = RoomInfo.RoomConfig.MapId;
        pRoom.nMaxPlayer = RoomInfo.RoomConfig.MaxPlayer;

        //�������λ��Ϣ
        for (int i = 0; i < RoomInfo.Seats.Count; i++)
        {
            DRoomSeatInfo pSeatInfo = RoomInfo.Seats[i];
            if (pSeatInfo.PlayerInfo == null) continue;

            AddRoomPlayer(ref pRoom, pSeatInfo);
        }

        pSelfRoom = pRoom;
    }

    public void AddRoomPlayer(ref ERoom room, DRoomSeatInfo seatInfo)
    {
        ERoom.RoomSlot pOldSlot = room.GetPlayerBySeat(seatInfo.SeatIdx);
        if(pOldSlot!=null)
        {
            room.RemovePlayer(pOldSlot.userId);
        }

        ERoom.RoomSlot pRoomSlot = new ERoom.RoomSlot();
        pRoomSlot.nSeatIdx = seatInfo.SeatIdx;
        pRoomSlot.isReady = seatInfo.IsReady;
        pRoomSlot.userId = seatInfo.PlayerInfo.PlayerId;

        pRoomSlot.player = new EUserInfo();
        pRoomSlot.player.nUserId = seatInfo.PlayerInfo.PlayerId;
        pRoomSlot.player.szPlatformId = seatInfo.PlayerInfo.PlatformId;
        pRoomSlot.player.szNickName = seatInfo.PlayerInfo.PlayerName;
        pRoomSlot.player.szHeadIcon = seatInfo.PlayerInfo.PlayerHead;

        room.AddPlayer(pRoomSlot);
    }

    #region �����б����

    public void AddPublicRoom(ERoomSimpleInfo roomInfo)
    {
        listPublicRooms.Add(roomInfo);
    }

    public ERoomSimpleInfo GetPublicRoomByIdx(int idx)
    {
        ERoomSimpleInfo pInfo = new ERoomSimpleInfo();

        if (idx < 0 || idx > listPublicRooms.Count) return pInfo;
        pInfo = listPublicRooms[idx];

        return pInfo;
    }

    public int GetPublicNum()
    {
        return listPublicRooms.Count;
    }

    public void ClearPublicRoom()
    {
        listPublicRooms.Clear();
    }

    #endregion

    public bool IsInRoom()
    {
        return pSelfRoom != null;
    }

    public void Dispose()
    {
        if (pSelfRoom != null)
        {
            pSelfRoom.Dispose();
            pSelfRoom = null;
        }
    }
}
