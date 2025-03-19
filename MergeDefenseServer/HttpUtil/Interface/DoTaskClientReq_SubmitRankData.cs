using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class DoTaskClientReq_SubmitRankData
{
    public static void Do(HttpListenerContext requestContext, string reqData)
    {
        try
        {
            ReqDataSubmitRankData reqDataObj = JsonConvert.DeserializeObject<ReqDataSubmitRankData>(reqData);
            if ((reqDataObj.uid == "") || (reqDataObj.rankType == ""))
            {
                BackDataSubmitRankData data = new BackDataSubmitRankData();
                data.serverCode = ServerCode.ParamError;
                data.rankType = reqDataObj.rankType;
                data.desc = "参数错误";
                HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));

                return;
            }
            else
            {
                // 检查排行是否生效
                {
                    bool isEnable = true;
                    string reason = "";

                    // 检查是否存在
                    if (!RankListManager.checkRankIsExist(reqDataObj.rankType))
                    {
                        isEnable = false;
                        reason = "RankNotExist";
                    }

                    if (!isEnable)
                    {
                        BackDataSubmitRankData data = new BackDataSubmitRankData();
                        data.serverCode = ServerCode.ParamError;
                        data.rankType = reqDataObj.rankType;
                        data.desc = reason;

                        HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));

                        return;
                    }
                }

                //// 分数超过上限，加入黑名单
                //if (reqDataObj.score > ConfigEntity.getInstance().data.blackScore)
                //{
                //    BlackListManager.setBlacklist(reqDataObj.uid,true);
                //}

                // 如果进入黑名单，则不处理
                if (BlackListManager.isInBlockList(reqDataObj.uid))
                {
                    BackDataSubmitRankData data = new BackDataSubmitRankData();
                    data.serverCode = ServerCode.UnknownError;
                    data.rankType = reqDataObj.rankType;

                    HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));

                    return;
                }

                if (reqDataObj.score > 0)
                {
                    RankListManager.submitData(reqDataObj);
                }

                {
                    BackDataSubmitRankData data = new BackDataSubmitRankData();
                    data.serverCode = ServerCode.OK;
                    data.rankType = reqDataObj.rankType;
                    HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
                }
            }
        }
        catch (Exception ex)
        {
            BackDataSubmitRankData data = new BackDataSubmitRankData();
            data.serverCode = ServerCode.UnknownError;
            data.desc = "未知错误";
            HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
            return;
        }
    }
}