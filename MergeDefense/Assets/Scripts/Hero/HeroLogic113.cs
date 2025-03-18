using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 兽族酋长
// 近战单体攻击
// 技能1：杀死一名敌人后获得5%攻速，持续5s。可叠加5层
// 技能2：第5次攻击变为必定暴击的范围伤害
// 技能3：每一波结束后30%概率获得5枚金币
public class HeroLogic113 : HeroBase
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
            if (!enemyLogic)
            {
                return;
            }

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
