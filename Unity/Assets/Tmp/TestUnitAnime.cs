using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestUnitAnime : MonoBehaviour
{
    public CUnitAnimeCtrl pCtrl;

    private void Start()
    {
        pCtrl.PlayAnime(pCtrl.emCurState, pCtrl.emCurDir, true);
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        if(GUILayout.Button("待机"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Idle, pCtrl.emCurDir);
        }

        if (GUILayout.Button("移动"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Move, pCtrl.emCurDir);
        }

        if (GUILayout.Button("攻击"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Atk, pCtrl.emCurDir);
        }

        if (GUILayout.Button("死亡"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Dead, pCtrl.emCurDir);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("上"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Up);
        }

        if (GUILayout.Button("下"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Down);
        }

        if (GUILayout.Button("左"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Left);
        }

        if (GUILayout.Button("右"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Right);
        }

        if (GUILayout.Button("上右"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.UpR);
        }

        if (GUILayout.Button("上左"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.UpL);
        }

        if (GUILayout.Button("下右"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.DownR);
        }

        if (GUILayout.Button("下左"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.DownL);
        }

        GUILayout.EndHorizontal();
    }
}
