using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIResType
{
    None = 0,

    MainMenu = 1000,        //主菜单
    GameInfo = 1001,        //游戏信息
    GiftEff = 1002,     //礼物特效
    GameEndResult = 1003,//结算界面
    WorldUI = 1004,      //世界UI
    Setting = 1005,      //设置UI
    NetError = 1006,     //网络异常弹窗
    Disconnect = 1007,      //断线界面


    ETNetMatch = 2001,//联机主界面
    ETNetUserList = 2002,   //用户列表

    Toast = 5001,       //消息气泡
    MsgBox = 5002,      //消息盒子
    NetWait = 5003,     //网络等待
    Loading = 5004,     //加载等待
    ModeSelect = 5005,  //模式选择
}
