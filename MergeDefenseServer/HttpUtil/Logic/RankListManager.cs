using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

public class RankListManager
{
    public static int rankListMaxCount = 100;
    public static Dictionary<string,List<RankListData>> dic_list = new Dictionary<string,List<RankListData>>();

    public static void init()
    {
        loadRankList();

        // 每24小时备份一次
        TimerUtil.start(3600000 * 24, startDingShiReq);
    }

    public static void loadRankList()
    {
        for(int i = (int)RankType.Start + 1; i < (int)RankType.End; i++)
        {
            string rankTypeStr = ((RankType)i).ToString();
            dic_list[rankTypeStr] = getRedisRankList(rankTypeStr);
        }
    }

    static void startDingShiReq(object source, System.Timers.ElapsedEventArgs e)
    {
        RedisUtil.BgSave();
    }

    public static void submitData(ReqDataSubmitRankData reqDataObj)
    {
        string rankTypeStr = reqDataObj.rankType.ToString();

        // 已在榜
        for(int i = 0; i < dic_list[rankTypeStr].Count; i++)
        {
            if (dic_list[rankTypeStr][i].uid == reqDataObj.uid)
            {
                if(reqDataObj.score > dic_list[rankTypeStr][i].score)
                {
                    RankListData rankListData = new RankListData();
                    rankListData.uid = reqDataObj.uid;
                    rankListData.name = reqDataObj.name;
                    rankListData.score = reqDataObj.score;
                    rankListData.score2 = reqDataObj.score2;
                    dic_list[rankTypeStr][i] = rankListData;
                    changeRedisRankData(rankTypeStr, rankListData);
                    return;
                }
                else if (reqDataObj.score == dic_list[rankTypeStr][i].score)
                {
                    if (reqDataObj.score2 > dic_list[rankTypeStr][i].score2)
                    {
                        RankListData rankListData = new RankListData();
                        rankListData.uid = reqDataObj.uid;
                        rankListData.name = reqDataObj.name;
                        rankListData.score = reqDataObj.score;
                        rankListData.score2 = reqDataObj.score2;
                        dic_list[rankTypeStr][i] = rankListData;
                        changeRedisRankData(rankTypeStr, rankListData);
                        return;
                    }
                }

                // 分数没增加，不予处理
                return;
            }
        }

        // 不在榜单，且排行榜还没满
        if(dic_list[rankTypeStr].Count < rankListMaxCount)
        {
            RankListData rankListData = new RankListData();
            rankListData.uid = reqDataObj.uid;
            rankListData.name = reqDataObj.name;
            rankListData.score = reqDataObj.score;
            rankListData.score2 = reqDataObj.score2;
            dic_list[rankTypeStr].Add(rankListData);
            changeRedisRankData(rankTypeStr, rankListData);
            return;
        }

        // 不在榜单，且排行榜已满
        {
            RankListData minScoreData = dic_list[rankTypeStr][0];
            for (int i = 1; i < dic_list[rankTypeStr].Count; i++)
            {
                if (dic_list[rankTypeStr][i].score < minScoreData.score)
                {
                    minScoreData = dic_list[rankTypeStr][i];
                }
                else if (dic_list[rankTypeStr][i].score == minScoreData.score)
                {
                    if (dic_list[rankTypeStr][i].score2 < minScoreData.score2)
                    {
                        minScoreData = dic_list[rankTypeStr][i];
                    }
                }
            }

            bool isCover = false;
            if (reqDataObj.score > minScoreData.score)
            {
                isCover = true;
            }
            else if (reqDataObj.score == minScoreData.score)
            {
                if (reqDataObj.score2 > minScoreData.score2)
                {
                    isCover = true;
                }
            }

            if(isCover)
            {
                // 删除被顶替的玩家数据
                {
                    string key = "userRankData-" + rankTypeStr + "-" + minScoreData.uid;
                    RedisUtil.getDatabase().SetRemove(rankTypeStr, minScoreData.uid);
                    RedisUtil.getDatabase().KeyDelete(key);
                    dic_list[rankTypeStr].Remove(minScoreData);
                }

                RankListData rankListData = new RankListData();
                rankListData.uid = reqDataObj.uid;
                rankListData.name = reqDataObj.name;
                rankListData.score = reqDataObj.score;
                rankListData.score2 = reqDataObj.score2;
                dic_list[rankTypeStr].Add(rankListData);
                changeRedisRankData(rankTypeStr, rankListData);
            }
        }
    }

    static List<RankListData> getRedisRankList(string rankType)
    {
        List<RankListData> list = new List<RankListData>();
        if (RedisUtil.getDatabase().KeyExists(rankType))
        {
            var uidList = RedisUtil.getDatabase().SetMembers(rankType);
            for (int i = 0; i < uidList.Length; i++)
            {
                RankListData userData = getUserCacheData(rankType,uidList[i].ToString());
                if(userData != null)
                {
                    list.Add(userData);
                }
            }
            return list;
        }
        else
        {
            return list;
        }
    }

    static void changeRedisRankData(string rankType, RankListData rankListData)
    {
        if(RedisUtil.getDatabase().SetContains(rankType, rankListData.uid))
        {
            {
                string key = "userRankData-" + rankType + "-" + rankListData.uid;
                RedisUtil.getDatabase().SetRemove(rankType, rankListData.uid);
                RedisUtil.getDatabase().KeyDelete(key);
            }
            RedisUtil.getDatabase().SetAdd(rankType, rankListData.uid);
            setUserCacheData(rankType,rankListData);
        }
        else
        {
            RedisUtil.getDatabase().SetAdd(rankType, rankListData.uid);
            setUserCacheData(rankType, rankListData);
        }
    }

    static void setUserCacheData(string rankType, RankListData rankListData)
    {
        string key = "userRankData-" + rankType + "-" + rankListData.uid;
        HashEntry[] hashFields = new HashEntry[4];
        hashFields[0] = new HashEntry("uid", rankListData.uid);
        hashFields[1] = new HashEntry("name", rankListData.name);
        hashFields[2] = new HashEntry("score", rankListData.score);
        hashFields[3] = new HashEntry("score2", rankListData.score2);
        RedisUtil.getDatabase().HashSet(key, hashFields);
    }

    static RankListData getUserCacheData(string rankType, string uid)
    {
        string key = "userRankData-" + rankType + "-" + uid;
        if (RedisUtil.getDatabase().KeyExists(key))
        {
            RankListData userData = new RankListData();
            var values = RedisUtil.getDatabase().HashGetAll(key);
            userData.uid = values[0].Value;
            userData.name = values[1].Value;
            userData.score = (int)values[2].Value;
            userData.score2 = (long)values[3].Value;
            return userData;
        }
        else
        {
            return null;
        }
    }

    public static bool checkRankIsExist(string rankType)
    {
        for (int i = (int)RankType.Start + 1; i < (int)RankType.End; i++)
        {
            if(rankType == ((RankType)i).ToString())
            {
                return true;
            }
        }

        return false;
    }

    public static void deleteRankData(string rankType)
    {
        for (int i = 0; i < dic_list[rankType].Count; i++)
        {
            string key = "userRankData-" + rankType + "-" + dic_list[rankType][i].uid;
            RedisUtil.getDatabase().KeyDelete(key);
        }

        RedisUtil.DeleteKey(rankType);
        init();
    }
}