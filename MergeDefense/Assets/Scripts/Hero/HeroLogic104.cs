using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 斧手
// 群体攻击
// 技能：每攻击三次必定暴击
public class HeroLogic104 : HeroBase
{
    int atkCount = 0;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("104_attack");
        bool isCrit = false;
        if (++atkCount == 3)
        {
            isCrit = true;
            atkCount = 0;
        }

        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
            {
                if (!isCrit)
                {
                    isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
                }

                int atk = Mathf.RoundToInt((heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1)));
                if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                {
                    --i;
                }
            }
        }
    }
}
