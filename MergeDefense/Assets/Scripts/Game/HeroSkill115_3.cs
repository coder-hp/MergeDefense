using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 对接触到的敌人造成600%的伤害，并附加20%的减速效果
public class HeroSkill115_3 : MonoBehaviour
{
    int atk;
    List<EnemyLogic> list_enemy = new List<EnemyLogic>();

    public void init(int _atk)
    {
        atk = _atk;
        Invoke("onInvokeDestroy",5);
    }

    void onInvokeDestroy()
    {
        for(int i = 0; i < list_enemy.Count; i++)
        {
            list_enemy[i].removeBuff(Consts.BuffType.MoveSpeed, "115_3");
        }
        DestroyImmediate(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            EnemyLogic enemyLogic = other.transform.GetComponent<EnemyLogic>();
            if (!list_enemy.Contains(enemyLogic))
            {
                if(!enemyLogic.damage(atk,false))
                {
                    list_enemy.Add(enemyLogic);
                    enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.MoveSpeed,-0.2f,5,"115_3"));
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyLogic enemyLogic = other.transform.GetComponent<EnemyLogic>();
            if (list_enemy.Contains(enemyLogic))
            {
                list_enemy.Remove(enemyLogic);
                enemyLogic.removeBuff(Consts.BuffType.MoveSpeed, "115_3");
            }
        }
    }
}
