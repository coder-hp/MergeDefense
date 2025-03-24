using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMissionLayer : MonoBehaviour
{
    public Transform btn_take;
    public Transform timeTrans;
    public Transform rewardTrans;
    public Text text_desc;

    void Start()
    {
        if(BattleMission.s_instance.isTakeMission)
        {
            btn_take.localScale = Vector3.zero;
        }

        if(BattleMission.s_instance.curMissionData.reward == "")
        {
            timeTrans.localPosition = new Vector3(0,-120,0);
            rewardTrans.localScale = Vector3.zero;
        }
        else
        {
            int rewardType = int.Parse(BattleMission.s_instance.curMissionData.reward.Split('_')[0]);
            int rewardCount = int.Parse(BattleMission.s_instance.curMissionData.reward.Split('_')[1]);
            rewardTrans.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("RewardType" + rewardType);
            rewardTrans.Find("Text").GetComponent<Text>().text = rewardCount.ToString();
        }

        text_desc.text = BattleMission.s_instance.curMissionData.desc;

        LayerManager.LayerShowAni(transform.Find("bg"));
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

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(gameObject);
        });
    }

    public void onClickTake()
    {
        onClickClose();
        BattleMission.s_instance.takeMission();
    }
}
