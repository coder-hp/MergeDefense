using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon110 : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;
    float atkRange = 1.9f;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic)
    {
        heroLogicBase = _heroLogicBase;
        enemyLogic = _enemyLogic;
        targetTrans = enemyLogic.transform;
        transform.position = heroLogicBase.flyWeaponPoint.position;
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
                if (heroLogicBase)
                {
                    // 先结算平A的伤害
                    for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                    {
                        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                        if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= atkRange)
                        {
                            EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                            if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                            {
                                --i;
                            }
                        }
                    }

                    // 判定技能2：攻击时，18%概率对范围内的敌人造成攻击力500%的伤害
                    if (RandomUtil.getRandom(1, 100) <= (18 + heroLogicBase.getAddSkillRate()))
                    {
                        int atk = heroLogicBase.getAtk();
                        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                        {
                            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                            {
                                if (EnemyManager.s_instance.list_enemy[i].damage(atk * 5, false))
                                {
                                    --i;
                                }
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
