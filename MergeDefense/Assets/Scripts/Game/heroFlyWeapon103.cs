using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon103 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        // 先结算平A的伤害
        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
            {
                EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                {
                    --i;
                }
            }
        }

        // 判定技能：攻击时，20%概率对范围内的敌人造成攻击力250%的伤害
        if (RandomUtil.getRandom(1, 100) <= (20 + heroLogicBase.getAddSkillRate()))
        {
            int atk = heroLogicBase.getAtk();
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                {
                    if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(atk * 2.5f), false))
                    {
                        --i;
                    }
                }
            }
        }
    }
}
