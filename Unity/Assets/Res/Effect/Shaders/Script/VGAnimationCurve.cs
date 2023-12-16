using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class VGAnimationCurve : MonoBehaviour
{
    private System.Diagnostics.Stopwatch stopWatch;
    private long startTime;
    private Vector3 pos;
    private Vector3 scale;
    private Vector3 euler;
    private Quaternion rot;

    [Serializable]
    public class VGAnimationInfo
    {
        public GameObject obj;
        public Transform trans;

        public Renderer render;
        public Material mat;
        public VGCurveData curveData;
    }
    public List<VGAnimationInfo> animationInfo = new List<VGAnimationInfo>();
    public string animatorControllerGuid;
    public float animationLength;
    public bool isLoop;

    private void Awake()
    {
        stopWatch = System.Diagnostics.Stopwatch.StartNew();
    }

    // Use this for initialization
    void Start()
    {

    }

    private void OnEnable()
    {
        startTime = stopWatch.ElapsedMilliseconds;
    }

    private void OnDestroy()
    {
        for (int i = 0; i < animationInfo.Count; i++)
        {
            if (animationInfo[i].mat != null)
            {
                Destroy(animationInfo[i].mat);
                animationInfo[i].mat = null;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateAnimation();
    }

    private void UpdateFloat(VGAnimationInfo animInfo, float time)
    {
        var info = animInfo.curveData.curveInfo;
        if (info.floatPropertyName.Count > 0)
        {
            if (animInfo.mat == null)
            {
                animInfo.mat = animInfo.render.material;
            }
        }
        for (int i = 0; i < info.floatPropertyName.Count; i++)
        {
            animInfo.mat.SetFloat(info.floatPropertyName[i], info.floatPropertyCurve[i].Evaluate(time));
        }
    }

    private Color tempColor;
    private void UpdateColor(VGAnimationInfo animInfo, float time)
    {
        var info = animInfo.curveData.curveInfo;
        if (info.colorPropertyName.Count > 0)
        {
            if (animInfo.mat == null)
            {
                animInfo.mat = animInfo.render.material;
            }
        }
        for (int i = 0; i < info.colorPropertyName.Count; i++)
        {
            tempColor.r = info.colorPropertyCurve[i * 4].Evaluate(time);
            tempColor.g = info.colorPropertyCurve[i * 4 + 1].Evaluate(time);
            tempColor.b = info.colorPropertyCurve[i * 4 + 2].Evaluate(time);
            tempColor.a = info.colorPropertyCurve[i * 4 + 3].Evaluate(time);
            animInfo.mat.SetColor(info.colorPropertyName[i], tempColor);
        }
    }

    private Vector4 vec;
    private void UpdateVector(VGAnimationInfo animInfo, float time)
    {
        var info = animInfo.curveData.curveInfo;
        if (info.vectorPropertyName.Count > 0)
        {
            if (animInfo.mat == null)
            {
                animInfo.mat = animInfo.render.material;
            }
        }
        for (int i = 0; i < info.vectorPropertyName.Count; i++)
        {
            vec.x = info.vectorPropertyCurve[i * 4].Evaluate(time);
            vec.y = info.vectorPropertyCurve[i * 4 + 1].Evaluate(time);
            vec.z = info.vectorPropertyCurve[i * 4 + 2].Evaluate(time);
            vec.w = info.vectorPropertyCurve[i * 4 + 3].Evaluate(time);
            animInfo.mat.SetVector(info.vectorPropertyName[i], vec);
        }
    }

    private void UpdateActive(VGAnimationInfo animInfo, float time)
    {
        var info = animInfo.curveData.curveInfo;
        if (animInfo.obj != null && info.useActive)
        {
            if (info.activeCurve.Evaluate(time) > 0.1)
            {
                animInfo.obj.SetActive(true);
            }
            else
            {
                animInfo.obj.SetActive(false);
            }
        }
        if(animInfo.render != null && info.useEnable)
        {
            if (info.enableCurve.Evaluate(time) > 0.1)
            {
                animInfo.render.enabled = true;
            }
            else
            {
                animInfo.render.enabled = false;
            }
        }
    }

    private void UpdateAnimation()
    {
        var time = (float)(stopWatch.ElapsedMilliseconds - startTime) / 1000;
        if(time > animationLength)
        {
            if (!isLoop)
            {
                return;
            }
            else
            {
                startTime = stopWatch.ElapsedMilliseconds;
                time = 0;
            }
        }

        for (int i = 0; i < animationInfo.Count; i++)
        {
            var animInfo = animationInfo[i];
            if(animInfo == null || animInfo.curveData == null)
            {
                continue;
            }
            var info = animInfo.curveData.curveInfo;
            if (info.posCurve.Length > 0)
            {
                pos.x = info.posCurve[0].Evaluate(time);
                pos.y = info.posCurve[1].Evaluate(time);
                pos.z = info.posCurve[2].Evaluate(time);
                animInfo.trans.localPosition = pos;
            }
            if(info.scaleCurve.Length > 0)
            {
                scale.x = info.scaleCurve[0].Evaluate(time);
                scale.y = info.scaleCurve[1].Evaluate(time);
                scale.z = info.scaleCurve[2].Evaluate(time);
                animInfo.trans.localScale = scale;
            }
            if(info.rotCurve.Length > 0)
            {
                rot.x = info.rotCurve[0].Evaluate(time);
                rot.y = info.rotCurve[1].Evaluate(time);
                rot.z = info.rotCurve[2].Evaluate(time);
                rot.w = info.rotCurve[3].Evaluate(time);
                animInfo.trans.localRotation = rot;
            }
            if (info.eulerCurve.Length > 0)
            {
                euler.x = info.eulerCurve[0].Evaluate(time);
                euler.y = info.eulerCurve[1].Evaluate(time);
                euler.z = info.eulerCurve[2].Evaluate(time);
                animInfo.trans.localEulerAngles = euler;
            }
            UpdateFloat(animInfo, time);
            UpdateColor(animInfo, time);
            UpdateVector(animInfo, time);
            UpdateActive(animInfo, time);
        }
    }

    public void ClearUnusedNode()
    {
        for (int i = animationInfo.Count - 1; i >= 0; i--)
        {
            if(animationInfo[i].trans == null)
            {
                animationInfo.RemoveAt(i);
            }
        }
    }

#if UNITY_EDITOR
    [ContextMenu("ConvertToAnimator")]
    public void ConvertToAnimator()
    {
        string path = AssetDatabase.GUIDToAssetPath(animatorControllerGuid);
        var ac = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(path);
        if(ac != null)
        {
            var animator = this.gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = ac;
            DestroyImmediate(this, true);
        }
    }
#endif
}
