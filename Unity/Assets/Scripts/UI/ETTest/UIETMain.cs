using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIETMain : MonoBehaviour
{
    public ETInit pInit;

    public GameObject objBoardLogin;
    public GameObject objBoardRoom;

    // Start is called before the first frame update
    void Start()
    {
        CNetConfigMgr.Ins.Init();

        ETGame.EventSystem.Add(DLLType.Main, typeof(CGlobalInit).Assembly);

        pInit.StartAsync(OnInitOver).Coroutine();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnInitOver()
    {
        Debug.Log("ET Init Suc");
    }
}
