using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Text;

public class CTBLLoader
{
    public class CLine
    {
        public List<string> szCells = new List<string>();
    }

    List<CLine> m_LineList = new List<CLine>();
    Dictionary<string, int> m_mapColName = new Dictionary<string, int>();
    CLine m_CurLine;

    public void LoadFromFile(string strFileName)
    {
        TextAsset file = (TextAsset)(Resources.Load(strFileName, typeof(TextAsset)));
        if (file == null)
        {
            Debug.LogError("载入TBL文件出错：" + strFileName);
        }

        string text = file.text;
#if TBL_ENGRYPT
        text = CEncryptHelper.AesDecrypt(text);
#endif

        string[] vals = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int nLineCounts = vals.Length;

        int nContentLineCounts = 0;
        for (int nIdx = 0; nIdx < nLineCounts; nIdx++)
        {
            if (!(vals[nIdx].StartsWith("//")))
            {
                BuildLineCell(vals[nIdx], nContentLineCounts);
                nContentLineCounts++;
            }
        }

        if (nContentLineCounts > 1)
            GotoLineByIndex(0);
    }

    //从绝对路径加载TBL
    public void LoadFromFileabAolutePath(string strFileName)
    {
        try
        {
            FileStream fs = new FileStream(strFileName, FileMode.Open);
            if (fs == null)
            {
                Debug.LogWarning("ServerConfig.config read failed");
                return;
            }
            byte[] pByte = new byte[fs.Length];
            fs.Read(pByte, 0, pByte.Length);
            fs.Close();

            //内容解密
            string szContent = Encoding.UTF8.GetString(pByte);
            LoadFromFileContent(szContent);
        }
        catch (Exception e)
        {
            Debug.LogError("Load TBL Fail:" + e.Message);
        }
    }

    public void LoadFromFileContent(string szContent)
    {
        string text = szContent;
        string[] vals = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int nLineCounts = vals.Length;

        int nContentLineCounts = 0;
        for (int nIdx = 0; nIdx < nLineCounts; nIdx++)
        {
            if (!(vals[nIdx].StartsWith("//")))
            {
                BuildLineCell(vals[nIdx], nContentLineCounts);
                nContentLineCounts++;
            }
        }

        if (nContentLineCounts > 1)
            GotoLineByIndex(0);
    }

    public void BuildLineCell(string szLine, int nIdx)
    {
        string[] vals = szLine.Trim().Split(new char[] { '\t' }, StringSplitOptions.None);
        int nCount = vals.Length;
        CLine Line = new CLine();
        for (int n = 0; n < nCount; n++)
        {
            Line.szCells.Add(vals[n]);
        }

        if (nIdx == 0)
        {
            m_LineList.Clear();
            BuildColName(Line);
        }
        else
        {
            m_LineList.Add(Line);
        }
    }

    public void BuildColName(CLine szLine)
    {
        m_mapColName.Clear();
        for (int nIdx = 0; nIdx < szLine.szCells.Count; nIdx++)
        {
            m_mapColName.Add(szLine.szCells[nIdx], nIdx);
        }
    }

    public int GetLineCount()
    {
        return m_LineList.Count;
    }

    public void GotoLineByIndex(int nIdx)
    {
        m_CurLine = m_LineList[nIdx];
    }

    public int GetIntByName(string name)
    {
        int nIdx;
        if (m_mapColName.TryGetValue(name, out nIdx))
        {
            int nRes;
            if (int.TryParse(m_CurLine.szCells[nIdx], out nRes))
            {
                return nRes;
            }
            return 0;
        }

        return 0;
    }

    public long GetLongByName(string name)
    {
        int nIdx;
        if (m_mapColName.TryGetValue(name, out nIdx))
        {
            long nRes;
            if (long.TryParse(m_CurLine.szCells[nIdx], out nRes))
            {
                return nRes;
            }
            return 0;
        }

        return 0;
    }

    public float GetFloatByName(string name)
    {
        int nIdx;
        if (m_mapColName.TryGetValue(name, out nIdx))
        {
            float fRes;
            if (float.TryParse(m_CurLine.szCells[nIdx], out fRes))
            {
                return fRes;
            }
            return 0;
        }

        return 0;
    }

    public string GetStringByName(string name)
    {
        int nIdx;
        if (m_mapColName.TryGetValue(name, out nIdx))
        {
            return m_CurLine.szCells[nIdx];
        }

        return "";
    }
}
