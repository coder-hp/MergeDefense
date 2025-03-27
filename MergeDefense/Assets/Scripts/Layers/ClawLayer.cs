using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClawLayer : MonoBehaviour
{
    public GameObject prefab_ball;
    public Transform clawTrans;
    public Transform btn_claw;
    public Transform ballPointTrans;
    public Transform clawCenterTrans;

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
            if (++addedBallCount > 33)
            {
                CancelInvoke("onInvokeAddBall");
                Invoke("readyClaw", 1);
                return;
            }

            ClawData clawData = ClawEntity.getInstance().getRandomEgg();
            Transform ballTrans = Instantiate(prefab_ball, ballPointTrans).transform;
            if (RandomUtil.getRandom(1, 100) <= 50)
            {
                ballTrans.localPosition = new Vector3(-330 + RandomUtil.getRandom(-60, 60), 750 + RandomUtil.getRandom(0, 50), 0);
            }
            else
            {
                ballTrans.localPosition = new Vector3(330 + RandomUtil.getRandom(-60, 60), 750 + RandomUtil.getRandom(0, 50), 0);
            }
            ballTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_claw().GetSprite("ball_" + clawData.eggstyle);
            ballTrans.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            ballTrans.name = clawData.id.ToString();
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

        clawTrans.DOLocalMoveY(-250, 2).SetEase(Ease.Linear).OnComplete(()=>
        {
            Invoke("startUp",1);
        });
    }

    void startUp()
    {
        clawTrans.Find("left/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 1f);
        clawTrans.Find("right/downGanZi").DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 1f);
        clawTrans.DOLocalMoveY(startPos.y, 2).SetEase(Ease.Linear).OnComplete(()=>
        {
            CancelInvoke("checkDropBall");
        });

        InvokeRepeating("checkDropBall",1f,0.1f);
    }

    void checkDropBall()
    {
        for (int i = 0; i < ballPointTrans.childCount; i++)
        {
            if (Vector2.Distance(ballPointTrans.GetChild(i).position, clawCenterTrans.position) <= 0.8f)
            {
                Transform ballTrans = ballPointTrans.GetChild(i);
                
                if(!ballTrans.CompareTag("Finish"))
                {
                    ClawData clawData = ClawEntity.getInstance().getData(int.Parse(ballTrans.name));
                    if (RandomUtil.getRandom(1, 100) <= clawData.droprate)
                    {
                        Collider2D collider2D = ballTrans.GetComponent<Collider2D>();
                        collider2D.enabled = false;
                        ballTrans.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
                        TimerUtil.getInstance().delayTime(0.5f, () =>
                        {
                            collider2D.enabled = true;
                        });
                        break;
                    }
                    else
                    {
                        ballTrans.tag = "Finish";
                    }
                }
            }
        }
    }

    private void OnDisable()
    {
        AudioScript.s_instance.playMusic("bgm_main", true);
    }
}
