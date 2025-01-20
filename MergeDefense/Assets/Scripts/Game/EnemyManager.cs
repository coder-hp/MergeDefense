using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager s_instance = null;

    List<EnemyLogic> list_enemy = new List<EnemyLogic>();

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
    }

    public void removeEnemy(EnemyLogic enemyLogic)
    {
        list_enemy.Remove(enemyLogic);
        GameUILayer.s_instance.refreshEnemyCount();
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
}
