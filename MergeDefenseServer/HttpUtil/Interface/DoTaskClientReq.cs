using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

class DoTaskClientReq
{
    public static void Do(HttpListenerContext requestContext, string reqData)
    {
        string jiekou = requestContext.Request.Url.AbsolutePath;
        try
        {
            //LogUtil.Log("收到请求原始数据--" + reqData);
            //reqData = CipherTextUtil.Decrypt(reqData);
            //LogUtil.Log("收到请求解密数据--" + jiekou + "  reqData=" + reqData);
            JsonConvert.DeserializeObject(reqData);
        }
        catch(Exception ex)
        {
            // 请求参数无法解析成json，不予处理
            // LogUtil.Log("请求参数无法解析成json，不予处理");
            return;
        }

        //LogUtil.Log("客户端请求接口--" + jiekou);

        if (("/" + ServerInterface.submitRankData.ToString()).Equals(jiekou))
        {
            DoTaskClientReq_SubmitRankData.Do(requestContext, reqData);
        }
        else if (("/" + ServerInterface.getRank.ToString()).Equals(jiekou))
        {
            DoTaskClientReq_GetRank.Do(requestContext, reqData);
        }
        else if (("/" + ServerInterface.getTime.ToString()).Equals(jiekou))
        {
            DoTaskClientReq_GetTime.Do(requestContext, reqData);
        }
        else
        {
            //LogUtil.Log("未知接口--" + jiekou);
        }
    }
}