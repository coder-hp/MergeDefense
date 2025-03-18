using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon118_fireBall : HeroFlyWeaponBase
{
    // 技能4：每攻击20次，喷射三个火球，每个火球对范围内的敌人造成攻击力2500%的伤害
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            int atk = heroLogicBase.getAtk();
            if (Vector3.Distance(transform.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= Consts.megaSkillRange)
            {
                EffectManager.s_instance.enemyDamage(EnemyManager.s_instance.list_enemy[i].transform.position, heroLogicBase.id);
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
                {
                    --i;
                }
            }
        }
    }
}
