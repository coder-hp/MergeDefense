using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 金骨
// 近战单体
// 技能1：攻击时，15%概率对范围内的敌人造成攻击力270%的伤害
// 技能2：攻击时，10%的概率获得1宝石
public class HeroLogic109 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("109_attack");

        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

        enemyLogic.damage(atk, isCrit);

        // 判定技能2：攻击时，10%的概率获得1宝石
        if (RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()))
        {
            GameUILayer.s_instance.changeDiamond(1);
        }

        // 判定技能1：攻击时，15%概率对范围内的敌人造成攻击力270%的伤害
        if (RandomUtil.getRandom(1, 100) <= (15 + heroLogicBase.getAddSkillRate()))
        {
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(atk * 2.7f), false))
                    {
                        --i;
                    }
                }
            }
        }
    }
}
