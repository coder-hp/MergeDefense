using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowBallLogic : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic)
    {
        heroLogicBase = _heroLogicBase;
        enemyLogic = _enemyLogic;
        targetTrans = enemyLogic.transform;
    }

    void Update()
    {
        if (enemyLogic)
        {
            float angle = CommonUtil.twoPointAngle(transform.position, targetTrans.position);
            transform.rotation = Quaternion.Euler(0, 0, angle);
            transform.position = Vector3.MoveTowards(transform.position, targetTrans.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTrans.position) <= 0.1f)
            {
                bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                int atk = (int)(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                Destroy(gameObject);

                // 如果没死，则判定技能：攻击附带20%减速效果，持续5s
                if (!enemyLogic.damage(atk, isCrit))
                {
                    for (int i = 0; i < enemyLogic.list_buffDatas.Count; i++)
                    {
                        // 如果已存在该buff,则重置时间
                        if (enemyLogic.list_buffDatas[i].buffType == Consts.BuffType.MoveSpeed && enemyLogic.list_buffDatas[i].from == "105")
                        {
                            enemyLogic.list_buffDatas[i].time = 5;
                            return;
                        }
                    }
                    enemyLogic.list_buffDatas.Add(new Consts.BuffData(Consts.BuffType.MoveSpeed,-0.2f,5, "105"));
                }
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
