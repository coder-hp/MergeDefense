using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 雪人
// 远程单体攻击
// 技能：攻击附带20%减速效果，持续5s
public class HeroLogic105 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/snowBall"), GameLayer.s_instance.flyPoint).transform;
        arrow.position = heroLogicBase.curStandGrid.position;
        arrow.GetComponent<SnowBallLogic>().init(heroLogicBase, enemyLogic);
    }
}
