using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager s_instance = null;

    [HideInInspector]
    public List<EnemyLogic> list_enemy = new List<EnemyLogic>();

    private void Awake()
    {
        s_instance = this;
    }

    private void Update()
    {
        for(int i = 0; i < list_enemy.Count; i++)
        {
            list_enemy[i].move();
        }
    }

    public int getEnemyCount()
    {
        return list_enemy.Count;
    }

    public void addEnemy(EnemyLogic enemyLogic)
    {
        list_enemy.Add(enemyLogic);
        GameUILayer.s_instance.refreshEnemyCount();
        GameUILayer.s_instance.checkIsShowBossRedKuang();
    }

    public void removeEnemy(EnemyLogic enemyLogic)
    {
        list_enemy.Remove(enemyLogic);
        GameUILayer.s_instance.refreshEnemyCount();
        GameUILayer.s_instance.checkIsShowBossRedKuang();
    }

    public EnemyLogic getMinDisTarget(Transform hero)
    {
        float tempDis;
        float minDis = 999;
        EnemyLogic enemyLogic = null;
        for (int i = 0; i < list_enemy.Count; i++)
        {
            tempDis = Vector3.Distance(hero.position, list_enemy[i].transform.position);
            if (tempDis < minDis)
            {
                minDis = tempDis;
                enemyLogic = list_enemy[i];
            }
        }
        return enemyLogic;
    }

    public EnemyLogic getHeroAtkTarget(HeroLogicBase heroLogicBase)
    {
        // 优先攻击boss
        for (int i = 0; i < list_enemy.Count; i++)
        {
            if(list_enemy[i].enemyWaveData.enemyType == 3)
            {
                if(Vector3.Distance(heroLogicBase.curStandGrid.position, list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    return list_enemy[i];
                }
            }
        }

        // 其次攻击精英怪
        for (int i = 0; i < list_enemy.Count; i++)
        {
            if (list_enemy[i].enemyWaveData.enemyType == 2)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    return list_enemy[i];
                }
            }
        }

        // 最后攻击小怪
        //for (int i = 0; i < list_enemy.Count; i++)
        //{
        //    if (Vector3.Distance(heroLogicBase.curStandGrid.position, list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
        //    {
        //        return list_enemy[i];
        //    }
        //}

        // 攻击最近的怪
        {
            EnemyLogic enemyLogic = getMinDisTarget(heroLogicBase.transform);
            if (enemyLogic && Vector3.Distance(heroLogicBase.curStandGrid.position, enemyLogic.transform.position) <= heroLogicBase.heroData.atkRange)
            {
                return enemyLogic;
            }
        }

        return null;
    }
}
