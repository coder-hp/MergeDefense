using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 女巫
// 远程群攻
// 技能1：每1s对范围内的敌人造成攻击力150%的伤害
// 技能2：攻击时，18%概率对范围内的敌人造成攻击力500%的伤害
public class HeroLogic110 : HeroBase
{
    private void Start()
    {
        InvokeRepeating("onInvokeSkill",3,3);
    }

    void onInvokeSkill()
    {
        // 技能1：每3s对范围内的敌人造成攻击力100% 的伤害，最多5个敌人
        if (heroLogicBase.isCanUpdate)
        {
            int atk = heroLogicBase.getAtk();
            int count = 0;
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(atk), false))
                    {
                        --i;
                    }

                    if(++count >= 5)
                    {
                        return;
                    }
                }
            }
        }
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("110_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon110"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon110>().init(heroLogicBase, enemyLogic);
    }
}
