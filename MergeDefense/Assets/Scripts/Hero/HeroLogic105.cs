using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 雪人
// 远程单体攻击
// 技能：攻击附带20%减速效果，持续3s
public class HeroLogic105 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("105_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon105"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<HeroFlyWeaponBase>().init(heroLogicBase, enemyLogic);
    }
}
