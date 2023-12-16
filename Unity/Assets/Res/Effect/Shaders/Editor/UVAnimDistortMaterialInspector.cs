using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class UVAnimDistortMaterialInspector : BaseShaderInspector
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // render the default gui
        base.OnGUI(materialEditor, properties);

        var targetMat = materialEditor.target as Material;
        var shaderKeyWords = targetMat.shaderKeywords;

        AddBlendOperation(materialEditor, properties);

        AddCullOperation(materialEditor, properties);

        AddMaskOperation(materialEditor, properties);

        AddDistortOperation(materialEditor, properties);

        AddClipOperation(materialEditor, properties);

        AddOffsetOperation(materialEditor, properties);

        AddColorSpaceOperation(materialEditor, properties);

        AddAlphaChannelOperation(materialEditor, properties);

        materialEditor.RenderQueueField();

        EditorUtility.SetDirty(targetMat);
    }
}
