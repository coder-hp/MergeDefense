using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 灭霸猫咪
// 近战单体攻击
// 技能1：每波WAVE开始时5%的概率消灭一半敌人
// 技能2：每波WAVE时长增加3s
// 技能3：每8s触发一次，普攻将替换为攻击力1000%的范围伤害，直到有敌人阵亡
public class HeroLogic119 : HeroBase
{
    bool isSkill3Trigger = false;

    private void Start()
    {
        Invoke("onInvokeSkill3",8);
    }

    void onInvokeSkill3()
    {
        isSkill3Trigger = true;
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("119_attack");

        if(!enemyLogic)
        {
            return;
        }

        // 技能3：每8s触发一次，普攻将替换为攻击力1000%的范围伤害，直到有敌人阵亡
        if (isSkill3Trigger)
        {
            if(enemyLogic.damage(heroLogicBase.getAtk() * 10, false))
            {
                isSkill3Trigger = false;
                Invoke("onInvokeSkill3", 8);
            }
        }
        else
        {
            bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
            int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));
            enemyLogic.damage(atk, isCrit);
        }
    }
}
