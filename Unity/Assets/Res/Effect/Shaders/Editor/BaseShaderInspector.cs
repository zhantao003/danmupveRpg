using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class BaseShaderInspector : ShaderGUI
{
    protected int blendIndex;
    protected int cullIndex;
    protected int srcMode = 1;
    protected int dstMode = 1;
    protected int cullMode = 0;
    protected string[] blendMode = { "Additive", "Alpha Blend" };
    protected string[] cullModeList = { "Off", "Front", "Back" };
    protected bool isDistort;
    protected bool useMask;
    protected bool useOffset;
    protected bool useCutout;
    protected bool useOnUI;
    protected bool useAlpha;
    protected float tileX = 800;
    protected float tileY = 800;

    private string[] useUIShader = { "VGame/Effects/MG General", "VGame/Effects/MG Distort UV Ani" };

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        materialEditor.SetDefaultGUIWidths();

        for (var i = 0; i < props.Length; i++)
        {
            if ((props[i].flags & (MaterialProperty.PropFlags.HideInInspector | MaterialProperty.PropFlags.PerRendererData)) != 0)
                continue;

            float h = materialEditor.GetPropertyHeight(props[i], props[i].displayName);
            Rect r = EditorGUILayout.GetControlRect(true, h, EditorStyles.layerMaskField);

            materialEditor.ShaderProperty(r, props[i], props[i].displayName);
        }
    }

    private bool HasProperty(string name, MaterialProperty[] properties)
    {
        try
        {
            FindProperty(name, properties);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        return true;
    }

    protected void AddFloatValue(MaterialEditor materialEditor, MaterialProperty[] properties, string name)
    {
        MaterialProperty heat = FindProperty(name, properties);
        if (heat != null)
        {
            materialEditor.ShaderProperty(heat, new GUIContent(heat.displayName));
        }
    }

    protected void AddBlendOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("SrcMode", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        srcMode = targetMat.GetInt("SrcMode");
        dstMode = targetMat.GetInt("DstMode");

        if (srcMode == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.One)
        {
            blendIndex = 0;
        }
        if (srcMode == (int)UnityEngine.Rendering.BlendMode.SrcAlpha &&
            dstMode == (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha)
        {
            blendIndex = 1;
        }

        blendIndex = EditorGUILayout.Popup("Blend Mode", blendIndex, blendMode);

        if (blendIndex == 0)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.One);
        }
        if (blendIndex == 1)
        {
            targetMat.SetInt("SrcMode", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            targetMat.SetInt("DstMode", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        }
    }

    protected void AddCullOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("CullMode", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        cullMode = targetMat.GetInt("CullMode");
        switch (cullMode)
        {
            case (int)UnityEngine.Rendering.CullMode.Off:
                {
                    cullIndex = 0;
                    break;
                }
            case (int)UnityEngine.Rendering.CullMode.Front:
                {
                    cullIndex = 1;
                    break;
                }
            case (int)UnityEngine.Rendering.CullMode.Back:
                {
                    cullIndex = 2;
                    break;
                }
        }

        cullIndex = EditorGUILayout.Popup("Cull Mode", cullIndex, cullModeList);
        switch (cullIndex)
        {
            case (int)UnityEngine.Rendering.CullMode.Off:
                {
                    targetMat.SetInt("CullMode", (int)UnityEngine.Rendering.CullMode.Off);
                    break;
                }
            case (int)UnityEngine.Rendering.CullMode.Front:
                {
                    targetMat.SetInt("CullMode", (int)UnityEngine.Rendering.CullMode.Front);
                    break;
                }
            case (int)UnityEngine.Rendering.CullMode.Back:
                {
                    targetMat.SetInt("CullMode", (int)UnityEngine.Rendering.CullMode.Back);
                    break;
                }
        }
    }

    protected void AddMaskNoUVOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("_MaskTex", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        useMask = shaderKeyWords.Contains("_MASK_ON");

        useMask = EditorGUILayout.Toggle("Use Mask", useMask);

        if (useMask)
        {
            MaterialProperty mask = FindProperty("_MaskTex", properties);
            materialEditor.ShaderProperty(mask, new GUIContent(mask.displayName, mask.textureValue));
        }

        if (useMask)
        {
            targetMat.EnableKeyword("_MASK_ON");
        }
        else
        {
            targetMat.DisableKeyword("_MASK_ON");
        }
    }

    protected void AddMaskOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("_MaskTex", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        useMask = shaderKeyWords.Contains("_MASK_ON");

        useMask = EditorGUILayout.Toggle("Use Mask", useMask);

        if (useMask)
        {
            MaterialProperty mask = FindProperty("_MaskTex", properties);
            materialEditor.ShaderProperty(mask, new GUIContent(mask.displayName, mask.textureValue));
            MaterialProperty maskU = FindProperty("_MaskU", properties);
            materialEditor.ShaderProperty(maskU, new GUIContent(maskU.displayName));
            MaterialProperty maskV = FindProperty("_MaskV", properties);
            materialEditor.ShaderProperty(maskV, new GUIContent(maskV.displayName));
        }

        if (useMask)
        {
            targetMat.EnableKeyword("_MASK_ON");
        }
        else
        {
            targetMat.DisableKeyword("_MASK_ON");
        }
    }
    protected void AddDistortOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("_NoiseTex", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        isDistort = shaderKeyWords.Contains("_DISTORT_ON");

        isDistort = EditorGUILayout.Toggle("Need Distort", isDistort);

        if (isDistort)
        {
            MaterialProperty noise = FindProperty("_NoiseTex", properties);
            materialEditor.ShaderProperty(noise, new GUIContent(noise.displayName, noise.textureValue));

            MaterialProperty heat = FindProperty("_HeatTime", properties);
            materialEditor.ShaderProperty(heat, new GUIContent(heat.displayName));
            MaterialProperty forceX = FindProperty("_ForceX", properties);
            materialEditor.ShaderProperty(forceX, new GUIContent(forceX.displayName));
            MaterialProperty forceY = FindProperty("_ForceY", properties);
            materialEditor.ShaderProperty(forceY, new GUIContent(forceY.displayName));
        }

        if (isDistort)
        {
            targetMat.EnableKeyword("_DISTORT_ON");
        }
        else
        {
            targetMat.DisableKeyword("_DISTORT_ON");
        }
    }

    protected void AddClipOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("_Cutout", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        useCutout = shaderKeyWords.Contains("Clip_ON");

        useCutout = EditorGUILayout.Toggle("Use Cutout", useCutout);

        if (useCutout)
        {
            MaterialProperty cutout = FindProperty("_Cutout", properties);
            materialEditor.ShaderProperty(cutout, new GUIContent(cutout.displayName));
        }

        if (useCutout)
        {
            targetMat.EnableKeyword("Clip_ON");
        }
        else
        {
            targetMat.DisableKeyword("Clip_ON");
        }
    }
    protected void AddOffsetOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        if (!HasProperty("_Offset", properties))
        {
            return;
        }

        var targetMat = materialEditor.target as Material;

        useOffset = targetMat.GetInt("_Offset") != 0;

        useOffset = EditorGUILayout.Toggle("Use ZTest Off", useOffset);

        targetMat.SetInt("_Offset", useOffset ? -100 : 0);
        targetMat.SetInt("_ZTestFactor", useOffset ? (int)UnityEngine.Rendering.CompareFunction.Disabled : (int)UnityEngine.Rendering.CompareFunction.LessEqual);
    }

    private bool CheckUseUIShader(string name)
    {
        foreach (var str in useUIShader)
        {
            if (str == name)
            {
                return true;
            }
        }
        return false;
    }

    protected void AddColorSpaceOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        useOnUI = shaderKeyWords.Contains("USE_LINEAR_INPUT");

        useOnUI = EditorGUILayout.Toggle("Use Linear Input(使用线性空间处理)", useOnUI);

        if (useOnUI)
        {
            targetMat.EnableKeyword("USE_LINEAR_INPUT");
        }
        else
        {
            targetMat.DisableKeyword("USE_LINEAR_INPUT");
        }
    }

    protected void AddAlphaChannelOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        useAlpha = shaderKeyWords.Contains("USE_RGB_ALPHA");

        useAlpha = EditorGUILayout.Toggle("Use RGB As Alpha", useAlpha);

        if (useAlpha)
        {
            targetMat.EnableKeyword("USE_RGB_ALPHA");
        }
        else
        {
            targetMat.DisableKeyword("USE_RGB_ALPHA");
        }
    }

    protected void AddFrameBlendOperation(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        Vector4 tilling = targetMat.GetVector("_Tiling");
        tileX = tilling.x;
        tileY = tilling.y;

        tileX = EditorGUILayout.FloatField("TileX", tileX);
        tileY = EditorGUILayout.FloatField("TileY", tileY);

        targetMat.SetVector("_Tiling", new Vector4(tileX, tileY, tileX / 100, tileY / 100));
    }


    [MenuItem("Art/Effect/MatSetLinear")]
    public static void SetAllMatLinear()
    {
        var assets = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
        foreach (var asset in assets)
        {
            var mat = asset as Material;
            var shaderKeyWords = mat.shaderKeywords;
            if (shaderKeyWords.Contains("USE_ON_UI"))
            {
                mat.DisableKeyword("USE_LINEAR_INPUT");
            }
            else
            {
                mat.EnableKeyword("USE_LINEAR_INPUT");
            }
        }
    }

    [MenuItem("Art/Effect/MatSetGamma")]
    public static void SetAllMatGamma()
    {
        var assets = Selection.GetFiltered(typeof(Material), SelectionMode.DeepAssets);
        foreach (var asset in assets)
        {
            var mat = asset as Material;
            mat.DisableKeyword("USE_LINEAR_INPUT");
        }
    }
}
