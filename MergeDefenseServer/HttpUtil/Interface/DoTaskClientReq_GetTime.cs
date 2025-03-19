using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class DoTaskClientReq_GetTime
{
    public static void Do(HttpListenerContext requestContext, string reqData)
    {
        BackDataGetTime data = new BackDataGetTime();
        HttpUtil.getInstance().sendData(requestContext, JsonConvert.SerializeObject(data));
    }
}