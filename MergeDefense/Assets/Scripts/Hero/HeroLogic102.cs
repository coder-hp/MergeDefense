using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 射手
// 单体攻击
// 技能：攻击时，15%概率射出2支箭，持续3s
public class HeroLogic102 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/arrow"), GameLayer.s_instance.flyPoint).transform;
        arrow.position = heroLogicBase.centerPoint.position;
        arrow.GetComponent<ArrowLogic>().init(heroLogicBase.getAtk(), enemyLogic);
    }
}
