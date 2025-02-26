using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    public static GameLayer s_instance = null;

    public List<Transform> enemyMoveFourPoint = new List<Transform>();
    public Camera camera3D;
    public Transform enemyPoint;
    public Transform heroPoint;
    public GameObject heroGrid;
    public Transform heroGridChoiced;
    public Transform attackRangeTrans;
    public Transform weaponChoiceKuang;
    public Transform heroQualityPoint;
    public Transform flyPoint;
    public Transform effectPoint;

    [HideInInspector]
    public List<Vector3> list_enemyMoveFourPos = new List<Vector3>();
    [HideInInspector]
    public int maxEnemyCount = 100;
    [HideInInspector]
    public int addedEnemyCount = 0;

    bool isMerging = false;

    private void Awake()
    {
        s_instance = this;
    }

    void Start()
    {
        for(int i = 0; i < enemyMoveFourPoint.Count; i++)
        {
            list_enemyMoveFourPos.Add(enemyMoveFourPoint[i].position);
        }

        LayerManager.ShowLayer(Consts.Layer.GameUILayer);
    }

    public void checkHeroMerge()
    {
        // 检测是否可以合并
        for (int i = 0; i < heroPoint.childCount; i++)
        {
            for (int j = 0; j < heroPoint.childCount; j++)
            {
                if(i != j)
                {
                    if (heroPoint.GetChild(i).childCount == 1 && heroPoint.GetChild(j).childCount == 1)
                    {
                        HeroLogicBase heroLogicBase1 = heroPoint.GetChild(i).GetChild(0).GetComponent<HeroLogicBase>();
                        HeroLogicBase heroLogicBase2 = heroPoint.GetChild(j).GetChild(0).GetComponent<HeroLogicBase>();
                        if ((heroLogicBase1.curStar < heroLogicBase1.heroData.maxStar) && (heroLogicBase1.heroData.id == heroLogicBase2.heroData.id) && (heroLogicBase1.curStar == heroLogicBase2.curStar))
                        {
                            heroLogicBase2.isMerge = true;
                            heroLogicBase2.GetComponent<BoxCollider>().enabled = false;
                            heroLogicBase2.transform.SetParent(transform);
                            Destroy(heroLogicBase2.starTrans.gameObject);
                            Destroy(heroLogicBase2.heroQualityTrans.gameObject);

                            float moveTime = Vector3.Distance(heroLogicBase1.transform.position, heroLogicBase2.transform.position) / 10f;
                            if(moveTime > 0.3f)
                            {
                                moveTime = 0.3f;
                            }

                            float jumpHight = Vector3.Distance(heroLogicBase1.transform.position, heroLogicBase2.transform.position) / 5f;
                            if (jumpHight < 0.5f)
                            {
                                jumpHight = 0.5f;
                            }
                            else if (jumpHight > 1)
                            {
                                jumpHight = 1f;
                            }

                            heroLogicBase2.transform.DOMove(heroLogicBase1.transform.position + new Vector3(0, jumpHight, 0), moveTime).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                heroLogicBase1.addStar();
                                EffectManager.heroMerge(heroLogicBase1.transform.position);

                                // 升星角色的合并动画
                                {
                                    Transform trans = heroLogicBase1.transform.Find("model");
                                    trans.DOLocalMoveY(jumpHight * 0.7f, moveTime * 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                                    {
                                        trans.DOLocalMoveY(0f, 0.1f).SetEase(Ease.InCubic);
                                    });

                                    trans.DOScaleY(1.3f, moveTime * 0.5f).SetEase(Ease.OutCubic).OnComplete(() =>
                                    {
                                        trans.DOScaleY(0.5f, 0.1f).SetEase(Ease.InCubic).OnComplete(() =>
                                        {
                                            trans.DOScaleY(1f, 0.1f).SetEase(Ease.OutCubic);
                                        });
                                    });
                                }

                                Destroy(heroLogicBase2.gameObject, moveTime * 0.5f);
                                Invoke("checkHeroMerge", 0.3f);
                            });

                            return;
                        }
                    }
                }
            }
        }

        isMerging = false;
    }

    public void addEnemy(EnemyWaveData enemyWaveData)
    {
        if (EnemyManager.s_instance.getEnemyCount() < maxEnemyCount)
        {
            ++addedEnemyCount;
            Instantiate(ObjectPool.getPrefab("Prefabs/Enemys/" + enemyWaveData.prefab), enemyPoint).GetComponent<EnemyLogic>().init(enemyWaveData);
        }
    }

    public bool addHero()
    {
        for (int i = 0; i < heroPoint.childCount; i++)
        {
            if (heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + RandomUtil.getRandom(101, 105)), heroPoint.GetChild(i)).transform;
                EffectManager.summonHero(heroTrans.position);

                if (!isMerging)
                {
                    isMerging = true;
                    Invoke("checkHeroMerge", 0.3f);
                }
                return true;
            }
        }

        return false;
    }

    public void addHeroById(int id)
    {
        for (int i = 0; i < heroPoint.childCount; i++)
        {
            if (heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + id), heroPoint.GetChild(i)).transform;
                EffectManager.summonHero(heroTrans.position);

                if (!isMerging)
                {
                    isMerging = true;
                    Invoke("checkHeroMerge", 0.3f);
                }
                return;
            }
        }
    }
}
