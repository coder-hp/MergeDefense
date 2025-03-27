using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClawLayer : MonoBehaviour
{
    public GameObject prefab_ball;
    public Transform clawTrans;
    public Transform btn_claw;
    public Transform ballPointTrans;

    float moveSpeed = 2;
    Vector3 startPos;
    Sequence seq_claw;

    private void Awake()
    {
        btn_claw.localScale = Vector3.zero;
    }

    void Start()
    {
        startPos = clawTrans.localPosition;
        InvokeRepeating("onInvokeAddBall",0.1f,0.1f);
    }

    private void OnEnable()
    {
        AudioScript.s_instance.playMusic("bgm_claw", true);
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

        if(++addedBallCount > 50)
        {
            btn_claw.localScale = Vector3.one;
            CancelInvoke("onInvokeAddBall");

            seq_claw = DOTween.Sequence();
            seq_claw.Append(clawTrans.DOLocalMoveX(250,1).SetEase(Ease.Linear))
               .Append(clawTrans.DOLocalMoveX(-250, 2).SetEase(Ease.Linear))
               .Append(clawTrans.DOLocalMoveX(0, 1).SetEase(Ease.Linear)).SetLoops(-1);
        }
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound_btn();
        seq_claw.Kill();
        clawTrans.DOLocalMoveY(-250, 2).SetEase(Ease.Linear).OnComplete(()=>
        {
            clawTrans.Find("left").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0,0, 100),0.5f).SetDelay(1);
            clawTrans.Find("right").GetChild(0).DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -100), 0.5f).SetDelay(1).OnComplete(()=>
            {
                clawTrans.DOLocalMoveY(startPos.y, 3).SetDelay(1).SetEase(Ease.Linear);
            });
        });
    }

    private void OnDisable()
    {
        AudioScript.s_instance.playMusic("bgm_main", true);
    }
}
