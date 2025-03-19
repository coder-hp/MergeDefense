using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class BlackListManager
{
    public static void setBlacklist(string id, bool isBlock)
    {
        if (isBlock)
        {
            RedisUtil.getDatabase().SetAdd("blockList", id);

            // 从所有排行榜里删除该玩家
            for (int i = (int)RankType.Start + 1; i < (int)RankType.End; i++)
            {
                string rankTypeStr = ((RankType)i).ToString();
                string key = "userRankData-" + rankTypeStr + "-" + id;
                RedisUtil.getDatabase().SetRemove(rankTypeStr, id);
                RedisUtil.getDatabase().KeyDelete(key);
            }

            RankListManager.loadRankList();
        }
        else
        {
            RedisUtil.getDatabase().SetRemove("blockList", id);
        }
    }

    public static bool isInBlockList(string id)
    {
        return RedisUtil.getDatabase().SetContains("blockList", id);
    }

    public static List<string> getBlackList()
    {
        List<string> list = new List<string>();

        var members = RedisUtil.getDatabase().SetMembers("blockList");
        for(int i = 0; i < members.Length; i++)
        {
            list.Add(members[i]);
        }

        return list;
    }
}