using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon118 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);
        _enemyLogic.damage(atk, isCrit);

        // 技能3：攻击时，10%概率对范围内的敌人造成攻击力1500%的伤害，并追加其最大生命值10%的伤害
        if (RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()))
        {
            atk = heroLogicBase.getAtk() * 15;
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
                {
                    EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                    if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
                    {
                        --i;
                    }
                    else
                    {
                        // 追加伤害
                        if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(EnemyManager.s_instance.list_enemy[i].fullHP * 0.1f), false))
                        {
                            --i;
                        }
                    }
                }
            }
        }
    }
}
