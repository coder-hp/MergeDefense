using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 精灵王
// 远程单体
// 技能1：我方单位攻速增加20%
// 技能2：每2s释放三支箭对范围内的敌人造成攻击力500%的伤害
// 技能3：攻击时，15%概率对范围内的敌人造成攻击力1000%的伤害
public class HeroLogic111 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("111_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon111"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon111>().init(heroLogicBase, enemyLogic);
    }
}
