using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 剑士
// 单体攻击
// 技能：攻击时，15%概率造成一次攻击力150%的斩击
public class HeroLogic101 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        enemyLogic.damage((int)(heroLogicBase.getAtk() * ((RandomUtil.getRandom(1,100) <= 15) ? 1.5f : 1)));
    }
}
