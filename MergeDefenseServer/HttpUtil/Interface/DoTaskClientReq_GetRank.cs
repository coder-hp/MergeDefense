using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class DoTaskClientReq_GetRank
{
    public static void Do(HttpListenerContext requestContext, string reqData)
    {
        try
        {
            ReqDataGetRank reqDataObj = JsonConvert.DeserializeObject<ReqDataGetRank>(reqData);

            if (RankListManager.checkRankIsExist(reqDataObj.rankType))
            {
                BackDataGetRank data = new BackDataGetRank();
                data.rankType = reqDataObj.rankType;
                data.serverCode = ServerCode.OK;
                data.list = RankListManager.dic_list[reqDataObj.rankType];
                HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
            }
            else
            {
                BackDataGetRank data = new BackDataGetRank();
                data.serverCode = ServerCode.ParamError;
                data.desc = "Rank Not Exist";
                HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
            }
        }
        catch (Exception ex)
        {
            BackDataGetRank data = new BackDataGetRank();
            data.serverCode = ServerCode.UnknownError;
            data.desc = "未知错误";
            HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
            LogUtil.Log("ffff");
            return;
        }
    }
}