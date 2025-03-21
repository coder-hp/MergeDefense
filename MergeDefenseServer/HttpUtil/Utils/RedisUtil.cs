using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RedisUtil
{
    //static string url = "127.0.0.1:6379,password=582254172";
    static string url = "";
    public static ConnectionMultiplexer redisInstance = null;
    static IDatabase database = null;

    public static void Connect()
    {
        LogUtil.Log("Start Connect Redis");

        try
        {
            url = ConfigEntity.getInstance().data.redis_url;
            redisInstance = ConnectionMultiplexer.Connect(url);
        }
        catch (Exception ex)
        {
            LogUtil.Log("Connect Redis Fail:" + ex);
            return;
        }

        LogUtil.Log("Connect Redis Success");

        database = redisInstance.GetDatabase(0);
    }

    public static IDatabase getDatabase()
    {
        if (redisInstance == null || !redisInstance.IsConnected)
        {
            Connect();
        }
        return database;
    }

    public static void BgSave()
    {
        if (redisInstance == null || !redisInstance.IsConnected)
        {
            LogUtil.Log("Redis Disconnect,Save Fail");
            Connect();
            return;
        }
        LogUtil.Log("Redis Start Save");
        database.Execute("BGSAVE");
        LogUtil.Log("Redis Save End");
    }

    public static void DeleteKey(string key)
    {
        database.KeyDelete(key);
    }

    public static void ClearAllData()
    {
        LogUtil.Log("Redis Start Delete All Data");
        database.Execute("FLUSHDB");
        LogUtil.Log("Redis Delete All Data End");
    }
}