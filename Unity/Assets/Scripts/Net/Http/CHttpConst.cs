using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHttpConst {
    public const string LoginVtuber = "authentication/login";               //主播登录
    public const string LoginViewer = "broadcaster/GetPlayerInfo";            //获取玩家信息
    public const string HeartBeat = "broadcaster/HeartBeat";              //心跳包

    //以下需要验证
    //1.时间戳与服务器不超过30s
    //2.防伪造 AES加密包体得到Signature，服务器验证这个Signature
    //3.防重放 Data+Timestamp 计算MD5并比对之前的发包
    public const string BroadcasterEXPGain = "broadcaster/BroadcasterEXPGain";      //主播经验增加
    public const string GameResult = "broadcaster/GameResult";                      //游戏结算
    public const string PlayerSpending = "broadcaster/PlayerSpending";              //单个玩家消费
    public const string PlayersSpendings = "broadcaster/PlayersSpendings";          //群体消费单包发送
    public const string PlayerGetPoints = "broadcaster/PlayerGetPoints";            //玩家增加经验
   


    //public const string GetGameInfo = "api2/getUserGameInfo";         //获取游戏通用信息
    //public const string GetEquipAvatar = "api2/getUserEquipAvatar";   //获取已装备Avatar
    //public const string SetEquipAvatar = "api2/setUserEquipAvatar";   //获取已装备Avatar
    //public const string GetAvatarListByPart = "api2/getUserAvatarListByPart";   //获取已装备Avatar
    //public const string GetQuestList = "api2/getUserQuestInfoList";  //获取当前任务列表
    //public const string GachaBox = "api2/gachaGiftBox";               //购买抽奖礼盒
    //public const string GachaChgAvatar = "api2/gachaChgAvatar";       //购买换装道具
    //public const string GachaQuestItem = "api2/gachaQuestItem";       //购买任务道具
    //public const string CostChgAvatar = "api2/costChgAvatar";         //消耗换装道具
    //public const string CostQuestItem = "api2/costQuestItem";         //消耗任务道具

    ////GM接口
    ////Avatar相关接口
    //public const string DEBUG_setAvatarBaseInfoArr = "api/set_avatarBaseInfoArr";
    //public const string DEBUG_getAvatarBaseInfoList = "api/get_avatarBaseList";
    //public const string DEBUG_removeAvatarBaseInfo = "api/remove_avatarBaseInfo";

    ////盲盒相关接口
    //public const string DEBUG_setGiftBoxConfig = "api/set_GiftBoxConfig";
    //public const string DEBUG_getGiftBoxConfigList = "api/get_GiftBoxConfigList";
    //public const string DEBUG_refreshGiftBoxConfig = "api/refresh_GiftBoxConfig";
}

public class CHttpRecordConst
{
    public const string AddGift = "api/influx/add";            //礼物统计
}