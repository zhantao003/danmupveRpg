using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerBaseInfo
{
    public enum EMUserType
    {
        Zhubo = 0,      //����
        Guanzhong = 1,  //����
    }

    public enum EMState
    {
        None,

        IdleQueue,       //�ŶӴ�����
        ActiveQueue,     //��Ϸ������
        Gaming,          //��Ϸ��
    }

    /// <summary>
    /// �û�UID
    /// </summary>
    public string uid;

    /// <summary>
    /// �û��ǳ�
    /// </summary>
    public string userName;

    /// <summary>
    /// �û�ͷ��
    /// </summary>
    public string userFace;

    /// <summary>
    /// ��˿ѫ�µȼ�
    /// </summary>
    public long fansMedalLevel;

    /// <summary>
    /// ��˿ѫ����
    /// </summary>
    public string fansMedalName;

    /// <summary>
    /// ����ķ�˿ѫ�����״̬
    /// </summary>
    public bool fansMedalWearingStatus;

    /// <summary>
    /// �󺽺��ȼ�
    /// </summary>
    public long guardLevel;

    /// <summary>
    /// �û�����
    /// </summary>
    public EMUserType emUserType = EMUserType.Guanzhong;

    /// <summary>
    /// �û�״̬
    /// </summary>
    public EMState emState = EMState.None;

    /// <summary>
    /// ��Ļ���յ�ֱ����
    /// </summary>
    public string roomId;

    /// <summary>
    /// ��Ϸ��
    /// </summary>
    public long nGameCoins;

    public CPlayerBaseInfo(string _uid, string _userName, string _userFace, long _fansMedalLevel, string _fansMedalName, bool _fansMedalWearingStatus, long _guardLevel, string _roomId, EMUserType userType)
    {
        Init(_uid, _userName, _userFace, _fansMedalLevel, _fansMedalName, _fansMedalWearingStatus, _guardLevel, _roomId, userType);
    }

    public void Init(string _uid, string _userName, string _userFace, long _fansMedalLevel, string _fansMedalName, bool _fansMedalWearingStatus, long _guardLevel, string _roomId, EMUserType userType)
    {
        uid = _uid;
        userName = _userName;
        userFace = _userFace;
        fansMedalLevel = _fansMedalLevel;
        fansMedalName = _fansMedalName;
        fansMedalWearingStatus = _fansMedalWearingStatus;
        guardLevel = _guardLevel;
        roomId = _roomId;
        emUserType = userType;

        nGameCoins = 0;
    }
}
