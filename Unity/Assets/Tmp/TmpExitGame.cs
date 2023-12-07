using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TmpExitGame : MonoBehaviour
{
    
    public void ExitGame()
    {
        Application.Quit();
    }

    public void OnClickSetting()
    {
        //UIManager.Instance.OpenUI(UIResType.Setting);
    }
}
