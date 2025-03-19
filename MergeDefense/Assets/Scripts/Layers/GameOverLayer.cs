using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverLayer : MonoBehaviour
{
    public Text text_wave;
    public Text text_damage;

    void Start()
    {
        Time.timeScale = 1;
        AudioScript.s_instance.playSound("gameOver");
        text_wave.text = GameFightData.s_instance.curBoCi.ToString();
        text_damage.text = CommonUtil.numToStrKMB(GameFightData.s_instance.allDamage);

        {
            string str = GameData.getMaxWaveDamage();
            int beforeMaxWave = int.Parse(str.Split('_')[0]);
            long beforeMaxDamage = long.Parse(str.Split('_')[1]);

            if(GameFightData.s_instance.curBoCi > beforeMaxWave)
            {
                GameData.setMaxWaveDamage(GameFightData.s_instance.curBoCi, GameFightData.s_instance.allDamage);
                submitRankData(GameFightData.s_instance.curBoCi, GameFightData.s_instance.allDamage);
            }
            else if (GameFightData.s_instance.curBoCi == beforeMaxWave)
            {
                if (GameFightData.s_instance.allDamage > beforeMaxDamage)
                {
                    GameData.setMaxWaveDamage(GameFightData.s_instance.curBoCi, GameFightData.s_instance.allDamage);
                    submitRankData(GameFightData.s_instance.curBoCi, GameFightData.s_instance.allDamage);
                }
            }
        }

        LayerManager.LayerShowAni(transform.Find("bg"));
    }

    void submitRankData(int wave,long damage)
    {
        ReqDataSubmitRankData reqData = new ReqDataSubmitRankData();
        reqData.rankType = RankType.GlobalRank.ToString();
        reqData.uid = GameData.getUID();
        reqData.name = GameData.getName();
        reqData.score = wave;
        reqData.score2 = damage;
        string reqDataStr = JsonConvert.SerializeObject(reqData);
        HttpUtil.s_instance.reqPost(Consts.getServerUrl() + ServerInterface.submitRankData.ToString(), reqDataStr, (result, data) =>
        {
            if (result)
            {
                BackDataSubmitRankData backData = JsonConvert.DeserializeObject<BackDataSubmitRankData>(data);
                if (backData.serverCode == ServerCode.OK)
                {
                    Debug.Log("排行数据提交成功");
                }
                else
                {
                    Debug.Log("排行数据提交失败1：" + backData.desc);
                }
            }
            else
            {
                Debug.Log("排行数据提交失败2：" + data);
            }
        });
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if (isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        for(int i = 0; i < LaunchScript.s_instance.canvas.transform.childCount; i++)
        {
            if(LaunchScript.s_instance.canvas.transform.GetChild(i) != transform)
            {
                Destroy(LaunchScript.s_instance.canvas.transform.GetChild(i).gameObject);
            }
        }

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(GameLayer.s_instance.gameObject);
            Destroy(gameObject);

            LayerManager.ShowLayer(Consts.Layer.MainLayer);
        });
    }
}
