using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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

                int star = RandomUtil.SelectProbability(GameFightData.s_instance.list_heroWeight) + 1;
                if(RandomUtil.getRandom(1,10000) <= GameFightData.s_instance.luckySummon)
                {
                    bool isSuper = false;
                    if (RandomUtil.getRandom(1,100) <= 90)
                    {
                        star += 1;
                    }
                    else
                    {
                        star += 2;
                        isSuper = true;
                    }

                    LayerManager.ShowLayer(Consts.Layer.LuckySummonLayer).GetComponent<LuckySummonLayer>().init(GameFightData.s_instance.luckySummon, isSuper,false);
                }

                if(star > 9)
                {
                    star = 9;
                }

                heroTrans.GetComponent<HeroLogicBase>().curStar = star;
                EffectManager.s_instance.summonHero(heroGrid.transform.GetChild(i).position);
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
                EffectManager.s_instance.summonHero(heroGrid.transform.GetChild(i).position);

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
                EffectManager.s_instance.summonHero(heroGrid.transform.GetChild(i).position);
                return;
            }
        }
    }

    public void summonMythicHero(HeroData heroData,string deleteHero_id_star = "", string deleteWeapon_type_level = "")
    {
        // 角色替换
        {
            HeroLogicBase heroLogicBase = null;
            for (int i = 0; i < heroData.list_summonWay.Count; i++)
            {
                if (heroData.list_summonWay[i][0] == 1)
                {
                    int heroId = heroData.list_summonWay[i][1];
                    int star = heroData.list_summonWay[i][2];

                    if(deleteHero_id_star != "")
                    {
                        heroId = int.Parse(deleteHero_id_star.Split('_')[0]);
                        star = int.Parse(deleteHero_id_star.Split('_')[1]);
                    }

                    for (int j = 0; j < HeroManager.s_instance.list_hero.Count; j++)
                    {
                        HeroLogicBase heroLogicBase_temp = HeroManager.s_instance.list_hero[j];
                        if (heroLogicBase_temp.id == heroId && heroLogicBase_temp.curStar >= star)
                        {
                            if (heroLogicBase == null)
                            {
                                heroLogicBase = heroLogicBase_temp;
                            }
                            else if (heroLogicBase_temp.curStar < heroLogicBase.curStar)
                            {
                                heroLogicBase = heroLogicBase_temp;
                            }
                        }
                    }

                    if (heroLogicBase)
                    {
                        Transform heroTrans = Instantiate(ObjectPool.getPrefab("Prefabs/Heros/hero" + heroData.id), heroLogicBase.transform.parent).transform;
                        heroTrans.GetComponent<HeroLogicBase>().curStar = 10;
                        //EffectManager.summonHero(heroGrid.transform.GetChild(i).position);

                        DestroyImmediate(heroLogicBase.gameObject);

                        TimerUtil.getInstance().delayTime(1, () =>
                        {
                            LayerManager.ShowLayer(Consts.Layer.GetMythicHeroLayer).GetComponent<GetMythicHeroLayer>().init(heroData.id);
                        });
                    }
                    break;
                }
            }
        }

        // 武器删除
        {
            WeaponBar weaponBar = null;
            UIItemWeapon uIItemWeapon = null;
            for (int i = 0; i < heroData.list_summonWay.Count; i++)
            {
                if (heroData.list_summonWay[i][0] == 2)
                {
                    int weaponType = heroData.list_summonWay[i][1];
                    int level = heroData.list_summonWay[i][2];

                    if (deleteWeapon_type_level != "")
                    {
                        weaponType = int.Parse(deleteWeapon_type_level.Split('_')[0]);
                        level = int.Parse(deleteWeapon_type_level.Split('_')[1]);
                    }

                    // 检索武器栏
                    {
                        for (int j = 0; j < GameUILayer.s_instance.list_weaponBar.Count; j++)
                        {
                            if (GameUILayer.s_instance.list_weaponBar[j].weaponData != null && GameUILayer.s_instance.list_weaponBar[j].weaponData.type == weaponType && GameUILayer.s_instance.list_weaponBar[j].weaponData.level >= level)
                            {
                                if (weaponBar == null)
                                {
                                    weaponBar = GameUILayer.s_instance.list_weaponBar[j];
                                }
                                else if (weaponBar.weaponData.level > GameUILayer.s_instance.list_weaponBar[j].weaponData.level)
                                {
                                    weaponBar = GameUILayer.s_instance.list_weaponBar[j];
                                }
                            }
                        }
                    }

                    // 检索武器格子
                    {
                        for (int j = 0; j < GameUILayer.s_instance.weaponGridTrans.childCount; j++)
                        {
                            if (GameUILayer.s_instance.weaponGridTrans.GetChild(j).childCount == 1)
                            {
                                UIItemWeapon uiItemWeapon_temp = GameUILayer.s_instance.weaponGridTrans.GetChild(j).GetChild(0).GetComponent<UIItemWeapon>();
                                if (uiItemWeapon_temp.weaponData.type == weaponType && uiItemWeapon_temp.weaponData.level >= level)
                                {
                                    if (uIItemWeapon == null)
                                    {
                                        uIItemWeapon = uiItemWeapon_temp;
                                    }
                                    else if (uIItemWeapon.weaponData.level > uiItemWeapon_temp.weaponData.level)
                                    {
                                        uIItemWeapon = uiItemWeapon_temp;
                                    }
                                }
                            }
                        }
                    }

                    break;
                }
            }

            if (weaponBar != null && uIItemWeapon != null)
            {
                if (uIItemWeapon.weaponData.level <= weaponBar.weaponData.level)
                {
                    Destroy(uIItemWeapon.gameObject);
                }
                else
                {
                    weaponBar.setData(null);
                }
            }
            else if (weaponBar != null)
            {
                weaponBar.setData(null);
            }
            else if (uIItemWeapon != null)
            {
                Destroy(uIItemWeapon.gameObject);
            }
        }

        GameUILayer.s_instance.checkMythicHeroProgress();
    }

    int[] mythicHeroProgress = new int[2];
    public int[] getMythicHeroProgress(HeroData heroData)
    {
        // 重置
        for (int i = 0; i < mythicHeroProgress.Length; i++)
        {
            mythicHeroProgress[i] = 0;
        }

        for (int i = 0; i < heroData.list_summonWay.Count; i++)
        {
            int summonType = heroData.list_summonWay[i][0];

            // 角色要求
            if (summonType == 1)
            {
                int id = heroData.list_summonWay[i][1];
                int star = heroData.list_summonWay[i][2];

                // 遍历已上场角色，检查条件是否满足
                for (int j = 0; j < HeroManager.s_instance.list_hero.Count; j++)
                {
                    HeroLogicBase heroLogicBase = HeroManager.s_instance.list_hero[j];
                    if (heroLogicBase.curStar <= 9)
                    {
                        if (id == 999)
                        {
                            if (heroLogicBase.curStar >= star)
                            {
                                mythicHeroProgress[i] = 1;
                                break;
                            }
                        }
                        else
                        {
                            if (heroLogicBase.id == id && heroLogicBase.curStar >= star)
                            {
                                mythicHeroProgress[i] = 1;
                                break;
                            }
                        }
                    }
                }
            }
            // 武器要求
            else if (summonType == 2)
            {
                int weaponType = heroData.list_summonWay[i][1];
                int level = heroData.list_summonWay[i][2];

                // 遍历已有武器，检查条件是否满足
                {
                    for (int j = 0; j < GameUILayer.s_instance.list_weaponBar.Count; j++)
                    {
                        if (weaponType == 999)
                        {
                            if (GameUILayer.s_instance.list_weaponBar[j].weaponData != null && GameUILayer.s_instance.list_weaponBar[j].weaponData.level >= level)
                            {
                                mythicHeroProgress[i] = 1;
                                break;
                            }
                        }
                        else
                        {
                            if (GameUILayer.s_instance.list_weaponBar[j].weaponData != null && GameUILayer.s_instance.list_weaponBar[j].weaponData.type == weaponType && GameUILayer.s_instance.list_weaponBar[j].weaponData.level >= level)
                            {
                                mythicHeroProgress[i] = 1;
                                break;
                            }
                        }
                    }

                    if (mythicHeroProgress[i] == 0)
                    {
                        for (int j = 0; j < GameUILayer.s_instance.weaponGridTrans.childCount; j++)
                        {
                            if (GameUILayer.s_instance.weaponGridTrans.GetChild(j).childCount == 1)
                            {
                                UIItemWeapon uiItemWeapon = GameUILayer.s_instance.weaponGridTrans.GetChild(j).GetChild(0).GetComponent<UIItemWeapon>();

                                if (weaponType == 999)
                                {
                                    if (uiItemWeapon.weaponData.level >= level)
                                    {
                                        mythicHeroProgress[i] = 1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (uiItemWeapon.weaponData.type == weaponType && uiItemWeapon.weaponData.level >= level)
                                    {
                                        mythicHeroProgress[i] = 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        return mythicHeroProgress;
    }
}
