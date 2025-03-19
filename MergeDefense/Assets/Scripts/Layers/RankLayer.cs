using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankLayer : MonoBehaviour
{
    List<RankListData> list;
    void Start()
    {
        ReqDataGetRank reqData = new ReqDataGetRank();
        reqData.rankType = RankType.GlobalRank.ToString();
        string reqDataStr = JsonConvert.SerializeObject(reqData);
        HttpUtil.s_instance.reqPost(Consts.getServerUrl() + ServerInterface.getRank.ToString(), reqDataStr ,(result, data) =>
        {
            if(result)
            {
                BackDataGetRank backData = JsonConvert.DeserializeObject<BackDataGetRank>(data);
                if (backData.serverCode == ServerCode.OK)
                {
                    list = backData.list;
                    getRankSuccess();
                }
                else
                {
                    Debug.Log("请求失败1：" + backData.desc);
                }
            }
            else
            {
                Debug.Log("请求失败2：" + data);
            }
        });
    }

    void getRankSuccess()
    {
        Debug.Log("拉去排行榜成功：数据条数：" + list.Count);
        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i].name + "  " + list[i].score + "  " + list[i].score2);
        }
    }
}
