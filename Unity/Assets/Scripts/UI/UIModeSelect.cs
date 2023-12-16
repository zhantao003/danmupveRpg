using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIModeSelect : UIBase
{
    public Toggle tog_LocalGame;
    public Toggle tog_BattleGame;
    public Image img_LocalGameHover;
    public Image img_BattleGameHover;

    public Button btn_JoinGame;

    public GameObject objNormal;
    public GameObject objLocalBuildChoice;

    public Toggle[] tog_HPChocies;
    public Text[] text_ModelName;
    public Text[] text_ModelDes;

    int modeIndex = 0;
    protected override void OnStart()
    {
        objNormal.SetActive(true);
        objLocalBuildChoice.SetActive(false);
        tog_LocalGame.onValueChanged.AddListener((bool isOn) => {
            if (isOn)
            {
                modeIndex = 0;
                img_LocalGameHover.gameObject.SetActive(true);
                img_BattleGameHover.gameObject.SetActive(false);
            }
        });
        tog_BattleGame.onValueChanged.AddListener((bool isOn) => {
            if (isOn)
            {
                modeIndex = 1;
                img_LocalGameHover.gameObject.SetActive(false);
                img_BattleGameHover.gameObject.SetActive(true);
            }
        });
        btn_JoinGame.onClick.AddListener(() => {
            if (modeIndex == 0)
            {
                objNormal.SetActive(false);
                objLocalBuildChoice.SetActive(true);
                //UIManager.Instance.OpenUI(UIResType.Loading);
                //CGameAntGlobalMgr.Ins.emGameType = CGameAntGlobalMgr.EMGameType.LocalPvP;

                //CTimeTickMgr.Inst.PushTicker(0.2f, delegate (object[] values)
                //{
                //    CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);
                //});
                //btn_JoinGame.onClick.RemoveAllListeners();
            }
            else if (modeIndex == 1) {
                AllReset();
                CGameAntGlobalMgr.Ins.emGameType = CGameAntGlobalMgr.EMGameType.NetPvP;
                CGameAntGlobalMgr.Ins.nHPLev = 2;
                ETHandlerReqLogin.Login(CDanmuSDKCenter.Ins.szUid,
                                        CDanmuSDKCenter.Ins.szNickName,
                                        CDanmuSDKCenter.Ins.szHeadIcon,
                                        0,
                                        0).Coroutine();
            }
        });
        //for(int i = 0;i< text_ModelName.Length;i++)
        //{
        //    ST_ModeValue modeValue = CTBLHandlerModeValue.Ins.GetInfo(i + 1);
        //    text_ModelName[i].text = modeValue.szModelName;
        //    text_ModelDes[i].text = modeValue.szModelDes.Replace("\\n", "\n"); ;
        //}
        //Debug.LogError(CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.MODELSELECT) + "===Select");
        for (int i = 0; i < tog_HPChocies.Length; i++)
        {
            tog_HPChocies[i].isOn = i == CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.MODELSELECT);
        }
    }

    public void AllReset()
    {
        CPlayerMgr.Ins.ClearAllPlayerInfo();
        CPlayerMgr.Ins.ClearAllPlayerUnit();
        CGameAntGlobalMgr.Ins.ClearCampList();
    }

    public void SelectBuildHP()
    {
        for(int i = 0;i < tog_HPChocies.Length;i++)
        {
            if(tog_HPChocies[i].isOn)
            {
                CGameAntGlobalMgr.Ins.nHPLev = i;
                CSystemInfoMgr.Inst.SaveModelSelect(i);
                CSystemInfoMgr.Inst.SaveFile();
                break;
            }
        }
        UIManager.Instance.OpenUI(UIResType.Loading);
        CGameAntGlobalMgr.Ins.emGameType = CGameAntGlobalMgr.EMGameType.LocalPvP;

        CTimeTickMgr.Inst.PushTicker(0.2f, delegate (object[] values)
        {
            CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameMap101);
        });
        btn_JoinGame.onClick.RemoveAllListeners();
    }

    public override void OnOpen()
    {

    }

    public override void OnClose()
    {

    }

    protected override void OnUpdate(float dt)
    {

    }
}
