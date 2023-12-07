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
    public string szRoomID;    //����ΨһID

    public int nMapId;      //��ͼID
    public int nMaxPlayer;  //�����������

    public class RoomSlot
    {
        public int nSeatIdx;
        public long userId;
        public bool isReady;
        public EUserInfo player;
    }

    // ������λ��Ϣ
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
    /// ��ȡ����Լ�����Ϣ
    /// </summary>
    public RoomSlot GetSelfSlot()
    {
        RoomSlot pRes = null;

        pRes = GetPlayerByID(EUserInfoMgr.Ins.pSelf.nUserId);

        return pRes;
    }

    /// <summary>
    /// ����List��������ȡRoomSlot����
    /// </summary>
    public RoomSlot GetPlayerByIdx(int idx)
    {
        if (idx < 0 || idx >= listRoomSlot.Count) return null;

        return listRoomSlot[idx];
    }

    /// <summary>
    /// ������λ�Ż�ȡRoomSlot����
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
    /// �������ID��ȡRoomSlot����
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
    /// ��ȡ�������������
    /// </summary>
    public int GetPlayerCount()
    {
        return listRoomSlot.Count;
    }

    /// <summary>
    /// �Ƿ�׼����
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
