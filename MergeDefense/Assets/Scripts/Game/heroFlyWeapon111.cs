using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon111 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        if (fixedAtk > 0)
        {
            EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);
            _enemyLogic.damage(fixedAtk, false);
        }
        else
        {
            // 先结算平A的伤害
            bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
            EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);
            _enemyLogic.damage(atk, isCrit);

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
}
