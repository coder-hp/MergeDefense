using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMission : MonoBehaviour
{
    float boatComeTime = 3;
    float waitTakeMissionTime = 10;
    Vector3 boatScale = Vector3.one;
    Vector3 boatPos = Vector3.one;
    Vector3 boatAwayPos = Vector3.one;

    Transform missionTrans = null;

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
        transform.position = boatAwayPos;
        transform.localScale = boatScale;
        transform.DOMove(boatPos, boatComeTime).OnComplete(() =>
        {
            if(missionTrans == null)
            {
                missionTrans = GameUILayer.s_instance.missionTrans;
            }
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
        missionTrans.localScale = Vector3.zero;
        transform.DOMove(boatAwayPos, boatComeTime).SetEase(Ease.Linear);
    }
}
