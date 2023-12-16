using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 屏幕分辨率信息
/// </summary>
public class ScreenResolutionInfo
{
    public int nReslutionX;     //分辨率X
    public int nReslutionY;     //分辨率Y

    public ScreenResolutionInfo(int nX, int nY)
    {
        nReslutionX = nX;
        nReslutionY = nY;
    }
} 

public class UISetting : UIBase
{
    public enum EMBoardType{
        Common,
        Graphycs,
    }

    public UISettingBoardCommon pBoardCommon;

    public override void OnOpen()
    {
        base.OnOpen();

        SetBoard(EMBoardType.Common);
    }

    public void SetBoard(EMBoardType board)
    {
        
        pBoardCommon.gameObject.SetActive(board == EMBoardType.Common);

        if(board == EMBoardType.Common)
        {
            pBoardCommon.OnOpen();
        }
    }

    public void OnClickBoardType(int board)
    {
        SetBoard((EMBoardType)board);
    }

    /// <summary>
    /// 退出
    /// </summary>
    public void OnClickClose()
    {
        if (pBoardCommon.bSetInfo)
        {
            pBoardCommon.bSetInfo = false;

            CSystemInfoMgr.Inst.SaveFile();
        }

        CloseSelf();
    }

    /// <summary>
    /// 保存并退出
    /// </summary>
    public void OnClickSave()
    {
        if (pBoardCommon.bSetInfo)
        {
            pBoardCommon.bSetInfo = false;

            CSystemInfoMgr.Inst.SaveFile();
        }

        CloseSelf();
    }

    public void OnClickExit()
    {
        CloseSelf();
    }
}
