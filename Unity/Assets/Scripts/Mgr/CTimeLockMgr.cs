using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// ʱͣ����
/// </summary>
public enum EMTimeLockType
{
    CameraSpeicalBaseBuild,          //�������д����(����)
    PlayHeroSkill1,                   //�ͷ�Ӣ�ۼ���
    PlayHeroSkill2,                   //�ͷ�Ӣ�ۼ���
    PlayHeroSkill3,                   //�ͷ�Ӣ�ۼ���
    CameraSpeicalNormalBuild,        //�������д����(��Ӫ������)
}

public class CTimeLockMgr : CSingleCompOdinBase<CTimeLockMgr>
{
    /// <summary>
    /// ʱͣ��Ϣ
    /// </summary>
    [System.Serializable]
    public class CTimeLockInfo
    {
        /// <summary>
        /// ʱͣʱ��
        /// </summary>
        public float fLockTime;
        /// <summary>
        /// ʱͣ����
        /// </summary>
        public float fTimeScale;
        /// <summary>
        ///ʱͣ����
        /// </summary>
        public EMTimeLockType emTimeLockType;

        public CTimeLockInfo()
        {

        }

        public CTimeLockInfo(CTimeLockInfo lockInfo)
        {
            fLockTime = lockInfo.fLockTime;
            fTimeScale = lockInfo.fTimeScale;
            emTimeLockType = lockInfo.emTimeLockType;
        }
    }

    [DictionaryDrawerSettings(KeyLabel = "ʱͣ����", ValueLabel = "ʱͣ��Ϣ")]
    public Dictionary<EMTimeLockType, CTimeLockInfo> dicTimeLockInfo = new Dictionary<EMTimeLockType, CTimeLockInfo>();

    /// <summary>
    /// ��ǰ���ڲ��ŵ�ʱͣ
    /// </summary>
    CTimeLockInfo curTimeLockInfo;

    float fCurTime;

    public void PlayTimeLock(EMTimeLockType emTimeLockType)
    {
        //if (curTimeLockInfo != null)
        //{
        //    if(curTimeLockInfo.emTimeLockType == EMTimeLockType.PlayHeroSkill1 ||
        //       curTimeLockInfo.emTimeLockType == EMTimeLockType.PlayHeroSkill2 ||
        //       curTimeLockInfo.emTimeLockType == EMTimeLockType.PlayHeroSkill3 )
        //    {
        //        if (emTimeLockType == EMTimeLockType.PlayHeroSkill1 ||
        //            emTimeLockType == EMTimeLockType.PlayHeroSkill2 ||
        //            emTimeLockType == EMTimeLockType.PlayHeroSkill3)
        //        {
                    
        //        }
        //        return;
        //    }
        //}
             
        curTimeLockInfo = new CTimeLockInfo(dicTimeLockInfo[emTimeLockType]);
        fCurTime = 0;
        CTimeMgr.fTimeScale = curTimeLockInfo.fTimeScale;
    }

    private void Update()
    {
        if (curTimeLockInfo == null) return;

        fCurTime += CTimeMgr.DeltaTimeUnScale;
        if(fCurTime >= curTimeLockInfo.fLockTime)
        {
            CTimeMgr.fTimeScale = 1f;
            curTimeLockInfo = null;
        }
    }

}
