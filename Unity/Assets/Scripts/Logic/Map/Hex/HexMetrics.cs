using UnityEngine;

public static class HexMetrics
{
    public const float DefaultHigh = 0;
    public const int DefaultHighInt = 0;
    /// <summary>
    /// 外径，中心到顶点距离
    /// </summary>
    public const float outerRadius = 0.45f;// 0.35f;

    /// <summary>
    /// 内径，中心到边距离，0.866025404为二分之根号三的近似值
    /// </summary>
    public const float Mathf_Sin_60 = 0.866025404f;
    public const float innerRadius = outerRadius * Mathf_Sin_60;

    /// <summary>
    /// 六边形的六个顶点坐标
    /// </summary>
    public static readonly Vector3[] corners = {
        new Vector3(0f,           outerRadius,          0f),
        new Vector3(innerRadius,  0.5f*outerRadius,     0f),
        new Vector3(innerRadius,  -0.5f*outerRadius,    0f),
        new Vector3(0f,           -outerRadius,         0f),
        new Vector3(-innerRadius, -0.5f*outerRadius,    0f),
        new Vector3(-innerRadius, 0.5f*outerRadius,     0f),
        new Vector3(0f,           outerRadius,          0f),//为了代码结构清晰加的 index=6
     };

    public static Vector3 GetPosFromHexGrid(int x, int y)
    {
        bool odd = ((y & 1) != 0);
        // 得到格子左下角坐标:
        float OGX = x * (2 * innerRadius);
        float OGY = y * (1.5f * outerRadius);
        // 奇数行要右移半个宽度：
        if (odd)
        {
            OGX += innerRadius;
        }
        // 偏移转到格子中心位置:
        Vector3 pos = new Vector3(OGX + innerRadius, OGY + outerRadius);// new Vector3(OGX + innerRadius, DefaultHigh, OGY + outerRadius);
        return pos;
    }

    public static bool GetHexGridFromPos(Vector3 pos, out int x, out int y)
    {
        float xWorld = pos.x;
        float yWorld = pos.z;
        int iGY = (int)(yWorld / (1.5 * outerRadius));
        bool odd = ((iGY & 1) != 0);
        // 奇：
        if (odd)
        {
            xWorld -= innerRadius;
        }
        int iGX = (int)(xWorld / (2 * innerRadius));
        // 得到格子左下角坐标:
        float OGX = iGX * (2 * innerRadius);
        float OGY = iGY * (1.5f * outerRadius);
        float refX = OGX + innerRadius;
        float refY = OGY + outerRadius * 0.5f;
        // 可能不在本格子内(因为可能位置在格格子下方的左下角或右下角):
        bool bOutProbably = yWorld < refY;
        if (bOutProbably)
        {
            // 得到Hex中心往下半个外边长的位置:
            float dx = Mathf.Abs(xWorld - refX) * (0.5f * outerRadius / innerRadius); // 乘( ../.. )使其变成正方形再来判断
            float dy = Mathf.Abs(yWorld - refY);
            float dt = dx + dy;
            // 在左半边:
            if (xWorld < refX)
            {
                // 不在本格子,而是在左下角:
                if (dt > outerRadius * 0.5f)
                {
                    iGY--; // 不管奇偶，下部都要y--
                           // 如果是偶数的左下，还要将x--
                    if (false == odd)
                    {
                        iGX--;
                    }
                }
            }
            // 在右半边
            else
            {
                // 不在本格子, 而是在右下角:
                if (dt > outerRadius * 0.5f)
                {
                    iGY--; // 不管奇偶，下部都要y--
                           // 如果是奇数的右下，还要将x++
                    if (odd)
                    {
                        iGX++;
                    }
                }
            }
        }
        x = iGX;
        y = iGY;
        return true;
    }
}