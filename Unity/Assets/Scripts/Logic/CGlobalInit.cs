using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CGlobalInit : CSingleCompBase<CGlobalInit>
{
    bool bInited = false;
    public bool bDebug = false;

    public ETModel.ETInit pETModel;

    //传入参数的房间号
    [ReadOnly]
    public string szArgRoomID = "";
    //传入参数的房间号
    [ReadOnly]
    public string szArgCode = "";

    [ShowIf("bDebug", true, true)]
    public CSceneFactory.EMSceneType emNextScene;

    private void Awake()
    {
        if (CGlobalInit.Ins.bInited)
        {
            Debug.Log("删掉管理器!");
            Destroy(gameObject);
            return;
        }

        //获取传入参数码
        CheckArgs();

        pETModel.StartAsync(this.OnETInitSuc).Coroutine();
    }

    void CheckArgs()
    {
        string[] args = Environment.GetCommandLineArgs();

        string szLogContent = "";
        if (args == null)
        {
            szLogContent = "无参数";
            Debug.LogWarning(szLogContent);
            return;
        }

        if (args.Length <= 0)
        {
            szLogContent = "长度为0传参";
            Debug.LogWarning(szLogContent);
            return;
        }

        for (int i = 0; i < args.Length; i++)
        {
            szLogContent += $"第{i + 1}个:" + args[i] + "\r\n";

            if (args[i].StartsWith("room_id"))  //B站
            {
                string[] arrContent = args[i].Split('=');
                if (arrContent != null && arrContent.Length == 2)
                {
                    szArgRoomID = arrContent[1];
                }
            }
            else if (args[i].StartsWith("code")) //B站
            {
                string[] arrContent = args[i].Split('=');
                if (arrContent != null && arrContent.Length == 2)
                {
                    szArgCode = arrContent[1];
                }
            }
            else if (args[i].EndsWith("-c"))    //快手
            {
                if (i + 1 < args.Length)
                {
                    szArgCode = args[i + 1];
                }
            }
        }

        Debug.LogWarning("应用传参：\r\n" + szLogContent);
    }

    void OnETInitSuc()
    {
        CLogTools.DebugLogReg();
        AssetBundle.SetAssetBundleDecryptKey(CEncryptHelper.ASSETKEY);
        CSystemInfoMgr.Inst.Init();
        CSceneMgr.Instance.Init();
        CResLoadMgr.Inst.Init();
        CTBLInfo.Inst.Init();
        CAudioMgr.Ins.Init();
        CNetConfigMgr.Ins.Init();
        CHttpMgr.Instance.Init();
        //CGameColorPetMgr.Ins.Init();

        bInited = true;

        if (bDebug)
        {
            CSceneMgr.Instance.LoadScene(emNextScene);
        }
        else
        {
            CSceneMgr.Instance.LoadScene(CSceneFactory.EMSceneType.MainMenu);
        }
    }
}
