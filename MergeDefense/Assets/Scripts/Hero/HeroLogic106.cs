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
        EffectManager.heroAttack(transform.position, heroLogicBase.id);

        // 技能判定
        if (RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
        {
            EffectManager.heroSkill(transform.position, heroLogicBase.id);
            for (int i = 0; i < GameLayer.s_instance.heroPoint.childCount; i++)
            {
                if (GameLayer.s_instance.heroPoint.GetChild(i).childCount > 0)
                {
                    HeroLogicBase heroLogicBase = GameLayer.s_instance.heroPoint.GetChild(i).GetChild(0).GetComponent<HeroLogicBase>();
                    if (Vector3.Distance(heroLogicBase.curStandGrid.position, heroLogicBase.curStandGrid.position) <= 1.6f)
                    {
                        heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi, 0.2f, 5, "106"));
                    }
                }
            }
        }
    }
}
