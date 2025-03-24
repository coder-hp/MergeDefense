using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleMission : MonoBehaviour
{
    public static BattleMission s_instance = null;

    public bool isTakeMission = false;
    public BattleMissionData curMissionData = null;

    int boatComeTime = 3;
    int waitTakeMissionTime = 60;
    int doMissionTime = 30;
    Vector3 boatScale = Vector3.one;
    Vector3 boatPos = Vector3.one;
    Vector3 boatAwayPos = Vector3.one;

    Transform missionTrans = null;
    Text text_time;
    Text text_progress;

    int restDoMissionTime = 30;
    int curMissionProgress = 0;

    private void Awake()
    {
        s_instance = this;
    }

    void Start()
    {
        boatScale = transform.localScale;
        boatPos = transform.position;
        boatAwayPos = boatPos + new Vector3(0, 4, 0);
        transform.localScale = Vector3.zero;
        Invoke("onInvokeNewMissnon",5 - boatComeTime);
    }

    void onInvokeNewMissnon()
    {
        isTakeMission = false;
        transform.position = boatAwayPos;
        transform.localScale = boatScale;
        transform.DOMove(boatPos, boatComeTime).OnComplete(() =>
        {
            int index = RandomUtil.SelectProbability(BattleMissionEntity.getInstance().list_weight);
            index = 0;
            curMissionData = BattleMissionEntity.getInstance().list[index];
            Debug.Log("新任务："+curMissionData.desc);
            
            if (missionTrans == null)
            {
                missionTrans = GameUILayer.s_instance.missionTrans;
                text_time = missionTrans.Find("timer/time").GetComponent<Text>();
                text_progress = missionTrans.Find("timer/progress").GetComponent<Text>();
            }

            curMissionProgress = 0;
            setMissionProgress(curMissionProgress);

            missionTrans.localPosition = CommonUtil.WorldPosToUI(GameLayer.s_instance.camera3D, boatPos);
            missionTrans.localScale = Vector3.one;
            missionTrans.Find("newMission").localScale = Vector3.one;
            missionTrans.Find("timer").localScale = Vector3.zero;

            InvokeRepeating("onInvokeNewMissnon",130,130);
            Invoke("onInvokeTakeMissionTimeOut", waitTakeMissionTime);
        });
    }

    // 超时未接取任务
    void onInvokeTakeMissionTimeOut()
    {
        curMissionData = null;
        missionTrans.localScale = Vector3.zero;
        transform.DOMove(boatAwayPos, boatComeTime).SetEase(Ease.Linear);
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
    }

    void setMissionProgress(int progress)
    {
        if(curMissionData.value == 0)
        {
            text_progress.text = "";
        }
        else
        {
            text_progress.text = progress + "/" + curMissionData.value;
        }
    }

    public void addMissionProgress()
    {
        ++curMissionProgress;
        if (curMissionProgress > curMissionData.value)
        {
            curMissionProgress = curMissionData.value;
        }
        setMissionProgress(curMissionProgress);
    }

    void onInvokeSecond()
    {
        if(--restDoMissionTime <= 0)
        {
            CancelInvoke("onInvokeSecond");

            curMissionData = null;
            missionTrans.localScale = Vector3.zero;
            transform.DOMove(boatAwayPos, boatComeTime).SetEase(Ease.Linear);
        }

        text_time.text = restDoMissionTime + "s";
    }
}
