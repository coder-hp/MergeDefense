using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon119 : HeroFlyWeaponBase
{
    public HeroLogic119 heroLogic119;

    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        // 技能3：每8s触发一次，普攻将替换为攻击力1000 % 的范围伤害，直到有敌人阵亡
        if (heroLogic119.isSkill3Trigger)
        {
            int atk = heroLogicBase.getAtk() * 10;
            bool isKillEnemy = false;
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                {
                    EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                    if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
                    {
                        --i;
                        isKillEnemy = true;
                    }
                }
            }

            if(isKillEnemy)
            {
                heroLogic119.skill3End();
            }
        }
        else
        {
            bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
            EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);
            _enemyLogic.damage(atk, isCrit);
        }
    }
}
