using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// 时停类型
/// </summary>
public enum EMTimeLockType
{
    CameraSpeicalBaseBuild,          //摄像机特写缩放(基地)
    PlayHeroSkill1,                   //释放英雄技能
    PlayHeroSkill2,                   //释放英雄技能
    PlayHeroSkill3,                   //释放英雄技能
    CameraSpeicalNormalBuild,        //摄像机特写缩放(兵营，炮塔)
}

public class CTimeLockMgr : CSingleCompOdinBase<CTimeLockMgr>
{
    /// <summary>
    /// 时停信息
    /// </summary>
    [System.Serializable]
    public class CTimeLockInfo
    {
        /// <summary>
        /// 时停时间
        /// </summary>
        public float fLockTime;
        /// <summary>
        /// 时停比例
        /// </summary>
        public float fTimeScale;
        /// <summary>
        ///时停类型
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

    [DictionaryDrawerSettings(KeyLabel = "时停类型", ValueLabel = "时停信息")]
    public Dictionary<EMTimeLockType, CTimeLockInfo> dicTimeLockInfo = new Dictionary<EMTimeLockType, CTimeLockInfo>();

    /// <summary>
    /// 当前正在播放的时停
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
