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
    /// 所属阵营
    /// </summary>
    public EMUnitCamp emCamp;
    /// <summary>
    /// 当前所属路线
    /// </summary>
    public EMStayPathType emPathType;
    /// <summary>
    /// 弹幕接收的直播间
    /// </summary>
    public string roomId;

    /// <summary>
    /// 单局游戏获得的经验
    /// </summary>
    public long nGameEarnExp;

    /// <summary>
    /// 单据游戏获得的经验档位
    /// </summary>
    public int nGameEarnExpShowIdx; 

    /// <summary>
    /// 玩家总经验
    /// </summary>
    public long nTotalExp;

    /// <summary>
    /// 玩家世界排名
    /// </summary>
    public long nWorldRank;

    /// <summary>
    /// 连胜场次
    /// </summary>
    public long nWinTimes;

    /// <summary>
    /// 击杀数
    /// </summary>
    public int nKillUnitCount;

    /// <summary>
    /// 当前已点赞次数
    /// </summary>
    public int nCurLikeCount;
    /// <summary>
    /// 到n次出一波弓箭手
    /// </summary>
    public int nMaxLikeCount = 4;

    /// <summary>
    /// 是否结算后增加过经验了
    /// </summary>
    public bool hasEarnToToalExp = false;

    public CPlayerBaseInfo(string _uid, string _userName, string _userFace, long _fansMedalLevel, string _fansMedalName, bool _fansMedalWearingStatus, long _guardLevel, string _roomId, EMUserType userType, long nTotalExp,long worldRank,long nWinTimes)
    {
        Init(_uid, _userName, _userFace, _fansMedalLevel, _fansMedalName, _fansMedalWearingStatus, _guardLevel, _roomId, userType, nTotalExp, worldRank, nWinTimes);
    }

    public void Init(string _uid, string _userName, string _userFace, long _fansMedalLevel, string _fansMedalName, bool _fansMedalWearingStatus, long _guardLevel, string _roomId, EMUserType userType,long _nTotalExp,long _nWorldRank,long _nWinTimes)
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
        nTotalExp = _nTotalExp;
        nGameEarnExp = 0;
        nGameEarnExpShowIdx = 0;
        emCamp = EMUnitCamp.Max;
        nWorldRank = _nWorldRank;
        nWinTimes = _nWinTimes;
        nKillUnitCount = 0;
    }
}
