using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class ExcelToJson : MonoBehaviour
{
    [MenuItem("HP/ExcelToJson/转换所有文件")]
    public static void start()
    {
        List<string> excelList = findAllExcelPath();

        for (int i = 0; i < excelList.Count; i++)
        {
            excelToJson(excelList[i]);
        }
        AssetDatabase.Refresh();
    }

    [MenuItem("HP/ExcelToJson/转换选中文件")]
    public static void ConvertChoiceFile()
    {
        if (Selection.objects.Length == 0)
        {
            Debug.LogError("先用鼠标选中文件");
            return;
        }

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            string path = AssetDatabase.GetAssetPath(Selection.objects[i]);
            path = path.Replace("Assets", Application.dataPath);
            if (!path.Contains(".meta"))
            {
                if (path.Contains(".csv"))
                {
                    excelToJson(path);
                }
                else
                {
                    Debug.LogError("不是CSV文件：" + path);
                }
            }
        }
        AssetDatabase.Refresh();
    }

    static List<string> findAllExcelPath()
    {
        List<string> excelList = new List<string>();
        string[] files = Directory.GetFiles(Application.dataPath + "/Excel");
        for (int i = 0; i < files.Length; i++)
        {
            if (!files[i].Contains(".meta"))
            {
                if (files[i].Contains(".csv"))
                {
                    excelList.Add(files[i]);
                }
            }
        }

        return excelList;
    }

    static void excelToJson(string path)
    {
        path = path.Replace("\\", "/");
        string fileName = "";
        string[] array = path.Split('/');
        fileName = array[array.Length - 1];
        fileName = fileName.Split('.')[0];

        Debug.Log("ExcelToJson--" + path);

        List<string> lineText = new List<string>();
        string data = HP.FileUtil.ReadFileByLine(path);
        data = data.Replace("\r", "");
        string temp = "";
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == '\n')
            {
                if (!string.IsNullOrEmpty(temp))
                {
                    lineText.Add(temp);
                    temp = "";
                }
            }
            else
            {
                temp += data[i];
            }
        }
        if (!string.IsNullOrEmpty(temp))
        {
            lineText.Add(temp);
        }

        int keyCount = lineText[0].Split(',').Length;
        string[] keyList = new string[keyCount];
        for (int i = 0; i < keyCount; i++)
        {
            keyList[i] = lineText[0].Split(',')[i];
        }

        JArray ja = new JArray();
        for (int i = 1; i < lineText.Count; i++)
        {
            string[] tempAray = lineText[i].Split(',');
            JObject tempJo = new JObject();
            for (int j = 0; j < keyList.Length; j++)
            {
                tempJo.Add(keyList[j], tempAray[j]);
            }
            ja.Add(tempJo);
        }

        // 导出json
        {
            string newJsonPath = Application.dataPath + "/Resources/Datas/" + fileName + ".json";
            if (File.Exists(newJsonPath))
            {
                File.Delete(newJsonPath);
            }

            string jsonData = ja.ToString();
            jsonData = jsonData.Replace("^", ",");
            //jsonData = jsonData.Replace("\x20", "");        // 去掉空格
            jsonData = jsonData.Replace("\x0d", "");        // 去掉回车
            jsonData = jsonData.Replace("\x0a", "");        // 去掉换行
            jsonData = jsonData.Replace("},", "},\n");
            Debug.Log("newJsonPath=" + newJsonPath);
            HP.FileUtil.WriteFileByLine(newJsonPath, jsonData, false);
        }
    }
}
