using DouyuDanmu;
using ETModel;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class UIScreenInfoInsertionMgr : CSingleCompOdinBase<UIScreenInfoInsertionMgr>
{
    [DictionaryDrawerSettings(KeyLabel = "礼物类型", ValueLabel = "Index对应礼物图")]
    public Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>> Camp1GiftNameToSprite = new Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>>();
    [DictionaryDrawerSettings(KeyLabel = "礼物类型", ValueLabel = "Index对应礼物图")]
    public Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>> Camp2GiftNameToSprite = new Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>>();
    [DictionaryDrawerSettings(KeyLabel = "礼物类型", ValueLabel = "Index对应礼物图")]
    public Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>> Camp3GiftNameToSprite = new Dictionary<CDanmuGiftConst, Dictionary<int, Sprite>>();
    //int[] showTimes = new int[] { 10,50,100 };

    //int[] showTimes = new int[] { 50, 100, 200, 500, 1000, 2000, 5000, 10000, 20000, 50000, 100000 };

    int[] showTimes = new int[] { 1000, 2000, 5000, 10000, 20000, 50000, 80000, 100000, 150000, 200000, 300000, 500000, 1000000 };

    //string[] showWords = new string[] { "渐入佳境……", "大杀特杀！", "其疾如风~！", "不动如山。", "侵略如火！", "如神一般！", " 超 ！ 神 ！", "横·扫·千·军！", "翻·云·覆·海！", "万·古·流·芳！", "冠·绝·一·世！" };
    public Gradient comboKillC;
    static Dictionary<InfoInsertionType, string> prefabPath = new Dictionary<InfoInsertionType, string>() {
         { InfoInsertionType.左玩家加入, "UI/UIInsert/RedPlayerJoinPrefab" },
         { InfoInsertionType.右玩家加入, "UI/UIInsert/BluePlayerJoinPrefab" },
         { InfoInsertionType.左送弓箭兵, "UI/UIInsert/PlayerSendGiftRed" },
         { InfoInsertionType.右送弓箭兵, "UI/UIInsert/PlayerSendGiftBlue" },

         { InfoInsertionType.左送盲盒, "UI/UIInsert/PlayerSendGiftRedBox" },
         { InfoInsertionType.右送盲盒, "UI/UIInsert/PlayerSendGiftBlueBox" },
         { InfoInsertionType.左送高级怪, "UI/UIInsert/PlayerSendGiftRedSpecial" },
         { InfoInsertionType.右送高级怪, "UI/UIInsert/PlayerSendGiftBlueSpecial" },
         { InfoInsertionType.左送精英怪, "UI/UIInsert/PlayerSendGiftRedRare" },
         { InfoInsertionType.右送精英怪, "UI/UIInsert/PlayerSendGiftBlueRare" },

         { InfoInsertionType.左送英雄, "UI/UIInsert/PlayerSendGiftRedHero" },
         { InfoInsertionType.右送英雄, "UI/UIInsert/PlayerSendGiftBlueHero" },
         { InfoInsertionType.左分数达标, "UI/UIInsert/PlayerAchivePointRed" },
         { InfoInsertionType.右分数达标, "UI/UIInsert/PlayerAchivePointBlue" },
         { InfoInsertionType.左击杀达标, "UI/UIInsert/ComboKillPrefab" },
         { InfoInsertionType.右击杀达标, "UI/UIInsert/ComboKillPrefab" },
         { InfoInsertionType.左阵营等级提升, "UI/UIInsert/CampAchiveLevelRED" },
         { InfoInsertionType.右阵营等级提升, "UI/UIInsert/CampAchiveLevelBlue" },
         { InfoInsertionType.左防御塔摧毁, "UI/UIInsert/BuildDestroyLeft" },
         { InfoInsertionType.右防御塔摧毁, "UI/UIInsert/BuildDestroyRight" },
         { InfoInsertionType.左兵营摧毁, "UI/UIInsert/BuildDestroyLeft" },
         { InfoInsertionType.右兵营摧毁, "UI/UIInsert/BuildDestroyRight" },
    };
    public Dictionary<InfoInsertionType, GameObject> prefabDic;
    public Dictionary<InfoInsertionType, (float ,Queue<InsertionInfoBase>)> infoInsertionDic;
    private void Start()
    {
        prefabDic = new Dictionary<InfoInsertionType, GameObject>();
        infoInsertionDic = new Dictionary<InfoInsertionType, (float, Queue<InsertionInfoBase>)>();
    }
    UIGameInfo uiInfo;
    public void SetAInsertion(InfoInsertionType insertionType, InsertionInfoBase IIClass)
    {
        if (uiInfo == null)
            uiInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
        switch (insertionType)
        {
            case InfoInsertionType.左玩家加入:
                //直接显示
                IIPlayerJoin leftJoin = IIClass as IIPlayerJoin;
                UIPlayerJoinComp leftJoiner = GetPrefabInstance(insertionType, uiInfo.ts_LeftJoin).GetComponent<UIPlayerJoinComp>();
                leftJoiner.Init(CBattleMgr.Ins.pRedCamp.pColor, leftJoin.playerInfo.userName, true);
                //前百玩家通缉令
                if (leftJoin.playerInfo.nWorldRank <= 100)
                    EnQueueInfoInsertion(insertionType, IIClass);

                break;
            case InfoInsertionType.右玩家加入:
                //直接显示
                IIPlayerJoin rightJoin = IIClass as IIPlayerJoin;
                UIPlayerJoinComp rightJoiner = GetPrefabInstance(insertionType, uiInfo.ts_RightJoin).GetComponent<UIPlayerJoinComp>();
                rightJoiner.Init(CBattleMgr.Ins.pBlueCamp.pColor, rightJoin.playerInfo.userName, false);
                //前百玩家通缉令
                if (rightJoin.playerInfo.nWorldRank <= 100)
                    EnQueueInfoInsertion(InfoInsertionType.左玩家加入, IIClass);
                break;
            //case InfoInsertionType.左分数达标:
            //    //进队列
            //    IIPlayerAchievePoint achive = IIClass as IIPlayerAchievePoint;
            //    bool canShow = false;
            //    for (int j = 0; j < showTimes.Length; j++)
            //    {
            //        if (achive.achivePoints == showTimes[j])
            //        {
            //            achive.JumpTime = 0.5f + j * 0.5f >2?2: 0.5f + j * 0.5f;
            //            canShow = true;
            //            break;
            //        }
            //    }
            //    if(canShow)
            //        EnQueueInfoInsertion(insertionType, achive);
            //    break;
            //case InfoInsertionType.右分数达标:
            //    //进队列
            //    IIPlayerAchievePoint achiveRight = IIClass as IIPlayerAchievePoint;
            //    bool canShowRight = false;
            //    for (int j = 0; j < showTimes.Length; j++)
            //    {
            //        if (achiveRight.achivePoints == showTimes[j])
            //        {
            //            achiveRight.JumpTime = 0.5f+ j * 0.5f;
            //            canShowRight = true;
            //            Debug.Log(achiveRight.achivePoints + "杀达成");
            //            break;
            //        }
            //    }
            //    if(canShowRight)
            //        EnQueueInfoInsertion(insertionType, achiveRight);

            //    break;
            case InfoInsertionType.左分数达标:
                {
                    //进队列
                    IIPlayerGameEarnPoint achive = IIClass as IIPlayerGameEarnPoint;
                    bool canShow = false;

                    if (achive.curShowIdx >= 0 && achive.curShowIdx < showTimes.Length)
                    {
                        if (showTimes[achive.curShowIdx] <= achive.achivePoints)
                        {
                            achive.JumpTime = 0.5f + achive.curShowIdx * 0.5f > 2 ? 2 : 0.5f + achive.curShowIdx * 0.5f;
                            achive.achivePoints = showTimes[achive.curShowIdx];
                            achive.playerInfo.nGameEarnExpShowIdx++;
                            
                            canShow = true;
                        }
                    }

                    if (canShow)
                        EnQueueInfoInsertion(insertionType, achive);

                }
                break;
            case InfoInsertionType.右分数达标:
                {
                    //进队列
                    IIPlayerGameEarnPoint achiveRight = IIClass as IIPlayerGameEarnPoint;
                    bool canShowRight = false;

                    if (achiveRight.curShowIdx >= 0 && achiveRight.curShowIdx < showTimes.Length)
                    {
                        if (showTimes[achiveRight.curShowIdx] <= achiveRight.achivePoints)
                        {
                            achiveRight.JumpTime = 0.5f + achiveRight.curShowIdx * 0.5f > 2 ? 2 : 0.5f + achiveRight.curShowIdx * 0.5f;
                            achiveRight.achivePoints = showTimes[achiveRight.curShowIdx];
                            achiveRight.playerInfo.nGameEarnExpShowIdx++;

                            canShowRight = true;
                        }
                    }

                    if (canShowRight)
                        EnQueueInfoInsertion(insertionType, achiveRight);
                }
                break;
            case InfoInsertionType.左阵营等级提升:
                //仅弹窗
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.右阵营等级提升:
                //仅弹窗
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.左送弓箭兵:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift leftGift = IIClass as IIPlayerSendGift;
                //UIPlayerSendGiftComp leftSender = GetPrefabInstance(insertionType, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftComp>();
                //leftSender.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, leftGift.giftType, (int)leftGift.giftNum, true);
                ////特效
                // CGameEffMgr.Ins.LeftPlayerSendGift(leftGift.giftType, (int)CBattleMgr.Ins.pRedCamp.emCamp);
                break;
            case InfoInsertionType.右送弓箭兵:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift rightGift = IIClass as IIPlayerSendGift;
                //UIPlayerSendGiftComp rightSender = GetPrefabInstance(insertionType, uiInfo.ts_RightSendGift).GetComponent<UIPlayerSendGiftComp>();
                //rightSender.Init(CBattleMgr.Ins.pBlueCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, rightGift.giftType, (int)rightGift.giftNum, false);
                ////特效
                // CGameEffMgr.Ins.RightPlayerSendGift(rightGift.giftType, (int)CBattleMgr.Ins.pBlueCamp.emCamp);
                break;
            case InfoInsertionType.左送高级怪:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift lGift = IIClass as IIPlayerSendGift;
                //UIPlayerSendGiftSpecialComp lSender = GetPrefabInstance(insertionType, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
                //lSender.Init(CBattleMgr.Ins.pRedCamp.pColor, lGift.playerInfo.userFace, lGift.playerInfo.userName, lGift.giftName, lGift.giftType, (int)lGift.giftNum, true);
                break;
            case InfoInsertionType.右送高级怪:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift rGift = IIClass as IIPlayerSendGift;
                //UIPlayerSendGiftSpecialComp rSender = GetPrefabInstance(insertionType, uiInfo.ts_RightSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
                //rSender.Init(CBattleMgr.Ins.pBlueCamp.pColor, rGift.playerInfo.userFace, rGift.playerInfo.userName, rGift.giftName, rGift.giftType, (int)rGift.giftNum, false);
                break;
            case InfoInsertionType.左送盲盒:
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.右送盲盒:
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.左送精英怪:
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.右送精英怪:
                EnQueueInfoInsertion(insertionType, IIClass);
                break;
            case InfoInsertionType.左送英雄:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift leftGiftHero = IIClass as IIPlayerSendGift;
                //leftGiftHero.dequeueDelay = 5f;
                //uiInfo.ts_LeftSendHero.SetAsLastSibling();
                //UIPlayerSendHeroComp leftSenderHero = GetPrefabInstance(insertionType, uiInfo.ts_LeftSendHero).GetComponent<UIPlayerSendHeroComp>();
                //leftSenderHero.Init(true, CBattleMgr.Ins.pRedCamp.emCamp, leftGiftHero.playerInfo.userFace, leftGiftHero.playerInfo.userName,
                //    "召唤 "+ leftGiftHero.giftName + " x " + leftGiftHero.giftNum);
                //UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左放英雄技能, new IIHeroShowSkill(leftGiftHero.playerInfo));
                
                
                //string.Format("召唤 <color={0}>{1}</color>", CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), leftGiftHero.giftName) + " x " + leftGiftHero.giftNum);
                //特效
                //CGameEffMgr.Ins.LeftPlayerSendGift(leftGiftHero.giftType, (int)CBattleMgr.Ins.pRedCamp.emCamp);
                break;
            case InfoInsertionType.右送英雄:
                EnQueueInfoInsertion(insertionType, IIClass);
                //直接显示
                //IIPlayerSendGift rightGiftHero = IIClass as IIPlayerSendGift;
                //rightGiftHero.dequeueDelay = 5f;
                //uiInfo.ts_RightSendHero.SetAsLastSibling();
                //UIPlayerSendHeroComp rightSenderHero = GetPrefabInstance(insertionType, uiInfo.ts_RightSendHero).GetComponent<UIPlayerSendHeroComp>();
                //rightSenderHero.Init(false, CBattleMgr.Ins.pBlueCamp.emCamp, rightGiftHero.playerInfo.userFace, rightGiftHero.playerInfo.userName,
                //   rightGiftHero.giftNum + " x " + "召唤 " + rightGiftHero.giftName);/*string.Format("<color={0}>{1}</color> 召唤", CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), rightGiftHero.giftName));*/
                //UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右放英雄技能, new IIHeroShowSkill(rightGiftHero.playerInfo));
                ////特效
                //CGameEffMgr.Ins.RightPlayerSendGift(rightGiftHero.giftType, (int)CBattleMgr.Ins.pBlueCamp.emCamp);
                break;
            case InfoInsertionType.左击杀达标:
                //IIPlayerKillCombo killCombo = IIClass as IIPlayerKillCombo;
                //string showWord = "";
                //for (int i = 0; i < showTimes.Length; i++)
                //{
                //    if (killCombo.killNum == showTimes[i])
                //    {
                //        showWord = showWords[i];
                //        break;
                //    }
                //}
                //if (string.IsNullOrEmpty(showWord))
                //    return;
                //UIComboKillComp leftCombo = GetPrefabInstance(insertionType, uiInfo.ts_ComboKill).GetComponent<UIComboKillComp>();
                //leftCombo.gameObject.SetActive(true);
                //leftCombo.Init(0.5f + Mathf.Clamp((killCombo.killNum / 10000f), 0, 0.5f), string.Format("<color={0}>{1}</color>", CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), killCombo.playerInfo.userName)+ killCombo.killNum + "杀" + showWord, comboKillC.Evaluate(killCombo.killNum / 2000f), 0.8f + Mathf.Clamp((killCombo.killNum / 500f), 0, 1.5f));
                break;
            case InfoInsertionType.右击杀达标:
                //IIPlayerKillCombo killComboRight = IIClass as IIPlayerKillCombo;
                //string showWordRight = "";
                //for (int i = 0; i < showTimes.Length; i++)
                //{
                //    if (killComboRight.killNum == showTimes[i])
                //    {
                //        showWordRight = showWords[i];
                //        break;
                //    }
                //}
                //if (string.IsNullOrEmpty(showWordRight))
                //    return;
                //UIComboKillComp rightCombo = GetPrefabInstance(insertionType, uiInfo.ts_ComboKill).GetComponent<UIComboKillComp>();
                //rightCombo.gameObject.SetActive(true);
                //rightCombo.Init(0.5f + Mathf.Clamp((killComboRight.killNum / 10000f), 0, 0.5f), string.Format("<color={0}>{1}</color>", CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), killComboRight.playerInfo.userName) + killComboRight.killNum + "杀" + showWordRight, comboKillC.Evaluate(killComboRight.killNum / 2000f), 0.8f + Mathf.Clamp((killComboRight.killNum / 500f), 0, 1.5f));
                break;
            case InfoInsertionType.左防御塔摧毁:
                //弹窗+特效
                IITowerDestroy leftTowerDestroy = IIClass as IITowerDestroy;
                UIBuildDestroyComp leftTowerDestroyWind = GetPrefabInstance(insertionType, uiInfo.ts_LeftBuildDestroy).GetComponent<UIBuildDestroyComp>();
                leftTowerDestroyWind.Init(string.Format("<color={0}>{1}{2}</color>"+ CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "towerdestroy"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName), GetLineCNName(leftTowerDestroy.emPathType)), true);

                //CGameEffMgr.Ins.LeftTowerDestoryed((int)leftTowerDestroy.unitCamp, leftTowerDestroy.emPathType);
                break;
            case InfoInsertionType.右防御塔摧毁:
                //弹窗+特效
                IITowerDestroy rightTowerDestroy = IIClass as IITowerDestroy;
                UIBuildDestroyComp rightTowerDestroyWind = GetPrefabInstance(insertionType, uiInfo.ts_RightBuildDestroy).GetComponent<UIBuildDestroyComp>();
                rightTowerDestroyWind.Init(string.Format("<color={0}>{1}{2}</color>"+ CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "towerdestroy"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName), GetLineCNName(rightTowerDestroy.emPathType)), false);


                //CGameEffMgr.Ins.RightTowerDestoryed((int)rightTowerDestroy.unitCamp, rightTowerDestroy.emPathType);
                break;
            case InfoInsertionType.左兵营摧毁:
                //弹窗+特效
                IICampDestroy leftCampDestroy = IIClass as IICampDestroy;
                UIBuildDestroyComp leftCampDestroyWind = GetPrefabInstance(insertionType, uiInfo.ts_LeftBuildDestroy).GetComponent<UIBuildDestroyComp>();
                leftCampDestroyWind.Init(string.Format("<color={0}>{1}{2}</color>" + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "bingyingdestroy"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pRedCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pRedCamp.szCampCNName), GetLineCNName(leftCampDestroy.emPathType)), true);


                //CGameEffMgr.Ins.LeftCampDestoryed((int)leftCampDestroy.unitCamp, leftCampDestroy.emPathType);
                break;
            case InfoInsertionType.右兵营摧毁:
                //弹窗+特效
                IICampDestroy rightCampDestroy = IIClass as IICampDestroy;
                UIBuildDestroyComp rightCampDestroyWind = GetPrefabInstance(insertionType, uiInfo.ts_RightBuildDestroy).GetComponent<UIBuildDestroyComp>();
                rightCampDestroyWind.Init(string.Format("<color={0}>{1}{2}</color>" + CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, "bingyingdestroy"), CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, CBattleMgr.Ins.pBlueCamp.szCampCNName), GetLineCNName(rightCampDestroy.emPathType)), false);


                //CGameEffMgr.Ins.RightCampDestoryed((int)rightCampDestroy.unitCamp, rightCampDestroy.emPathType);
                break;
            case InfoInsertionType.左放英雄技能:
                //特效
                IIHeroShowSkill leftHeroSkill = IIClass as IIHeroShowSkill;
                CGameEffMgr.Ins.LeftHeroSkill((int)CBattleMgr.Ins.pRedCamp.emCamp);
                break;
            case InfoInsertionType.右放英雄技能:
                //特效
                IIHeroShowSkill rightHeroSkill = IIClass as IIHeroShowSkill;
                CGameEffMgr.Ins.RightHeroSkill((int)CBattleMgr.Ins.pBlueCamp.emCamp);
                break;
        }
    }

    void EnQueueInfoInsertion(InfoInsertionType insertionType, InsertionInfoBase IIClass) {
        if (infoInsertionDic.ContainsKey(insertionType))
        {
            (float count, Queue<InsertionInfoBase> info) = infoInsertionDic[insertionType];
            info.Enqueue(IIClass);
        }
        else {
            Queue<InsertionInfoBase> info = new Queue<InsertionInfoBase>();
            info.Enqueue(IIClass);
            infoInsertionDic[insertionType] = (IIClass.dequeueDelay, info);
        }
    }
    GameObject GetPrefabInstance(InfoInsertionType iiType, Transform parent)
    {
        if (prefabDic.ContainsKey(iiType))
        {
            return Instantiate(prefabDic[iiType], parent);
        }
        else
        {
            prefabDic[iiType] = CResLoadMgr.Inst.SynLoad(prefabPath[iiType]) as GameObject;

        }
        return Instantiate(prefabDic[iiType],parent);
    }



    #region 需要进队列的
    //private float blueQueueCounter;
    //private float redQueueCounter;
    //private float playerAchivePointTime = 3f;
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, Random.Range(1, 101), Random.Range(1, 10));
        //    p.emCamp = EMUnitCamp.Red;
        //    SetAInsertion(InfoInsertionType.左玩家加入, new IIPlayerJoin(p));
        //}
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, Random.Range(1, 101), Random.Range(1, 10));
        //    p.emCamp = EMUnitCamp.Blue;
        //    SetAInsertion(InfoInsertionType.右玩家加入, new IIPlayerJoin(p));
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Red;
        //    SetAInsertion(InfoInsertionType.左分数达标, new IIPlayerAchievePoint(p, 10000));
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Blue;
        //    SetAInsertion(InfoInsertionType.右分数达标, new IIPlayerAchievePoint(p, 10000));
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Red;
        //    CDanmuGiftConst gift = (CDanmuGiftConst)Random.Range(2, 5);
        //    SetAInsertion(InfoInsertionType.左送礼物, new IIPlayerSendGift(p, gift, gift.ToString(),Random.Range(1,101),0));
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Blue;
        //    CDanmuGiftConst gift = (CDanmuGiftConst)Random.Range(2, 5);
        //    SetAInsertion(InfoInsertionType.右送礼物, new IIPlayerSendGift(p, gift, gift.ToString(), Random.Range(1, 101), 0));
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Red;
        //    CDanmuGiftConst gift = CDanmuGiftConst.Hero;
        //    SetAInsertion(InfoInsertionType.左送英雄, new IIPlayerSendGift(p, gift, gift.ToString(), Random.Range(1, 101), 0));
        //}
        //if (Input.GetKeyDown(KeyCode.V))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Blue;
        //    CDanmuGiftConst gift = CDanmuGiftConst.Hero;
        //    SetAInsertion(InfoInsertionType.右送英雄, new IIPlayerSendGift(p, gift, gift.ToString(), Random.Range(1, 101), 0));
        //}
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    EMStayPathType path = (EMStayPathType)Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.左防御塔摧毁, new IITowerDestroy(EMUnitCamp.Red, path));
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    EMStayPathType path = (EMStayPathType)Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.右防御塔摧毁, new IITowerDestroy(EMUnitCamp.Red, path));
        //}
        //if (Input.GetKeyDown(KeyCode.Y))
        //{
        //    EMStayPathType path =(EMStayPathType) Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.左兵营摧毁, new IICampDestroy( EMUnitCamp.Red, path));
        //}
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    EMStayPathType path = (EMStayPathType)Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.右兵营摧毁, new IICampDestroy(EMUnitCamp.Red, path));
        //}
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Red;
        //    EMStayPathType path = (EMStayPathType)Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.左放英雄技能, new IIHeroShowSkill(p));
        //}
        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    CPlayerBaseInfo p = new CPlayerBaseInfo("111", "xxx", "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png", 0, "", false, 0, "aaaa", CPlayerBaseInfo.EMUserType.Guanzhong, 0, 99, 1);
        //    p.emCamp = EMUnitCamp.Blue;
        //    EMStayPathType path = (EMStayPathType)Random.Range(0, 3);
        //    SetAInsertion(InfoInsertionType.右放英雄技能, new IIHeroShowSkill(p));
        //}
        HandleInfoInsertionQueue();
    }

    private void HandleInfoInsertionQueue() {
        var keys = new List<InfoInsertionType>(infoInsertionDic.Keys);

        for (int i = 0; i < keys.Count; i++)
        {
            var key = keys[i];
            var QueueInfo = infoInsertionDic[key];
            (float count, Queue<InsertionInfoBase> queue) = QueueInfo;
            count += Time.deltaTime;
            if (queue.Count == 0 || count < queue.Peek().dequeueDelay)
            {
                infoInsertionDic[key] = (count, queue);
                continue;
            }
            //处理不同的显示队列
            //if (key == InfoInsertionType.左分数达标 )
            //{
            //    IIPlayerAchievePoint achive = queue.Dequeue() as IIPlayerAchievePoint;
            //    RedPlayerAchievePoint(achive);
            //    count = 0;
            //}
            //else if (key == InfoInsertionType.右分数达标)
            //{
            //    IIPlayerAchievePoint achive = queue.Dequeue() as IIPlayerAchievePoint;
            //    BluePlayerAchievePoint(achive);
            //    count = 0;
            //}
            if (key == InfoInsertionType.左分数达标)
            {
                IIPlayerGameEarnPoint achive = queue.Dequeue() as IIPlayerGameEarnPoint;
                RedPlayerAchievePoint(achive);
                count = 0;
            }
            else if (key == InfoInsertionType.右分数达标)
            {
                IIPlayerGameEarnPoint achive = queue.Dequeue() as IIPlayerGameEarnPoint;
                BluePlayerAchievePoint(achive);
                count = 0;
            }
            else if (key == InfoInsertionType.左阵营等级提升)
            {
                IICampAchieveLevel achiveLevel = queue.Dequeue() as IICampAchieveLevel;
                RedCampAchieveLevel(achiveLevel);
                count = 0;
            }
            else if (key == InfoInsertionType.右阵营等级提升)
            {
                IICampAchieveLevel achiveLevel = queue.Dequeue() as IICampAchieveLevel;
                BlueCampAchieveLevel(achiveLevel);
                count = 0;
            }
            else if (key == InfoInsertionType.左玩家加入)
            {
                IIPlayerJoin join = queue.Dequeue() as IIPlayerJoin;
                uiInfo.c_TopPlayerJoin.Play(join.playerInfo);
                count = 0;
            }
            else if (key == InfoInsertionType.右玩家加入)
            {
                IIPlayerJoin join = queue.Dequeue() as IIPlayerJoin;
                uiInfo.c_TopPlayerJoin.Play(join.playerInfo);
                count = 0;
            }
            else if (key == InfoInsertionType.左送盲盒)
            {
                IIPlayerSendBlindBox leftGiftBlindBox = queue.Dequeue() as IIPlayerSendBlindBox;
                uiInfo.ts_LeftSendBlindBox.SetAsLastSibling();
                UIPlayerSendBlindBoxComp leftBox = GetPrefabInstance(key, uiInfo.ts_LeftSendBlindBox).GetComponent<UIPlayerSendBlindBoxComp>();
                leftBox.Init(leftGiftBlindBox.drawIndex, leftGiftBlindBox.rewardDes, true);
                count = 0;
            }
            else if (key == InfoInsertionType.右送盲盒)
            {
                IIPlayerSendBlindBox rightGiftBlindBox = queue.Dequeue() as IIPlayerSendBlindBox;
                uiInfo.ts_RightSendBlindBox.SetAsLastSibling();
                UIPlayerSendBlindBoxComp rightBox = GetPrefabInstance(key, uiInfo.ts_RightSendBlindBox).GetComponent<UIPlayerSendBlindBoxComp>();
                rightBox.Init(rightGiftBlindBox.drawIndex, rightGiftBlindBox.rewardDes, false);
                count = 0;
            }
            else if(key == InfoInsertionType.左送精英怪)
            {
                IIPlayerSendGift leftGift = queue.Dequeue() as IIPlayerSendGift;
               
                if (leftGift.giftType == CDanmuGiftConst.Soldier_Archer)
                {
                    UIPlayerSendGiftComp leftSender = GetPrefabInstance(InfoInsertionType.左送弓箭兵, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftComp>();
                    leftSender.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, leftGift.giftType, (int)leftGift.giftNum, true);
                }
                else if (leftGift.giftType == CDanmuGiftConst.Soldier_BoxLv2)
                {
                    uiInfo.ts_LeftSendRare.SetAsLastSibling();
                    UIPlayerSendGiftRareComp leftSenderRare = GetPrefabInstance(InfoInsertionType.左送精英怪, uiInfo.ts_LeftSendRare).GetComponent<UIPlayerSendGiftRareComp>();
                    leftSenderRare.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, (int)leftGift.giftNum, true, CBattleMgr.Ins.pRedCamp.emCamp);
                }
                else if (leftGift.giftType == CDanmuGiftConst.Hero)
                {
                    uiInfo.ts_LeftSendRare.SetAsLastSibling();
                    UIPlayerSendHeroComp leftSenderHero = GetPrefabInstance(InfoInsertionType.左送英雄, uiInfo.ts_LeftSendHero).GetComponent<UIPlayerSendHeroComp>();
                    leftSenderHero.Init(true, CBattleMgr.Ins.pRedCamp.emCamp, leftGift.playerInfo.userFace, leftGift.playerInfo.userName,
                        "召唤 " + leftGift.giftName + " x " + leftGift.giftNum);
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左放英雄技能, new IIHeroShowSkill(leftGift.playerInfo));
                }
                else if (leftGift.giftType == CDanmuGiftConst.Soldier_Lv1 ||
                        leftGift.giftType == CDanmuGiftConst.Soldier_Lv2 ||
                        leftGift.giftType == CDanmuGiftConst.Soldier_Lv3)
                {
                    UIPlayerSendGiftSpecialComp lSender = GetPrefabInstance(InfoInsertionType.左送高级怪, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
                    lSender.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, leftGift.giftType, (int)leftGift.giftNum, true);
                }
                count = 0;
            }
            else if (key == InfoInsertionType.右送精英怪)
            {
                IIPlayerSendGift rightGift = queue.Dequeue() as IIPlayerSendGift;
                if (rightGift.giftType == CDanmuGiftConst.Soldier_Archer)
                {
                    UIPlayerSendGiftComp rightSender = GetPrefabInstance(InfoInsertionType.右送弓箭兵, uiInfo.ts_RightSendGift).GetComponent<UIPlayerSendGiftComp>();
                    rightSender.Init(CBattleMgr.Ins.pBlueCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, rightGift.giftType, (int)rightGift.giftNum, false);
                }
                else if (rightGift.giftType == CDanmuGiftConst.Soldier_BoxLv2)
                {
                    uiInfo.ts_RightSendRare.SetAsLastSibling();
                    UIPlayerSendGiftRareComp rightSenderRare = GetPrefabInstance(InfoInsertionType.右送精英怪, uiInfo.ts_RightSendRare).GetComponent<UIPlayerSendGiftRareComp>();
                    rightSenderRare.Init(CBattleMgr.Ins.pBlueCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, (int)rightGift.giftNum, false, CBattleMgr.Ins.pBlueCamp.emCamp);
                }
                else if (rightGift.giftType == CDanmuGiftConst.Hero)
                {
                    uiInfo.ts_LeftSendRare.SetAsLastSibling();
                    UIPlayerSendHeroComp rightSenderHero = GetPrefabInstance(InfoInsertionType.右送英雄, uiInfo.ts_RightSendHero).GetComponent<UIPlayerSendHeroComp>();
                    rightSenderHero.Init(false, CBattleMgr.Ins.pBlueCamp.emCamp, rightGift.playerInfo.userFace, rightGift.playerInfo.userName,
                       rightGift.giftNum + " x " + "召唤 " + rightGift.giftName);/*string.Format("<color={0}>{1}</color> 召唤", CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), rightGiftHero.giftName));*/
                    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右放英雄技能, new IIHeroShowSkill(rightGift.playerInfo));
                }
                else if (rightGift.giftType == CDanmuGiftConst.Soldier_Lv1 ||
                         rightGift.giftType == CDanmuGiftConst.Soldier_Lv2 ||
                         rightGift.giftType == CDanmuGiftConst.Soldier_Lv3)
                {
                    UIPlayerSendGiftSpecialComp lSender = GetPrefabInstance(InfoInsertionType.右送高级怪, uiInfo.ts_RightSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
                    lSender.Init(CBattleMgr.Ins.pRedCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, rightGift.giftType, (int)rightGift.giftNum, false);
                }
                count = 0;
            }
            //else if (key == InfoInsertionType.左送高级怪)
            //{
            //    IIPlayerSendGift leftGift = queue.Dequeue() as IIPlayerSendGift;
            //    UIPlayerSendGiftSpecialComp lSender = GetPrefabInstance(key, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
            //    lSender.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, leftGift.giftType, (int)leftGift.giftNum, true);
            //    count = 0;
            //}
            //else if (key == InfoInsertionType.右送高级怪)
            //{
            //    IIPlayerSendGift rightGift = queue.Dequeue() as IIPlayerSendGift;
            //    UIPlayerSendGiftSpecialComp lSender = GetPrefabInstance(key, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftSpecialComp>();
            //    lSender.Init(CBattleMgr.Ins.pRedCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, rightGift.giftType, (int)rightGift.giftNum, true);
            //    count = 0;
            //}
            //else if (key == InfoInsertionType.左送英雄)
            //{
            //    IIPlayerSendGift leftGiftHero = queue.Dequeue() as IIPlayerSendGift;
            //    uiInfo.ts_LeftSendHero.SetAsLastSibling();
            //    UIPlayerSendHeroComp leftSenderHero = GetPrefabInstance(key, uiInfo.ts_LeftSendHero).GetComponent<UIPlayerSendHeroComp>();
            //    leftSenderHero.Init(true, CBattleMgr.Ins.pRedCamp.emCamp, leftGiftHero.playerInfo.userFace, leftGiftHero.playerInfo.userName,
            //        "召唤 " + leftGiftHero.giftName + " x " + leftGiftHero.giftNum);
            //    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.左放英雄技能, new IIHeroShowSkill(leftGiftHero.playerInfo)); 
            //    count = 0;
            //}
            //else if (key == InfoInsertionType.右送英雄)
            //{
            //    IIPlayerSendGift rightGiftHero = queue.Dequeue() as IIPlayerSendGift;
            //    uiInfo.ts_RightSendHero.SetAsLastSibling();
            //    UIPlayerSendHeroComp rightSenderHero = GetPrefabInstance(key, uiInfo.ts_RightSendHero).GetComponent<UIPlayerSendHeroComp>();
            //    rightSenderHero.Init(false, CBattleMgr.Ins.pBlueCamp.emCamp, rightGiftHero.playerInfo.userFace, rightGiftHero.playerInfo.userName,
            //       rightGiftHero.giftNum + " x " + "召唤 " + rightGiftHero.giftName);/*string.Format("<color={0}>{1}</color> 召唤", CHelpTools.ColorToHTML(CBattleMgr.Ins.pBlueCamp.pColor), rightGiftHero.giftName));*/
            //    UIScreenInfoInsertionMgr.Ins.SetAInsertion(InfoInsertionType.右放英雄技能, new IIHeroShowSkill(rightGiftHero.playerInfo));
            //    count = 0;
            //}
            //else if(key == InfoInsertionType.左送弓箭兵)
            //{
            //    IIPlayerSendGift leftGift = queue.Dequeue() as IIPlayerSendGift;
            //    UIPlayerSendGiftComp leftSender = GetPrefabInstance(key, uiInfo.ts_LeftSendGift).GetComponent<UIPlayerSendGiftComp>();
            //    leftSender.Init(CBattleMgr.Ins.pRedCamp.pColor, leftGift.playerInfo.userFace, leftGift.playerInfo.userName, leftGift.giftName, leftGift.giftType, (int)leftGift.giftNum, true);
            //    count = 0;
            //}
            //else if (key == InfoInsertionType.右送弓箭兵)
            //{
            //    IIPlayerSendGift rightGift = queue.Dequeue() as IIPlayerSendGift;
            //    UIPlayerSendGiftComp rightSender = GetPrefabInstance(key, uiInfo.ts_RightSendGift).GetComponent<UIPlayerSendGiftComp>();
            //    rightSender.Init(CBattleMgr.Ins.pBlueCamp.pColor, rightGift.playerInfo.userFace, rightGift.playerInfo.userName, rightGift.giftName, rightGift.giftType, (int)rightGift.giftNum, false);
            //    count = 0;
            //}

            infoInsertionDic[key] = (count, queue);
        }
    }


    //private void RedPlayerAchievePoint(IIPlayerAchievePoint achive)
    //{
    //    UIPlayerAchivePointComp achiver = GetPrefabInstance(InfoInsertionType.左分数达标, uiInfo.ts_LeftAchivePoint).GetComponent<UIPlayerAchivePointComp>();

    //    achiver.Init(CBattleMgr.Ins.pRedCamp.pColor, achive.playerInfo.userName, achive.achivePoints, true, achive.playerInfo.userFace, achive.JumpTime);
    //}

    //private void BluePlayerAchievePoint(IIPlayerAchievePoint achive)
    //{
    //    UIPlayerAchivePointComp achiver = GetPrefabInstance(InfoInsertionType.
    //       右分数达标, uiInfo.ts_RightAchivePoint).GetComponent<UIPlayerAchivePointComp>();

    //    achiver.Init(CBattleMgr.Ins.pBlueCamp.pColor, achive.playerInfo.userName, achive.achivePoints, false, achive.playerInfo.userFace, achive.JumpTime);
    //}

    private void RedPlayerAchievePoint(IIPlayerGameEarnPoint achive)
    {
        UIPlayerAchivePointComp achiver = GetPrefabInstance(InfoInsertionType.左分数达标, uiInfo.ts_LeftAchivePoint).GetComponent<UIPlayerAchivePointComp>();

        achiver.Init(CBattleMgr.Ins.pRedCamp.pColor, achive.playerInfo.userName, achive.achivePoints, true, achive.playerInfo.userFace, achive.JumpTime);
    }

    private void BluePlayerAchievePoint(IIPlayerGameEarnPoint achive)
    {
        UIPlayerAchivePointComp achiver = GetPrefabInstance(InfoInsertionType.
           右分数达标, uiInfo.ts_RightAchivePoint).GetComponent<UIPlayerAchivePointComp>();

        achiver.Init(CBattleMgr.Ins.pBlueCamp.pColor, achive.playerInfo.userName, achive.achivePoints, false, achive.playerInfo.userFace, achive.JumpTime);
    }

    private void RedCampAchieveLevel(IICampAchieveLevel achive)
    {
        UICampLevelUpComp achiver = GetPrefabInstance(InfoInsertionType.左阵营等级提升, uiInfo.ts_LeftCampLevelAchive).GetComponent<UICampLevelUpComp>();

        achiver.Init(achive.baseUnit.pCampInfo.pColor, CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, achive.baseUnit.pCampInfo.szCampCNName) , achive.level-1, achive.level, true, "");
    }

    private void BlueCampAchieveLevel(IICampAchieveLevel achive)
    {
        UICampLevelUpComp achiver = GetPrefabInstance(InfoInsertionType.右阵营等级提升, uiInfo.ts_RightCampLevelAchive).GetComponent<UICampLevelUpComp>();

        achiver.Init(achive.baseUnit.pCampInfo.pColor, CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, achive.baseUnit.pCampInfo.szCampCNName), achive.level - 1, achive.level, false, "");
    }

    #endregion

    private static string GetLineCNName(EMStayPathType path) {
        if (path == EMStayPathType.Up)
        {
            return "上路";
        }
        if (path == EMStayPathType.Center)
        {
            return "中路";
        }
        if (path == EMStayPathType.Down)
        {
            return "下路";
        }
        return "中路";
    }
}
public enum InfoInsertionType
{
    左玩家加入,
    右玩家加入,
    左送弓箭兵,
    右送弓箭兵,
    左送盲盒,
    右送盲盒,
    左送精英怪,
    右送精英怪,
    左送英雄,
    右送英雄,
    左分数达标,//队列  todo
    右分数达标,//队列  todo
    左阵营等级提升,
    右阵营等级提升,
    左击杀达标,//大杀特杀向上飘屏 todo
    右击杀达标,//大杀特杀向上飘屏 todo
    左防御塔摧毁,
    右防御塔摧毁,
    左兵营摧毁,
    右兵营摧毁,
    左放英雄技能,
    右放英雄技能,
    左送高级怪,
    右送高级怪,
}

public class InsertionInfoBase {
    /// <summary>
    /// 排队显示时间
    /// </summary>
    public float dequeueDelay = 0f;
}
public class IIPlayerJoin : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    public IIPlayerJoin(CPlayerBaseInfo playerInfo) {
        this.playerInfo = playerInfo;
        this.dequeueDelay = 4.5f;
    }
}
public class IIPlayerSendGift : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    public CDanmuGiftConst giftType;
    public string giftName;
    public int giftNum;


    public IIPlayerSendGift(CPlayerBaseInfo playerInfo, CDanmuGiftConst giftType, string giftName, int giftNum, int blindBox)
    {
        this.playerInfo = playerInfo;
        this.giftType = giftType;
        this.giftName = giftName;
        this.giftNum = giftNum;
        this.dequeueDelay = 3f;
    }
}
public class IIPlayerSendBlindBox : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    public int drawIndex;
    public string rewardDes;
    public DelegateNFuncCall call = null;
    public IIPlayerSendBlindBox(CPlayerBaseInfo playerInfo, int drawIndex,string rewardDes, DelegateNFuncCall call)
    {
        this.playerInfo = playerInfo;
        this.drawIndex = drawIndex;
        this.rewardDes = rewardDes;
        this.call = call;
        this.dequeueDelay = 7f;
    }
}


public class IIPlayerAchievePoint : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    /// <summary>
    /// 达到的击杀数量
    /// </summary>
    public int achivePoints;
    /// <summary>
    /// 数字跳动时间
    /// </summary>
    private float jumpTime;

    public IIPlayerAchievePoint(CPlayerBaseInfo playerInfo, int achivePoints) {
        this.playerInfo = playerInfo;
        this.achivePoints = achivePoints;
       
    }

    public float JumpTime
    {
        get => jumpTime; set
        {
            jumpTime = value;
            this.dequeueDelay = jumpTime + 1.5f;
        }
    }
}

public class IIPlayerGameEarnPoint : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    /// <summary>
    /// 累积获得的功勋
    /// </summary>
    public long achivePoints;

    public int curShowIdx;

    /// <summary>
    /// 数字跳动时间
    /// </summary>
    private float jumpTime;

    public IIPlayerGameEarnPoint(CPlayerBaseInfo playerInfo, long achivePoints, int idx)
    {
        this.playerInfo = playerInfo;
        this.achivePoints = achivePoints;
        this.curShowIdx = idx;
    }

    public float JumpTime
    {
        get => jumpTime; set
        {
            jumpTime = value;
            this.dequeueDelay = jumpTime + 1.5f;
        }
    }
}

public class IICampAchieveLevel : InsertionInfoBase
{
    public CBaseUnit baseUnit;
    public int level;

    public IICampAchieveLevel(CBaseUnit baseUnit, int level)
    {
        this.baseUnit = baseUnit;
        this.dequeueDelay = 3f;
        this.level = level;
    }
}

public class IITowerDestroy : InsertionInfoBase
{
    public EMUnitCamp unitCamp;
    public EMStayPathType emPathType;
    public IITowerDestroy(EMUnitCamp unitCamp, EMStayPathType emPathType)
    {
        this.unitCamp = unitCamp;
        this.emPathType = emPathType;
    }
}

public class IICampDestroy : InsertionInfoBase
{
    public EMUnitCamp unitCamp;
    public EMStayPathType emPathType;
    public IICampDestroy(EMUnitCamp unitCamp, EMStayPathType emPathType)
    {
        this.unitCamp = unitCamp;
        this.emPathType = emPathType;
    }
}
public class IIHeroShowSkill : InsertionInfoBase
{
    public CPlayerBaseInfo playerInfo;
    public IIHeroShowSkill(CPlayerBaseInfo playerInfo)
    {
        this.playerInfo = playerInfo;
    }
}

public class IIPlayerKillCombo : InsertionInfoBase {
    public CPlayerBaseInfo playerInfo;
    public int killNum;
    public IIPlayerKillCombo(CPlayerBaseInfo playerInfo,int killNum)
    {
        this.playerInfo = playerInfo;
        this.killNum = killNum;
    }
}
