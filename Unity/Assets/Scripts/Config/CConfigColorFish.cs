using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.CreateAssetMenu(fileName = "GameColorFlag", menuName = "GameConfig/ColorFlag")]
public class CConfigColorFish : SerializedScriptableObject
{
    public Dictionary<string, int> dicConfigsInt = new Dictionary<string, int>();
    public Dictionary<string, string> dicConfigsString = new Dictionary<string, string>();
    public Dictionary<string, Color> dicConfigColor = new Dictionary<string, Color>();

    public int GetInt(string key)
    {
        if (dicConfigsInt.ContainsKey(key))
            return dicConfigsInt[key];

        return 0;
    }

    public string GetString(string key)
    {
        if (dicConfigsString.ContainsKey(key))
            return dicConfigsString[key];

        return "";
    }

    public Color GetColor(string key)
    {
        if (dicConfigColor.ContainsKey(key))
            return dicConfigColor[key];

        return Color.white;
    }

    public string GetColorHex(string key)
    {
        if (dicConfigColor.ContainsKey(key))
            return ColorUtility.ToHtmlStringRGB(dicConfigColor[key]);

        return "FFFFFF";
    }
}
