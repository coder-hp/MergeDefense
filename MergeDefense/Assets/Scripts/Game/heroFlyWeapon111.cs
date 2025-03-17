using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon111 : MonoBehaviour
{
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;
    int fixedAtk = 0;

    public void init(HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic,int _fixedAtk = 0)
    {
        fixedAtk = _fixedAtk;
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
                    if(fixedAtk > 0)
                    {
                        EffectManager.s_instance.enemyDamage(enemyLogic.transform.position, heroLogicBase.id);
                        enemyLogic.damage(fixedAtk, false);
                    }
                    else
                    {
                        // 先结算平A的伤害
                        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                        EffectManager.s_instance.enemyDamage(enemyLogic.transform.position, heroLogicBase.id);
                        enemyLogic.damage(atk, isCrit);

                        // 判定技能3：攻击时，15%概率对范围内的敌人造成攻击力1000%的伤害
                        if (RandomUtil.getRandom(1, 100) <= (15 + heroLogicBase.getAddSkillRate()))
                        {
                            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                            {
                                if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                                {
                                    if (EnemyManager.s_instance.list_enemy[i].damage(atk * 10, false))
                                    {
                                        --i;
                                    }
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
