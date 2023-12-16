using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGameObserverConst
{
    public const string BaseDestroy = "basedestroy";            //基地被摧毁
    public const string BarracksDestroy = "barracksdestroy";    //兵营被摧毁
    public const string TowerDestroy = "towerdestroy";          //塔被摧毁
    public const string UnitDead = "unitdead";                  //单位被击杀
    public const string UnitJoin = "unitjoin";                  //单位加入
    public const string AddBaseExp = "addbaseexp";              //增加基地经验
    public const string BuildHPChg = "buildhpchg";              //建筑血量变化
    public const string PlayerJoin = "playerjoin";              //玩家加入
}

/// <summary>
/// 游戏观察者监听器
/// </summary>
public class CGameObserverMgr
{
    public static void SendMsg(string eventType, CLocalNetMsg msg)
    {
        switch (eventType)
        {
            case CGameObserverConst.BuildHPChg:
                {
                    EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    CPlayerUnit.EMUnitType unitType = (CPlayerUnit.EMUnitType)msg.GetInt("unittype");
                    EMStayPathType emPathType = (EMStayPathType)msg.GetInt("pathtype");     //上中下路
                    int nCurHP = msg.GetInt("curhp");
                    int nMaxHP = msg.GetInt("maxhp");
                    string atkuid = msg.GetString("atkuid");
                    if (unitCamp == EMUnitCamp.Blue)     //右方
                    {

                    }
                    else if (unitCamp == EMUnitCamp.Red)     //左方
                    {

                    }
                    
                    if (unitType == CPlayerUnit.EMUnitType.Base)     //基地
                    {
                        UIGameInfo uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
                        if (unitCamp == EMUnitCamp.Red)
                        {
                            float fillHp = 1.0f * nCurHP / nMaxHP;

                            uiInfo.img_HpSliLeft.fillAmount = fillHp;
                            if (fillHp > 0)
                                uiInfo.txt_HpLeft.text = (fillHp * 100f).ToString("F0") + "%";
                            else
                                uiInfo.txt_HpLeft.text =  "0%";
                        }
                        else
                        {
                            float fillHp = 1.0f * nCurHP / nMaxHP;
                            uiInfo.img_HpSliRight.fillAmount = fillHp;
                            if (fillHp > 0)
                                uiInfo.txt_HpRight.text = (fillHp * 100f).ToString("F0") + "%";
                            else
                                uiInfo.txt_HpRight.text =  "0%";
                        }
                    }
                    else if (unitType == CPlayerUnit.EMUnitType.Barracks)   //兵营
                    {

                    }
                    else if (unitType == CPlayerUnit.EMUnitType.Tower)  //塔
                    {

                    }
                }
                break;
            case CGameObserverConst.BaseDestroy:
                {
                    ///获取被摧毁的基地阵营所属
                    EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    CBattleMgr.Ins.GameEnd();
                    CLockStepMgr.Ins.ClearAllList();
                    AStarFindPath.Ins.dicMapSlots.Clear();
                    UIGameEndResult uiResult = UIManager.Instance.GetUI(UIResType.GameEndResult) as UIGameEndResult;
                    uiResult.GameEndRequest(unitCamp);
                }
                break;
            case CGameObserverConst.BarracksDestroy:
                {
                    EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    EMStayPathType emPathType = (EMStayPathType)msg.GetInt("pathtype");
                    if (unitCamp == EMUnitCamp.Blue)
                    {
                        CBattleMgr.Ins.mapMgr.pRedBase.OnBarracksDestroy(emPathType);
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右兵营摧毁, new IICampDestroy(unitCamp, emPathType));
                    }
                    else if (unitCamp == EMUnitCamp.Red)
                    {
                        CBattleMgr.Ins.mapMgr.pBlueBase.OnBarracksDestroy(emPathType);
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左兵营摧毁, new IICampDestroy(unitCamp, emPathType));
                    }
                }
                break;
            case CGameObserverConst.TowerDestroy:
                {
                    EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    EMStayPathType emPathType = (EMStayPathType)msg.GetInt("pathtype");
                    if (unitCamp == EMUnitCamp.Blue)
                    {
                        //CBattleMgr.Ins.mapMgr.pRedBase.OnBarracksDestroy(emPathType);
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右防御塔摧毁, new IITowerDestroy(unitCamp, emPathType));
                    }
                    else if (unitCamp == EMUnitCamp.Red)
                    {
                        //CBattleMgr.Ins.mapMgr.pBlueBase.OnBarracksDestroy(emPathType);
                        UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左防御塔摧毁, new IITowerDestroy(unitCamp, emPathType));
                    }
                }
                break;
            case CGameObserverConst.UnitDead:
                {
                    EMUnitCamp deadUnitCamp = (EMUnitCamp)msg.GetInt("camp");
                    CPlayerUnit.EMUnitType unitType = (CPlayerUnit.EMUnitType)msg.GetInt("unittype");
                    EMUnitLev unitLev = (EMUnitLev)msg.GetInt("unitlev");
                    EMStayPathType emPathType = (EMStayPathType)msg.GetInt("pathtype");
                    long nlGold = msg.GetLong("gold");
                    int nBuffID = msg.GetInt("buffid");
                    string atkuid = msg.GetString("atkuid");
                    CPlayerUnit.EMUnitType atkunitType = (CPlayerUnit.EMUnitType)msg.GetInt("atkunittype");
                    ///判断是否为有buff光环的单位死亡
                    if (nBuffID > 0)
                    {
                        List<CPlayerUnit> playerUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(deadUnitCamp);
                        ///移除buff
                        for (int i = 0; i < playerUnits.Count; i++)
                        {
                            playerUnits[i].BuffChg(false, nBuffID, 1);
                        }
                        UIGameInfo uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
                        if (deadUnitCamp == EMUnitCamp.Red)
                        {
                            CBattleMgr.Ins.mapMgr.pRedBase.ChgBuff(nBuffID, 1, false);
                            if (CBattleMgr.Ins.mapMgr.pRedBase.GetBuffLayer(nBuffID) > 0)
                            {
                                uiInfo.txt_uiRedAddPro.text = "x" + (CBattleMgr.Ins.mapMgr.pRedBase.GetAddValueByCamp() + CBattleMgr.Ins.mapMgr.pRedBase.GetBuffNumPer(nBuffID));
                                //uiInfo.txt_uiRedAddPro.text = CBattleMgr.Ins.mapMgr.pRedBase.GetAddProByCamp() + "(" + "光环+" + CBattleMgr.Ins.mapMgr.pRedBase.GetBuffNumPer(nBuffID) + "%)";
                            }
                            else
                            {
                                uiInfo.txt_uiRedAddPro.text = "x" + CBattleMgr.Ins.mapMgr.pRedBase.GetAddValueByCamp();
                                //uiInfo.txt_uiRedAddPro.text = CBattleMgr.Ins.mapMgr.pRedBase.GetAddProByCamp();
                            }
                        }
                        else
                        {
                            CBattleMgr.Ins.mapMgr.pBlueBase.ChgBuff(nBuffID, 1, false);
                            if (CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffLayer(nBuffID) > 0)
                            {
                                uiInfo.txt_uiBlueAddPro.text = (CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp() + CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(nBuffID)) + "x";
                                //uiInfo.txt_uiBlueAddPro.text = "x" + CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp() + "(光环x" + CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(nBuffID) + ")";
                                //uiInfo.txt_uiBlueAddPro.text = CBattleMgr.Ins.mapMgr.pBlueBase.GetAddProByCamp() + "(" + "光环+" + CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(nBuffID) + "%)";
                            }
                            else
                            {
                                uiInfo.txt_uiBlueAddPro.text = CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp() + "x";
                                //uiInfo.txt_uiBlueAddPro.text = CBattleMgr.Ins.mapMgr.pBlueBase.GetAddProByCamp();
                            }
                        }
                    }
                    
                    if (deadUnitCamp == EMUnitCamp.Red)
                    {
                        CBattleMgr.Ins.mapMgr.pRedBase.ChgBattle(nlGold, false);
                    }
                    else if (deadUnitCamp == EMUnitCamp.Blue)
                    {
                        CBattleMgr.Ins.mapMgr.pBlueBase.ChgBattle(nlGold, false);
                    }

                    //被击杀单位或者击杀者是玩家单位，加经验和弹击杀信息
                    if (unitType != CPlayerUnit.EMUnitType.Unit || atkunitType != CPlayerUnit.EMUnitType.Unit)
                        return;

                    string atkunituid = msg.GetString("atkunituid");
                    string deadName = msg.GetString("unitname");
                    string szprefabname = msg.GetString("szprefabname");
                    CPlayerBaseInfo playerInfo = CPlayerMgr.Ins.GetPlayer(atkuid);
                    if (playerInfo != null)
                    {
                        playerInfo.nKillUnitCount += 1;
                        //if (playerInfo.emCamp == EMUnitCamp.Red)
                        //{
                        //    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左分数达标, new IIPlayerAchievePoint(playerInfo, playerInfo.nKillUnitCount));
                        //}
                        //else if (playerInfo.emCamp == EMUnitCamp.Blue)
                        //{
                        //    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右分数达标, new IIPlayerAchievePoint(playerInfo, playerInfo.nKillUnitCount));
                        //}
                      
                        UnitDeadGainPlayerEXP(playerInfo, nlGold);

                        if (playerInfo.emCamp == EMUnitCamp.Red)
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左分数达标, new IIPlayerGameEarnPoint(playerInfo, playerInfo.nGameEarnExp, playerInfo.nGameEarnExpShowIdx));
                        }
                        else if (playerInfo.emCamp == EMUnitCamp.Blue)
                        {
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右分数达标, new IIPlayerGameEarnPoint(playerInfo, playerInfo.nGameEarnExp, playerInfo.nGameEarnExpShowIdx));
                        }
                        if (unitType == CPlayerUnit.EMUnitType.Unit)
                        {
                            if (unitLev == EMUnitLev.Lv1)
                            {
                                if (Random.Range(0, 100) > 20)
                                {
                                    return;
                                }
                            }
                            else if (unitLev <= EMUnitLev.Lv3)
                            {
                                if (Random.Range(0, 100) > 50)
                                {
                                    return;
                                }
                            }
                            else
                            {

                            }
                        }
                        if (CPlayerMgr.Ins.dicPlayerAliveAvatar.ContainsKey(szprefabname))
                        {
                            CPlayerUnit atackUnit = CPlayerMgr.Ins.dicPlayerAliveAvatar[szprefabname].Find((x) => x.szSelfUid == atkunituid);
                            if (atackUnit != null)
                            {
                                ShowWorldKillInfo(atackUnit, deadName);
                            }
                        }
                    }
                    
                }
                break;
            case CGameObserverConst.UnitJoin:
                {
                    //EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    //EMStayPathType emPathType = (EMStayPathType)msg.GetInt("pathtype");
                    //int nBuffID = msg.GetInt("buffid");
                    //long nlGold = msg.GetLong("gold");
                    /////判断是否为有buff光环的单位加入
                    //if (nBuffID > 0)
                    //{
                    //    List<CPlayerUnit> playerUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(unitCamp);
                    //    ///增加buff
                    //    for (int i = 0; i < playerUnits.Count; i++)
                    //    {
                    //        playerUnits[i].BuffChg(true, nBuffID, 1);
                    //    }
                    //    UIGameInfo uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
                    //    if (unitCamp == EMUnitCamp.Red)
                    //    {
                    //        CBattleMgr.Ins.mapMgr.pRedBase.ChgBuff(nBuffID, 1, true);
                    //        uiInfo.txt_uiRedAddPro.text = "x" + (CBattleMgr.Ins.mapMgr.pRedBase.GetAddValueByCamp() + CBattleMgr.Ins.mapMgr.pRedBase.GetBuffNumPer(nBuffID));
                    //    }
                    //    else 
                    //    {
                    //        CBattleMgr.Ins.mapMgr.pBlueBase.ChgBuff(nBuffID, 1, true);
                    //        uiInfo.txt_uiBlueAddPro.text = (CBattleMgr.Ins.mapMgr.pBlueBase.GetAddValueByCamp() + CBattleMgr.Ins.mapMgr.pBlueBase.GetBuffNumPer(nBuffID)) + "x";
                    //    }
                    //}
                    //if (unitCamp == EMUnitCamp.Red)
                    //{
                    //    CBattleMgr.Ins.mapMgr.pRedBase.ChgBattle(nlGold, true);
                    //}
                    //else if (unitCamp == EMUnitCamp.Blue)
                    //{
                    //    CBattleMgr.Ins.mapMgr.pBlueBase.ChgBattle(nlGold, true);
                    //}
                }
                break;
            case CGameObserverConst.AddBaseExp:
                {
                    EMUnitCamp unitCamp = (EMUnitCamp)msg.GetInt("camp");
                    int nExp = msg.GetInt("exp");
                    bool lvUp = false;
                    if (unitCamp == EMUnitCamp.Blue)
                    {
                        lvUp= CBattleMgr.Ins.mapMgr.pBlueBase.AddExp(nExp);
                        if (lvUp)
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右阵营等级提升, new IICampAchieveLevel(CBattleMgr.Ins.mapMgr.pBlueBase, CBattleMgr.Ins.mapMgr.pBlueBase.nLev));
                    }
                    else if (unitCamp == EMUnitCamp.Red)
                    {
                        lvUp= CBattleMgr.Ins.mapMgr.pRedBase.AddExp(nExp);
                        if (lvUp)
                            UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左阵营等级提升, new IICampAchieveLevel(CBattleMgr.Ins.mapMgr.pRedBase, CBattleMgr.Ins.mapMgr.pRedBase.nLev));
                    }
                    if(lvUp)
                    {
                        ///刷新阵营buff
                        List<CPlayerUnit> playerUnits = CPlayerMgr.Ins.GetAliveUnitByCamp(unitCamp);
                        for (int i = 0; i < playerUnits.Count; i++)
                        {
                            //playerUnits[i].RefreshCampInfo();
                        }
                    }

                    UIGameInfo uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
                    if (unitCamp == EMUnitCamp.Red)
                    {
                        if (CBattleMgr.Ins.mapMgr.pRedBase.nLev == CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp.Length)
                        {
                            uiInfo.img_ExpSliLeft.fillAmount = 1.0f;
                            uiInfo.txt_ExpLeft.text = "MAX";
                        }
                        else
                        {
                            if (CBattleMgr.Ins.mapMgr.pRedBase.nLev == 1)
                            {
                                uiInfo.img_ExpSliLeft.fillAmount = 1.0f * CBattleMgr.Ins.mapMgr.pRedBase.nCurExp / CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp[1];
                                uiInfo.txt_ExpLeft.text = (1.0f * CBattleMgr.Ins.mapMgr.pRedBase.nCurExp / CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp[1] * 100f).ToString("f0") + "%";
                            }
                            else
                            {
                                float nowLvExtraExp = CBattleMgr.Ins.mapMgr.pRedBase.nCurExp - CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp[CBattleMgr.Ins.mapMgr.pRedBase.nLev-1];
                                float nextLvExpNeed = CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp[CBattleMgr.Ins.mapMgr.pRedBase.nLev] - CBattleMgr.Ins.mapMgr.pRedBase.nNeedExp[CBattleMgr.Ins.mapMgr.pRedBase.nLev-1];
                                uiInfo.img_ExpSliLeft.fillAmount = 1.0f * nowLvExtraExp / nextLvExpNeed;
                                uiInfo.txt_ExpLeft.text = (1.0f * nowLvExtraExp / nextLvExpNeed * 100f).ToString("f0") + "%";
                            }
                        }
                    }
                    else
                    {
                        if (CBattleMgr.Ins.mapMgr.pBlueBase.nLev == CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp.Length)
                        {
                            uiInfo.img_ExpSliRight.fillAmount = 1.0f;
                            uiInfo.txt_ExpRight.text = "MAX";
                        }
                        else
                        {
                            if (CBattleMgr.Ins.mapMgr.pBlueBase.nLev == 1)
                            {
                                uiInfo.img_ExpSliRight.fillAmount = 1.0f * CBattleMgr.Ins.mapMgr.pBlueBase.nCurExp / CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp[1];
                                uiInfo.txt_ExpRight.text = (1.0f * CBattleMgr.Ins.mapMgr.pBlueBase.nCurExp / CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp[1] * 100f).ToString("f0") + "%";
                            }
                            else
                            {
                                float nowLvExtraExp = CBattleMgr.Ins.mapMgr.pBlueBase.nCurExp - CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp[CBattleMgr.Ins.mapMgr.pBlueBase.nLev-1];
                                float nextLvExpNeed = CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp[CBattleMgr.Ins.mapMgr.pBlueBase.nLev] - CBattleMgr.Ins.mapMgr.pBlueBase.nNeedExp[CBattleMgr.Ins.mapMgr.pBlueBase.nLev-1];
                                uiInfo.img_ExpSliRight.fillAmount = 1.0f * nowLvExtraExp / nextLvExpNeed;
                                uiInfo.txt_ExpRight.text = (1.0f * nowLvExtraExp / nextLvExpNeed * 100f).ToString("f0") + "%";
                            }
                        }
                    }
                }
                break;
            case CGameObserverConst.PlayerJoin:
                {
                    string szJoinUID = msg.GetString("uid");
                    CPlayerBaseInfo playerInfo = CPlayerMgr.Ins.GetPlayer(szJoinUID);
                    if(playerInfo != null)
                    {
                        UnitDeadGainPlayerEXP(playerInfo, 0);
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 加玩家功勋，更新功勋排名
    /// </summary>
    /// <param name="info"></param>
    /// <param name="gainExp"></param>
    static void UnitDeadGainPlayerEXP(CPlayerBaseInfo info, long gainExp) {
        if (info == null)
            return;
        UIGameInfo uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
        if (uiInfo == null) return;
        info.nGameEarnExp += gainExp;
        if (info.emCamp == EMUnitCamp.Red)
        {
            if (CPlayerMgr.Ins.top3ScorePlayersLeft.Count < 3)
            {
                //如果排行榜人数小于3，直接加入然后排序
                if (!CPlayerMgr.Ins.top3ScorePlayersLeft.Exists(x => x.uid == info.uid))
                {
                    CPlayerMgr.Ins.top3ScorePlayersLeft.Add(info);
                }
                else
                {
                    CPlayerBaseInfo cp = CPlayerMgr.Ins.top3ScorePlayersLeft.Find(x => x.uid == info.uid);
                    int index = CPlayerMgr.Ins.top3ScorePlayersLeft.IndexOf(cp);
                    CPlayerMgr.Ins.top3ScorePlayersLeft[index] = info;
                }
                CPlayerMgr.Ins.top3ScorePlayersLeft.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
            }
            else
            {
                //如果前三不存在这个玩家，而且他的经验高于第三，去掉第三，加入该玩家然后排序
                if (!CPlayerMgr.Ins.top3ScorePlayersLeft.Exists(x => x.uid == info.uid) && CPlayerMgr.Ins.top3ScorePlayersLeft[2].nGameEarnExp < info.nGameEarnExp)
                {
                    CPlayerMgr.Ins.top3ScorePlayersLeft.RemoveAt(2);
                    CPlayerMgr.Ins.top3ScorePlayersLeft.Add(info);
                    CPlayerMgr.Ins.top3ScorePlayersLeft.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
                }
                else if (CPlayerMgr.Ins.top3ScorePlayersLeft.Exists(x => x.uid == info.uid))
                {
                    //如果存在这个玩家，更新信息后排序
                    CPlayerBaseInfo cp = CPlayerMgr.Ins.top3ScorePlayersLeft.Find(x => x.uid == info.uid);
                    int index = CPlayerMgr.Ins.top3ScorePlayersLeft.IndexOf(cp);
                    CPlayerMgr.Ins.top3ScorePlayersLeft[index] = info;
                    //if (index != 0 && info.nGameEarnExp > CPlayerMgr.Ins.top3ScorePlayersLeft[index - 1].nGameEarnExp)
                    CPlayerMgr.Ins.top3ScorePlayersLeft.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
                }

            }
            for (int i = 0; i < CPlayerMgr.Ins.top3ScorePlayersLeft.Count; i++)
            {
                uiInfo.c_LeftTop3.SetRankInfo(i, CPlayerMgr.Ins.top3ScorePlayersLeft[i].userName, (int)CPlayerMgr.Ins.top3ScorePlayersLeft[i].nWinTimes, CPlayerMgr.Ins.top3ScorePlayersLeft[i].userFace, (int)CPlayerMgr.Ins.top3ScorePlayersLeft[i].guardLevel);
            }
        }
        else
        {
            if (CPlayerMgr.Ins.top3ScorePlayersRight.Count < 3)
            {
                if (!CPlayerMgr.Ins.top3ScorePlayersRight.Exists(x => x.uid == info.uid))
                {
                    CPlayerMgr.Ins.top3ScorePlayersRight.Add(info);
                }
                else
                {
                    CPlayerBaseInfo cp = CPlayerMgr.Ins.top3ScorePlayersRight.Find(x => x.uid == info.uid);
                    int index = CPlayerMgr.Ins.top3ScorePlayersRight.IndexOf(cp);
                    CPlayerMgr.Ins.top3ScorePlayersRight[index] = info;
                }
                CPlayerMgr.Ins.top3ScorePlayersRight.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
            }
            else
            {
                if (!CPlayerMgr.Ins.top3ScorePlayersRight.Exists(x => x.uid == info.uid) && CPlayerMgr.Ins.top3ScorePlayersRight[2].nGameEarnExp < info.nGameEarnExp)
                {
                    CPlayerMgr.Ins.top3ScorePlayersRight.RemoveAt(2);
                    CPlayerMgr.Ins.top3ScorePlayersRight.Add(info);
                    CPlayerMgr.Ins.top3ScorePlayersRight.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
                }
                else if (CPlayerMgr.Ins.top3ScorePlayersRight.Exists(x => x.uid == info.uid))
                {
                    CPlayerBaseInfo cp = CPlayerMgr.Ins.top3ScorePlayersRight.Find(x => x.uid == info.uid);
                    int index = CPlayerMgr.Ins.top3ScorePlayersRight.IndexOf(cp);
                    CPlayerMgr.Ins.top3ScorePlayersRight[index] = info;
                    //if(index!=0&&info.nGameEarnExp> CPlayerMgr.Ins.top3ScorePlayersRight[index-1].nGameEarnExp)
                    CPlayerMgr.Ins.top3ScorePlayersRight.Sort((x, y) => -x.nGameEarnExp.CompareTo(y.nGameEarnExp));
                }
            }
            for (int i = 0; i < CPlayerMgr.Ins.top3ScorePlayersRight.Count; i++)
            {
                uiInfo.c_RightTop3.SetRankInfo(i, CPlayerMgr.Ins.top3ScorePlayersRight[i].userName, (int)CPlayerMgr.Ins.top3ScorePlayersRight[i].nWinTimes, CPlayerMgr.Ins.top3ScorePlayersRight[i].userFace, (int)CPlayerMgr.Ins.top3ScorePlayersRight[i].guardLevel);
            }
        }
    }
    /// <summary>
    /// 显示击杀信息
    /// </summary>
    /// <param name="killer"></param>
    /// <param name="beKillerName"></param>
    static void ShowWorldKillInfo(CPlayerUnit killer,string beKillerName) {
        UIWorldCanvas worldUI = UIManager.Instance.GetUI(UIResType.WorldUI) as UIWorldCanvas;
        worldUI.SetKillInfo(killer, beKillerName);
    }
}
