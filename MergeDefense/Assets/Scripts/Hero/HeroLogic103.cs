using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 法师
// 群体攻击
// 技能：攻击时，20%概率对范围内的敌人造成攻击力250%的伤害
public class HeroLogic103 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("103_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/magicBall"), GameLayer.s_instance.flyPoint).transform;
        arrow.position = heroLogicBase.curStandGrid.position;
        arrow.GetComponent<MagicBallLogic>().init(heroLogicBase, enemyLogic);
    }
}
