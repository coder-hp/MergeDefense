using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HP
{
    public class FileUtil
    {
        public static string ReadFileByLine(string file_path)
        {
            if (File.Exists(file_path))
            {
                using (StreamReader sr = new StreamReader(file_path, Encoding.UTF8))
                {
                    string data = sr.ReadToEnd();
                    sr.Close();
                    sr.Dispose();
                    return data;
                } 
            }

            return "";
        }

        public static void WriteFileByLine(string file_path, string str_info,bool isAppend)
        {
            using (StreamWriter sw = new StreamWriter(file_path, isAppend, Encoding.UTF8))
            {
                sw.WriteLine(str_info); sw.Close();
                sw.Dispose();
            }
        }
    }
}