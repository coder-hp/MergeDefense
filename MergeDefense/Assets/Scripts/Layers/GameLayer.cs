using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameLayer : MonoBehaviour
{
    public static GameLayer s_instance = null;

    public List<Transform> enemyMoveFourPoint = new List<Transform>();
    public Camera camera3D;
    public Camera cameraEffect;
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
    public Material matrial_attackRange;

    private void Awake()
    {
        s_instance = this;

        // 屏幕适配
        if((Screen.width / (float)Screen.height) < (1080f / 1920f))
        {
            float standardValue = (1080f / 1920f) * 9.6f;
            float cameraSize = standardValue / ((float)Screen.width / (float)Screen.height);
            camera3D.orthographicSize = cameraSize;
            cameraEffect.orthographicSize = cameraSize;
        }
    }

    void Start()
    {
        for (int i = 0; i < enemyMoveFourPoint.Count; i++)
        {
            list_enemyMoveFourPos.Add(enemyMoveFourPoint[i].position);
        }

        matrial_attackRange = attackRangeTrans.GetComponent<MeshRenderer>().material;

        LayerManager.ShowLayer(Consts.Layer.GameUILayer);
    }

    public void addEnemy(EnemyWaveData enemyWaveData)
    {
        if (EnemyManager.s_instance.getEnemyCount() < Consts.maxEnemyCount)
        {
            ++GameFightData.s_instance.addedEnemyCount;
            Instantiate(ObjectPool.getPrefab("Prefabs/Enemys/" + enemyWaveData.prefab), enemyPoint).GetComponent<EnemyLogic>().init(enemyWaveData);
        }
    }

    public bool addHero()
    {
        for (int i = 0; i < heroPoint.childCount; i++)
        {
            if (heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + GameFightData.s_instance.randomSummonHero()), heroPoint.GetChild(i)).transform;
                heroTrans.GetComponent<HeroLogicBase>().curStar = RandomUtil.SelectProbability(GameFightData.s_instance.list_heroWeight) + 1;
                EffectManager.summonHero(heroGrid.transform.GetChild(i).position);
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
                heroTrans.GetComponent<HeroLogicBase>().curStar = RandomUtil.SelectProbability(GameFightData.s_instance.list_heroWeight) + 1;
                EffectManager.summonHero(heroGrid.transform.GetChild(i).position);

                return;
            }
        }
    }

    public void addHeroByIdStar(int id,int star)
    {
        for (int i = 0; i < heroPoint.childCount; i++)
        {
            if (heroPoint.GetChild(i).childCount == 0)
            {
                Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + id), heroPoint.GetChild(i)).transform;
                heroTrans.GetComponent<HeroLogicBase>().curStar = star;
                EffectManager.summonHero(heroGrid.transform.GetChild(i).position);
                return;
            }
        }
    }

    public void addMythicHero(HeroLogicBase heroLogicBase,int id,int star)
    {
        if(heroLogicBase)
        {
            Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + id), heroLogicBase.transform.parent).transform;
            heroTrans.GetComponent<HeroLogicBase>().curStar = star;
            //EffectManager.summonHero(heroGrid.transform.GetChild(i).position);

            DestroyImmediate(heroLogicBase.gameObject);

            TimerUtil.getInstance().delayTime(1, () =>
            {
                LayerManager.ShowLayer(Consts.Layer.GetMythicHeroLayer).GetComponent<GetMythicHeroLayer>().init(id);
            });
        }
    }

    private void OnDestroy()
    {
        EffectManager.clear();

        //if (GameUILayer.s_instance)
        //{
        //    Destroy(GameUILayer.s_instance.gameObject);
        //}

        //if(HeroInfoPanel.s_instance)
        //{
        //    Destroy(HeroInfoPanel.s_instance.gameObject);
        //}

        //if (WeaponInfoPanel.s_instance)
        //{
        //    Destroy(WeaponInfoPanel.s_instance.gameObject);
        //}

        //if (WeaponShopPanel.s_instance)
        //{
        //    Destroy(WeaponShopPanel.s_instance.gameObject);
        //}

        //if (KillEnemyRewardPanel.s_instance)
        //{
        //    Destroy(KillEnemyRewardPanel.s_instance.gameObject);
        //}

        //if (BossRewardPanel.s_instance)
        //{
        //    Destroy(BossRewardPanel.s_instance.gameObject);
        //}
    }
}
