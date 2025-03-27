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

    int addedBallCount = 0;
    void onInvokeAddBall()
    {
        for(int i = 0; i < 3; i++)
        {
            Transform ballTrans = Instantiate(prefab_ball, ballPointTrans).transform;
            if (RandomUtil.getRandom(1, 100) <= 50)
            {
                ballTrans.localPosition = new Vector3(-330 + RandomUtil.getRandom(-60, 60), 750 + RandomUtil.getRandom(0, 50), 0);
            }
            else
            {
                ballTrans.localPosition = new Vector3(330 + RandomUtil.getRandom(-60, 60), 750 + RandomUtil.getRandom(0, 50), 0);
            }
            ballTrans.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
        }

        addedBallCount += 3;

        if (addedBallCount > 33)
        {
            CancelInvoke("onInvokeAddBall");
            Invoke("readyClaw",1);
        }
    }

    void readyClaw()
    {
        btn_claw.localScale = Vector3.one;
        seq_claw = DOTween.Sequence();
        seq_claw.Append(clawTrans.DOLocalMoveX(158, 1).SetEase(Ease.Linear))
           .Append(clawTrans.DOLocalMoveX(-158, 2).SetEase(Ease.Linear))
           .Append(clawTrans.DOLocalMoveX(0, 1).SetEase(Ease.Linear)).SetLoops(-1);
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound_btn();
        seq_claw.Kill();

        clawTrans.Find("left/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -100), 0.5f);
        clawTrans.Find("right/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -100), 0.5f);

        clawTrans.DOLocalMoveY(-230, 2).SetEase(Ease.Linear).OnComplete(()=>
        {
            clawTrans.Find("left/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0,0, 0),0.5f).SetDelay(1);
            clawTrans.Find("right/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.5f).SetDelay(1).OnComplete(()=>
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
