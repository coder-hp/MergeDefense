using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBallLogic : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;
    float damageRange = 1.9f;

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
            //transform.LookAt(targetTrans);
            transform.position = Vector3.MoveTowards(transform.position, targetTrans.position, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetTrans.position) <= 0.1f)
            {
                // 技能：攻击时，20%概率对范围内的敌人造成攻击力250%的伤害
                bool isTriggerSkill = RandomUtil.getRandom(1,100) <= (20 + heroLogicBase.getAddSkillRate()) ? true : false;
                for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                {
                    if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= damageRange)
                    {
                        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                        if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                        {
                            --i;
                        }
                        // 没死的话，判定技能伤害
                        else if(isTriggerSkill)
                        {
                            if (EnemyManager.s_instance.list_enemy[i].damage((int)(atk * 2.5f),false))
                            {
                                --i;
                            }
                        }
                    }
                }

                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
