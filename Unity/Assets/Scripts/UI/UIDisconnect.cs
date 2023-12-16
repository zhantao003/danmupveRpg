using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YYDanmu;

public class UIDisconnect : UIBase
{
    public GameObject objShow;

    private void Start()
    {
        objShow.SetActive(false);
    }

    public static void Show()
    {
        UIManager.Instance.OpenUI(UIResType.Disconnect);

        UIDisconnect uiDisconnect = UIManager.Instance.GetUI(UIResType.Disconnect) as UIDisconnect;
        if (uiDisconnect == null) return;
        uiDisconnect.objShow.SetActive(true);
    }


    public void Disconnect()
    {
        objShow.SetActive(false);
        //TODO:¶Ï¿ªÍøÂçÁ´½Ó
        if (SessionComponent.Instance != null &&
            SessionComponent.Instance.Session != null)
        {
            SessionComponent.Instance.Session.Dispose();
        }
        if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
        {
            YYOpenClient pYYClient = CDanmuSDKCenter.Ins.arrPlatformMgr[(int)CDanmuSDKCenter.Ins.emPlatform].GetComponent<YYOpenClient>();
            pYYClient.EndRound();
        }
        CLockStepMgr.Ins.ClearAllList();
        AStarFindPath.Ins.dicMapSlots.Clear();
      
        CloseSelf(); 
        
        CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.GameModeSelect102);

      
    }
}
