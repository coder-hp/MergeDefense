using DG.Tweening;
using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMission : MonoBehaviour
{
    public static BattleMission s_instance = null;

    public bool isTakeMission = false;
    public BattleMissionData curMissionData = null;

    Animator animator;
    int firstComeTime = 41;         // 第一次船来的时间，默认41
    int repeatComeTime = 130;       // 后续船来的时间，默认130
    int boatComeAniTime = 7;
    int waitTakeMissionTime = 60;
    int doMissionTime = 30;

    Transform missionTrans = null;
    Text text_time;
    Text text_progress;

    int restDoMissionTime = 30;
    int curMissionProgress = 0;
    bool isCompleteMission = false;

    private void Awake()
    {
        s_instance = this;
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        Invoke("onInvokeNewMissnon", firstComeTime - boatComeAniTime);
    }

    void onInvokeNewMissnon()
    {
        isTakeMission = false;
        isCompleteMission = false;
        animator.Play("enter");
    }

    public void onBoatCome()
    {
        animator.Play("idle");
        int index = RandomUtil.SelectProbability(BattleMissionEntity.getInstance().list_weight);
        //index = 16;
        curMissionData = BattleMissionEntity.getInstance().list[index];
        Debug.Log("新任务：" + curMissionData.desc);

        if (missionTrans == null)
        {
            missionTrans = GameUILayer.s_instance.missionTrans;
            text_time = missionTrans.Find("timer/time").GetComponent<Text>();
            text_progress = missionTrans.Find("timer/progress").GetComponent<Text>();
        }

        curMissionProgress = 0;
        setMissionProgress(curMissionProgress);

        missionTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, transform.position) + new Vector2(0,65);
        missionTrans.localScale = Vector3.one;
        missionTrans.Find("newMission").localScale = Vector3.one;
        missionTrans.Find("timer").localScale = Vector3.zero;

        InvokeRepeating("onInvokeNewMissnon", repeatComeTime, repeatComeTime);
        Invoke("onInvokeTakeMissionTimeOut", waitTakeMissionTime);
    }

    // 超时未接取任务
    void onInvokeTakeMissionTimeOut()
    {
        curMissionData = null;
        missionTrans.localScale = Vector3.zero;
        animator.Play("out");
    }

    // 玩家接取任务
    public void takeMission()
    {
        CancelInvoke("onInvokeTakeMissionTimeOut");

        isTakeMission = true;
        restDoMissionTime = doMissionTime;

        missionTrans.Find("newMission").localScale = Vector3.zero;
        missionTrans.Find("timer").localScale = Vector3.one;

        InvokeRepeating("onInvokeSecond",1,1);

        switch (curMissionData.id)
        {
            case 9:
            case 10:
            case 11:
            case 12:
            case 13:
                {
                    checkWeaponMission();
                    break;
                }

            case 17:
                {
                    GameUILayer.s_instance.btn_summon_gold.text = GameFightData.s_instance.getCurSummonGold().ToString();
                    GameUILayer.s_instance.btn_summon_gold.transform.parent.Find("discount").localScale = Vector3.one;
                    break;
                }

            case 18:
                {
                    GameUILayer.s_instance.btn_forge_gold.text = GameFightData.s_instance.getCurForgeGold().ToString();
                    GameUILayer.s_instance.btn_forge_gold.transform.parent.Find("discount").localScale = Vector3.one;
                    break;
                }
        }
    }

    void setMissionProgress(int progress)
    {
        if(curMissionData.shortDesc != "")
        {
            text_progress.text = curMissionData.shortDesc;
        }
        else
        {
            switch (curMissionData.id)
            {
                case 1:
                case 2:
                case 6:
                case 7:
                case 8:
                    {
                        text_progress.text = progress + "/" + curMissionData.value;
                        break;
                    }

                case 3:
                case 4:
                case 5:
                    {
                        text_progress.text = CommonUtil.numToStrKMB(progress) + "/" + CommonUtil.numToStrKMB(curMissionData.value);
                        break;
                    }
            }
        }
    }

    public void addMissionProgress(int value)
    {
        curMissionProgress += value;
        if (curMissionProgress >= curMissionData.value)
        {
            curMissionProgress = curMissionData.value;
            completeMission();
        }
        setMissionProgress(curMissionProgress);
    }

    void completeMission()
    {
        if(isCompleteMission)
        {
            return;
        }

        ToastScript.show("任务完成");
        isCompleteMission = true;

        if (curMissionData.reward != "")
        {
            int rewardType = int.Parse(curMissionData.reward.Split('_')[0]);
            int rewardCount = int.Parse(curMissionData.reward.Split('_')[1]);

            if(rewardType == (int)Consts.RewardType.BattleGold)
            {
                GameUILayer.s_instance.changeGold(rewardCount);
            }
            else if (rewardType == (int)Consts.RewardType.BattleGem)
            {
                GameUILayer.s_instance.changeDiamond(rewardCount);
            }
        }
    }

    void onInvokeSecond()
    {
        // 任务时间到
        if(--restDoMissionTime <= 0)
        {
            CancelInvoke("onInvokeSecond");

            int mission_id = curMissionData.id;

            curMissionData = null;
            missionTrans.localScale = Vector3.zero;
            animator.Play("out");

            switch (mission_id)
            {
                case 17:
                    {
                        GameUILayer.s_instance.btn_summon_gold.text = GameFightData.s_instance.getCurSummonGold().ToString();
                        GameUILayer.s_instance.btn_summon_gold.transform.parent.Find("discount").localScale = Vector3.zero;
                        break;
                    }

                case 18:
                    {
                        GameUILayer.s_instance.btn_forge_gold.text = GameFightData.s_instance.getCurForgeGold().ToString();
                        GameUILayer.s_instance.btn_forge_gold.transform.parent.Find("discount").localScale = Vector3.zero;
                        break;
                    }
            }
        }

        text_time.text = restDoMissionTime + "s";
    }

    public void checkWeaponMission()
    {
        if(isCompleteMission)
        {
            return;
        }

        switch (curMissionData.id)
        {
            // 装备五把不同的武器
            case 9:
                {
                    Dictionary<int, int> dic = new Dictionary<int, int>();
                    for(int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
                    {
                        if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
                        {
                            dic[GameUILayer.s_instance.list_weaponBar[i].weaponData.type] = 1;
                        }
                    }
                    curMissionProgress = dic.Count;
                    text_progress.text = curMissionProgress + "/" + 5;

                    if(curMissionProgress >= curMissionData.value)
                    {
                        completeMission();
                    }

                    break;
                }

            case 10:
            case 11:
            case 12:
            case 13:
                {
                    Dictionary<int, int> dic = new Dictionary<int, int>();
                    for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
                    {
                        if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null)
                        {
                            if (dic.ContainsKey(GameUILayer.s_instance.list_weaponBar[i].weaponData.type))
                            {
                                ++dic[GameUILayer.s_instance.list_weaponBar[i].weaponData.type];
                            }
                            else
                            {
                                dic[GameUILayer.s_instance.list_weaponBar[i].weaponData.type] = 1;
                            }
                        }
                    }
                    curMissionProgress = 0;
                    foreach (KeyValuePair<int, int> kvp in dic)
                    {
                        if(kvp.Value > curMissionProgress)
                        {
                            curMissionProgress = kvp.Value;
                        }
                    }
                    text_progress.text = curMissionProgress + "/" + curMissionData.value;

                    if (curMissionProgress >= curMissionData.value)
                    {
                        completeMission();
                    }
                    break;
                }
        }
    }
}
