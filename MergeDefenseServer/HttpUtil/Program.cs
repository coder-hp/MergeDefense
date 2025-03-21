using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static void Main(string[] args)
    {
        // 连接Redis
        RedisUtil.Connect();

        try
        {
            RankListManager.init();
        }
        catch(Exception ex)
        {
            LogUtil.Log(ex);
        }
        
        HttpUtil.getInstance().connect();

        while (true)
        {
            string rankName = RankType.GlobalRank.ToString();

            LogUtil.Log("", false);
            LogUtil.Log("------------------------------------", false);
            LogUtil.Log("0:Clear Log", false);
            LogUtil.Log("1:Save All Data", false);
            LogUtil.Log("2:Delete All Data", false);
            LogUtil.Log("3:Add BlackList", false);
            LogUtil.Log("4:Remove BlackList", false);
            LogUtil.Log("5:Show BlackList", false);
            LogUtil.Log("6:Show " + rankName + " Data", false);
            LogUtil.Log("7:Delete " + rankName + " Data", false);
            LogUtil.Log("8:Insert " + rankName + " Data", false);
            LogUtil.Log("9:Check Redis Connect State", false);
            LogUtil.Log("------------------------------------", false);

            Console.Write("Wait Input:");
            string inputData = Console.ReadLine();

            // 清空Log
            if (inputData == "0")
            {
                Console.Clear();
            }
            // 保存所有数据
            else if (inputData == "1")
            {
                RedisUtil.BgSave();
            }
            // 清除所有数据
            else if (inputData == "2")
            {
                RedisUtil.ClearAllData();
            }
            // 增加黑名单
            else if (inputData == "3")
            {
                Console.Write("Input id:");
                string id = Console.ReadLine();
                BlackListManager.setBlacklist(id,true);
                Console.WriteLine("Added BlackList:" + id);
            }
            // 删除黑名单
            else if (inputData == "4")
            {
                Console.Write("输入id:");
                string id = Console.ReadLine();
                BlackListManager.setBlacklist(id, false);
                Console.WriteLine("Deleted BlackList:" + id);
            }
            // 显示黑名单
            else if (inputData == "5")
            {
                List<string> list = BlackListManager.getBlackList();
                for(int i = 0; i < list.Count; i++)
                {
                    Console.WriteLine(list[i]);
                }
            }
            // 读取最新活动数据
            else if (inputData == "6")
            {
                List<RankListData> list = RankListManager.dic_list[rankName];
                for (int i = 0; i < list.Count; i++)
                {
                    LogUtil.Log("***" + list[i].uid + "  " + list[i].score + "  " + list[i].score2);
                }
                LogUtil.Log("***数据条数：" + list.Count);
            }
            // 清除最新活动数据
            else if (inputData == "7")
            {
                RankListManager.deleteRankData(rankName);
                LogUtil.Log(rankName + " data is deleted");
            }
            // 插入最新活动数据
            else if (inputData == "8")
            {
                for(int i = 0; i < 150; i++)
                {
                    ReqDataSubmitRankData data = new ReqDataSubmitRankData();
                    data.rankType = RankType.GlobalRank.ToString();
                    data.uid = "uid-" + i;
                    data.name = "name-" + i;
                    data.score = i;
                    data.score2 = i * 10;
                    RankListManager.submitData(data);
                }
                LogUtil.Log(rankName + " data is inserted");
            }
            // 检查redis连接状态
            else if (inputData == "9")
            {
                if(RedisUtil.redisInstance == null || !RedisUtil.redisInstance.IsConnected)
                {
                    LogUtil.Log("Redis Not Connect");
                }
                else
                {
                    LogUtil.Log("Redis Connected");
                }
            }
        }
    }
}