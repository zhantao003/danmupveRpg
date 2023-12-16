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

        if(GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Idle, pCtrl.emCurDir);
        }

        if (GUILayout.Button("�ƶ�"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Move, pCtrl.emCurDir);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Atk, pCtrl.emCurDir);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(EMUnitAnimeState.Dead, pCtrl.emCurDir);
        }

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        if (GUILayout.Button("��"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Up);
        }

        if (GUILayout.Button("��"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Down);
        }

        if (GUILayout.Button("��"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Left);
        }

        if (GUILayout.Button("��"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.Right);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.UpR);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.UpL);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.DownR);
        }

        if (GUILayout.Button("����"))
        {
            pCtrl.PlayAnime(pCtrl.emCurState, EMUnitAnimeDir.DownL);
        }

        GUILayout.EndHorizontal();
    }
}
