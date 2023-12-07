using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System;

public class ExcelToolInfo
{
    public string szFileName;
    public string szAssetBundle;
    public FileInfo pFileInfo;
    public bool bExport;
}

public class ExcelTools : EditorWindow
{
    /// <summary>
    /// 当前编辑器窗口实例
    /// </summary>
    private static ExcelTools instance;

    /// <summary>
    /// Excel文件列表
    /// </summary>
    private static List<ExcelToolInfo> excelList;

    /// <summary>
    /// 配表路径
    /// </summary>
    private static string pathExcel;  

    /// <summary>
    /// 根目录	
    /// </summary>
    private static string pathRoot;

    /// <summary>
    /// xlxs文件绝对路径
    /// </summary>
    private static string pathFinal;

    /// <summary>
    /// 输出目录
    /// </summary>
    private static string pathOutput;

    /// <summary>
    /// 滚动窗口初始位置
    /// </summary>
    private static Vector2 scrollPos;

    /// <summary>
    /// 输出格式索引
    /// </summary>
    private static int indexOfFormat = 0;

    /// <summary>
    /// 输出格式
    /// </summary>
    private static string[] formatOption = new string[] { "TXT", "CSV", "XML", "LUA" };

    /// <summary>
    /// 编码索引
    /// </summary>
    private static int indexOfEncoding = 0;

    /// <summary>
    /// 编码选项
    /// </summary>
    private static string[] encodingOption = new string[] { "UTF-8", "GB2312" };

    /// <summary>
    /// 是否保留原始文件
    /// </summary>
    private static bool keepSource = true;

    /// <summary>
    /// 是否加密
    /// </summary>
    private static bool isEncrypt = true;

    /// <summary>
    /// 显示当前窗口	
    /// </summary>
    [MenuItem("Tools/ExcelTools")]
    static void ShowExcelTools()
    {
        Init();
        //加载Excel文件
        LoadExcel();
        instance.Show();
    }

    private static void Init()
    {
        //获取当前实例
        instance = EditorWindow.GetWindow<ExcelTools>();

        //初始化
        pathExcel = "配置表/";
        pathRoot = Path.Combine(Application.dataPath, $"../../");
        pathFinal = Path.Combine(pathRoot, pathExcel);
        pathOutput = Path.Combine(Application.dataPath, "Resources/TBL/");

        //注意这里需要对路径进行处理
        //目的是去除Assets这部分字符以获取项目目录
        //我表示Windows的/符号一直没有搞懂
        pathExcel = pathExcel.Replace('\\', '/').Substring(0, pathExcel.LastIndexOf("/"));
        pathRoot = pathRoot.Replace('\\', '/').Substring(0, pathRoot.LastIndexOf("/"));
        pathFinal = pathFinal.Replace('\\', '/').Substring(0, pathFinal.LastIndexOf("/"));
        pathOutput = pathOutput.Replace('\\', '/').Substring(0, pathOutput.LastIndexOf("/"));

        excelList = new List<ExcelToolInfo>();
        scrollPos = new Vector2(instance.position.x, instance.position.y + 75);
    }

    /// <summary>
    /// 加载Excel
    /// </summary>
    private static void LoadExcel()
    {
        if (excelList == null) excelList = new List<ExcelToolInfo>();
        excelList.Clear();

        DirectoryInfo pFileRoot = new DirectoryInfo(pathFinal);

        if (!pFileRoot.Exists)
        {
            return;
        }

        List<FileInfo> listFileInfos = LocalFileManage.GetAllFilesInPath(pFileRoot, new string[] { ".xlsx" });
        if (listFileInfos.Count <= 0) return;

        for (int i = 0; i < listFileInfos.Count; i++)
        {
            FileInfo fileInfo = listFileInfos[i];
            if (fileInfo.Name.Contains("~$"))
            {
                continue;
            }

            ExcelToolInfo pExcelInfo = new ExcelToolInfo();
            pExcelInfo.bExport = true;

            string szFileName = fileInfo.Name;
            string szCheck = pathExcel.Replace('/', '\\');

            int nIdx = fileInfo.FullName.IndexOf(szCheck);
            if (nIdx >= 0)
            {
                szFileName = fileInfo.FullName.Substring(nIdx + szCheck.Length + 1).Replace('\\', '/');
            }

            pExcelInfo.pFileInfo = fileInfo;
            pExcelInfo.szFileName = szFileName;

            if (pExcelInfo.szFileName.Contains("AdvModelConfig"))
            {
                pExcelInfo.szAssetBundle = "advmodel";
            }
            else
            {
                pExcelInfo.szAssetBundle = "tbl";
            }

            excelList.Add(pExcelInfo);
        }
    }


    void OnGUI()
    {
        DrawOptions();
        DrawExport();
    }

    /// <summary>
    /// 绘制插件界面配置项
    /// </summary>
    private void DrawOptions()
    {
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择格式类型:", GUILayout.Width(85));
        indexOfFormat = EditorGUILayout.Popup(indexOfFormat, formatOption, GUILayout.Width(125));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("请选择编码类型:", GUILayout.Width(85));
        indexOfEncoding = EditorGUILayout.Popup(indexOfEncoding, encodingOption, GUILayout.Width(125));
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("文件目录:", GUILayout.Width(85));
        pathExcel = EditorGUILayout.TextField(pathExcel);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("根目录:", GUILayout.Width(85));
        pathRoot = EditorGUILayout.TextField(pathRoot);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("绝对路径:", GUILayout.Width(85));
        pathFinal = Path.Combine(pathRoot, pathExcel).Replace('\\', '/');
        //pathFinal = pathFinal.Substring(0, pathFinal.LastIndexOf("/"));
        EditorGUILayout.LabelField(pathFinal);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("输出文件路径:", GUILayout.Width(85));
        pathOutput = EditorGUILayout.TextField(pathOutput);
        GUILayout.EndHorizontal();

        keepSource = GUILayout.Toggle(keepSource, "保留Excel源文件");
        isEncrypt = GUILayout.Toggle(isEncrypt, "是否加密");
    }

    /// <summary>
    /// 绘制插件界面输出项
    /// </summary>
    private void DrawExport()
    {
        if (excelList == null) return;
        if (excelList.Count < 1)
        {
            EditorGUILayout.LabelField("路径中没有Excel文件!");
        }
        else
        {
            EditorGUILayout.LabelField("下列项目将被转换为" + formatOption[indexOfFormat] + ":");
            GUILayout.BeginVertical();
            scrollPos = GUILayout.BeginScrollView(scrollPos, false, true, GUILayout.Height(600));
            foreach (ExcelToolInfo s in excelList)
            {
                GUILayout.BeginHorizontal();
                s.bExport = GUILayout.Toggle(s.bExport, s.szFileName);
                s.szAssetBundle = GUILayout.TextField(s.szAssetBundle, GUILayout.Width(85));
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();

            //输出
            if (GUILayout.Button("转换"))
            {
                Convert();
            }
        }
    }

    /// <summary>
    /// 转换Excel文件
    /// </summary>
    private static void Convert()
    {
        DateTime now = DateTime.Now;

        foreach (ExcelToolInfo fileInfo in excelList)
        {
            //获取Excel文件的绝对路径
            string excelPath = fileInfo.pFileInfo.ToString();
            //Debug.Log("转换：" + excelPath);

            //构造Excel工具类
            ExcelUtility excel = new ExcelUtility(excelPath);

            //判断编码类型
            Encoding encoding = null;
            if (indexOfEncoding == 0 || indexOfEncoding == 3)
            {
                encoding = Encoding.GetEncoding("utf-8");
            }
            else if (indexOfEncoding == 1)
            {
                encoding = Encoding.GetEncoding("gb2312");
            }

            //判断输出类型
            string output = pathOutput + "/" + fileInfo.szFileName;
            string outputFolder = output.Substring(0, output.LastIndexOf('/'));
            //Debug.Log(outputFolder);
            if(!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            if (indexOfFormat == 0)
            {
                output = output.Replace(".xlsx", ".txt");
                excel.ConvertToTableTxt(output, isEncrypt, encoding);
            }
            else if (indexOfFormat == 1)
            {
                output = output.Replace(".xlsx", ".csv");
                excel.ConvertToCSV(output, encoding);
            }
            else if (indexOfFormat == 2)
            {
                output = output.Replace(".xlsx", ".xml");
                excel.ConvertToXml(output);
            }
            else if (indexOfFormat == 3)
            {
                output = excelPath.Replace(".xlsx", ".lua");
                excel.ConvertToLua(output, encoding);
            }

            //判断是否保留源文件
            if (!keepSource)
            {
                //FileUtil.DeleteFileOrDirectory(excelPath);
            }
        }

        //刷新本地资源
        AssetDatabase.Refresh();

        //设置Assetbundle
        foreach (ExcelToolInfo fileInfo in excelList)
        {
            if (!fileInfo.bExport) continue;

            string output = pathOutput + "/" + fileInfo.szFileName;
            output = output.Replace(".xlsx", ".txt");
            output = output.Substring(output.IndexOf("Assets"));

            AssetImporter importer = AssetImporter.GetAtPath(output);
            if (importer != null)
            {
                importer.assetBundleName = fileInfo.szAssetBundle;
            }
        }

        Debug.Log("Excel转换成功，耗时：" + (DateTime.Now - now).TotalMilliseconds + "ms");

        //转换完后关闭插件
        //这样做是为了解决窗口
        //再次点击时路径错误的Bug

        instance.Close();
    }

    void OnSelectionChange()
    {
        //当选择发生变化时重绘窗体
        Show();
        //LoadExcel();
        Repaint();
    }
}
