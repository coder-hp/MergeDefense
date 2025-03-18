using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon102 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);
        _enemyLogic.damage(atk, isCrit);
    }
}
