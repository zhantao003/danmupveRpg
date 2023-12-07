using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHttpConst {
    public const string LoginVtuber = "api2/login_vtb";               //主播登录
    public const string LoginViewer = "api2/login_viewer";            //观众登录

    public const string GetGameInfo = "api2/getUserGameInfo";         //获取游戏通用信息
    public const string GetEquipAvatar = "api2/getUserEquipAvatar";   //获取已装备Avatar
    public const string SetEquipAvatar = "api2/setUserEquipAvatar";   //获取已装备Avatar
    public const string GetAvatarListByPart = "api2/getUserAvatarListByPart";   //获取已装备Avatar
    public const string GetQuestList = "api2/getUserQuestInfoList";  //获取当前任务列表
    public const string GachaBox = "api2/gachaGiftBox";               //购买抽奖礼盒
    public const string GachaChgAvatar = "api2/gachaChgAvatar";       //购买换装道具
    public const string GachaQuestItem = "api2/gachaQuestItem";       //购买任务道具
    public const string CostChgAvatar = "api2/costChgAvatar";         //消耗换装道具
    public const string CostQuestItem = "api2/costQuestItem";         //消耗任务道具

    //GM接口
    //Avatar相关接口
    public const string DEBUG_setAvatarBaseInfoArr = "api/set_avatarBaseInfoArr";
    public const string DEBUG_getAvatarBaseInfoList = "api/get_avatarBaseList";
    public const string DEBUG_removeAvatarBaseInfo = "api/remove_avatarBaseInfo";

    //盲盒相关接口
    public const string DEBUG_setGiftBoxConfig = "api/set_GiftBoxConfig";
    public const string DEBUG_getGiftBoxConfigList = "api/get_GiftBoxConfigList";
    public const string DEBUG_refreshGiftBoxConfig = "api/refresh_GiftBoxConfig";
}
