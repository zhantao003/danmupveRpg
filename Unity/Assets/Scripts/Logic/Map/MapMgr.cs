using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CCreatRange
{
    public Vector2Int vXRange;
    public Vector2Int vYRange;
}

[System.Serializable]
public class CBornPos
{
    [Header("上路出生点")]
    public CCreatRange vUpBornPos;
    [Header("中路出生点")]
    public CCreatRange vCenterBornPos;
    [Header("下路出生点")]
    public CCreatRange vDownBornPos;
    [Header("当前出生点没位置时在该区域找点位生成")]
    public CCreatRange vSpecialBornPos;
}

public enum EMStayPathType
{
    Up,             //上路
    Center,         //中路
    Down,           //下路
}

public class MapMgr : MonoBehaviour
{
    static MapMgr ins = null;
    public static MapMgr Ins
    {
        get
        {
            if (ins == null)
            {
                ins = FindObjectOfType<MapMgr>();
            }

            return ins;
        }
    }

    public int nXCount;
    public int nYCount;
    public GameObject objSlot;
    public Transform tranParent;

    [Header("每行的Layer差值")]
    public int nLayerLerp = 10;
    [Header("红方出生点")]
    public CBornPos vRedBornPos;
    [Header("蓝方出生点")]
    public CBornPos vBlueBornPos;

    public List<MapSlot> listMapSlots = new List<MapSlot>();
    
    public int nBlueCheckX;
    public int nRedCheckX;
    public int nBlueDashCheckX;
    public int nRedDashCheckX;
    public int nBackDefendCheckX;
    [Header("蓝方基地")]
    public CBaseUnit pBlueBase;
    [Header("蓝方兵营")]
    public CBarracksUnit[] pBlueBarracks;
    [Header("蓝方塔")]
    public CTowerUnit[] pBlueTowers;
    [Header("蓝方建筑")]
    public CNormalBuildUnit[] pBlueBuilds;
    [Header("蓝方装饰建筑")]
    public CBuildIdleUnit[] pBlueBuildIdle;
    [Header("红方基地")]
    public CBaseUnit pRedBase;
    [Header("红方兵营")]
    public CBarracksUnit[] pRedBarracks;
    [Header("红方塔")]
    public CTowerUnit[] pRedTowers;
    [Header("红方建筑")]
    public CNormalBuildUnit[] pRedBuilds;
    [Header("红方装饰建筑")]
    public CBuildIdleUnit[] pRedBuildIdle;

    public bool bTest;


    private void Start()
    {
        if(bTest)
        {
            Init();
        }
        objSlot.SetActive(false);
    }

    public void Init()
    {
        CLockStepMgr.Ins.Init();
        CTBLInfo.Inst.Init();
        listMapSlots.Clear();
        AStarFindPath.Ins.vecMapSize = new Vector2Int(nXCount, nYCount);
        for (int x = 0; x < nXCount; x++)
        {
            for (int y = 0; y < nYCount; y++)
            {
                Vector2Int vPos = new Vector2Int(x, y);
                GameObject objNewSlot = GameObject.Instantiate(objSlot);
                objNewSlot.gameObject.name = $"map_{vPos.x}:{vPos.y}";
                Transform tranNewSlot = objNewSlot.GetComponent<Transform>();
                tranNewSlot.SetParent(tranParent);

                //计算地块的坐标
                FixVector3 v64SlotPos = HexMetrics.GetPosFromHexGrid(vPos.x, vPos.y);
                tranNewSlot.localPosition = v64SlotPos.ToVector3();

                MapSlot mapSlot = objNewSlot.GetComponent<MapSlot>();
                mapSlot.v64SlotPos = v64SlotPos;
                mapSlot.vecPos = vPos;
                mapSlot.SetText();
                mapSlot.SetRenderLayer((nYCount - y - 1) * nLayerLerp + x);
                listMapSlots.Add(mapSlot);
                AStarFindPath.Ins.dicMapSlots.Add(vPos, mapSlot);
                objNewSlot.SetActive(true);
            }
        }
        CBattleMgr.Ins.AddNewPlayer(EMUnitCamp.Blue, EMStayPathType.Center, "soldier_small_001blue", "", 0, EMUnitLev.Lv5);
        
        if (pBlueBase == null ||
            pRedBase == null) return;
        //设置红方基地和兵营
        pRedBase.Init(EMUnitCamp.Red);
        pRedBase.InitUniqueIdx();

        for (int i = 0; i < pRedBarracks.Length; i++)
        {
            pRedBarracks[i].Init(EMUnitCamp.Red);
            pRedBarracks[i].InitUniqueIdx();
        }

        for (int i = 0; i < pRedTowers.Length; i++)
        {
            pRedTowers[i].Init(EMUnitCamp.Red);
            pRedTowers[i].InitUniqueIdx();
        }

        for (int i = 0; i < pRedBuilds.Length; i++)
        {
            pRedBuilds[i].Init(EMUnitCamp.Red);
            pRedBuilds[i].InitUniqueIdx();
        }

        for (int i = 0; i < pRedBuildIdle.Length; i++)
        {
            pRedBuildIdle[i].Init(EMUnitCamp.Red);
        }

        //设置蓝方基地和兵营
        pBlueBase.Init(EMUnitCamp.Blue);
        pBlueBase.InitUniqueIdx();

        for (int i = 0;i < pBlueBarracks.Length;i++)
        {
            pBlueBarracks[i].Init(EMUnitCamp.Blue);
            pBlueBarracks[i].InitUniqueIdx();
        }

        for (int i = 0; i < pBlueTowers.Length; i++)
        {
            pBlueTowers[i].Init(EMUnitCamp.Blue);
            pBlueTowers[i].InitUniqueIdx();
        }

        for (int i = 0; i < pBlueBuilds.Length; i++)
        {
            pBlueBuilds[i].Init(EMUnitCamp.Blue);
            pBlueBuilds[i].InitUniqueIdx();
        }

        for(int i=0; i<pBlueBuildIdle.Length; i++)
        {
            pBlueBuildIdle[i].Init(EMUnitCamp.Blue);
        }
    }

    public void RefreshSearch()
    {
        //设置红方基地和兵营
        pRedBase.RefreshSearch();
        pRedBase.SetState(CPlayerUnit.EMState.Idle);
        for (int i = 0; i < pRedTowers.Length; i++)
        {
            pRedTowers[i].RefreshSearch();
            pRedTowers[i].SetState(CPlayerUnit.EMState.Idle);
        }
        //设置蓝方基地和兵营
        pBlueBase.RefreshSearch();
        pBlueBase.SetState(CPlayerUnit.EMState.Idle);
        for (int i = 0; i < pBlueTowers.Length; i++)
        {
            pBlueTowers[i].RefreshSearch();
            pBlueTowers[i].SetState(CPlayerUnit.EMState.Idle);
        }
    }

    public int GetAllBuildHP(EMUnitCamp unitCamp)
    {
        int nTotalHP = 0;

        if(unitCamp == EMUnitCamp.Blue)
        {
            if (!pBlueBase.IsDead())
            {
                nTotalHP += pBlueBase.pUnitData.nCurHP;
            }
            for (int i = 0; i < pBlueBarracks.Length; i++)
            {
                if (!pBlueBarracks[i].IsDead())
                {
                    nTotalHP += pBlueBarracks[i].pUnitData.nCurHP;
                }
            }
            for (int i = 0; i < pBlueTowers.Length; i++)
            {
                if (!pBlueTowers[i].IsDead())
                {
                    nTotalHP += pBlueTowers[i].pUnitData.nCurHP;
                }
            }
        }
        else if(unitCamp == EMUnitCamp.Red)
        {
            if (!pRedBase.IsDead())
            {
                nTotalHP += pRedBase.pUnitData.nCurHP;
            }
            for (int i = 0; i < pRedBarracks.Length; i++)
            {
                if (!pRedBarracks[i].IsDead())
                {
                    nTotalHP += pRedBarracks[i].pUnitData.nCurHP;
                }
            }
            for (int i = 0; i < pRedTowers.Length; i++)
            {
                if (!pRedTowers[i].IsDead())
                {
                    nTotalHP += pRedTowers[i].pUnitData.nCurHP;
                }
            }
        }

        return nTotalHP;
    }

    public void SetBuildHP()
    {
        //设置红方基地和兵营
        pRedBase.SetWorldUI();
        for (int i = 0; i < pRedBarracks.Length; i++)
        {
            pRedBarracks[i].SetWorldUI();
        }
        for (int i = 0; i < pRedTowers.Length; i++)
        {
            pRedTowers[i].SetWorldUI();
        }
        for (int i = 0; i < pRedBuilds.Length; i++)
        {
            pRedBuilds[i].SetWorldUI();
        }
        //设置蓝方基地和兵营
        pBlueBase.SetWorldUI();
        for (int i = 0; i < pBlueBarracks.Length; i++)
        {
            pBlueBarracks[i].SetWorldUI();
        }
        for (int i = 0; i < pBlueTowers.Length; i++)
        {
            pBlueTowers[i].SetWorldUI();
        }
        for (int i = 0; i < pBlueBuilds.Length; i++)
        {
            pBlueBuilds[i].SetWorldUI();
        }
    }

    public MapSlot GetCreatPos(EMUnitCamp camp,EMStayPathType pathType,CPlayerUnit unit)
    {
        MapSlot slot = null;

        CBornPos bornPos = null;
        if(camp == EMUnitCamp.Blue)
        {
            bornPos = vBlueBornPos;
        }
        else if(camp == EMUnitCamp.Red)
        {
            bornPos = vRedBornPos;
        }

        CCreatRange vBornPos = null;
        switch(pathType)
        {
            case EMStayPathType.Up:
                {
                    vBornPos = bornPos.vUpBornPos;
                }
                break;
            case EMStayPathType.Center:
                {
                    vBornPos = bornPos.vCenterBornPos;
                }
                break;
            case EMStayPathType.Down:
                {
                    vBornPos = bornPos.vDownBornPos;
                }
                break;
        }

        bool bGetSlot = false;
        List<MapSlot> mapSlots = new List<MapSlot>();
        for(int x = vBornPos.vXRange.x; x <= vBornPos.vXRange.y;x++)
        {
            if (bGetSlot)
                break;
            for (int y = vBornPos.vYRange.x; y <= vBornPos.vYRange.y; y++)
            {
                MapSlot mapSlot = null;
                AStarFindPath.Ins.GetMapSlot(ref mapSlot,new Vector2Int(x, y));
                if (mapSlot == null) continue;
                if (mapSlot.bCanMove(unit))
                {
                    bGetSlot = true;
                    mapSlots.Add(mapSlot);
                    break;
                }
            }
        }

        if(mapSlots.Count <= 0)
        {
            int nCenterX = (vBornPos.vXRange.x + vBornPos.vXRange.y) / 2;
            int nCenterY = (vBornPos.vYRange.x + vBornPos.vYRange.y) / 2;
            vBornPos = bornPos.vSpecialBornPos;
            int nMinX = bornPos.vSpecialBornPos.vXRange.x - nCenterX;
            int nMaxX = bornPos.vSpecialBornPos.vXRange.y - nCenterX;
            int nMinY = bornPos.vSpecialBornPos.vYRange.x - nCenterY;
            int nMaxY = bornPos.vSpecialBornPos.vYRange.y - nCenterY;
            int nCheckX = Mathf.Abs(nMinX) > Mathf.Abs(nMaxX) ? Mathf.Abs(nMinX) : Mathf.Abs(nMaxX);
            int nCheckY = Mathf.Abs(nMinY) > Mathf.Abs(nMaxY) ? Mathf.Abs(nMinY) : Mathf.Abs(nMaxY);
            //Debug.LogError(nCheckX + "====" + nCheckY + "===" + bornPos.vSpecialBornPos.vXRange + "===" + bornPos.vSpecialBornPos.vYRange + "===" + nCenterX + "====" + nCenterY);
            for (int i = 0;i < nCheckX; i++)
            {
                for (int j = 0; j < nCheckY; j++)
                {
                    if (i <= nMaxX)
                    {
                        if (j <= nMaxY)
                        {
                            MapSlot mapSlot = null;
                            AStarFindPath.Ins.GetMapSlot(ref mapSlot,new Vector2Int(i + nCenterX, j + nCenterY));
                            if (mapSlot != null &&
                                mapSlot.bCanMove(unit))
                            {
                                bGetSlot = true;
                                mapSlots.Add(mapSlot);
                                break;
                            }
                        }
                        if (-j >= nMinY)
                        {
                            MapSlot mapSlot = null;
                            AStarFindPath.Ins.GetMapSlot(ref mapSlot,new Vector2Int(i + nCenterX, -j + nCenterY));
                            if (mapSlot != null &&
                                mapSlot.bCanMove(unit))
                            {
                                bGetSlot = true;
                                mapSlots.Add(mapSlot);
                                break;
                            }
                        }
                    }
                    if (-i >= nMinX)
                    {
                        if (j <= nMaxY)
                        {
                            MapSlot mapSlot = null;
                            AStarFindPath.Ins.GetMapSlot(ref mapSlot,new Vector2Int(-i + nCenterX, j + nCenterY));
                            if (mapSlot != null &&
                                mapSlot.bCanMove(unit))
                            {
                                bGetSlot = true;
                                mapSlots.Add(mapSlot);
                                break;
                            }
                        }
                        if (-j >= nMinY)
                        {
                            MapSlot mapSlot = null;
                            AStarFindPath.Ins.GetMapSlot(ref mapSlot,new Vector2Int(-i + nCenterX, -j + nCenterY));
                            if (mapSlot != null &&
                                mapSlot.bCanMove(unit))
                            {
                                bGetSlot = true;
                                mapSlots.Add(mapSlot);
                                break;
                            }
                        }
                    }
                }
                if (bGetSlot)
                    break;
            }
        }

        if (mapSlots.Count > 0)
        {
            //int nRandomIdx = CLockStepMgr.Ins.GetRandomInt(0, mapSlots.Count);
            slot = mapSlots[0];
        }
        else
        {
        }
        return slot;
    }

    public MapSlot GetOutCanMovePos()
    {
        MapSlot mapSlot = null;



        return mapSlot;
    }

    List<MapSlot> listRandomSlot = new List<MapSlot>();
    public MapSlot GetRandomIdleSlot()
    {
        listRandomSlot.Clear();
        MapSlot slot = null;

        for(int i = 0;i < listMapSlots.Count;i++)
        {
            MapSlot getSlot = listMapSlots[i];
            if (getSlot == null) continue;
            if (getSlot.pStayFlyUnit != null ||
                getSlot.pStayGroundUnit != null)
                continue;
            if (getSlot.emSlotType !=  EMSlotType.Normal)
                continue;
            listRandomSlot.Add(getSlot);
        }

        int nRandomIdx = Random.Range(0, listRandomSlot.Count);

        slot = listRandomSlot[nRandomIdx];

        return slot;
    }

    public bool CheckGoBase(int nX,EMUnitCamp camp)
    {
        bool bGoBase = false;

        if(camp == EMUnitCamp.Blue)
        {
            if(nX <= nBlueCheckX)
            {
                bGoBase = true;
            }
        }
        else if(camp == EMUnitCamp.Red)
        {
            if (nX >= nRedCheckX)
            {
                bGoBase = true;
            }
        }

        return bGoBase;
    }

    public bool CheckDashRange(int nX, EMUnitCamp camp)
    {
        bool bDash = true;

        if (camp == EMUnitCamp.Blue)
        {
            if (nX <= nBlueDashCheckX)
            {
                bDash = false;
            }
        }
        else if (camp == EMUnitCamp.Red)
        {
            if (nX >= nRedDashCheckX)
            {
                bDash = false;
            }
        }

        return bDash;
    }

    /// <summary>
    /// 将地图格子信息转成网络消息
    /// </summary>
    /// <returns></returns>
    public CLocalNetArrayMsg ToNetMsg()
    {
        CLocalNetArrayMsg pMapSlotMsg = new CLocalNetArrayMsg();
        for(int i = 0;i < listMapSlots.Count;i++)
        {
            pMapSlotMsg.AddMsg(listMapSlots[i].ToNetMsg());
        }
        return pMapSlotMsg;
    }



    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.T))
    //    {
    //        ShowTest(EMUnitAnimeDir.Left);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Y))
    //    {
    //        ShowTest(EMUnitAnimeDir.Right);
    //    }
    //    if (Input.GetKeyDown(KeyCode.U))
    //    {
    //        ShowTest(EMUnitAnimeDir.Up);
    //    }
    //    if (Input.GetKeyDown(KeyCode.I))
    //    {
    //        ShowTest(EMUnitAnimeDir.Down);
    //    }
    //    if (Input.GetKeyDown(KeyCode.G))
    //    {
    //        ShowTest(EMUnitAnimeDir.UpR);
    //    }
    //    if (Input.GetKeyDown(KeyCode.H))
    //    {
    //        ShowTest(EMUnitAnimeDir.UpL);
    //    }
    //    if (Input.GetKeyDown(KeyCode.J))
    //    {
    //        ShowTest(EMUnitAnimeDir.DownL);
    //    }
    //    if (Input.GetKeyDown(KeyCode.K))
    //    {
    //        ShowTest(EMUnitAnimeDir.DownR);
    //    }
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        bDoTestA = !bDoTestA;
    //    }
    //}



    //public List<MapSlot> testSlots = new List<MapSlot>();
    //public void ShowTest(EMUnitAnimeDir dir)
    //{
    //    for (int i = 0; i < testSlots.Count; i++)
    //    {
    //        testSlots[i].ActiveRenderColor(false);
    //    }
    //    if (bDoTestA)
    //    {
    //        AStarFindPath.Ins.GetLineSlotsByDir(ref testSlots,8, new Vector2Int(10, 10), dir, true);
    //    }
    //    else
    //    {
    //        AStarFindPath.Ins.GetHengSaoSlotsByDir(ref testSlots,new Vector2Int(10, 10), dir);
    //    }
    //    for (int i = 0; i < testSlots.Count; i++)
    //    {
    //        testSlots[i].ActiveRenderColor(true);
    //    }
    //}


    //public bool bDoTestA;

}
