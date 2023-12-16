using FixMath.NET;
using UnityEngine;

public static class UtilGeometry
{
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="posX">Position Horizon</param>
    /// <param name="posY">Position Vertical</param>
    /// <returns></returns>
    public static FixVector3 CreateVector3(float posX, float posY)
    {
        return new FixVector3((Fix64)posX, (Fix64)posY, Fix64.Zero);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="posX">Position Horizon</param>
    /// <param name="posY">Position Vertical</param>
    /// <returns></returns>
    public static Vector2 CreateVector2(float posX, float posY)
    {
        return new Vector2(posX, posY);
    }

}

