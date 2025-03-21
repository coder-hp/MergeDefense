using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankLayer : MonoBehaviour
{
    public GameObject item_rank;
    public Transform list_content;

    List<RankListData> list_rank = new List<RankListData>();

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
                    getRankSuccess(backData.list);
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

    void getRankSuccess(List<RankListData> list)
    {
        Debug.Log("拉取排行榜成功：数据条数：" + list.Count);

        list_rank = list;

        // 先排序
        //for (int i = 0; i < list_rank.Count - 1; i++)
        //{
        //    for (int j = 0; j < list_rank.Count - i - 1; j++)
        //    {
        //        if (list_rank[j].score < list_rank[j + 1].score)
        //        {
        //            // 交换它们的位置
        //            RankListData temp = list_rank[j];
        //            list_rank[j] = list_rank[j + 1];
        //            list_rank[j + 1] = temp;
        //        }
        //        else if (list_rank[j].score == list_rank[j + 1].score)
        //        {
        //            if (list_rank[j].score2 < list_rank[j + 1].score2)
        //            {
        //                // 交换它们的位置
        //                RankListData temp = list_rank[j];
        //                list_rank[j] = list_rank[j + 1];
        //                list_rank[j + 1] = temp;
        //            }
        //        }
        //    }
        //}

        for (int i = 0; i < list.Count; i++)
        {
            Debug.Log(list[i].uid + "  " + list[i].score2);
            Transform itemTrans = Instantiate(item_rank, list_content).transform;
            itemTrans.Find("rank").GetComponent<Text>().text = (i + 1).ToString();
            itemTrans.Find("name").GetComponent<Text>().text = list[i].name;
            itemTrans.Find("wave").GetComponent<Text>().text = list[i].score.ToString();
            itemTrans.Find("damage").GetComponent<Text>().text = CommonUtil.numToStrKMB(list[i].score2);
        }
    }

    public void onClickClose()
    {
        AudioScript.s_instance.playSound_btn();
        Destroy(gameObject);
    }
}
