using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CTBLConfigAttri : Attribute
{
    public string tblName; // 配置表名字

    public string tblFilePath;  //本地相对路径文件夹

    public CTBLConfigAttri(string value, string file = "")
    {
        tblName = value;
        tblFilePath = file;
    }
}

