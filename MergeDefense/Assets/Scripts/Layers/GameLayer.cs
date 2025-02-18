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
    public Transform heroQualityPoint;
    public Transform flyPoint;
    public Transform effectPoint;

    [HideInInspector]
    public List<Vector3> list_enemyMoveFourPos = new List<Vector3>();
    [HideInInspector]
    public int maxEnemyCount = 100;

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
                            heroLogicBase2.transform.DOMove(heroLogicBase1.transform.position, 0.5f).OnComplete(() =>
                            {
                                heroLogicBase1.addStar();
                                EffectManager.heroMerge(heroLogicBase1.transform.position);
                                Destroy(heroLogicBase2.gameObject);
                                Invoke("checkHeroMerge", 0.5f);
                            });
                            return;
                        }
                    }
                }
            }
        }

        isMerging = false;
    }

    public void addEnemy(int id)
    {
        if (EnemyManager.s_instance.getEnemyCount() < maxEnemyCount)
        {
            Instantiate(ObjectPool.getPrefab("Prefabs/Enemys/enemy" + id), enemyPoint);
        }
    }

    public void addHero()
    {
        for(int i = 0; i < heroPoint.childCount; i++)
        {
            if(heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + RandomUtil.getRandom(101,102)), heroPoint.GetChild(i)).transform;

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
