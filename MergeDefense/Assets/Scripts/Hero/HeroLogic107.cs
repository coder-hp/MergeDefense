using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 小恶魔
// 远程单体暴击
// 技能：攻击时，10%概率造成0.8s[眩晕]
public class HeroLogic107 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("107_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon107"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon107>().init(heroLogicBase, enemyLogic);
    }
}
