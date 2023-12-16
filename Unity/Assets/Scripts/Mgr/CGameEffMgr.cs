using BarrageGrab.ProtoEntity;
using DouyuDanmu;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSkillName
{
    public string Skill1;
    public string Skill2;
    public string Skill3;
    public string Skill4;
}

public class CGameEffMgr : CSingleCompOdinBase<CGameEffMgr>
{
    public List<CSkillName> cHeroSkillName;
    //public Dictionary<int, List<Animator>> giftEffsLeftAnimsDic;
    //public Dictionary<int, List<Animator>> giftEffsRightAnimsDic;

    //public List<Animator> towerDestroyLeftAnims;
    //public List<Animator> towerDestroyRightAnims;

    //public List<Animator> campDestroyLeftAnims;
    //public List<Animator> campDestroyRightAnims;

    public List<Animator> heroSkillLeftAnims;
    public List<Animator> heroSkillRightAnims;


    //Coroutine[] leftSendGiftAnimCs;
    //Coroutine[] rightSendGiftAnimCs;

    //Coroutine leftTowerDestroyAnimCs;
    //Coroutine rightTowerDestroyAnimCs;

    //Coroutine leftCampDestroyAnimCs;
    //Coroutine rightCampDestroyAnimCs;

    Coroutine leftNormalShowAnimCs;
    Coroutine rightNormalShowAnimCs;

    Coroutine leftHeroSkillAnimCs;
    Coroutine rightHeroSkillAnimCs;

    private void Awake()
    {
        //leftSendGiftAnimCs = new Coroutine[10];
        //rightSendGiftAnimCs = new Coroutine[10];

    }

    int GiftNameToLv(CDanmuGiftConst giftType)
    {
        //    Soldier_Archer,
        //Soldier_BoxLv1,
        //Soldier_BoxLv2,
        //Hero
        if (giftType == CDanmuGiftConst.Soldier_Archer)
        {
            return 2;
        }
        else if (giftType == CDanmuGiftConst.Soldier_BoxLv1)
        {
            return 3;
        }
        else if (giftType == CDanmuGiftConst.Soldier_BoxLv2)
        {
            return 4;
        }
        else if (giftType == CDanmuGiftConst.Hero)
        {
            return 5;
        }
        return -1;
    }
    #region 送礼特效
    //public void LeftPlayerSendGift(CDanmuGiftConst giftType, int camp)
    //{
    //    int giftLv = GiftNameToLv(giftType);
    //    if (giftLv < 0)
    //        return;
    //    //低级礼物不能顶掉高级礼物动画
    //    for (int i = giftLv + 1; i < 4; i++)
    //    {
    //        if (leftSendGiftAnimCs[i] != null)
    //            return;
    //    }

    //    if (leftSendGiftAnimCs[giftLv] != null)
    //    {
    //        StopCoroutine(leftSendGiftAnimCs[giftLv]);
    //        leftSendGiftAnimCs[giftLv] = null;
    //    }
    //    leftSendGiftAnimCs[giftLv] = StartCoroutine(IEPlayGiftEffLeft(giftLv, camp));

    //}

    //public void RightPlayerSendGift(CDanmuGiftConst giftType, int camp)
    //{
    //    int giftLv = GiftNameToLv(giftType);
    //    if (giftLv < 0)
    //        return;
    //    //低级礼物不能顶掉高级礼物动画
    //    for (int i = giftLv + 1; i < 4; i++)
    //    {
    //        if (rightSendGiftAnimCs[i] != null)
    //            return;

    //    }
    //    if (rightSendGiftAnimCs[giftLv] != null)
    //    {
    //        StopCoroutine(rightSendGiftAnimCs[giftLv]);
    //        rightSendGiftAnimCs[giftLv] = null;
    //    }
    //    rightSendGiftAnimCs[giftLv] = StartCoroutine(IEPlayGiftEffRight(giftLv, camp));

    //}

    //IEnumerator IEPlayGiftEffLeft(int giftType, int camp)
    //{
    //    Animator anim;
    //    anim = giftEffsLeftAnimsDic[giftType][camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    leftSendGiftAnimCs[giftType] = null;
    //}

    //IEnumerator IEPlayGiftEffRight(int giftType, int camp)
    //{
    //    Animator anim;
    //    anim = giftEffsRightAnimsDic[giftType][camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    rightSendGiftAnimCs[giftType] = null;
    //}

    #endregion

    #region 塔被摧毁
    //public void LeftTowerDestoryed(int camp, EMStayPathType path)
    //{
    //    if (leftTowerDestroyAnimCs != null)
    //    {
    //        StopCoroutine(leftTowerDestroyAnimCs);
    //        leftTowerDestroyAnimCs = null;
    //    }
    //    leftTowerDestroyAnimCs = StartCoroutine(IETowerDestroyLeft(camp));
    //}
    //public void RightTowerDestoryed(int camp, EMStayPathType path)
    //{
    //    if (rightTowerDestroyAnimCs != null)
    //    {
    //        StopCoroutine(rightTowerDestroyAnimCs);
    //        rightTowerDestroyAnimCs = null;
    //    }
    //    rightTowerDestroyAnimCs = StartCoroutine(IETowerDestroyRight(camp));
    //}

    //IEnumerator IETowerDestroyLeft(int camp)
    //{
    //    Animator anim;
    //    anim = towerDestroyLeftAnims[camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    leftTowerDestroyAnimCs = null;
    //}
    //IEnumerator IETowerDestroyRight(int camp)
    //{
    //    Animator anim;
    //    anim = towerDestroyRightAnims[camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    rightTowerDestroyAnimCs = null;
    //}
    #endregion
    #region 兵营被摧毁
    //public void LeftCampDestoryed(int camp, EMStayPathType path)
    //{
    //    if (leftCampDestroyAnimCs != null)
    //    {
    //        StopCoroutine(leftCampDestroyAnimCs);
    //        leftCampDestroyAnimCs = null;
    //    }
    //    leftCampDestroyAnimCs = StartCoroutine(IECampDestroyLeft(camp));
    //}
    //public void RightCampDestoryed(int camp, EMStayPathType path)
    //{
    //    if (rightCampDestroyAnimCs != null)
    //    {
    //        StopCoroutine(rightCampDestroyAnimCs);
    //        rightCampDestroyAnimCs = null;
    //    }
    //    rightCampDestroyAnimCs = StartCoroutine(IECampDestroyRight(camp));
    //}

    //IEnumerator IECampDestroyLeft(int camp)
    //{
    //    Animator anim;
    //    anim = campDestroyLeftAnims[camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    leftCampDestroyAnimCs = null;
    //}
    //IEnumerator IECampDestroyRight(int camp)
    //{
    //    Animator anim;
    //    anim = campDestroyRightAnims[camp];
    //    anim.transform.SetAsLastSibling();
    //    anim.gameObject.SetActive(false);
    //    yield return new WaitForEndOfFrame();
    //    anim.gameObject.SetActive(true);
    //    anim.CrossFadeInFixedTime("Eff", 0f);
    //    yield return new WaitForSeconds(5.3f);
    //    anim.gameObject.SetActive(false);

    //    rightCampDestroyAnimCs = null;
    //}
    #endregion

    #region 小兵送礼特写

    public void LeftNormalShow(int camp)
    {
        if (leftNormalShowAnimCs != null)
        {
            StopCoroutine(leftNormalShowAnimCs);
            leftNormalShowAnimCs = null;
        }
        leftNormalShowAnimCs = StartCoroutine(IENormalShowLeft(camp));
    }

    public void RightNormalShow(int camp)
    {
        if (rightNormalShowAnimCs != null)
        {
            StopCoroutine(rightNormalShowAnimCs);
            rightNormalShowAnimCs = null;
        }
        rightNormalShowAnimCs = StartCoroutine(IENormalShowRight(camp));
    }

    IEnumerator IENormalShowLeft(int camp)
    {
        Animator anim;
        anim = heroSkillLeftAnims[camp];
        anim.transform.GetComponent<UISkillTexts>().SetSkillName(cHeroSkillName[camp]);
        anim.transform.SetAsLastSibling();
        anim.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        anim.gameObject.SetActive(true);
        anim.CrossFadeInFixedTime("Eff", 0f);
        yield return new WaitForSeconds(2f);
        //CTimeMgr.fTimeScale = 1f;
        yield return new WaitForSeconds(3.3f);
        anim.gameObject.SetActive(false);

        leftHeroSkillAnimCs = null;
    }
    IEnumerator IENormalShowRight(int camp)
    {
        Animator anim;
        anim = heroSkillRightAnims[camp];
        anim.transform.GetComponent<UISkillTexts>().SetSkillName(cHeroSkillName[camp]);
        anim.transform.SetAsLastSibling();
        anim.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        anim.gameObject.SetActive(true);
        anim.CrossFadeInFixedTime("Eff", 0f);
        yield return new WaitForSeconds(2f);
        //CTimeMgr.fTimeScale = 1f;
        yield return new WaitForSeconds(3.3f);
        anim.gameObject.SetActive(false);
        rightHeroSkillAnimCs = null;
    }

    #endregion

    #region 英雄技能特效
    //float leftSkillGap = 12f;
    //float leftSkillCount = 12f;
    public void LeftHeroSkill(int camp)
    {
        //if (leftSkillCount < leftSkillGap)
        //    return;
        //switch(camp)
        //{
        //    case 0:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill1);
        //        }
        //        break;
        //    case 1:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill2);
        //        }
        //        break;
        //    case 2:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill3);
        //        }
        //        break;
        //}
        
        //CTimeMgr.fTimeScale = 1f / 10f;
        if (leftHeroSkillAnimCs != null)
        {
            StopCoroutine(leftHeroSkillAnimCs);
            leftHeroSkillAnimCs = null;
        }
        leftHeroSkillAnimCs = StartCoroutine(IEHeroSkillLeft(camp));
        //leftSkillCount = 0;
    }
    //float rightSkillGap = 12f;
    //float rightSkillCount = 12f;
    public void RightHeroSkill(int camp)
    {
        //if (rightSkillCount < rightSkillGap)
        //    return;
        //switch (camp)
        //{
        //    case 0:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill1);
        //        }
        //        break;
        //    case 1:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill2);
        //        }
        //        break;
        //    case 2:
        //        {
        //            CTimeLockMgr.Ins.PlayTimeLock(EMTimeLockType.PlayHeroSkill3);
        //        }
        //        break;
        //}
        //CTimeMgr.fTimeScale = 1f / 10f;
        if (rightHeroSkillAnimCs != null)
        {
            StopCoroutine(rightHeroSkillAnimCs);
            rightHeroSkillAnimCs = null;
        }
        rightHeroSkillAnimCs = StartCoroutine(IEHeroSkillRight(camp));
        //rightSkillCount = 0;
    }

    IEnumerator IEHeroSkillLeft(int camp)
    {
        Animator anim;
        anim = heroSkillLeftAnims[camp];
        anim.transform.GetComponent<UISkillTexts>().SetSkillName(cHeroSkillName[camp]);
        anim.transform.SetAsLastSibling();
        anim.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        anim.gameObject.SetActive(true);
        anim.CrossFadeInFixedTime("Eff", 0f);
        yield return new WaitForSeconds(2f);
        //CTimeMgr.fTimeScale = 1f;
        yield return new WaitForSeconds(3.3f);
        anim.gameObject.SetActive(false);

        leftHeroSkillAnimCs = null;
    }
    IEnumerator IEHeroSkillRight(int camp)
    {
        Animator anim;
        anim = heroSkillRightAnims[camp];
        anim.transform.GetComponent<UISkillTexts>().SetSkillName(cHeroSkillName[camp]);
        anim.transform.SetAsLastSibling();
        anim.gameObject.SetActive(false);
        yield return new WaitForEndOfFrame();
        anim.gameObject.SetActive(true);
        anim.CrossFadeInFixedTime("Eff", 0f);
        yield return new WaitForSeconds(2f);
        //CTimeMgr.fTimeScale = 1f;
        yield return new WaitForSeconds(3.3f);
        anim.gameObject.SetActive(false);
        rightHeroSkillAnimCs = null;
    }
    #endregion

    public static float GetAnimatorLength(Animator animator, string name)
    {
        //动画片段时间长度
        float length = 0;
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name.Equals(name))
            {
                length = clip.length;
                break;
            }
        }
        return length;
    }

    //private void Update()
    //{
    //    //leftSkillCount += CTimeMgr.DeltaTime;
    //    //rightSkillCount += CTimeMgr.DeltaTime;

    //}
}
