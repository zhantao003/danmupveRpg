using OpenBLive.Runtime.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerUnit : MonoBehaviour
{
    //�û���Ϣ
    public CPlayerBaseInfo pInfo;

    public Transform tranNameRoot;

    public string uid;

    public enum EMState
    {
        Idle,               //����
        Move,               //�ƶ�
    }

    /// <summary>
    /// ��ǰ��״̬
    /// </summary>
    public EMState emCurState = EMState.Idle;
    /// <summary>
    /// ״̬��
    /// </summary>
    public FSMManager pFSM;

    public DelegateNFuncCall dlgRecycle;            //�����¼�


    public void Init(CPlayerBaseInfo baseInfo)
    {
        pInfo = baseInfo;
        uid = pInfo.uid;
        InitFSM();
        SetState(EMState.Idle);
    }

    protected virtual void InitFSM()
    {
        pFSM = new FSMManager(this);
        pFSM.AddState((int)EMState.Idle, new FSMUnitIdle());
        pFSM.AddState((int)EMState.Move, new FSMUnitMove());
    }

    private void FixedUpdate()
    {
        if (pFSM != null)
        {
            pFSM.FixedUpdate(CTimeMgr.FixedDeltaTime);
        }
    }

    private void Update()
    {
        if (pFSM != null)
        {
            pFSM.Update(CTimeMgr.DeltaTime);
        }
    }

    public virtual void SetState(EMState state, CLocalNetMsg msg = null)
    {
        pFSM.ChangeMainState((int)state, msg);
    }

    public virtual void PlayAnime(string anime, float crossfade = 0.1F, float startlerp = 0F)
    {
        //szAnima = anime;
        //pAvatar.PlayAnime(anime, crossfade, startlerp);
        //if (pAnime == null) return;

        //pAnime.CrossFade(anime, crossfade, 0, startlerp);
    }

    //public void SendExitDM()
    //{
    //    CDanmuChat dm = new CDanmuChat();
    //    dm.uid = uid;
    //    dm.nickName = pInfo.userName;
    //    dm.headIcon = pInfo.userFace;
    //    dm.content = "�뿪";
    //    dm.roomId = CDanmuBilibiliMgr.Ins.lRoomID;
    //    dm.Mock();
    //}

    public void Recycle()
    {
        dlgRecycle?.Invoke();
    }
}
