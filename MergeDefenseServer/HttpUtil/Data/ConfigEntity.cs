using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ConfigData
{
    public string server_url;
    public string server_interface;
    public string redis_url = "";
}

public class ConfigEntity
{
    static ConfigEntity s_instance = null;
    public ConfigData data;

    public static ConfigEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new ConfigEntity();
            s_instance.init();
        }

        return s_instance;
    }

    void init()
    {
        string fileContent = FileUtil.readFile("data/config.json");
        data = JsonConvert.DeserializeObject<ConfigData>(fileContent);
    }
}