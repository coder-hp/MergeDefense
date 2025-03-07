using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 射手
// 单体攻击
// 技能：攻击时，25%概率射出2支箭
public class HeroLogic102 : HeroBase
{
    int baseSkillRate = 25;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("102_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon102"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon102>().init(heroLogicBase, enemyLogic);

        if(RandomUtil.getRandom(1,100) <= (baseSkillRate + heroLogicBase.getAddSkillRate()))
        {
            Transform arrow2 = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon102"), GameLayer.s_instance.flyPoint).transform;
            arrow2.GetComponent<heroFlyWeapon102>().init(heroLogicBase, enemyLogic);

            // 把第一支箭往前挪一挪
            arrow.position = Vector3.MoveTowards(arrow.position, enemyLogic.transform.position, 1);
        }
    }
}
