using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon103 : MonoBehaviour
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
        transform.position = heroLogicBase.flyWeaponPoint.position;
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
                if (heroLogicBase)
                {
                    bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                    int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

                    enemyLogic.damage(atk, isCrit);
                    EffectManager.s_instance.enemyDamage(enemyLogic.transform.position, heroLogicBase.id);

                    // 技能：攻击时，20%概率对范围内的敌人造成攻击力250%的伤害
                    if (RandomUtil.getRandom(1, 100) <= (20 + heroLogicBase.getAddSkillRate()))
                    {
                        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                        {
                            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange * 0.5f)
                            {
                                if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(atk * 2.5f), false))
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
