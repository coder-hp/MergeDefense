using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小恶魔
// 远程单体暴击
// 技能：攻击时，10%概率造成0.8s[眩晕]
public class HeroLogic107 : HeroBase
{
    int baseSkillRate = 10;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("107_attack");
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        if (!enemyLogic.damage(atk, isCrit))
        {
            // 如果没死，则判定技能
            if (RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
            {
                enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.Stun, 0, 0.8f, "107"));
            }
        }
    }
}
