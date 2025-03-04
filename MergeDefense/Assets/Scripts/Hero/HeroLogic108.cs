using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 野猪人
// 近战群体减益
// 技能：攻击时，35%概率使敌人5s内承受伤害+20%
public class HeroLogic108 : HeroBase
{
    int baseSkillRate = 35;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("108_attack");
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        EffectManager.heroAttack(transform.position, heroLogicBase.id);
        if (!enemyLogic.damage(atk, isCrit))
        {
            // 如果没死，则判定技能
            if (RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
            {
                EffectManager.heroSkill(transform.position, heroLogicBase.id);
                enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.DamageBaiFenBi, 0.2f, 5, "108"));
            }
        }
    }
}
