using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 圣骑士
// 单体攻击
// 技能：攻击时，15%概率提升周围单位20%的攻击力，持续5s
public class HeroLogic106 : HeroBase
{
    int baseSkillRate = 15;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("106_attack");
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        enemyLogic.damage(atk, isCrit);

        // 技能判定
        if (RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
        {
            for (int i = 0; i < HeroManager.s_instance.list_hero.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, HeroManager.s_instance.list_hero[i].curStandGrid.position) <= 1.6f)
                {
                    HeroManager.s_instance.list_hero[i].addBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi, 0.2f, 5, "106", false, false));
                }
            }
        }
    }
}
