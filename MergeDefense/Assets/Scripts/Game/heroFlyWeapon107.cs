using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heroFlyWeapon107 : HeroFlyWeaponBase
{
    public override void atkEnemy(EnemyLogic _enemyLogic)
    {
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

        EffectManager.s_instance.enemyDamage(_enemyLogic.transform.position, heroLogicBase.id);

        if (!_enemyLogic.damage(atk, isCrit))
        {
            // 如果没死，则判定技能:攻击时，10%概率造成0.8s[眩晕]
            bool isTriggerSkill = RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()) ? true : false;
            if (isTriggerSkill)
            {
                _enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.Stun, 0, 0.8f, "", false, false));
            }
        }
    }
}
