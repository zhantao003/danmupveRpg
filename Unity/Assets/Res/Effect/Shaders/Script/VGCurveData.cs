using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VGCurveInfo
{
    public AnimationCurve[] posCurve;
    public AnimationCurve[] eulerCurve;
    public AnimationCurve[] rotCurve;
    public AnimationCurve[] scaleCurve;
    public bool useActive;
    public AnimationCurve activeCurve;
    public bool useEnable;
    public AnimationCurve enableCurve;
    public List<string> floatPropertyName;
    public List<AnimationCurve> floatPropertyCurve;
    public List<string> colorPropertyName;
    public List<AnimationCurve> colorPropertyCurve;
    public List<string> vectorPropertyName;
    public List<AnimationCurve> vectorPropertyCurve;
}

[System.Serializable]
public class VGCurveData : UnityEngine.ScriptableObject
{
    //public List<VGCurveInfo> animationInfo = new List<VGCurveInfo>();
    public VGCurveInfo curveInfo;
}
