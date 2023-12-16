using SharedLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CFreeCreatCount
{
    public int nPlayerCount;
    public int nCreatCount;
}

/// <summary>
/// 游戏全局管理器
/// </summary>
public class CGameAntGlobalMgr : CSingleCompBase<CGameAntGlobalMgr>
{
    public enum EMGameType
    {
        LocalPvP,   //本地对战
        NetPvP,     //联机对战
    }

    public EMGameType emGameType = EMGameType.LocalPvP;

    [Header("游戏配置")]
    public CConfigColorFish pStaticConfig;
    [Header("免费生成兵的数量")]
    public CFreeCreatCount[] freeCreatCounts;

    public int nHPLev;

    public int nRoundCount = 1;

    public void Init()
    {
        
    }

    public Dictionary<string, string> leftPlayerUids = new Dictionary<string, string>();
    public Dictionary<string, string> rightPlayerUids = new Dictionary<string, string>();

    public void LoginPlayer(string uid, string nickname, string headIcon, long vipLv,int joinTeamLeft0Right1, DelegateNFuncCall call = null)
    {
        if (CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.MainMenu ||
            CSceneMgr.Instance.m_objCurScene.emSceneType == CSceneFactory.EMSceneType.GameModeSelect102)
        {
            return;
        }
        if (joinTeamLeft0Right1 == 0)
        {
            if (!leftPlayerUids.ContainsKey(uid))
            {
                leftPlayerUids.Add(uid, nickname);
            }
        }
        else if (joinTeamLeft0Right1 == 1)
        {
            if (!rightPlayerUids.ContainsKey(uid))
            {
                rightPlayerUids.Add(uid, nickname);
            }
        }
        Debug.LogWarning("Login Info {uid:" + uid + "} {NickName:" + nickname + "}  {IsJoinLeft0Right1:" + joinTeamLeft0Right1 + "}");
        CPlayerNetHelper.Login(uid, nickname, headIcon, vipLv, call);
    }

    public int GetFreeCreatCount()
    {
        int nCreatCount = 0;
        int playerCount = CPlayerMgr.Ins.GetAllBaseInfoCount();
        for (int i = 0; i < freeCreatCounts.Length; i++)
        {
            if (playerCount > freeCreatCounts[i].nPlayerCount)
            {
                nCreatCount = freeCreatCounts[i].nCreatCount;
            }
        }
        return nCreatCount;
    }

    public void AddNewPlayerByLocal(string uid, string nickname, string headIcon, long vipLv, EMUnitCamp camp, EMStayPathType pathType)
    {
        CPlayerBaseInfo pPlayer = new CPlayerBaseInfo(uid, nickname, headIcon, 0, "", true, vipLv,
                                                      CDanmuSDKCenter.Ins.szRoomId,
                                                      CPlayerBaseInfo.EMUserType.Guanzhong, 0,99,1);
        pPlayer.emCamp = camp;
        pPlayer.emPathType = pathType;
        CPlayerMgr.Ins.AddPlayer(pPlayer);
    }

    public void ClearCampList()
    {
        leftPlayerUids.Clear();
        rightPlayerUids.Clear();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.Instance.OpenUI(UIResType.Setting);
        }
    }

}
