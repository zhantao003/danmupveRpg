using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISkillTexts : MonoBehaviour
{
    public Text[] texts;

    public void SetSkillName(CSkillName skillName) 
    {

        texts[0].text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, skillName.Skill1);
        texts[1].text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, skillName.Skill2);
        texts[2].text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, skillName.Skill3);
        texts[3].text = CTBLLanguageInfo.Inst.GetContent(EMLanguageContentType.Game, skillName.Skill4);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
