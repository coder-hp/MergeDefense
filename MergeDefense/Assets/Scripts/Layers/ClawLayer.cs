using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClawLayer : MonoBehaviour
{
    public GameObject prefab_ball;
    public GameObject prefab_splitBall;
    public Transform clawTrans;
    public Transform ballPointTrans;
    public Transform clawCenterTrans;
    public Transform rewardPanel;
    public Transform rewardGrid;
    public Button btn_claw;
    public Image img_jiantouLeft;
    public Image img_jiantouRight;

    Vector3 startPos;
    Sequence seq_claw;

    void Start()
    {
        startPos = clawTrans.localPosition;
        InvokeRepeating("onInvokeAddBall",0.1f,0.1f);

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
            ballTrans.Find("up").GetComponent<Image>().sprite = AtlasUtil.getAtlas_claw().GetSprite("ball_" + clawData.eggstyle + "_1");
            ballTrans.Find("down").GetComponent<Image>().sprite = AtlasUtil.getAtlas_claw().GetSprite("ball_" + clawData.eggstyle + "_2");
            ballTrans.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -10);
            ballTrans.name = clawData.id.ToString();

            if(clawData.eggstyle == 4)
            {
                ballTrans.localScale = new Vector3(1.3f, 1.3f, 1.3f);

                List<int> list_hero = new List<int>();
                for(int j = 0; j < HeroEntity.getInstance().list.Count; j++)
                {
                    if (HeroEntity.getInstance().list[j].quality == 4)
                    {
                        if (!GameData.isUnlockHero(HeroEntity.getInstance().list[i].id))
                        {
                            list_hero.Add(HeroEntity.getInstance().list[j].id);
                        }
                    }
                }
                if(list_hero.Count == 0)
                {
                    for (int j = 0; j < HeroEntity.getInstance().list.Count; j++)
                    {
                        if (HeroEntity.getInstance().list[j].quality == 4)
                        {
                            list_hero.Add(HeroEntity.getInstance().list[j].id);
                        }
                    }
                }
                int heroId = list_hero[RandomUtil.getRandom(0, list_hero.Count - 1)];

                Transform iconTrans = ballTrans.Find("icon");
                iconTrans.localScale = Vector3.one;
                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + heroId);
                iconTrans.name = heroId.ToString();
            }
        }
    }

    Color color_alpha = new Color(1, 1, 1, 0.5f);
    void readyClaw()
    {
        btn_claw.interactable = true;
        img_jiantouRight.color = Color.white;

        seq_claw = DOTween.Sequence();
        seq_claw.Append(clawTrans.DOLocalMoveX(158, 1).OnComplete(()=>
                {
                    img_jiantouLeft.color = Color.white;
                    img_jiantouRight.color = color_alpha;
                }).SetEase(Ease.Linear))
                .Append(clawTrans.DOLocalMoveX(-158, 2).OnComplete(() =>
                {
                    img_jiantouLeft.color = color_alpha;
                    img_jiantouRight.color = Color.white;
                }).SetEase(Ease.Linear))
                .Append(clawTrans.DOLocalMoveX(0, 1).SetEase(Ease.Linear)).SetLoops(-1);
    }

    public void onClickClaw()
    {
        AudioScript.s_instance.playSound("clawDown");
        btn_claw.interactable = false;
        seq_claw.Kill();
        img_jiantouLeft.color = color_alpha;
        img_jiantouRight.color = color_alpha;

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
            Invoke("showReward",1);
        });

        InvokeRepeating("checkDropBall",1f,0.1f);
    }

    void checkDropBall()
    {
        for (int i = 0; i < ballPointTrans.childCount; i++)
        {
            if (Vector2.Distance(ballPointTrans.GetChild(i).position, clawCenterTrans.position) <= 1f)
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

    void showReward()
    {
        rewardPanel.localScale = Vector3.one;

        int eggCount = 0;
        for (int i = 0; i < ballPointTrans.childCount; i++)
        {
            if (Vector2.Distance(ballPointTrans.GetChild(i).position, clawCenterTrans.position) <= 1f)
            {
                ++eggCount;
                Transform ballTrans = ballPointTrans.GetChild(i);
                ClawData clawData = ClawEntity.getInstance().getData(int.Parse(ballTrans.name));
                Transform trans = Instantiate(prefab_splitBall, rewardGrid).transform;
                trans.Find("up").GetComponent<Image>().sprite = AtlasUtil.getAtlas_claw().GetSprite("ball_" + clawData.eggstyle + "_1");
                trans.Find("down").GetComponent<Image>().sprite = AtlasUtil.getAtlas_claw().GetSprite("ball_" + clawData.eggstyle + "_2");
                Transform iconTrans = trans.Find("icon");
                if (clawData.eggstyle == 4)
                {
                    trans.Find("up").localScale = Vector3.zero;
                    trans.Find("light").localScale = Vector3.one;
                    trans.Find("light").DORotate(new Vector3(0f, 0f, -360), 4, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
                    iconTrans.localScale = Vector3.one;
                    int heroId = int.Parse(ballTrans.GetChild(1).name);
                    if(GameData.isUnlockHero(heroId))
                    {
                        iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("heroSuiPian4");
                        GameData.changeHeroSuiPian(4, 5);
                        iconTrans.Find("count").GetComponent<Text>().text = "x5";
                    }
                    else
                    {
                        iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + heroId);
                        GameData.changeHeroSuiPian(1, clawData.count);
                        iconTrans.Find("count").GetComponent<Text>().text = HeroEntity.getInstance().getData(heroId).name;
                    }
                }
                else
                {
                    float shakeTime = 0.05f;
                    Sequence seq = DOTween.Sequence();
                    seq.Append(trans.DOLocalRotateQuaternion(Quaternion.Euler(0,0,25), shakeTime).SetEase(Ease.Linear))
                       .Append(trans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, -25), shakeTime * 2).SetEase(Ease.Linear))
                       .Append(trans.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), shakeTime).SetEase(Ease.Linear)).SetLoops(2);

                    switch (clawData.rewardtype)
                    {
                        case (int)Consts.RewardType.Gold:
                            {
                                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("RewardType" + clawData.rewardtype);
                                GameData.changeMyGold(clawData.count,"claw");
                                iconTrans.Find("count").GetComponent<Text>().text = "x" + clawData.count;
                                break;
                            }

                        case (int)Consts.RewardType.Diamond:
                            {
                                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("RewardType" + clawData.rewardtype);
                                GameData.changeMyDiamond(clawData.count);
                                iconTrans.Find("count").GetComponent<Text>().text = "x" + clawData.count;
                                break;
                            }

                        case (int)Consts.RewardType.SuiPianBai:
                            {
                                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("heroSuiPian" + 1);
                                GameData.changeHeroSuiPian(1, clawData.count);
                                iconTrans.Find("count").GetComponent<Text>().text = "x" + clawData.count;
                                break;
                            }

                        case (int)Consts.RewardType.SuiPianLan:
                            {
                                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("heroSuiPian" + 2);
                                GameData.changeHeroSuiPian(2, clawData.count);
                                iconTrans.Find("count").GetComponent<Text>().text = "x" + clawData.count;
                                break;
                            }

                        case (int)Consts.RewardType.SuiPianZi:
                            {
                                iconTrans.GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("heroSuiPian" + 3);
                                GameData.changeHeroSuiPian(3, clawData.count);
                                iconTrans.Find("count").GetComponent<Text>().text = "x" + clawData.count;
                                break;
                            }
                    }
                }
                ballTrans.localScale = Vector3.zero;
                trans.Find("up").GetComponent<Image>().DOFade(0, 0.3f).SetDelay(1).OnComplete(()=>
                {
                    iconTrans.DOScale(1, 0.2f);
                });
            }
        }

        if (eggCount == 0)
        {
            Invoke("close", 2);
        }
        else
        {
            TimerUtil.getInstance().delayTime(1.3f, () =>
            {
                AudioScript.s_instance.playSound("eggOpen");
            });
            Invoke("close", 4);
        }
    }

    void close()
    {
        Destroy(gameObject);
        AudioScript.s_instance.playMusic("bgm_main", true);
    }
}
