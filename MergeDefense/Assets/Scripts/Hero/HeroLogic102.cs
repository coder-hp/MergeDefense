using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 射手
// 单体攻击
// 技能：攻击时，25%概率射出2支箭
public class HeroLogic102 : HeroBase
{
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/arrow"), GameLayer.s_instance.flyPoint).transform;
        arrow.position = heroLogicBase.curStandGrid.position;
        arrow.GetComponent<ArrowLogic>().init(heroLogicBase, enemyLogic);

        if(RandomUtil.getRandom(1,100) <= (25 + heroLogicBase.getAddSkillRate()))
        {
            Transform arrow2 = Instantiate(ObjectPool.getPrefab("Prefabs/Games/arrow"), GameLayer.s_instance.flyPoint).transform;
            arrow2.position = heroLogicBase.curStandGrid.position;
            arrow2.GetComponent<ArrowLogic>().init(heroLogicBase, enemyLogic);

            // 把第一支箭往前挪一挪
            arrow.position = Vector3.MoveTowards(arrow.position, enemyLogic.transform.position, 1);
        }
    }
}
