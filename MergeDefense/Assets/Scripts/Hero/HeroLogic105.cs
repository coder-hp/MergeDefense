using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 射手
// 单体攻击
// 技能：攻击时，25%概率射出2支箭
public class HeroLogic105 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/snowBall"), GameLayer.s_instance.flyPoint).transform;
        arrow.position = heroLogicBase.curStandGrid.position;
        arrow.GetComponent<SnowBallLogic>().init(heroLogicBase, enemyLogic);
    }
}
