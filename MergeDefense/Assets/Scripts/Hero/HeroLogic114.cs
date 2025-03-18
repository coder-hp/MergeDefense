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
    private void Start()
    {
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi, 0.1f, 9999, "114", true, true));
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.CritRate, 5, 9999, "114", true, true));

        heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.CritDamage,0.5f,999,"114",true,false));
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("114_attack");

        if (!enemyLogic)
        {
            return;
        }

        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
        enemyLogic.damage(atk, isCrit);
    }

    private void OnDestroy()
    {
        // 删除全局buff
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.AtkBaiFenBi,"114");
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.CritRate,"114");
    }
}
