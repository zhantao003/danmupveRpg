using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class ParaBolaInfo
{
    public List<Vector3> listVecPos;   ///移动移动路径
    public float fLineDis;             ///抛物线长度
}

public class CHelpTools
{
    public const string szMapSlotName = "MapSlot_";
    //获取当前时间戳
    //TimeZoneInfo.cur
    protected static DateTime pOldTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
    
    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.Now - pOldTime;
        return (long)ts.TotalMilliseconds;
    }

    /// <summary>
    /// 计算字符串在指定text控件中的长度
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static int CalculateLengthOfText(string message, Text tex)
    {
        int totalLength = 0;
        Font myFont = tex.font;  //chatText is my Text component
        myFont.RequestCharactersInTexture(message, tex.fontSize, tex.fontStyle);
        CharacterInfo characterInfo = new CharacterInfo();
        char[] arr = message.ToCharArray();
        foreach (char c in arr)
        {
            myFont.GetCharacterInfo(c, out characterInfo, tex.fontSize);
            totalLength += characterInfo.advance;
        }
        return totalLength;
    }

    /// <summary>
    /// 切割字符串转整型数组
    /// </summary>
    /// <param name="szContent"></param>
    /// <param name="szSplit"></param>
    /// <returns></returns>
    public static int[] IntCutByString(string szContent, char[] szSplit)
    {
        int[] nRes;

        if (IsStringEmptyOrNone(szContent))
        {
            nRes = null;
        }
        else
        {
            string[] szEles = szContent.Split(szSplit, System.StringSplitOptions.RemoveEmptyEntries);
            nRes = new int[szEles.Length];
            for (int idx = 0; idx < szEles.Length; idx++)
            {
                int nEle = 0;
                if (int.TryParse(szEles[idx], out nEle))
                {
                    nRes[idx] = nEle;
                }
                else
                {
                    nRes[idx] = 0;
                    Debug.LogError("Error Content:" + szEles[idx]);
                }
            }
        }

        return nRes;
    }

    /// <summary>
    /// 切割字符串转浮点型数组
    /// </summary>
    /// <param name="szContent"></param>
    /// <param name="szSplit"></param>
    /// <returns></returns>
    public static float[] FloatCutByString(string szContent, char[] szSplit)
    {
        float[] nRes;

        if (IsStringEmptyOrNone(szContent))
        {
            nRes = null;
        }
        else
        {
            string[] szEles = szContent.Split(szSplit, System.StringSplitOptions.RemoveEmptyEntries);
            nRes = new float[szEles.Length];
            for (int idx = 0; idx < szEles.Length; idx++)
            {
                float nEle = float.Parse(szEles[idx]);
                nRes[idx] = nEle;
            }
        }

        return nRes;
    }

    public static string[] StringCutByString(string szContent, char[] szSplit)
    {
        string[] szRes;

        if (IsStringEmptyOrNone(szContent))
        {
            szRes = null;
        }
        else
        {
            string[] szEles = szContent.Split(szSplit, System.StringSplitOptions.RemoveEmptyEntries);
            szRes = new string[szEles.Length];
            for (int idx = 0; idx < szEles.Length; idx++)
            {
                szRes[idx] = szEles[idx];
            }
        }

        return szRes;
    }

    /// <summary>
    /// 获取所有指定被包裹的字符串的数组
    /// </summary>
    /// <param name="content"></param>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static string[] GetAllStringInCenter(string content, char left, char right)
    {
        List<string> arrRes = new List<string>();
        int nLeftIdx = -1;
        int nRightIdx = -1;

        nLeftIdx = content.IndexOf(left, 0);
        if (nLeftIdx < 0) return null;

        nRightIdx = content.IndexOf(right, nLeftIdx);
        if (nRightIdx < 0) return null;

        string szSlot = content.Substring(nLeftIdx + 1, nRightIdx - nLeftIdx - 1);
        arrRes.Add(szSlot);

        while (nLeftIdx >=0 && nRightIdx >= 0)
        {
            nLeftIdx = content.IndexOf(left, nRightIdx);
            if (nLeftIdx < 0) break;

            nRightIdx = content.IndexOf(right, nLeftIdx);
            if (nRightIdx < 0) break;

            string newSzSlot = content.Substring(nLeftIdx + 1, nRightIdx - nLeftIdx - 1);
            arrRes.Add(newSzSlot);
        }

        return arrRes.ToArray();
    }

    /// <summary>
    /// 化简字符串长度
    /// </summary>
    /// <param name="targetStr"></param>
    /// <param name="targetLength">目标长度，英文字符==1，中文字符==2</param>
    /// <returns></returns>
    public static string AbbrevStringWithinLength(string targetStr, int targetLength, string abbrevPostfix)
    {
        //C#实际统计：一个中文字符长度==1，英文字符长度==1
        //UI显示要求：一个中文字符长度==2，英文字符长度==1

        //校验参数
        if (string.IsNullOrEmpty(targetStr) || targetLength <= 0)
            return targetStr;
        //字符串长度 * 2 <= 目标长度，即使是全中文也在长度范围内
        if (targetStr.Length * 2 <= targetLength)
            return targetStr;
        //遍历字符
        char[] chars = targetStr.ToCharArray();
        int curLen = 0;
        for (int i = 0; i < chars.Length; i++)
        {
            //累加字符串长度
            if (chars[i] >= 0x4e00 && chars[i] <= 0x9fbb)
                curLen += 2;
            else
                curLen += 1;

            //如果当前位置累计长度超过目标长度，取0~i-1，即Substring(0,i)
            if (curLen > targetLength)
                return targetStr.Substring(0, i) + abbrevPostfix;
        }
        return targetStr;
    }

    ///<summary>
    ///生成随机字符串 
    ///</summary>
    ///<param name="length">目标字符串的长度</param>
    ///<param name="useNum">是否包含数字，1=包含，默认为包含</param>
    ///<param name="useLow">是否包含小写字母，1=包含，默认为包含</param>
    ///<param name="useUpp">是否包含大写字母，1=包含，默认为包含</param>
    ///<param name="useSpe">是否包含特殊字符，1=包含，默认为不包含</param>
    ///<param name="custom">要包含的自定义字符，直接输入要包含的字符列表</param>
    ///<returns>指定长度的随机字符串</returns>
    public static string GetRandomString(int length, bool useNum, bool useLow, bool useUpp, bool useSpe, string custom)
    {
        byte[] b = new byte[4];
        new System.Security.Cryptography.RNGCryptoServiceProvider().GetBytes(b);
        System.Random r = new System.Random(BitConverter.ToInt32(b, 0));
        string s = null, str = custom;
        if (useNum == true) { str += "0123456789"; }
        if (useLow == true) { str += "abcdefghijklmnopqrstuvwxyz"; }
        if (useUpp == true) { str += "ABCDEFGHIJKLMNOPQRSTUVWXYZ"; }
        if (useSpe == true) { str += "!\"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~"; }
        for (int i = 0; i < length; i++)
        {
            s += str.Substring(r.Next(0, str.Length - 1), 1);
        }
        return s;
    }

    /// <summary>  
    /// 根据GUID获取19位的唯一数字序列  
    /// </summary>  
    /// <returns></returns>  
    public static long GuidToLongID()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0);
    }

    /// <summary>
    /// 字符串是否空或者none
    /// </summary>
    /// <returns></returns>
    public static bool IsStringEmptyOrNone(string content)
    {
        return string.IsNullOrEmpty(content) || 
               content.ToLower().Equals("none") ||
               content.ToLower().Equals("null");
    }

    /// <summary>
    /// 递归查找子节点
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="goName"></param>
    /// <returns></returns>
    public static Transform FindChild(Transform trans, string goName)
    {
        Transform child = trans.Find(goName);
        if (child != null)
            return child;

        Transform go = null;
        for (int i = 0; i < trans.childCount; i++)
        {
            child = trans.GetChild(i);
            go = FindChild(child, goName);
            if (go != null)
                return go;
        }
        return null;
    }

    /// <summary>
    /// 设置节点及其所有子节点的层级
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="layer"></param>
    public static void SetLayer(Transform parent, int layer)
    {
        if (parent.childCount > 0)//如果子物体存在
        {
            for (int i = 0; i < parent.childCount; i++)//遍历子物体是否还有子物体
            {
                SetLayer(parent.GetChild(i), layer);//这里是只将最后一个无子物体的对象设置层级
            }
            parent.gameObject.layer = layer;//将存在的子物体遍历结束后需要把当前子物体节点进行层级设置
        }
        else					//无子物体
        {
            parent.gameObject.layer = layer;
        }
    }

    //获得向量绕轴旋转后的向量
    public static Vector3 GetDirRonateTarget(Vector3 vDir, Vector3 vAxis, float fAngle)
    {
        Vector3 vRes;
        vRes = Quaternion.AngleAxis(fAngle, vAxis) * vDir;
        return vRes;
    }

    //获得两个向量的夹角
    public static float GetAngleBetweenVecs(Vector3 from, Vector3 to)
    {
        float angle = Mathf.Acos(Vector3.Dot(from.normalized, to.normalized)) * Mathf.Rad2Deg;
        return angle;
    }

    //获取计时器的显示格式00:00
    public static string GetTimeCounter(int sec)
    {
        string szTime = "00:00";
        int nMin = sec / 60;
        int nSec = sec % 60;

        szTime = ((nMin < 10) ? ("0" + nMin) : nMin.ToString()) + ":" +
                 ((nSec < 10) ? ("0" + nSec) : nSec.ToString());

        return szTime;
    }

    /// <summary>
    /// 时间转化为13位时间戳
    /// </summary>
    /// <param name="_time"></param>
    /// <returns></returns>
    public static long ConvertDateTimeToUtc(DateTime _time)
    {
        DateTime time = TimeZoneInfo.ConvertTimeToUtc(new DateTime(1970, 1, 1, 0, 0, 0, 0));
        long t = (_time.Ticks - time.Ticks) / 10000;
        return t;
    }

    /// <summary>
    /// 将时间戳转化为时间
    /// </summary>
    /// <param name="nlTime"></param>
    /// <returns></returns>
    public static DateTime GetDateTime(long nlTime)
    {
        DateTime startTime = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), TimeZoneInfo.Local);
        DateTime dt = startTime.AddSeconds(nlTime * 0.001F);

        
        return dt;
    }

    /// <summary>
    /// 判断是否为同一天
    /// </summary>
    /// <param name="nlTime"></param>
    /// <returns></returns>
    public static bool bCheckLerpCurByDay(long nlTime)
    {
        bool bCheck = false;

        DateTime curTime = DateTime.Now;
        DateTime checkTime = GetDateTime(nlTime);

        bCheck = curTime.Date.Equals(checkTime.Date);

        return bCheck;
    }

    /// <summary>
    /// 获取该时间与当前时间相差的时间（分钟）
    /// </summary>
    /// <returns></returns>
    public static int GetTimeLerpByMinute(long nlTime,int nMaxMinute = 0)
    {
        TimeSpan timeSpan = new TimeSpan();
        int nMinute = 0;
        DateTime curTime = DateTime.Now;
        DateTime checkTime = GetDateTime(nlTime);
        timeSpan = curTime - checkTime;

        double dGetMinute = timeSpan.TotalMinutes;
        if (nMaxMinute > 0)
        {
            if(dGetMinute > nMaxMinute)
            {
                nMinute = nMaxMinute;
            }
            else
            {
                nMinute = (int)dGetMinute;
            }
        }
        else
        {
            nMinute = (int)dGetMinute;
        }

        return nMinute;
    }

    public static long GetCurTime()
    {
        return CTimeMgr.NowMillonsSec();
    }

    public static long GenerateId()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0);
    }

    public static bool IsIntInList(int target, int[] arrCheck)
    {
        for(int i=0; i<arrCheck.Length; i++)
        {
            if (target == arrCheck[i]) return true;
        }

        return false;
    }

    /// <summary>
    /// 获取抛物线信息
    /// </summary>
    /// <param name="vecNormal"></param>
    /// <param name="vecStartPos"></param>
    /// <returns></returns>
    public static ParaBolaInfo GetParaBolaPos(Vector3 vecStartPos, Vector3 vecTargetPos,Vector3 vecCenterPos)
    {
        ParaBolaInfo paraBolaInfo = new ParaBolaInfo();
        paraBolaInfo.listVecPos = new List<Vector3>();
        paraBolaInfo.fLineDis = 0;
        Vector3 StartPos = vecStartPos;
        Vector3 EndPos = vecTargetPos;
        //EndPos.y = 0;  //设置目标高度为地板
        Vector3 CenterPos = vecCenterPos;
        Vector3 newPos = vecStartPos;
        Vector3 lastPos = vecStartPos;
        int nPoint = 30;
        for (int i = 1; i <= nPoint; i++)
        {
            float fLerp = (float)i / (float)(nPoint);
            newPos = GetCurvePoint(StartPos, CenterPos, EndPos, fLerp);
            paraBolaInfo.listVecPos.Add(newPos);
            paraBolaInfo.fLineDis += Vector3.Distance(lastPos, newPos);
            lastPos = newPos;
        }
        return paraBolaInfo;
    }

    /// <summary>
    /// 返回曲线在某一时间t上的点
    /// </summary>
    /// <param name="_p0">起始点</param>
    /// <param name="_p1">中间点</param>
    /// <param name="_p2">终止点</param>
    /// <param name="t">当前时间t(0.0~1.0)</param>
    /// <returns></returns>
    public static Vector3 GetCurvePoint(Vector3 _p0, Vector3 _p1, Vector3 _p2, float t)
    {
        t = Mathf.Clamp(t, 0.0f, 1.0f);
        float x = ((1 - t) * (1 - t)) * _p0.x + 2 * t * (1 - t) * _p1.x + t * t * _p2.x;
        float y = ((1 - t) * (1 - t)) * _p0.y + 2 * t * (1 - t) * _p1.y + t * t * _p2.y;
        float z = ((1 - t) * (1 - t)) * _p0.z + 2 * t * (1 - t) * _p1.z + t * t * _p2.z;
        Vector3 pos = new Vector3(x, y, z);
        return pos;
    }   

    public static string GetGoldSZ(long money)
    {
        string szGold = string.Empty;

        if(money > 1000000)
        {
            float nMoney = (float)money * 0.000001f;
            szGold = nMoney.ToString("f2") + "M";
        }
        else if(money > 1000)
        {
            float nMoney = (float)money * 0.001f;
            szGold = nMoney.ToString("f2") + "K";
        }
        else
        {
            szGold = money.ToString();
        }

        return szGold;
    }

    public static Vector3[] GetColliderNormal(Transform tranSelf, BoxCollider collider)
    {
        Vector3[] vecNormals = new Vector3[2];
        ///获取碰撞盒子大小 
        Vector3 vecColSize = collider.bounds.size;
        ///计算位置
        Vector3 vecForPos = tranSelf.position + (tranSelf.forward * (vecColSize.z / 2f));// new Vector3(0, 0, vecColSize.z / 2f);
        Vector3 vecBackPos = tranSelf.position + (-tranSelf.forward * (vecColSize.z / 2f)); //new Vector3(0, 0, -vecColSize.z / 2f);
                                                                                             ///计算向量
        Vector3 vecForNormal = tranSelf.position - vecBackPos;
        Vector3 vecBackNormal = tranSelf.position - vecForPos;

        vecNormals[0] = vecBackNormal;
        vecNormals[1] = vecForNormal;
        
        //Debug.Log(transform.position + new Vector3(-vecColSize.x / 2f, -vecColSize.y / 2f, vecColSize.z / 2f));
        //Debug.Log(transform.position + new Vector3(-vecColSize.x / 2f, -vecColSize.y / 2f, -vecColSize.z / 2f));
        //Debug.Log(transform.position + new Vector3(vecColSize.x / 2f, -vecColSize.y / 2f, vecColSize.z / 2f));
        //Debug.Log(transform.position + new Vector3(vecColSize.x / 2f, -vecColSize.y / 2f, -vecColSize.z / 2f));

        return vecNormals;
    }

    //public static string GetCostIconName(CostType costType)
    //{
    //    string szIconName = string.Empty;
    //    switch (costType)
    //    {
    //        case CostType.Gold:
    //            szIconName = "Icon_Gold";
    //            break;
    //    }
    //    return szIconName;
    //}

    /// <summary>
    /// 随机排列数组元素
    /// </summary>
    /// <param name="myList"></param>
    /// <returns></returns>
    public static List<int> ListRandom(List<int> myList)
    {

        System.Random ran = new System.Random();
        List<int> newList = new List<int>();
        int index = 0;
        int temp = 0;
        for (int i = 0; i < myList.Count; i++)
        {

            index = ran.Next(0, myList.Count - 1);
            if (index != i)
            {
                temp = myList[i];
                myList[i] = myList[index];
                myList[index] = temp;
            }
        }
        return myList;
    }

    /// <summary>
    /// 2D射线检测结果
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public static List<RaycastResult> GraphicRaycasterByUI(Vector2 pos, EventSystem eventSystem, GraphicRaycaster graphic)
    {
        PointerEventData mPointer = new PointerEventData(eventSystem);
        mPointer.position = pos;
        List<RaycastResult> results = new List<RaycastResult>();
        if (graphic != null)
            graphic.Raycast(mPointer, results);
        return results;
    }

    /// <summary>
    /// 3D射线检测
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="pCheckLayerMask"></param>
    /// <param name="lineRenderer"></param>
    /// <returns></returns>
    public static RaycastHit[] RaycasterByWorld(Vector3 vecPos, LayerMask pCheckLayerMask, LineRenderer lineRenderer = null)
    {
        Ray ray = Camera.main.ScreenPointToRay(vecPos);
        RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, 999f, pCheckLayerMask);
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
        }
        return hits;
    }

    /// <summary>
    /// 3D射线检测
    /// </summary>
    /// <param name="vecPos"></param>
    /// <param name="pCheckLayerMask"></param>
    /// <param name="lineRenderer"></param>
    /// <returns></returns>
    public static RaycastHit2D[] RaycasterBy2DWorld(Vector3 vecPos, LayerMask pCheckLayerMask, LineRenderer lineRenderer = null)
    {
        Ray ray = Camera.main.ScreenPointToRay(vecPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 999f, pCheckLayerMask);
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, ray.origin);
            lineRenderer.SetPosition(1, ray.origin + ray.direction * 100);
        }
        return hits;
    }

    /// <summary>
    /// 获取鼠标停留处UI
    /// </summary>
    /// <param name="canvas"></param>
    /// <returns></returns>
    public static GameObject GetOverUI(GameObject canvas)
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        GraphicRaycaster gr = canvas.GetComponent<GraphicRaycaster>();
        List<RaycastResult> results = new List<RaycastResult>();
        gr.Raycast(pointerEventData, results);
        if (results.Count != 0)
        {
            return results[0].gameObject;
        }

        return null;
    }


    #region Tween动画曲线

    /// <summary>
    /// 抖动动画曲线
    /// </summary>
    public static AnimationCurve pShakeAnimation = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.35f, 0.5f),
            new Keyframe(0.7f, -0.1f),
            new Keyframe(1f, 0f)
        );

    /// <summary>
    /// 角色休息动画缩放曲线
    /// </summary>
    public static AnimationCurve pRoleIdle = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, -0.5f),
            new Keyframe(0.7f, 1f),
            new Keyframe(1f, 0f)
        );

    /// <summary>
    /// 角色移动动画缩放曲线
    /// </summary>
    public static AnimationCurve pRoleRun = new AnimationCurve(
           new Keyframe(0f, 0f),
            new Keyframe(0.25f, -0.5f),
            new Keyframe(0.7f, 1f),
            new Keyframe(1f, 0f)
       );

    /// <summary>
    /// 角色攻击动画缩放曲线
    /// </summary>
    public static AnimationCurve pRoleAtk = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, -0.5f),
            new Keyframe(0.7f, 1f),
            new Keyframe(1f, 0f)
       );

    /// <summary>
    /// 角色攻击动画缩放曲线
    /// </summary>
    public static AnimationCurve pRoleFreeze = new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, -0.5f),
            new Keyframe(0.7f, 1f),
            new Keyframe(1f, 0f)
       );

    #endregion

    #region 碰撞检测

    /// <summary>
    /// 方形碰撞检测
    /// </summary>
    public static GameObject[] BoxCheck(Vector3 center, Vector3 halfSize, Quaternion ronation, LayerMask lMsk, bool showRange = false)
    {
        //Quaternion.LookRotation(FWD, Vector3.up);
        if (showRange)
        {
            GameObject objCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            objCube.transform.position = center;
            objCube.transform.localScale = halfSize * 2f;
            objCube.transform.rotation = ronation;
        }

        Collider[] arrCol = Physics.OverlapBox(center, halfSize, ronation, lMsk.value);
        if (arrCol == null) return null;

        GameObject[] arrRes = new GameObject[arrCol.Length];
        for (int i = 0; i < arrCol.Length; i++)
        {
            arrRes[i] = arrCol[i].gameObject;
        }

        return arrRes;
    }

    /// <summary>
    /// 圆形碰撞检测
    /// </summary>
    public static GameObject[] SphereCheck(Vector3 center, float radius, LayerMask lMsk, bool showRange = false)
    {
        //Quaternion.LookRotation(FWD, Vector3.up);
        if (showRange)
        {
            GameObject objCube = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objCube.transform.position = center;
            objCube.transform.localScale = Vector3.one * radius * 2;
        }

        Collider[] arrCol = Physics.OverlapSphere(center, radius, lMsk.value);
        if (arrCol == null) return null;

        GameObject[] arrRes = new GameObject[arrCol.Length];
        for (int i = 0; i < arrCol.Length; i++)
        {
            arrRes[i] = arrCol[i].gameObject;
        }

        return arrRes;
    }

    #endregion

}
