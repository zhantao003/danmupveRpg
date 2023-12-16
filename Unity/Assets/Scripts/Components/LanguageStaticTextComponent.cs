using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanguageStaticTextComponent : MonoBehaviour
{
    public Text text;
    public EMLanguageContentType type;
    public string key;
    private void Start()
    {
        if (text == null) text = GetComponent<Text>();
        if (text != null) {
            text.text = CTBLLanguageInfo.Inst.GetContent(type, key);
        }
    }
}
