using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 剑士
// 单体攻击
// 技能：攻击时，15%概率造成一次攻击力150%的斩击
public class HeroLogic101 : HeroBase
{
    int baseSkillRate = 15;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("101_attack");

        if(!enemyLogic)
        {
            return;
        }

        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

        if (!enemyLogic.damage(atk, isCrit))
        {
            if(RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
            {
                //EffectManager.heroSkill(transform.position,heroLogicBase.id);
                enemyLogic.damage(Mathf.RoundToInt(heroLogicBase.getAtk() * 1.5f),false);
            }
        }
    }
}
