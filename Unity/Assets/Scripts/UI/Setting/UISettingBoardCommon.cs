using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISettingBoardCommon : MonoBehaviour
{
    [Header("分辨率下拉框")]
    public Dropdown uiDropReslution;
    /// <summary>
    /// 保存所有分辨率的信息
    /// </summary>
    List<ScreenResolutionInfo> listResolutionInfos = new List<ScreenResolutionInfo>();
    
    [Header("全屏模式Tog")]
    public Toggle uiTogFullScreen;
    [Header("主音量滑动条")]
    public Slider uiSliderMasterVolume;
    [Header("音效滑动条")]
    public Slider uiSliderEffectVolume;
    [Header("音乐滑动条")]
    public Slider uiSliderBGM;
    [Header("主音量数值")]
    public Text uiLabelMaterVolume;
    [Header("音效数值")]
    public Text uiLabelEffectVolume;
    [Header("音乐数值")]
    public Text uiLabelBGM;

    /// <summary>
    /// 是否设置过信息
    /// </summary>
    public bool bSetInfo;

    public void OnOpen() {
        if (UIManager.Instance.GetUI(UIResType.MainMenu) != null)
        {
            
        }
        else
        {
            
        }
        Init();
    }

    public void Init()
    {
        if (listResolutionInfos.Count <= 0)
        {
            ///设置下拉框信息
            SetDropDownInfo();
            ///设置Tog信息
            SetTogInfo();
            ///设置滑动条信息
            SetSliderInfo();
        }
        bSetInfo = false;
    }

    /// <summary>
    /// 设置下拉框信息
    /// </summary>
    public void SetDropDownInfo()
    {
        ///获取当前屏幕的分辨率
        int nCurScreenWidth = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.RESOLUTIONX);
        int nCurScreenHeight = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.RESOLUTIONY);

        ///设置分辨率下拉框
        uiDropReslution.onValueChanged.AddListener(ChgResolution);
        int nCurChoice = Screen.resolutions.Length - 1;
        List<string> listStrReslution = new List<string>();

        for (int i = 0; i < 4; i++)
        {
            ScreenResolutionInfo resolutionInfo = null;

            //if (CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.QQNow ||
            //    CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.KuaiShou ||
            //    CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinYS ||
            //    CDanmuSDKCenter.Ins.emPlatform == CDanmuSDKCenter.EMPlatform.DouyinOpen)
            //{
                if (i == 0)
                {
                    if (nCurScreenWidth == 540 && nCurScreenHeight == 960)
                    {
                        nCurChoice = 0;
                    }

                    resolutionInfo = new ScreenResolutionInfo(540, 960);
                }
                else if (i == 1)
                {
                    if (nCurScreenWidth == 720 && nCurScreenHeight == 1280)
                    {
                        nCurChoice = 1;
                    }

                    resolutionInfo = new ScreenResolutionInfo(720, 1280);
                }
                else if (i == 2)
                {
                    if (nCurScreenWidth == 810 && nCurScreenHeight == 1440)
                    {
                        nCurChoice = 2;
                    }

                    resolutionInfo = new ScreenResolutionInfo(810, 1440);
                }
                else if (i == 3)
                {
                    if (nCurScreenWidth == 1080 && nCurScreenHeight == 1920)
                    {
                        nCurChoice = 3;
                    }

                    resolutionInfo = new ScreenResolutionInfo(1080, 1920);
                }
            //}
            //else
            //{
            //    if (i == 0)
            //    {
            //        if (nCurScreenWidth == 1280 && nCurScreenHeight == 720)
            //        {
            //            nCurChoice = 0;
            //        }

            //        resolutionInfo = new ScreenResolutionInfo(1280, 720);
            //    }
            //    else if (i == 1)
            //    {
            //        if (nCurScreenWidth == 1440 && nCurScreenHeight == 810)
            //        {
            //            nCurChoice = 1;
            //        }

            //        resolutionInfo = new ScreenResolutionInfo(1440, 810);
            //    }
            //    else if (i == 2)
            //    {
            //        if (nCurScreenWidth == 1600 && nCurScreenHeight == 900)
            //        {
            //            nCurChoice = 2;
            //        }

            //        resolutionInfo = new ScreenResolutionInfo(1600, 900);
            //    }
            //    else if (i == 3)
            //    {
            //        if (nCurScreenWidth == 1920 && nCurScreenHeight == 1080)
            //        {
            //            nCurChoice = 3;
            //        }

            //        resolutionInfo = new ScreenResolutionInfo(1920, 1080);
            //    }
            //}

            listResolutionInfos.Add(resolutionInfo);
            listStrReslution.Add(resolutionInfo.nReslutionX + "x" + resolutionInfo.nReslutionY);
        }

        uiDropReslution.ClearOptions();
        uiDropReslution.AddOptions(listStrReslution);
        uiDropReslution.SetValueWithoutNotify(nCurChoice);
    }

    
    /// <summary>
    /// 设置Tog信息
    /// </summary>
    public void SetTogInfo()
    {
        uiTogFullScreen.onValueChanged.AddListener(SetFullScreen);
        uiTogFullScreen.isOn = CSystemInfoMgr.Inst.GetBool(CSystemInfoConst.FULLSCREEN);

    }

    /// <summary>
    /// 设置滑动条信息
    /// </summary>
    public void SetSliderInfo()
    {
        uiSliderMasterVolume.onValueChanged.AddListener(SetMaterVolume);
        uiSliderEffectVolume.onValueChanged.AddListener(SetEffectVolume);
        uiSliderBGM.onValueChanged.AddListener(SetBGM);
        uiSliderMasterVolume.value = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.ALLSOUND) * 0.01F;
        uiLabelMaterVolume.text = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.ALLSOUND).ToString();

        uiSliderEffectVolume.value = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.AUDIO) * 0.01F;
        uiLabelEffectVolume.text = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.AUDIO).ToString();

        uiSliderBGM.value = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.BGM) * 0.01F;
        uiLabelBGM.text = CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.BGM).ToString();
    }

    #region DropDownEvent

    /// <summary>
    /// 设置分辨率
    /// </summary>
    /// <param name="nIdx"></param>
    public void ChgResolution(int nIdx)
    {
        
        bSetInfo = true;
        CSystemInfoMgr.Inst.SetResolution(listResolutionInfos[nIdx].nReslutionX,
                                          listResolutionInfos[nIdx].nReslutionY,
                                          CSystemInfoMgr.Inst.GetBool(CSystemInfoConst.FULLSCREEN));
    }

    #endregion

    #region TogEvent

    

    public void SetFullScreen(bool bValue)
    {
        
        bSetInfo = true;
        CSystemInfoMgr.Inst.SetResolution(CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.RESOLUTIONX),
                                          CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.RESOLUTIONY),
                                          bValue);
    }


    #endregion

    #region SliderEvent

    public void SetMaterVolume(float fValue)
    {
        bSetInfo = true;
        int nFinalValue = (int)(fValue * 100F);
        uiLabelMaterVolume.text = nFinalValue.ToString("f0");

        CSystemInfoMgr.Inst.SaveAllSoundSet(nFinalValue);
        //Debug.Log(CSystemInfoMgr.Inst.GetInt(CSystemInfoConst.AUDIO) + "======Audio");
    }

    public void SetEffectVolume(float fValue)
    {
        bSetInfo = true;
        int nFinalValue = (int)(fValue * 100F);
        uiLabelEffectVolume.text = nFinalValue.ToString("f0");

        CSystemInfoMgr.Inst.SaveAudioSet(nFinalValue);
    }

    public void SetBGM(float fValue)
    {
        bSetInfo = true;
        int nFinalValue = (int)(fValue * 100F);
        uiLabelBGM.text = nFinalValue.ToString("f0");

        CSystemInfoMgr.Inst.SaveBgmSet(nFinalValue);
    }

    #endregion

     /// <summary>
    /// 修复网络
    /// </summary>
    public void OnClickRepairWeb()
    {
        //CDanmuMgr.Ins.EndGame(true);

        //UIManager.Instance.OpenUI(UIResType.RepaireNet);
    }

}
