using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ST_LanguageInfo
{
    public string strID; //ID
    public string strContent;    //对应内容
}

public enum EMLanguageType
{
    CN = 0,  //中文
    EN,     //英文
    JP,     //日文
    YN,     //印尼

    Max,
}

public enum EMLanguageContentType
{
    UI = 0,
    Game,
    Hero,
    HeroEquip,
    Role,
    RoleSkill,
    NPC,
    ChoiceEvent,

    Max,
}

//TBL委托方法
public delegate void DelegateLoadLanguageTBL(EMLanguageContentType contentType, CTBLLoader loader);

public class CTBLLanguageInfo
{
    #region Instance

    private CTBLLanguageInfo() { }
    private static CTBLLanguageInfo m_Instance = null;
    public static CTBLLanguageInfo Inst
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = new CTBLLanguageInfo();
            }
            return m_Instance;
        }
    }

    #endregion

    public string[] strLanguage = new string[4]
    {
        "简体中文",
        "English",
        "日語",
        "印尼"
    };

    Dictionary<EMLanguageContentType, Dictionary<string, string>> dicAllContents = new Dictionary<EMLanguageContentType, Dictionary<string, string>>();

    /// <summary>
    /// 当前的语言类型
    /// </summary>
    public EMLanguageType curLanguageType;

    public void Clear()
    {
        dicAllContents.Clear();
    }

    public string GetContent(EMLanguageContentType contentType, string key)
    {
        if (!dicAllContents.ContainsKey(contentType)) return null;
        Dictionary<string, string> dicContent = dicAllContents[contentType];
        if (dicContent == null) 
            return $"error content type:{contentType.ToString()}";

        string strContent = null;
        
        if (!dicContent.TryGetValue(key, out strContent))
        {
            strContent = $"error content:{key}";
        }

        return strContent;
    }

    /// <summary>
    /// 读取文字信息,通过AssetBundle
    /// </summary>
    /// <param name="pTBLBundle"></param>
    public void LoadLanguageByBundle(AssetBundle pTBLBundle)
    {
        Clear();

        for(int i=0; i<(int)EMLanguageContentType.Max; i++)
        {
            EMLanguageContentType emContentType = (EMLanguageContentType)i;

            Dictionary<string, string> dicContentSlot = new Dictionary<string, string>();
            dicAllContents.Add(emContentType, dicContentSlot);

            LoadTBLByBundle(pTBLBundle, GetFileName(emContentType), emContentType, LoadContent);
        }
    }

    void LoadContent(EMLanguageContentType contentType, CTBLLoader loader)
    {
        Dictionary<string, string> dicContent = dicAllContents[contentType];
        if (dicContent == null) return;

        //Debug.Log("文字表：" + contentType.ToString());
        
        for (int i = 0; i < loader.GetLineCount(); i++)
        {
            loader.GotoLineByIndex(i);
            string strID = loader.GetStringByName("id");
            if (dicContent.ContainsKey(strID))
            {
                Debug.LogError($"{strID} 存在相同元素----dicLanguageInfoUI");
                continue;
            }
            //Debug.Log(strID + "== Language====" + contentType);
            dicContent.Add(strID, loader.GetStringByName("content"));
        }
    }

    /// <summary>
    /// 获取该类型文件路径
    /// </summary>
    /// <param name="contentType"></param>
    /// <returns></returns>
    string GetFileName(EMLanguageContentType contentType)
    {
        return $"StringContent{contentType.ToString()}_{curLanguageType.ToString()}";
    }

    public void LoadTBLByBundle(AssetBundle pBundle, string szConfig, EMLanguageContentType contentType, DelegateLoadLanguageTBL dlg)
    {
        TextAsset textasset = pBundle.LoadAsset<TextAsset>(szConfig);

        CTBLLoader loader = new CTBLLoader();
        if (textasset == null)
        {
            loader.LoadFromFileContent("");
        }
        else
        {
            string szContent = textasset.text;
#if TBL_ENGRYPT
            szContent = CEncryptHelper.AesDecrypt(szContent);
#endif
            loader.LoadFromFileContent(szContent);
        }
        if (dlg != null)
        {
            dlg(contentType, loader);
        }
    }

    public void LoadTBLAbsolutePath(string szPath, EMLanguageContentType contentType, DelegateLoadLanguageTBL dlg)
    {
        CTBLLoader loader = new CTBLLoader();
        loader.LoadFromFileabAolutePath(szPath);

        if (dlg != null)
        {
            dlg(contentType, loader);
        }
    }
}
