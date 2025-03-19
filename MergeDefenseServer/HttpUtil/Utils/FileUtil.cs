using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

class FileUtil
{
    public static string readFile(string path)
    {
        StreamReader sr = new StreamReader(path, System.Text.Encoding.GetEncoding("utf-8"));
        string data = sr.ReadToEnd().ToString();
        sr.Close();

        return data;
    }

    public static void WriteFile(string path, string data)
    {
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
        {
            using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
            {
                sw.WriteLine(data);
                sw.Close();
            }
        }
    }

    public static void changeFileName(string oldPath, string newPath)
    {
        if (File.Exists(oldPath))
        {
            File.Move(oldPath, newPath);
        }
    }
}
