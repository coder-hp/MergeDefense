using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class JsonUtils
{
    static public List<T> loadJsonToList<T>(string jsonName)
    {
        string path = "datas/" + jsonName;
        TextAsset jsonData = Resources.Load<TextAsset>(path);
        return JsonConvert.DeserializeObject<List<T>>(jsonData.ToString());
	}
}
