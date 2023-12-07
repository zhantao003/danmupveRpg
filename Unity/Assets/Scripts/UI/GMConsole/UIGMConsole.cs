using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGMConsole : MonoBehaviour
{
    public InputField uiInputIP;
    public InputField uiInputPort;

    public Text uiLabelResContent;
    public GameObject objConfigAvatarBase;
    public GameObject objConfigGiftBox;

    public UIGMToast uiToast;

    private void Start()
    {
        AssetBundle.SetAssetBundleDecryptKey(CEncryptHelper.ASSETKEY);

        CSystemInfoMgr.Inst.Init();
        CResLoadMgr.Inst.Init();
        CTBLInfo.Inst.Init();
        CNetConfigMgr.Ins.Init();
        CHttpMgr.Instance.Init();

        uiInputIP.text = CNetConfigMgr.Ins.GetHttpServerIP();
        uiInputPort.text = CNetConfigMgr.Ins.GetHttpServerPort().ToString();
    }

    //public void OnClickAvatarBaseConfig()
    //{
    //    objConfigAvatarBase.SetActive(true);
    //    objConfigAvatarBase.GetComponent<UIGMAvatarBaseConfig>().Init();
    //}

    //public void OnClickGiftBoxBaseConfig()
    //{
    //    objConfigGiftBox.SetActive(true);
    //    objConfigGiftBox.GetComponent<UIGMGiftBoxBaseConfig>().Init();
    //}

    public static void SetResContent(string res)
    {
        UIGMConsole uiGM = FindObjectOfType<UIGMConsole>();
        uiGM.uiLabelResContent.text = res;
    }

    public static void ShowToast(string res)
    {
        UIGMConsole uiGM = FindObjectOfType<UIGMConsole>();
        uiGM.uiToast.SetContent(res);
    }

    public static void RefreshURL()
    {
        UIGMConsole uiGM = FindObjectOfType<UIGMConsole>();

        CNetConfigMgr.Ins.msgContent.SetString("httpserver", uiGM.uiInputIP.text);
        CNetConfigMgr.Ins.msgContent.SetInt("httpport", int.Parse(uiGM.uiInputPort.text));

        CHttpMgr.Instance.Init();
    }
}
