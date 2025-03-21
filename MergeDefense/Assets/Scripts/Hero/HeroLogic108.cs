using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 野猪人
// 近战群体减益
// 技能：攻击时，35%概率使敌人获得[易伤]，持续5s
public class HeroLogic108 : HeroBase
{
    int baseSkillRate = 35;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("108_attack");

        if (!enemyLogic)
        {
            return;
        }

        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
            {
                bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                {
                    --i;
                }
                // 如果没死，则判定技能
                else if (RandomUtil.getRandom(1, 100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
                {
                    enemyLogic.addBuff(new Consts.BuffData(Consts.BuffType.YiShang, 0.2f, 5,"", false, false));
                }
            }
        }
    }
}
