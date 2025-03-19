using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using Newtonsoft.Json;

class HttpUtil
{
    static HttpUtil s_httpUtil = null;

    string url = "";
    private static HttpListener httpRequest;   // 请求监听  
    private static HttpListenerContext requestContext;

    bool isShowLog = false;

    public static HttpUtil getInstance()
    {
        if (s_httpUtil == null)
        {
            s_httpUtil = new HttpUtil();
        }

        return s_httpUtil;
    }

    public void connect()
    {
        try
        {
            httpRequest = new HttpListener();
                
            // 注册接口
            {
                // 地址
                url = ConfigEntity.getInstance().data.server_url;

                // 接口
                string[] interface_array = ConfigEntity.getInstance().data.server_interface.Split(';');
                for (int i = 0; i < interface_array.Length; i++)
                {
                    if (interface_array[i] != "")
                    {
                        string urlPath = url + interface_array[i] + "/";
                        httpRequest.Prefixes.Add(urlPath);
                        LogUtil.Log("Listen Url    " + urlPath);
                    }
                }                
            }

            httpRequest.Start();        //允许该监听地址接受请求的传入
            httpRequest.BeginGetContext(Result, null);

            LogUtil.Log("Start Listen Client Req:", true);
        }
        catch (Exception ex)
        {
            LogUtil.Log("HttpUtil.connect Error----" + ex.ToString());
        }
    }

    void Result(IAsyncResult ar)
    {
        //继续异步监听
        httpRequest.BeginGetContext(Result, null);

        try
        {
            //获得context对象
            HttpListenerContext requestContext = httpRequest.EndGetContext(ar);
            var request = requestContext.Request;

            string jiekou = requestContext.Request.Url.AbsolutePath;

            if (request.HttpMethod == "POST" && request.InputStream != null)
            {
                Stream stream = requestContext.Request.InputStream;
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string reqData = reader.ReadToEnd();

                if (isShowLog)
                {
                    LogUtil.Log("Receive POST Req--" + jiekou + "  reqData=" + reqData);
                }

                DoTaskClientReq.Do(requestContext, reqData);
            }
            else if (request.HttpMethod == "GET")
            {
                if (isShowLog)
                {
                    string reqData = HttpUtility.UrlDecode(requestContext.Request.QueryString["param"], Encoding.UTF8);

                    if (isShowLog)
                    {
                        LogUtil.Log("Receive GET Req--" + jiekou + "  reqData=" + reqData);
                    }

                    DoTaskClientReq.Do(requestContext, reqData);
                }
            }
        }
        catch (Exception ex)
        {
            LogUtil.Log("HttpUtil.Result----" + ex.ToString());
        }
    } 

    public void sendData(HttpListenerContext request,string backData)
    {
        try
        {
            //Thread.Sleep(5000);

            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(backData);
            //request.Response.ContentLength64 = buffer.Length;
            request.Response.ContentType = "application/json;charset=UTF-8";
            request.Response.ContentEncoding = Encoding.UTF8;
            Stream output = request.Response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();

            if (isShowLog)
            {
                LogUtil.Log("Send To Client Data:" + backData, true);
                //LogUtil.Log("已回复", true);
            }
        }
        catch (Exception ex)
        {
            //LogUtil.Log("HttpUtil.sendData异常----" + ex.ToString());
        }
    }
}