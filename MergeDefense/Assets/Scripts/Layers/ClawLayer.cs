using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLayer : MonoBehaviour
{
    public GameObject prefab_ball;
    public Transform clawTrans;
    public Transform ballPointTrans;

    float moveSpeed = 2;

    void Start()
    {
        InvokeRepeating("onInvokeAddBall",0.2f,0.2f);
    }

    public void clawMove(bool isLeft)
    {
        if(isLeft)
        {
            clawTrans.Translate(-new Vector3(moveSpeed * Time.deltaTime,0,0));
        }
        else
        {
            clawTrans.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
        }
    }

    int addedBallCount = 0;
    void onInvokeAddBall()
    {
        Transform ballTrans =  Instantiate(prefab_ball, ballPointTrans).transform;
        ballTrans.localPosition = new Vector3(RandomUtil.getRandom(-400,400),450,0);

        if(++addedBallCount > 20)
        {
            CancelInvoke("onInvokeAddBall");
        }
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound_btn();
        clawTrans.DOLocalMoveY(-600, 3).SetEase(Ease.Linear).OnComplete(()=>
        {
            clawTrans.Find("left").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0,0, 90),0.5f).SetDelay(1);
            clawTrans.Find("right").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -90), 0.5f).SetDelay(1).OnComplete(()=>
            {
                clawTrans.DOLocalMoveY(860, 3).SetDelay(1).SetEase(Ease.Linear);
            });
        });
    }
}
