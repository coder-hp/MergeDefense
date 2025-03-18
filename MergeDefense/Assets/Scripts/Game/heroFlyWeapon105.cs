using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon105 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

        EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);

        // 如果没死，则判定技能：攻击附带20%减速效果，持续3s
        if (!_enemyLogic.damage(atk, isCrit))
        {
            _enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.MoveSpeed, -0.2f, 3, "105", false, false));
        }
    }
}
