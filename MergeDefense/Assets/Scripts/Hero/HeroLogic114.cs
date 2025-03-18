using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 亚瑟王
// 近战单体攻击
// 技能1：我方单位攻击增加10%，暴击率增加5%
// 技能2：攻击时，15%概率生成一道剑气，对剑气上的敌人造成攻击力1500%的伤害，并附加30%的减速效果，持续5s
// 技能3：自身暴伤增加50%，装备的剑类武器每一级额外提升5%
public class HeroLogic114 : HeroBase
{
    int atkCount = 0;
    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("113_attack");
        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;

        // 技能2：第5次攻击变为必定暴击的范围伤害
        if (++atkCount >= 5)
        {
            atkCount = 0;
            isCrit = true;
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    if (EnemyManager.s_instance.list_enemy[i].damage(atk, isCrit))
                    {
                        if (isCanAddAtkSpeedBuff())
                        {
                            heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.AtkSpeed, 0.1f, 5, "113", false, true));
                        }
                        --i;
                    }
                }
            }
        }
        else
        {
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
            if (enemyLogic.damage(atk, isCrit))
            {
                if (isCanAddAtkSpeedBuff())
                {
                    heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.AtkSpeed, 0.1f, 5, "113", false, true));
                }
            }
        }
    }

    bool isCanAddAtkSpeedBuff()
    {
        int count = 0;
        for(int i = 0; i < heroLogicBase.list_buffDatas.Count; i++)
        {
            if (heroLogicBase.list_buffDatas[i].buffType == Consts.BuffType.AtkSpeed && heroLogicBase.list_buffDatas[i].from == "113")
            {
                ++count;
            }
        }

        return count >= 5 ? false : true;
    }
}
