using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSceneNetGame : CSceneBase
{
    public override void OnSceneStart()
    {
        CLockStepMgr.Ins.InitPhysicOnly();
        CBattleMgr.Ins.Init();
        ///预加载
#if PRELOADASSET
        CBattleMgr.Ins.PreLoadEvent();
#endif

        UIManager.Instance.RefreshUI();
        UIManager.Instance.OpenUI(UIResType.GameInfo);
        UIManager.Instance.OpenUI(UIResType.GiftEff);
        UIManager.Instance.OpenUI(UIResType.WorldUI);
        
        ///设置建筑血条UI
        CBattleMgr.Ins.mapMgr.SetBuildHP();
        ///抽奖表演UI
        UIGameInfo uiGameInfo = UIManager.Instance.GetUI(UIResType.GameInfo) as UIGameInfo;
        uiGameInfo.slotBG.gameObject.SetActive(true);

        uiGameInfo.redSlot.DrawFun((int)CBattleMgr.Ins.pRedCamp.emCamp, UnityEngine.Random.Range(1f, 1.5f));

        uiGameInfo.blueSlot.DrawFun((int)CBattleMgr.Ins.pBlueCamp.emCamp, UnityEngine.Random.Range(1f, 1.5f));

        /////抽奖ui动画开始
        //CBattleMgr.Ins.StartChouJiangUI();

        ETHandlerReqGameReady.Request().Coroutine();
    }

    public override void OnSceneLeave()
    {
        UIManager.Instance.ClearUI();
        //if (CBulletMgr.Ins != null)
        //{
        //    CBulletMgr.Ins.ClearAllBulletUnit();
        //}
        CPlayerMgr.Ins.ClearAllPlayerInfo();
        CPlayerMgr.Ins.ClearAllPlayerUnit();
        CAudioMgr.Ins.ClearAllAudio();
        CGameAntGlobalMgr.Ins.ClearCampList();

        //网络断开链接
        ERoomInfoMgr.Ins.Dispose();
    }
}
