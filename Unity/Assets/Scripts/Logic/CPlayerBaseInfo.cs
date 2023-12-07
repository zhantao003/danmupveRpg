using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerBaseInfo
{
    public enum EMUserType
    {
        Zhubo = 0,      //主播
        Guanzhong = 1,  //观众
    }

    public enum EMState
    {
        None,

        IdleQueue,       //排队待机中
        ActiveQueue,     //游戏待机中
        Gaming,          //游戏中
    }

    /// <summary>
    /// 用户UID
    /// </summary>
    public string uid;

    /// <summary>
    /// 用户昵称
    /// </summary>
    public string userName;

    /// <summary>
    /// 用户头像
    /// </summary>
    public string userFace;

    /// <summary>
    /// 粉丝勋章等级
    /// </summary>
    public long fansMedalLevel;

    /// <summary>
    /// 粉丝勋章名
    /// </summary>
    public string fansMedalName;

    /// <summary>
    /// 佩戴的粉丝勋章佩戴状态
    /// </summary>
    public bool fansMedalWearingStatus;

    /// <summary>
    /// 大航海等级
    /// </summary>
    public long guardLevel;

    /// <summary>
    /// 用户类型
    /// </summary>
    public EMUserType emUserType = EMUserType.Guanzhong;

    /// <summary>
    /// 用户状态
    /// </summary>
    public EMState emState = EMState.None;

    /// <summary>
    /// 弹幕接收的直播间
    /// </summary>
    public string roomId;

    /// <summary>
    /// 游戏币
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
