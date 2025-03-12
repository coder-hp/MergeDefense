using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 泰坦
// 群体攻击
// 技能1：攻击范围内的敌人获得[易伤]
// 技能2：杀死敌人后攻击力翻倍，持续2S
// 技能3：攻击时，15%概率造成2s[眩晕]
public class HeroLogic112 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("112_attack");

        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
            {
                bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                int atk = Mathf.RoundToInt((heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1)));
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                {
                    --i;
                }
            }
        }
    }
}
