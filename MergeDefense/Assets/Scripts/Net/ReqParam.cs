using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum ServerInterface
{
    submitRankData,
    getRank,
    getTime,
}

public enum ServerCode
{
    OK = 1,
    ServerError,                // 服务器错误2
    ParamError,                 // 客户端传的参数错误3
    UnknownError = 100,         // 未知错误100
}

public enum RankType
{
    Start,
    GlobalRank,
    End,
}

public class RankListData
{
    public string uid = "";
    public string name = "";
    public int score = 0;
    public long score2 = 0;
}

//-------------------------------提交数据----------------------------------------------
public class ReqDataSubmitRankData
{
    public string rankType;
    public string uid = "";
    public string name = "";
    public int score = 0;
    public long score2 = 0;
}

public class BackDataSubmitRankData
{
    public string rankType;
    public ServerCode serverCode = ServerCode.OK;
    public string desc = "";
}

//-------------------------------获取排行榜----------------------------------------------
public class ReqDataGetRank
{
    public string rankType;
}

public class BackDataGetRank
{
    public string rankType;
    public ServerCode serverCode = ServerCode.OK;
    public List<RankListData> list;
    public string desc = "";
}

//-------------------------------获取时间戳----------------------------------------------
public class ReqDataGetTime
{
    public string key = "Haibara";
}

public class BackDataGetTime
{
    public long time = 0;
    public BackDataGetTime()
    {
        time = CommonUtil.getTimeStamp_Second();
    }
}
