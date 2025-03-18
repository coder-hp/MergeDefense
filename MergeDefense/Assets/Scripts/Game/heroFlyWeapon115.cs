using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon115 : MonoBehaviour
{
    HeroLogic115 heroLogic115;
    HeroLogicBase heroLogicBase;
    EnemyLogic enemyLogic;
    Transform targetTrans;
    float moveSpeed = 10;
    float atkRange = 1.9f;

    public void init(HeroLogic115 _heroLogic115, HeroLogicBase _heroLogicBase, EnemyLogic _enemyLogic)
    {
        heroLogic115 = _heroLogic115;
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
                                // 技能1：杀死敌人后，攻击力+10
                                heroLogic115.heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.Atk,10,9999,"",true,true));
                                --i;
                            }
                        }
                    }

                    // 判定技能2：攻击时，10%概率对范围内的敌人造成攻击力2000%的伤害，并禁锢3s
                    if (RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()))
                    {
                        int atk = heroLogicBase.getAtk();
                        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                        {
                            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                            {
                                if (EnemyManager.s_instance.list_enemy[i].damage(atk * 20, false))
                                {
                                    // 技能1：杀死敌人后，攻击力+10
                                    heroLogic115.heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.Atk, 10, 9999, "", true, true));
                                    --i;
                                }
                                else
                                {
                                    EnemyManager.s_instance.list_enemy[i].addBuff(new Consts.BuffData(Consts.BuffType.Stun,0,3,"115", false, false));
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
