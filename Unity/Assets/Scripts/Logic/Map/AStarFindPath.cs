using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 转向
/// </summary>
public enum EMRotateType
{
    Left,           //左边
    Right,          //右边
}

/// <summary>
/// 寻敌类型 
/// </summary>
public enum EMSearchType
{
    Single = 0,             //单体
    All = 1,                //全体
    RandomCount = 2,        //随机多个
    Around = 3,             //自身范围内X格
    Near = 4,               //最近距离
    Far = 5,                //最远距离
    ProLow = 6,             //X属性百分比最低 
    TargetBuff = 7,         //携带指定buff
    TargetBuffSubType = 8,  //携带类型buff

    Self = 100,             //找自己
    SelfNear = 101,         //找自己和离得最近的
}

/// <summary>
/// 技能范围类型
/// </summary>
public enum EMSkillRangeType
{
    NormalAtk = 0,          //普通攻击范围
    Around = 1,             //周边范围
    Forward = 2,            //前方直线范围
}

public class AStarFindPath : CSingleMgrBase<AStarFindPath>
{
    public Dictionary<Vector2Int, MapSlot> dicMapSlots = new Dictionary<Vector2Int, MapSlot>();
    [Header("地图尺寸")]
    public Vector2Int vecMapSize;

    /// <summary>
    /// 计算周围格子用的点
    /// </summary>
    Vector2Int[] vecCheckAroundPoints;

    public Dictionary<EMUnitAnimeDir, Vector2Int[]> dicHeroHenSaoPointsByDir;

    public Vector2Int[] vecHeroAtkPoints;

    void CheckAroundPoints()
    {
        if (vecCheckAroundPoints == null ||
            vecCheckAroundPoints.Length <= 0)
        {
            vecCheckAroundPoints = new Vector2Int[]
            {
                new Vector2Int(-1, 1),      //左上
                new Vector2Int(0, 1),       //右上
                new Vector2Int(-1, -1),     //左下
                new Vector2Int(0, -1),      //右下
                new Vector2Int(-1, 0),      //左
                new Vector2Int(1, 0),       //右
            };
        }

        if(dicHeroHenSaoPointsByDir == null)
        {
            dicHeroHenSaoPointsByDir = new Dictionary<EMUnitAnimeDir, Vector2Int[]>();
            Vector2Int[] vIntPointUpL = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, -1),
                new Vector2Int(1, 0),
            };
            Vector2Int[] vIntPointUpR = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(0, -1),
                new Vector2Int(-1, 0),
            };
            Vector2Int[] vIntPointDownL = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(-1, 1),
                new Vector2Int(1, 0),
            };
            Vector2Int[] vIntPointDownR = new Vector2Int[]
            {
               new Vector2Int(0, 0),
               new Vector2Int(0, 1),
               new Vector2Int(-1, 0),
            };
            Vector2Int[] vIntPointLeft = new Vector2Int[]
            {
               new Vector2Int(0,0),
               new Vector2Int(0, -1),
               new Vector2Int(0, 1),
            };
            Vector2Int[] vIntPointRight = new Vector2Int[]
            {
               new Vector2Int(0,0),
               new Vector2Int(-1, -1),
               new Vector2Int(-1, 1),
            };
            Vector2Int[] vIntPointUp = new Vector2Int[]
            {
                new Vector2Int(0, 0),
                new Vector2Int(1, 0),
               new Vector2Int(-1, 0),
            };
            Vector2Int[] vIntPointDown = new Vector2Int[]
            {
                 new Vector2Int(0, 0),
               new Vector2Int(1, 0),
               new Vector2Int(-1, 0),
            };
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.UpL, vIntPointUpL);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.UpR, vIntPointUpR);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.DownL, vIntPointDownL);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.DownR, vIntPointDownR);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.Left, vIntPointLeft);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.Right, vIntPointRight);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.Up, vIntPointUp);
            dicHeroHenSaoPointsByDir.Add(EMUnitAnimeDir.Down, vIntPointDown);
            ///判断点为奇数时，y=1时，x + 1
        }
    }

    /// <summary>
    /// 获取一个格子面对另一个格子的方向
    /// </summary>
    /// <param name="pStart"></param>
    /// <param name="pTarget"></param>
    /// <returns></returns>
    public void GetNextMoveDir(ref EMUnitAnimeDir emUnitAnimeDir, MapSlot pStart,MapSlot pTarget)
    {
        emUnitAnimeDir = EMUnitAnimeDir.Down;
        Vector2Int vLerp = new Vector2Int(pTarget.vecPos.x - pStart.vecPos.x, pTarget.vecPos.y - pStart.vecPos.y);
        //判断奇偶行
        int nDoubleRawStart = pStart.vecPos.y % 2;
        int nDoubleRawTarget = pTarget.vecPos.y % 2;
        //获取对应的方向
        if(Mathf.Abs(vLerp.y) <= 0)
        {
            if(vLerp.x > 0)
            {
                emUnitAnimeDir = EMUnitAnimeDir.Right;
            }
            else if(vLerp.x < 0)
            {
                emUnitAnimeDir = EMUnitAnimeDir.Left;
            }
        }
        else if(vLerp.y > 0)
        {
            if (vLerp.x > 0)
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpR;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpR;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpR;
                }
            }
            else if (vLerp.x < 0)
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpL;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpL;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpL;
                }
            }
            else
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.Up;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpL;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.UpR;
                }
            }
        }
        else if(vLerp.y < 0)
        {
            if (vLerp.x > 0)
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownR;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownR;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownR;
                }
            }
            else if (vLerp.x < 0)
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownL;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownL;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownL;
                }
            }
            else
            {
                if (nDoubleRawStart == nDoubleRawTarget)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.Down;
                }
                else if (nDoubleRawStart == 1 &&
                         nDoubleRawTarget == 0)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownL;
                }
                else if (nDoubleRawStart == 0 &&
                         nDoubleRawTarget == 1)
                {
                    emUnitAnimeDir = EMUnitAnimeDir.DownR;
                }
            }
        }
        //return emUnitAnimeDir;
    }

    /// <summary>
    /// 获取周围的格子 
    /// </summary>
    /// <param name="unit"></param>
    /// <param name="nRange"></param>
    /// <param name="vBaseInt"></param>
    /// <param name="listBlockSlot"></param>
    /// <returns></returns>
    public void GetAroundSlot(ref List<MapSlot> slots,int nRange, Vector2Int vBaseInt,List<MapSlot> listBlockSlot = null)
    {
        slots = new List<MapSlot>();
        int nXMaxLerp = 0;
        int nXMinLerp = 0;
        MapSlot UpSlot = null;
        MapSlot DownSlot = null;
        for (int y = 0; y <= nRange; y++)
        {
            if (y == 0)
            {
                
            }
            else
            {
                if ((vBaseInt.y + y) % 2 == 0)
                {
                    nXMinLerp += 1;
                }
                else if ((vBaseInt.y + y) % 2 == 1)
                {
                    nXMaxLerp -= 1;
                }
            }
            for (int x = -nRange + nXMinLerp; x <= nRange + nXMaxLerp; x++)
            {
                if (y == 0)
                {
                    GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                }
                else
                {
                    GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                    GetMapSlot(ref DownSlot, new Vector2Int(x + vBaseInt.x, -y + vBaseInt.y));
                }
                if (UpSlot != null)
                {
                    if (listBlockSlot == null)
                    {
                        slots.Add(UpSlot);
                    }
                    else
                    {
                        if (listBlockSlot.Contains(UpSlot))
                        {
                            
                        }
                        else
                        {
                            slots.Add(UpSlot);
                        }
                    }
                }
                if (DownSlot != null)
                {
                    if (listBlockSlot == null)
                    {
                        slots.Add(DownSlot);
                    }
                    else
                    {
                        if (listBlockSlot.Contains(DownSlot))
                        {

                        }
                        else
                        {
                            slots.Add(DownSlot);
                        }
                    }
                }
            }
        }
        //return slots;
    }

    /// <summary>
    /// 获取最外围一圈的格子
    /// </summary>
    /// <param name="nRange"></param>
    /// <param name="vBaseInt"></param>
    /// <returns></returns>
    public void GetOutAroundSlot(int nRange, Vector2Int vBaseInt,ref List<MapSlot> slots)
    {
        slots = new List<MapSlot>();
        if (nRange <= 0) return;
        int nXMaxLerp = 0;
        int nXMinLerp = 0;
        int nX = 0;
        MapSlot UpSlot = null;
        MapSlot DownSlot = null;
        for (int y = 0; y <= nRange; y++)
        {
            if (y == 0)
            {

            }
            else
            {
                if ((vBaseInt.y + y) % 2 == 0)
                {
                    nXMinLerp += 1;
                }
                else if ((vBaseInt.y + y) % 2 == 1)
                {
                    nXMaxLerp -= 1;
                }
            }

            if (y < nRange)
            {
                for (int x = 0; x < 2; x++)
                {
                    //MapSlot UpSlot = null;
                    //MapSlot DownSlot = null;
                    if (x == 0)
                    {
                        nX = -nRange + nXMinLerp;
                    }
                    else if (x == 1)
                    {
                        nX = nRange + nXMaxLerp;
                    }
                    if (y == 0)
                    {
                        GetMapSlot(ref UpSlot,new Vector2Int(nX + vBaseInt.x, y + vBaseInt.y));
                    }
                    else
                    {
                        GetMapSlot(ref UpSlot,new Vector2Int(nX + vBaseInt.x, y + vBaseInt.y));
                        GetMapSlot(ref DownSlot,new Vector2Int(nX + vBaseInt.x, -y + vBaseInt.y));
                    }
                    if (UpSlot != null)
                    {
                        slots.Add(UpSlot);
                    }
                    if (DownSlot != null)
                    {
                        slots.Add(DownSlot);
                    }
                }
            }
            else
            {
                for (int x = -nRange + nXMinLerp; x <= nRange + nXMaxLerp; x++)
                {
                    if (y == 0)
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                    }
                    else
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                        GetMapSlot(ref DownSlot, new Vector2Int(x + vBaseInt.x, -y + vBaseInt.y));
                    }
                    if (UpSlot != null)
                    {
                        slots.Add(UpSlot);
                    }
                    if (DownSlot != null)
                    {
                        slots.Add(DownSlot);
                    }
                }
            }
        }
        //return slots;
    }

    public void GetOutAroundTarget(int nRange, Vector2Int vBaseInt, EMUnitCamp emTargetCamp, CPlayerUnit checkUnit, ref CPlayerUnit target)
    {
        if (nRange <= 0) return;
        int nXMaxLerp = 0;
        int nXMinLerp = 0;
        int nX = 0;
        MapSlot UpSlot = null;
        MapSlot DownSlot = null;
        CPlayerUnit colUnit = null;
        for (int y = 0; y <= nRange; y++)
        {
            if (y == 0)
            {

            }
            else
            {
                if ((vBaseInt.y + y) % 2 == 0)
                {
                    nXMinLerp += 1;
                }
                else if ((vBaseInt.y + y) % 2 == 1)
                {
                    nXMaxLerp -= 1;
                }
            }

            if (y < nRange)
            {
                for (int x = 0; x < 2; x++)
                {
                    //MapSlot UpSlot = null;
                    //MapSlot DownSlot = null;
                    if (x == 0)
                    {
                        nX = -nRange + nXMinLerp;
                    }
                    else if (x == 1)
                    {
                        nX = nRange + nXMaxLerp;
                    }
                    if (y == 0)
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(nX + vBaseInt.x, y + vBaseInt.y));
                    }
                    else
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(nX + vBaseInt.x, y + vBaseInt.y));
                        GetMapSlot(ref DownSlot, new Vector2Int(nX + vBaseInt.x, -y + vBaseInt.y));
                    }
                    if (UpSlot != null)
                    {
                        colUnit = UpSlot.pStayGroundUnit;
                        if (colUnit == null)
                        {
                            if (checkUnit.pUnitData.bCanAtkFly)
                            {
                                colUnit = UpSlot.pStayFlyUnit;
                            }
                        }
                        if (colUnit != null &&
                            !colUnit.IsDead() &&
                            colUnit != checkUnit &&
                            colUnit.emCamp == emTargetCamp)
                        {
                            target = colUnit;
                            return;
                        }
                    }
                    if (DownSlot != null)
                    {
                        colUnit = DownSlot.pStayGroundUnit;
                        if (colUnit == null)
                        {
                            if (checkUnit.pUnitData.bCanAtkFly)
                            {
                                colUnit = DownSlot.pStayFlyUnit;
                            }
                        }
                        if (colUnit != null &&
                            !colUnit.IsDead() &&
                            colUnit != checkUnit &&
                            colUnit.emCamp == emTargetCamp)
                        {
                            target = colUnit;
                            return;
                        }
                    }
                }
            }
            else
            {
                for (int x = -nRange + nXMinLerp; x <= nRange + nXMaxLerp; x++)
                {
                    if (y == 0)
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                    }
                    else
                    {
                        GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                        GetMapSlot(ref DownSlot, new Vector2Int(x + vBaseInt.x, -y + vBaseInt.y));
                    }
                    if (UpSlot != null)
                    {
                        colUnit = UpSlot.pStayGroundUnit;
                        if (colUnit == null)
                        {
                            if (checkUnit.pUnitData.bCanAtkFly)
                            {
                                colUnit = UpSlot.pStayFlyUnit;
                            }
                        }
                        if (colUnit != null &&
                            !colUnit.IsDead() &&
                            colUnit != checkUnit &&
                            colUnit.emCamp == emTargetCamp)
                        {
                            target = colUnit;
                            return;
                        }
                    }
                    if (DownSlot != null)
                    {
                        colUnit = DownSlot.pStayGroundUnit;
                        if (colUnit == null)
                        {
                            if (checkUnit.pUnitData.bCanAtkFly)
                            {
                                colUnit = DownSlot.pStayFlyUnit;
                            }
                        }
                        if (colUnit != null &&
                            !colUnit.IsDead() &&
                            colUnit != checkUnit &&
                            colUnit.emCamp == emTargetCamp)
                        {
                            target = colUnit;
                            return;
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取对应横扫范围的格子
    /// </summary>
    /// <returns></returns>
    public void GetHengSaoSlotsByDir(ref List<MapSlot> slots, Vector2Int vBaseInt, EMUnitAnimeDir dir)
    {
        CheckAroundPoints();
        slots = new List<MapSlot>();
        int isDoubleRaw = 0;
        int x = 0;
        int y = 0;
        for(int i = 0;i < dicHeroHenSaoPointsByDir[dir].Length;i++)
        {
            isDoubleRaw = 0;
            if (Mathf.Abs(dicHeroHenSaoPointsByDir[dir][i].y) == 1)
            {
                isDoubleRaw = vBaseInt.y % 2;
            }
            x = vBaseInt.x + dicHeroHenSaoPointsByDir[dir][i].x + isDoubleRaw;
            y = vBaseInt.y + dicHeroHenSaoPointsByDir[dir][i].y;
            MapSlot mapSlot = null;
            GetMapSlot(ref mapSlot,new Vector2Int(x, y));
            if (mapSlot != null)
            {
                slots.Add(mapSlot);
            }
        }
        //return slots;
    }

    /// <summary>
    /// 获取该格子对应方向的直线距离的格子
    /// </summary>
    /// <param name="nRange"></param>
    /// <param name="vBaseInt"></param>
    /// <param name="dir"></param>
    /// <returns></returns>
    public void GetLineSlotsByDir(ref List<MapSlot> slots, int nRange, Vector2Int vBaseInt, EMUnitAnimeDir dir,bool bGetBackSlot = false)
    {
        slots.Clear();

        int nAddValueX = 0;
        int nAddValueY = 0;
        int x = 0;
        int y = 0;
        int nBaseDoubleRaw = vBaseInt.y % 2;
        int isDoubleRaw = 0;
        if (bGetBackSlot && nRange >= 1)
        {
            for (int i = 1; i <= nRange; i++)
            {
                switch (dir)
                {
                    case EMUnitAnimeDir.DownR:
                        {
                            nAddValueY += 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 1)
                            {
                                nAddValueX -= 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.DownL:
                        {
                            nAddValueY += 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 0)
                            {
                                nAddValueX += 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.UpR:
                        {
                            nAddValueY -= 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 1)
                            {
                                nAddValueX -= 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.UpL:
                        {
                            nAddValueY -= 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 0)
                            {
                                nAddValueX += 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.Right:
                        {
                            nAddValueY = 0;
                            nAddValueX -= 1;
                        }
                        break;
                    case EMUnitAnimeDir.Left:
                        {
                            nAddValueY = 0;
                            nAddValueX += 1;
                        }
                        break;
                    case EMUnitAnimeDir.Down:
                        {
                            nAddValueY += 1;
                            nAddValueX = 0;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                        }
                        break;
                    case EMUnitAnimeDir.Up:
                        {
                            nAddValueY -= 1;
                            nAddValueX = 0;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                        }
                        break;
                }
                x = vBaseInt.x + nAddValueX;
                y = vBaseInt.y + nAddValueY;

                if (i > 0 &&
                    (dir == EMUnitAnimeDir.Up ||
                    dir == EMUnitAnimeDir.Down))
                {
                    MapSlot mapSlot = null;
                    GetMapSlot(ref mapSlot,new Vector2Int(x, y));
                    if (mapSlot != null)
                    {
                        slots.Add(mapSlot);
                    }
                    if (nBaseDoubleRaw != isDoubleRaw)
                    {
                        MapSlot mapSlot2 = null;
                        if (nBaseDoubleRaw == 0)
                        {
                            GetMapSlot(ref mapSlot2,new Vector2Int(x - 1, y));
                            if (mapSlot2 != null)
                            {
                                slots.Add(mapSlot2);
                            }
                        }
                        else
                        {
                            GetMapSlot(ref mapSlot2,new Vector2Int(x + 1, y));
                            if (mapSlot2 != null)
                            {
                                slots.Add(mapSlot2);
                            }
                        }
                    }
                }
                else
                {
                    MapSlot mapSlot = null;
                    GetMapSlot(ref mapSlot,new Vector2Int(x, y));
                    if (mapSlot != null)
                    {
                        slots.Add(mapSlot);
                    }
                }
            }
        }
        nAddValueX = 0;
        nAddValueY = 0;
        for (int i = 0;i <= nRange;i++)
        {
            if (i == 0)
            {
                x = vBaseInt.x;
                y = vBaseInt.y;
            }
            else
            {
                switch (dir)
                {
                    case EMUnitAnimeDir.UpL:
                        {
                            nAddValueY += 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 1)
                            {
                                nAddValueX -= 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.UpR:
                        {
                            nAddValueY += 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 0)
                            {
                                nAddValueX += 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.DownL:
                        {
                            nAddValueY -= 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 1)
                            {
                                nAddValueX -= 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.DownR:
                        {
                            nAddValueY -= 1;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                            if (isDoubleRaw == 0)
                            {
                                nAddValueX += 1;
                            }
                        }
                        break;
                    case EMUnitAnimeDir.Left:
                        {
                            nAddValueY = 0;
                            nAddValueX -= 1;
                        }
                        break;
                    case EMUnitAnimeDir.Right:
                        {
                            nAddValueY = 0;
                            nAddValueX += 1;
                        }
                        break;
                    case EMUnitAnimeDir.Up:
                        {
                            nAddValueY += 1;
                            nAddValueX = 0;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                        }
                        break;
                    case EMUnitAnimeDir.Down:
                        {
                            nAddValueY -= 1;
                            nAddValueX = 0;
                            isDoubleRaw = (vBaseInt.y + nAddValueY) % 2;
                        }
                        break;
                }
                x = vBaseInt.x + nAddValueX;
                y = vBaseInt.y + nAddValueY;
            }
            if (i > 0 &&
                (dir == EMUnitAnimeDir.Up ||
                dir == EMUnitAnimeDir.Down))
            {
                MapSlot mapSlot = null;
                GetMapSlot(ref mapSlot,new Vector2Int(x, y));
                if (mapSlot != null)
                {
                    slots.Add(mapSlot);
                }
                if (nBaseDoubleRaw != isDoubleRaw)
                {
                    MapSlot mapSlot2 = null;
                    if (nBaseDoubleRaw == 0)
                    {
                        GetMapSlot(ref mapSlot2,new Vector2Int(x - 1, y));
                        if (mapSlot2 != null)
                        {
                            slots.Add(mapSlot2);
                        }
                    }
                    else
                    {
                        GetMapSlot(ref mapSlot2,new Vector2Int(x + 1, y));
                        if (mapSlot2 != null)
                        {
                            slots.Add(mapSlot2);
                        }
                    }
                }
            }
            else
            {
                MapSlot mapSlot = null;
                GetMapSlot(ref mapSlot,new Vector2Int(x, y));
                if (mapSlot != null)
                {
                    slots.Add(mapSlot);
                }
            }
        }

        //return slots;
    }

    /// <summary>
    /// 获取对应方向半圆范围的格子
    /// </summary>
    /// <returns></returns>
    public void GetHalfSphereSlotsByDir(ref List<MapSlot> slots,int nRange, Vector2Int vBaseInt, EMUnitAnimeDir dir)
    {
        slots = new List<MapSlot>();
        int nAddUpSlotX = 0;
        int nAddDownSlotX = 0;
        int nXMax = 0;
        int nXMin = 0;
        int nXMaxLerp = 0;
        int nXMinLerp = 0;
        int nDoubleValue = 0;
        MapSlot UpSlot = null;
        MapSlot DownSlot = null;
        for (int y = 0; y <= nRange; y++)
        {
            nDoubleValue = (vBaseInt.y + y) % 2;
            if (y == 0)
            {

            }
            else
            {
                if (nDoubleValue == 0)
                {
                    nXMinLerp += 1;
                }
                else if (nDoubleValue == 1)
                {
                    nXMaxLerp -= 1;
                }
            }
            if (dir == EMUnitAnimeDir.Up ||
                dir == EMUnitAnimeDir.Down ||
                dir == EMUnitAnimeDir.Left ||
                dir == EMUnitAnimeDir.Right)
            {
                if(dir == EMUnitAnimeDir.Left)
                {
                    if (vBaseInt.y % 2 == 1)
                    {
                        nXMax = 0;
                    }
                    else
                    {
                        nXMax = nDoubleValue == 0 ? 0 : -1;
                    }
                }
                else
                {
                    nXMax = nRange + nXMaxLerp;
                }
                if (dir == EMUnitAnimeDir.Right)
                {
                    if (vBaseInt.y % 2 == 1)
                    {
                        nXMin = nDoubleValue == 0 ? 1 : 0;
                    }
                    else
                    {
                        nXMin = 0;
                    }
                }
                else
                {
                    nXMin = -nRange + nXMinLerp;
                }
                for (int x = nXMin; x <= nXMax; x++)
                {
                    if (y == 0)
                    {
                        AStarFindPath.Ins.GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                    }
                    else
                    {
                        if (dir == EMUnitAnimeDir.Up)
                        {
                            AStarFindPath.Ins.GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                        }
                        else if (dir == EMUnitAnimeDir.Down)
                        {
                            AStarFindPath.Ins.GetMapSlot(ref DownSlot, new Vector2Int(x + vBaseInt.x, -y + vBaseInt.y));
                        }
                        else
                        {
                            AStarFindPath.Ins.GetMapSlot(ref UpSlot, new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                            AStarFindPath.Ins.GetMapSlot(ref DownSlot, new Vector2Int(x + vBaseInt.x, -y + vBaseInt.y));
                        }
                    }
                    if (UpSlot != null)
                    {
                        slots.Add(UpSlot);
                    }
                    if (DownSlot != null)
                    {
                        slots.Add(DownSlot);
                    }
                }
            }
            else if(dir == EMUnitAnimeDir.UpL ||
                dir == EMUnitAnimeDir.UpR ||
                dir == EMUnitAnimeDir.DownL ||
                dir == EMUnitAnimeDir.DownR)
            {
                if (dir == EMUnitAnimeDir.UpL ||
                       dir == EMUnitAnimeDir.DownL)
                {
                    if (vBaseInt.y % 2 == 1)
                    {
                        nXMax = 0;
                    }
                    else
                    {
                        nXMax = nDoubleValue == 0 ? 0 : -1;
                    }
                }
                else
                {
                    nXMax = nRange + nXMaxLerp;
                }
                if (dir == EMUnitAnimeDir.UpR ||
                    dir == EMUnitAnimeDir.DownR)
                {
                    if (vBaseInt.y % 2 == 1)
                    {
                        nXMin = nDoubleValue == 0 ? 1 : 0;
                    }
                    else
                    {
                        nXMin = 0;
                    }
                }
                else
                {
                    nXMin = -nRange + nXMinLerp;
                }
                if (y > 0)
                {
                    if (nDoubleValue == 0)
                    {
                        if ((vBaseInt.y + y) % 2 == 0)
                        {

                        }
                        else
                        {
                            if (dir == EMUnitAnimeDir.UpR)
                            {
                                nAddUpSlotX -= 1;
                            }
                        }
                        if ((vBaseInt.y - y) % 2 == 0)
                        {

                        }
                        else
                        {
                            if (dir == EMUnitAnimeDir.UpR)
                            {
                                nAddDownSlotX += 1;
                            }
                        }
                    }
                    else
                    {
                        if ((y + vBaseInt.y) % 2 == 0)
                        {

                        }
                        else
                        {
                            if (dir == EMUnitAnimeDir.UpR)
                            {
                                nAddUpSlotX -= 1;
                            }
                        }
                        if ((-y + vBaseInt.y) % 2 == 0)
                        {

                        }
                        else
                        {
                            if (dir == EMUnitAnimeDir.UpR)
                            {
                                nAddDownSlotX += 1;
                            }
                        }
                    }
                }
                if (y == 0)
                {
                    for (int x = nXMin; x <= nXMax; x++)
                    {
                        AStarFindPath.Ins.GetMapSlot(ref UpSlot,new Vector2Int(x + vBaseInt.x, y + vBaseInt.y));
                        if (UpSlot != null)
                        {
                            slots.Add(UpSlot);
                        }
                    }
                }
                else
                {
                    switch (dir)
                    {
                        case EMUnitAnimeDir.UpL:
                            {

                            }
                            break;
                        case EMUnitAnimeDir.UpR:
                            {

                            }
                            break;
                        case EMUnitAnimeDir.DownL:
                            {

                            }
                            break;
                        case EMUnitAnimeDir.DownR:
                            {

                            }
                            break;
                    }

                    for (int x = nXMin; x <= nXMax; x++)
                    {
                        AStarFindPath.Ins.GetMapSlot(ref UpSlot,new Vector2Int(x + vBaseInt.x + nAddUpSlotX, y + vBaseInt.y));
                        AStarFindPath.Ins.GetMapSlot(ref DownSlot,new Vector2Int(x + vBaseInt.x + nAddDownSlotX, -y + vBaseInt.y));
                        if (UpSlot != null)
                        {
                            slots.Add(UpSlot);
                        }
                        if (DownSlot != null)
                        {
                            slots.Add(DownSlot);
                        }
                    }
                }
            }
        }
        //return slots;
    }

    /// <summary>
    /// 获取对应坐标的格子
    /// </summary>
    /// <param name="vecPos"></param>
    /// <returns></returns>
    public void GetMapSlot(ref MapSlot mapSlot, Vector2Int vecPos)
    {
        mapSlot = null;
        dicMapSlots.TryGetValue(vecPos, out mapSlot);

        //return mapSlot;
    }

    /// <summary>
    /// 获取横向移动目标
    /// </summary>
    /// <param name="start"></param>
    /// <param name="unit"></param>
    /// <returns></returns>
    public void pGetNextMovePosByDir(ref MapSlot slot,ref EMUnitAnimeDir preDir, MapSlot start, CPlayerUnit unit, EMUnitAnimeDir dir, CPlayerUnit pTarget = null)
    {
        slot = null;
        MapSlot mapSlot = null;
        CheckAroundPoints();
        int nTargetRange = pTarget == null ? 0 : (int)pTarget.emStayRange;
        
        int nIdx = 0;
        int x = 0;
        int y = 0;
        int isDoubleRaw = 0;
        bool bTouchBuild = false;
        Vector2Int vAdd = Vector2Int.zero;
        int nStayRange = (int)unit.emStayRange + 1;
        if (unit.pMoveTarget != null &&
            nStayRange > 0)
        {
            GetNextPointIdxByMoveDir(dir, 0, ref nIdx);
            vAdd = vecCheckAroundPoints[nIdx];
            if (Mathf.Abs(vAdd.y) >= 1)
            {
                isDoubleRaw = start.vecPos.y % 2;
            }
            x = start.vecPos.x + vAdd.x + isDoubleRaw;
            y = start.vecPos.y + vAdd.y;
            GetMapSlot(ref mapSlot, new Vector2Int(x, y));
            if (unit.emStayRange > CPlayerUnit.EMStayRange.Normal)
            {
                x += vAdd.x;
                y += vAdd.y;
                GetMapSlot(ref mapSlot, new Vector2Int(x, y));
                if (mapSlot != null &&
                    mapSlot.pStayGroundUnit != null &&
                    mapSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                {
                    bTouchBuild = true;
                    nTargetRange += (int)mapSlot.pStayGroundUnit.emStayRange;
                }
            }
            Vector2Int vLerp = new Vector2Int(unit.pMoveTarget.vecPos.x - start.vecPos.x, unit.pMoveTarget.vecPos.y - start.vecPos.y);
            //判断奇偶行
            int nDoubleRawStart = start.vecPos.y % 2;
            int nDoubleRawTarget = unit.pMoveTarget.vecPos.y % 2;
            //获取对应的方向
            if (Mathf.Abs(vLerp.y) <= nStayRange + nTargetRange)
            {
                if (vLerp.x > 0)
                {
                    dir = EMUnitAnimeDir.Right;
                }
                else if (vLerp.x < 0)
                {
                    dir = EMUnitAnimeDir.Left;
                }
            }
            //获取对应的方向
            else if (Mathf.Abs(vLerp.x) <= nStayRange + nTargetRange)
            {
                if (vLerp.y > 0)
                {
                    dir = EMUnitAnimeDir.Up;
                }
                else if (vLerp.y < 0)
                {
                    dir = EMUnitAnimeDir.Down;
                }
            }
        }
        
        for (int i = 0; i < 5; i++)
        {
            GetNextPointIdxByMoveDir(dir, i,ref nIdx);
            if (nIdx >= vecCheckAroundPoints.Length) continue;
            isDoubleRaw = 0;
            x = 0;
            y = 0;
            vAdd = vecCheckAroundPoints[nIdx];
            
            if (!bTouchBuild &&
                i == 1 &&
                preDir != EMUnitAnimeDir.Max)
            {
                x = 0;
                y = 0;
                vAdd = vecCheckAroundPoints[(int)preDir];

                if (Mathf.Abs(vAdd.y) >= 1)
                {
                    isDoubleRaw = start.vecPos.y % 2;
                }
                x = (int)start.vecPos.x + vAdd.x + isDoubleRaw;
                y = (int)start.vecPos.y + vAdd.y;
                GetMapSlot(ref mapSlot, new Vector2Int(x, y));
                if (mapSlot != null &&
                    mapSlot.bCanMove(unit))
                {
                    preDir = (EMUnitAnimeDir)nIdx;
                    slot = mapSlot;
                    break;
                }
            }
            if (Mathf.Abs(vAdd.y) >= 1)
            {
                isDoubleRaw = start.vecPos.y % 2;
            }
            x = start.vecPos.x + vAdd.x + isDoubleRaw;
            y = start.vecPos.y + vAdd.y;
            GetMapSlot(ref mapSlot,new Vector2Int(x, y));

            if (mapSlot != null &&
                mapSlot.bCanMove(unit))
            {
                preDir = (EMUnitAnimeDir)nIdx;
                slot = mapSlot;
                //if (i == 0 &&
                //    mapSlot.pStayGroundUnit.emUnitType == CPlayerUnit.EMUnitType.Build &&
                //    pTarget != null)
                //{
                //}
                break;
            }
            else
            {
                //if (i == 0)
                //{
                //    if (unit.emStayRange > CPlayerUnit.EMStayRange.Normal)
                //    {
                //        x += vAdd.x;
                //        y += vAdd.y;
                //        GetMapSlot(ref mapSlot, new Vector2Int(x, y));
                //        if (mapSlot != null &&
                //            mapSlot.pStayGroundUnit != null &&
                //            mapSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                //        {
                //            Debug.LogError(preDir + "===CheckDir===" + dir);
                //            bTouchBuild = true;
                //        }
                //    }
                //    else
                //    {
                //        if (mapSlot != null &&
                //            mapSlot.pStayGroundUnit != null &&
                //            mapSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                //        {
                //            bTouchBuild = true;
                //        }
                //    }
                //}
                
            }
            
        }

        //return slot;
    }


    public void GetNextPointIdxByMoveDir(EMUnitAnimeDir dir,int nCurNum,ref int nIdx)
    {
        nIdx = 0;
        switch (dir)
        {
            case EMUnitAnimeDir.UpL:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 4;           //左
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 2;           //左下
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 5;           //右
                    }
                }
                break;
            case EMUnitAnimeDir.UpR:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 5;           //右
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 3;           //右下
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 4;           //左
                    }
                }
                break;
            case EMUnitAnimeDir.DownL:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 2;           //左下
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 4;           //左
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 3;           //右下
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 5;           //右
                    }
                }
                break;
            case EMUnitAnimeDir.DownR:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 3;           //右下
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 5;           //右
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 2;           //左下
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 4;           //左
                    }
                }
                break;
            case EMUnitAnimeDir.Left:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 4;           //左
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 2;           //左下
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 3;           //右下
                    }
                }
                break;
            case EMUnitAnimeDir.Right:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 5;           //右
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 3;           //右下
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 2;           //左下
                    }
                }
                break;
            case EMUnitAnimeDir.Up:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 1;           //右上
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 0;           //左上
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 4;           //左
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 5;           //右
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 99;
                    }
                }
                break;
            case EMUnitAnimeDir.Down:
                {
                    if (nCurNum == 0)
                    {
                        nIdx = 2;           //左下
                    }
                    else if (nCurNum == 1)
                    {
                        nIdx = 3;           //右下
                    }
                    else if (nCurNum == 2)
                    {
                        nIdx = 4;           //左
                    }
                    else if (nCurNum == 3)
                    {
                        nIdx = 5;           //右
                    }
                    else if (nCurNum == 4)
                    {
                        nIdx = 99;
                    }
                }
                break;
        }
        //return nIdx;
    }
    
    public void Clear()
    {
        dicMapSlots.Clear();
    }

    #region 根据范围类型获取格子

    /// <summary>
    /// 获取范围内所有的格子
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="nRange"></param>
    /// <param name="emCamp"></param>
    /// <param name="emSkillRangeType"></param>
    /// <returns></returns>
    public void GetMapSlotByRange(ref List<MapSlot> listSlot, Vector2Int vecPos, int nRange, EMSkillRangeType emSkillRangeType,CPlayerUnit pCheckUnit = null, Vector2Int vecDir = (default))
    {
        listSlot.Clear();
        switch (emSkillRangeType)
        {
            case EMSkillRangeType.NormalAtk:
                GetAroundRangeMapSlot(vecPos, nRange, ref listSlot);
                break;
            case EMSkillRangeType.Around:               //周边范围
                GetAroundRangeMapSlot(vecPos, nRange, ref listSlot);
                break;
            case EMSkillRangeType.Forward:              //前方范围
                GetForwarRangeMapSlot(vecPos, nRange, vecDir,ref listSlot);
                break;
        }
        if(pCheckUnit != null)
        {
            for(int i = 0;i < listSlot.Count;)
            {
                if(pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Ground &&
                   listSlot[i].pStayGroundUnit != null &&
                   listSlot[i].pStayGroundUnit.szSelfUid != pCheckUnit.szSelfUid)
                {
                    listSlot.RemoveAt(i);
                }
                else if (pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Fly &&
                        listSlot[i].pStayFlyUnit != null &&
                        listSlot[i].pStayFlyUnit.szSelfUid != pCheckUnit.szSelfUid)
                {
                    listSlot.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
        //return listSlot;
    }

    /// <summary>
    /// 判断周围格子是否有被占领的
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="nRange"></param>
    /// <param name="pCheckUnit"></param>
    /// <returns></returns>
    public bool bGetAllAroundSlot(Vector2Int vecPos, int nRange, CPlayerUnit pCheckUnit)
    {
        bool bGetAllSlot = true;

        int nBaseX = vecPos.x;
        int nBaseY = vecPos.y;
        int nCheckRange = nRange;
        int nXMaxLerp = 0;
        int nXMinLerp = 0;
        int nCurY = 0;
        MapSlot UpSlot = null;
        MapSlot DownSlot = null;
        for (int y = 0; y <= nCheckRange; y++)
        {
            nCurY = nBaseY + y;
            if (y == 0)
            {

            }
            else
            {
                if (nCurY % 2 == 0)
                {
                    nXMinLerp += 1;
                }
                else if (nCurY % 2 == 1)
                {
                    nXMaxLerp -= 1;
                }
            }
            for (int x = -nCheckRange + nXMinLerp; x <= nCheckRange + nXMaxLerp; x++)
            {
                if (y == 0)
                {
                    AStarFindPath.Ins.GetMapSlot(ref UpSlot,new Vector2Int(x + nBaseX, y + nBaseY));
                }
                else
                {
                    AStarFindPath.Ins.GetMapSlot(ref UpSlot,new Vector2Int(x + nBaseX, y + nBaseY));
                    AStarFindPath.Ins.GetMapSlot(ref DownSlot,new Vector2Int(x + nBaseX, -y + nBaseY));
                }
                if (UpSlot != null)
                {
                    if (pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Ground &&
                        UpSlot.pStayGroundUnit != null &&
                        UpSlot.pStayGroundUnit.szSelfUid != pCheckUnit.szSelfUid)
                    {
                        bGetAllSlot = false;
                    }
                    else if (pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Fly)
                    {
                        if(UpSlot.pStayGroundUnit != null &&
                           UpSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                        {
                            bGetAllSlot = false;
                        }
                        if(UpSlot.pStayFlyUnit != null &&
                           UpSlot.pStayFlyUnit.szSelfUid != pCheckUnit.szSelfUid)
                        {
                            bGetAllSlot = false;
                        }
                    }
                }
                if (DownSlot != null)
                {
                    if (pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Ground &&
                        DownSlot.pStayGroundUnit != null &&
                        DownSlot.pStayGroundUnit.szSelfUid != pCheckUnit.szSelfUid)
                    {
                        bGetAllSlot = false;
                    }
                    else if (pCheckUnit.emMoveType == CPlayerUnit.EMMoveType.Fly)
                    {
                        if (DownSlot.pStayGroundUnit != null &&
                            DownSlot.pStayGroundUnit.emUnitType != CPlayerUnit.EMUnitType.Unit)
                        {
                            bGetAllSlot = false;
                        }
                        if(DownSlot.pStayFlyUnit != null &&
                           DownSlot.pStayFlyUnit.szSelfUid != pCheckUnit.szSelfUid)
                        {
                            bGetAllSlot = false;
                        }
                    }
                }
                if(!bGetAllSlot)
                    return bGetAllSlot;
            }
        }
        return bGetAllSlot;
    }

    /// <summary>
    /// 获取周边范围的所有格子
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="nRange"></param>
    /// <param name="emCamp"></param>
    /// <returns></returns>
    void GetAroundRangeMapSlot(Vector2Int vecPos, int nRange,ref List<MapSlot> slots)
    {
        List<MapSlot> listBlockSlot = new List<MapSlot>();
        MapSlot mapSlot = null;
        GetMapSlot(ref mapSlot,vecPos);
        listBlockSlot.Add(mapSlot);
        GetAroundSlot(ref slots, nRange, vecPos, listBlockSlot);
    }

    /// <summary>
    /// 获取前方范围的所有格子
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="nRange"></param>
    /// <param name="emCamp"></param>
    /// <param name="vecDir"></param>
    /// <returns></returns>
    void GetForwarRangeMapSlot(Vector2Int vecPos, int nRange, Vector2Int vecDir,ref List<MapSlot> listSlot)
    {
        listSlot = new List<MapSlot>();

        for (int i = 1; i <= nRange; i++)
        {
            int x = (int)vecPos.x + i * vecDir.x;
            int y = (int)vecPos.y + i * vecDir.y;
            if (x < vecMapSize.x && x >= 0 && y < vecMapSize.y && y >= 0)
            {
                listSlot.Add(dicMapSlots[new Vector2Int(x, y)]);
            }
        }

    }

    #endregion

    #region A星寻路

    int nMaxFindCount = 200;

    /// <summary>
    /// 获取路径长度
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public int nGetPathLength(CPlayerUnit moveUnit,MapSlot start, MapSlot end, List<MapSlot> listCheckPathRange = null)
    {
        int nLength = -1;
        bool finded = false;

        List<MapSlot> ShortestPath = new List<MapSlot>();
        List<MapSlot> OpenedNodes = new List<MapSlot>();
        List<MapSlot> ClosedNodes = new List<MapSlot>();
        List<MapSlot> mapSlots = new List<MapSlot>();
        ///重置起点信息
        MapSlot Current = start;
        Current.parent = null;
        Current.gCost = 0;
        Current.hCost = 0;
        OpenedNodes.Add(Current);
        MapSlot target = null;
        MapSlot item = null;
        if (start.vecPos != end.vecPos)
        {
            for (int j = 0; j < nMaxFindCount; j++)
            //while (OpenedNodes.Count > 0 && !finded)
            {
                target = null;
                if (OpenedNodes.Count > 0)
                {
                    target = OpenedNodes[0];
                    for (int i = 1; i < OpenedNodes.Count; i++)
                    {
                        if (OpenedNodes[i].fCost < target.fCost)
                        {
                            target = OpenedNodes[i];
                        }
                    }
                }
                else
                {
                    break;
                }
                Current = target;
                OpenedNodes.Remove(Current);
                ClosedNodes.Add(Current);
                //获取周围的点
                mapSlots = GetNeibourhoodByMan(Current);
                //不在closed列表中的，不在opened列表中的 ， 可以通行的，  添加到opened列表中
                for (int i = 0; i < mapSlots.Count; i++)
                {
                    if (listCheckPathRange != null &&
                        !listCheckPathRange.Contains(mapSlots[i])) continue;
                    item = mapSlots[i];
                    if (item != null &&
                        !ClosedNodes.Contains(item) &&
                        item.bCanMove(moveUnit))
                    {
                        int G = Current.gCost + 1;
                        int H = Mathf.Abs(item.vecPos.x - end.vecPos.x) +
                                Mathf.Abs(item.vecPos.y - end.vecPos.y);

                        if (OpenedNodes.Contains(item))//这个节点已经在打开列表中了  计算权重保留更优的
                        {
                            if (item.fCost > G + H ||
                                (item.fCost == G + H &&
                                 item.hCost > H))//新的节点更优 更新节点
                            {
                                item.parent = Current;
                                item.gCost = G;
                                item.hCost = H;
                            }
                            else//老节点更优  不动
                            {

                            }
                        }
                        else
                        {
                            //将节点加入到打开列表
                            item.parent = Current;
                            item.gCost = G;
                            item.hCost = H;
                            OpenedNodes.Add(item);
                            if (item.vecPos == end.vecPos)
                            {
                                finded = true;
                                break;//目的节点
                            }
                        }
                    }
                }
            }
        }
        if (finded)
        {
            Current = OpenedNodes[OpenedNodes.Count - 1];
            do
            {
                ShortestPath.Insert(0, Current);
                Current = Current.parent;
            }
            while (Current != null &&
            Current.parent != null);
            nLength = ShortestPath.Count;
        }
        else
        {
            nLength = -1;
        }
        //Debug.LogError("GetPath Length ===" + nLength + "====" + start.vecPos + "===" + end.vecPos);
        return nLength;
    }

    /// <summary>
    /// A星寻路获取路径
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public List<MapSlot> ComputePath(CPlayerUnit moveUnit, MapSlot start, MapSlot end, List<MapSlot> listCheckPathRange = null)
    {
        bool finded = false;

        List<MapSlot> ShortestPath = new List<MapSlot>();
        List<MapSlot> OpenedNodes = new List<MapSlot>();
        List<MapSlot> ClosedNodes = new List<MapSlot>();
        List<MapSlot> mapSlots = new List<MapSlot>();
        ///重置起点信息
        MapSlot Current = start;
        MapSlot pBestSlot = null;
        Current.parent = null;
        Current.gCost = 0;
        Current.hCost = 0;
        OpenedNodes.Add(Current);
        bool bOverFindFunc = false;     //判断是否结束寻路事件
        MapSlot target = null;
        MapSlot item = null;
        if (start.vecPos != end.vecPos)
        {
            for (int j = 0; j < nMaxFindCount; j++)
            //while (OpenedNodes.Count > 0 && !finded)
            {
                target = null;
                if (OpenedNodes.Count > 0)
                {
                    target = OpenedNodes[0];
                    for (int i = 1; i < OpenedNodes.Count; i++)
                    {
                        if (OpenedNodes[i].fCost < target.fCost)
                        {
                            target = OpenedNodes[i];
                        }
                    }
                }
                else
                {
                    break;
                }
                //Debug.LogError(target.vecPos + "==Target==" + target.fCost);
                Current = target;
                OpenedNodes.Remove(Current);
                ClosedNodes.Add(Current);
                //获取周围的点
                mapSlots = GetNeibourhoodByMan(Current);
                //不在closed列表中的，不在opened列表中的 ， 可以通行的，  添加到opened列表中
                for (int i = 0; i < mapSlots.Count; i++)
                {
                    if (listCheckPathRange != null &&
                        !listCheckPathRange.Contains(mapSlots[i])) continue;
                    item = mapSlots[i];
                    if (item != null &&
                        !ClosedNodes.Contains(item) &&
                        item.bCanMove(moveUnit))
                    {
                        int G = Current.gCost + 1;
                        int H = Mathf.Abs(item.vecPos.x - end.vecPos.x) +
                                Mathf.Abs(item.vecPos.y - end.vecPos.y);

                        if (OpenedNodes.Contains(item))//这个节点已经在打开列表中了  计算权重保留更优的
                        {
                            if (item.fCost > G + H ||
                                (item.fCost == G + H &&
                                 item.hCost > H))//新的节点更优 更新节点
                            {
                                item.parent = Current;
                                item.gCost = G;
                                item.hCost = H;
                                if (pBestSlot == null)
                                {
                                    pBestSlot = item;
                                }
                                else if (item.vecPos != start.vecPos &&
                                        item.fCost > 0 &&
                                        (pBestSlot.fCost > item.fCost) ||
                                        (pBestSlot.fCost == item.fCost &&
                                         pBestSlot.hCost > item.hCost))
                                {
                                    pBestSlot = item;
                                }
                            }
                            else//老节点更优  不动
                            {

                            }
                        }
                        else
                        {
                            //将节点加入到打开列表
                            item.parent = Current;
                            item.gCost = G;
                            item.hCost = H;
                            OpenedNodes.Add(item);
                            if (item.vecPos == end.vecPos)
                            {
                                finded = true;
                                bOverFindFunc = true;
                                break;//目的节点
                            }
                        }
                        if (pBestSlot == null)
                        {
                            pBestSlot = item;
                        }
                        else if (item.vecPos != start.vecPos &&
                                item.fCost > 0 &&
                                (pBestSlot.fCost > item.fCost) ||
                                (pBestSlot.fCost == item.fCost &&
                                 pBestSlot.hCost > item.hCost))
                        {
                            pBestSlot = item;
                        }
                    }
                }
                if (bOverFindFunc)
                    break;
            }
        }
        if (finded)
        {
            Current = OpenedNodes[OpenedNodes.Count - 1];
            do
            {
                ShortestPath.Insert(0, Current);
                Current = Current.parent;
            }
            while (Current != null &&
            Current.parent != null);
        }
        else if (pBestSlot != null)
        {
            Current = pBestSlot;
            do
            {
                ShortestPath.Insert(0, Current);
                Current = Current.parent;
            }
            while (Current != null &&
            Current.parent != null);
        }
        //Debug.LogError(start.vecPos + "===GetPath ====" + end.vecPos + "====" + ShortestPath.Count + "====" + finded + "====" + ShortestPath[ShortestPath.Count - 1].vecPos + "===="+ ShortestPath[0].vecPos);
        return ShortestPath;
    }

    /// <summary>
    /// A星寻路获取路径
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    public MapSlot GetNextSlotByComputePath(CPlayerUnit moveUnit, MapSlot start, MapSlot end, List<MapSlot> listCheckPathRange = null)
    {
        bool finded = false;
        MapSlot pNextSlot = null;
        List<MapSlot> ShortestPath = new List<MapSlot>();
        List<MapSlot> OpenedNodes = new List<MapSlot>();
        List<MapSlot> ClosedNodes = new List<MapSlot>();
        List<MapSlot> mapSlots = new List<MapSlot>();
        ///重置起点信息
        MapSlot Current = start;
        MapSlot pBestSlot = null;
        Current.parent = null;
        Current.gCost = 0;
        Current.hCost = 0;
        OpenedNodes.Add(Current);
        bool bOverFindFunc = false;     //判断是否结束寻路事件
        MapSlot target = null;
        MapSlot item = null;
        if (start.vecPos != end.vecPos)
        {
            for (int j = 0; j < nMaxFindCount; j++)
            //while (OpenedNodes.Count > 0 && !finded)
            {
                target = null;
                if (OpenedNodes.Count > 0)
                {
                    target = OpenedNodes[0];
                    for (int i = 1; i < OpenedNodes.Count; i++)
                    {
                        if (OpenedNodes[i].fCost < target.fCost)
                        {
                            target = OpenedNodes[i];
                        }
                    }
                }
                else
                {
                    break;
                }
                //Debug.LogError(target.vecPos + "==Target==" + target.fCost);
                Current = target;
                OpenedNodes.Remove(Current);
                ClosedNodes.Add(Current);
                //获取周围的点
                mapSlots = GetNeibourhoodByMan(Current);
                //不在closed列表中的，不在opened列表中的 ， 可以通行的，  添加到opened列表中
                for (int i = 0; i < mapSlots.Count; i++)
                {
                    if (listCheckPathRange != null &&
                        !listCheckPathRange.Contains(mapSlots[i])) continue;
                    item = mapSlots[i];
                    if (item != null &&
                        !ClosedNodes.Contains(item) &&
                        item.bCanMove(moveUnit))
                    {
                        int G = Current.gCost + 1;
                        int H = Mathf.Abs(item.vecPos.x - end.vecPos.x) +
                                Mathf.Abs(item.vecPos.y - end.vecPos.y);

                        if (OpenedNodes.Contains(item))//这个节点已经在打开列表中了  计算权重保留更优的
                        {
                            if (item.fCost > G + H ||
                                (item.fCost == G + H &&
                                 item.hCost > H))//新的节点更优 更新节点
                            {
                                item.parent = Current;
                                item.gCost = G;
                                item.hCost = H;
                                if (pBestSlot == null)
                                {
                                    pBestSlot = item;
                                }
                                else if (item.vecPos != start.vecPos &&
                                        item.fCost > 0 &&
                                        (pBestSlot.fCost > item.fCost) ||
                                        (pBestSlot.fCost == item.fCost &&
                                         pBestSlot.hCost > item.hCost))
                                {
                                    pBestSlot = item;
                                }
                            }
                            else//老节点更优  不动
                            {

                            }
                        }
                        else
                        {
                            //将节点加入到打开列表
                            item.parent = Current;
                            item.gCost = G;
                            item.hCost = H;
                            OpenedNodes.Add(item);
                            if (item.vecPos == end.vecPos)
                            {
                                finded = true;
                                bOverFindFunc = true;
                                break;//目的节点
                            }
                        }
                        if (pBestSlot == null)
                        {
                            pBestSlot = item;
                        }
                        else if (item.vecPos != start.vecPos &&
                                item.fCost > 0 &&
                                (pBestSlot.fCost > item.fCost) ||
                                (pBestSlot.fCost == item.fCost &&
                                 pBestSlot.hCost > item.hCost))
                        {
                            pBestSlot = item;
                        }
                    }
                }
                if (bOverFindFunc)
                    break;
            }
        }
        if (finded)
        {
            Current = OpenedNodes[OpenedNodes.Count - 1];
            do
            {
                ShortestPath.Insert(0, Current);
                Current = Current.parent;
            }
            while (Current != null &&
            Current.parent != null);
        }
        else if (pBestSlot != null)
        {
            Current = pBestSlot;
            do
            {
                ShortestPath.Insert(0, Current);
                Current = Current.parent;
            }
            while (Current != null &&
            Current.parent != null);
        }
        if (ShortestPath.Count > 0)
        {
            pNextSlot = ShortestPath[0];
        }
        //Debug.LogError(start.vecPos + "===GetPath ====" + end.vecPos + "====" + ShortestPath.Count + "====" + finded + "====" + ShortestPath[ShortestPath.Count - 1].vecPos + "===="+ ShortestPath[0].vecPos);
        return pNextSlot;
    }

    /// <summary>
    /// 获取相邻的格子
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private List<MapSlot> GetNeibourhoodByMan(MapSlot node)
    {
        CheckAroundPoints();
        List<MapSlot> list = new List<MapSlot>();
        int isDoubleRaw = node.vecPos.y % 2;
        for (int i = 0; i < vecCheckAroundPoints.Length; i++)
        {
            int x = (int)node.vecPos.x + vecCheckAroundPoints[i].x;
            if (i <= 3)
            {
                x += isDoubleRaw;
            }
            int y = (int)node.vecPos.y + vecCheckAroundPoints[i].y;
            if (x < vecMapSize.x && x >= 0 && y < vecMapSize.y && y >= 0)
            {
                list.Add(dicMapSlots[new Vector2Int(x, y)]);
            }
        }
        return list;
    }

    #endregion

}
