using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class CGlobalInit : CSingleCompBase<CGlobalInit>
{
    bool bInited = false;
    public bool bDebug = false;

    public ETModel.ETInit pETModel;

    //��������ķ����
    [ReadOnly]
    public string szArgRoomID = "";
    //��������ķ����
    [ReadOnly]
    public string szArgCode = "";

    //����Token
    [ReadOnly]
    public string szArgToken = "";

    //��������Ϸ��ز���
    [ReadOnly]
    public bool isDouyinCloud = false;
    [ReadOnly]
    public int nDouyinCloudScreenWidth = 1080;
    [ReadOnly]
    public int nDouyinCloudScreenHeight = 1920;
    [ReadOnly]
    public bool nDouyinCloudFullScreen = false;

    [ShowIf("bDebug", true, true)]
    public CSceneFactory.EMSceneType emNextScene;
    [Header("�ı���������")]
    public EMLanguageType emLanguageType;

    private void Awake()
    {
        if (CGlobalInit.Ins.bInited)
        {
            Debug.Log("ɾ��������!");
            Destroy(gameObject);
            return;
        }

        CLogTools.DebugLogReg();

        //��ȡ���������
        CheckArgs();
        CheckArgsDouyinCloud();
        CGameAntGlobalMgr.Ins.Init();

        pETModel.StartAsync(this.OnETInitSuc).Coroutine();


        //string sz = "����{0}��Ū";
        //StringBuilder stringBuilder = new StringBuilder();
        //stringBuilder.AppendFormat(sz, 5513);
        //Debug.Log(stringBuilder.ToString() + "====");
        //stringBuilder.Clear();
        //string sz2 = "��{0}Ū";
        //stringBuilder.AppendFormat(sz2, 5513);
        //Debug.Log(stringBuilder.ToString() + "====");
    }

    void CheckArgs()
    {
        string[] args = Environment.GetCommandLineArgs();

        string szLogContent = "";
        if (args == null)
        {
            szLogContent = "�޲���";
            Debug.LogWarning(szLogContent);
            return;
        }

        if (args.Length <= 0)
        {
            szLogContent = "����Ϊ0����";
            Debug.LogWarning(szLogContent);
            return;
        }

        for (int i = 0; i < args.Length; i++)
        {
            szLogContent += $"��{i + 1}��:" + args[i] + "\r\n";

            if (args[i].StartsWith("room_id"))  //Bվ
            {
                string[] arrContent = args[i].Split('=');
                if (arrContent != null && arrContent.Length == 2)
                {
                    szArgRoomID = arrContent[1];
                }
            }
            else if (args[i].StartsWith("code")) //Bվ
            {
                string[] arrContent = args[i].Split('=');
                if (arrContent != null && arrContent.Length == 2)
                {
                    szArgCode = arrContent[1];
                }
            }
            else if (args[i].EndsWith("-c"))    //����
            {
                if (i + 1 < args.Length)
                {
                    szArgCode = args[i + 1];
                }
            }
            else if (args[i].StartsWith("-token"))  //������YY
            {
                if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
                {
                    int nFirstIdx = args[i].IndexOf('=', 0);
                    szArgToken = args[i].Substring(nFirstIdx + 1);
                    Debug.LogWarning("����Token��" + szArgToken);
                }
                else if(CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.YY)
                {
                    if (i + 1 < args.Length)
                    {
                        szArgToken = args[i + 1];
                        Debug.LogWarning("����Token��" + szArgToken);
                    }
                } 
            }
        }

        Debug.LogWarning("Ӧ�ô��Σ�\r\n" + szLogContent);
    }

    Dictionary<string, int> dicDouyinArgsKV = new Dictionary<string, int>();
    void CheckArgsDouyinCloud()
    {
        List<string> listArgKeys = new List<string>();
        listArgKeys.AddRange(new string[] {
            "-screen-fullscreen",
            "-screen-height",
            "-screen-width",
            "-cloud-game",
            "-mobile",
        });

        string[] args = Environment.GetCommandLineArgs();

        int i = 0;
        dicDouyinArgsKV.Clear();
        while (i < args.Length - 1)
        {
            var key = args[i];
            if (listArgKeys.Contains(key))
            {
                if (int.TryParse(args[i + 1], out var intVar))
                {
                    dicDouyinArgsKV.Add(key, intVar);
                    i += 2;
                    continue;
                }
            }

            i++;
        }

        CheckCloudResolution(dicDouyinArgsKV);
    }

    void CheckCloudResolution(Dictionary<string, int> keyValues)
    {
        //Ĭ�Ϸֱ��ʺ�ȫ��״̬
        var fullScreen = false;
        var screenWidth = 1080;
        var screenHeight = 1920;

        nDouyinCloudScreenWidth = 1080;
        nDouyinCloudScreenHeight = 1920;

        if (keyValues.TryGetValue("-screen-fullscreen", out var full))
        {
            nDouyinCloudFullScreen = (full == 1);
        }

        if (keyValues.TryGetValue("-screen-height", out var height)
            && keyValues.TryGetValue("-screen-width", out var width))
        {
            nDouyinCloudScreenWidth = width;
            nDouyinCloudScreenHeight = height;
        }

        if (keyValues.TryGetValue("-cloud-game", out var cloudGame))
        {
            isDouyinCloud = (cloudGame == 1);
        }

        if (!isDouyinCloud)
        {
            Screen.SetResolution(screenWidth, screenHeight, fullScreen);
        }
        else
        {
            Screen.SetResolution(nDouyinCloudScreenWidth, nDouyinCloudScreenHeight, nDouyinCloudFullScreen);
        }
    }

    void OnETInitSuc()
    {
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
