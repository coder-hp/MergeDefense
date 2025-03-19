using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class CipherTextUtil
{
    static string key = "";     // 秘钥：里面是6个特殊字符，看不见的

    // 加密数据
    public static string Encryption(string data)
    {
        string encryptionData = "";
        for (int i = 0; i < data.Length; i++)
        {
            encryptionData += (char)(((int)data[i]) ^ 9);
            //encryptionData += (char)(((int)data[i]) + (int)key[i % key.Length]);
        }

        return encryptionData;
    }

    // 解密数据
    public static string Decrypt(string data)
    {
        string decryptData = "";
        for (int i = 0; i < data.Length; i++)
        {
            decryptData += (char)(((int)data[i]) ^ 9);
            //decryptData += (char)(((int)data[i]) - (int)key[i % key.Length]);
        }

        return decryptData;
    }
}