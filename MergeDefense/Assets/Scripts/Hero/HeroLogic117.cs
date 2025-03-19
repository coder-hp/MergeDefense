using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 收割者
// 近战单体
// 技能1：全体敌人减速10%
// 技能2：攻击时，10%概率击杀血量不满的敌人（对精英和BOSS无效）
// 技能3：攻击时，15%概率对范围内的敌人造成攻击力1000%的伤害
public class HeroLogic117 : HeroBase
{
    private void Start()
    {
        GameFightData.s_instance.addGlobalEnemyBuff(new Consts.BuffData(Consts.BuffType.MoveSpeed,-0.1f,9999,"117", true, true));
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("117_attack");

        bool isCrit = RandomUtil.getRandom(1, 100) <= heroLogicBase.getCritRate() ? true : false;
        int atk = Mathf.RoundToInt(heroLogicBase.getAtk() * (isCrit ? heroLogicBase.getCritDamageXiShu() : 1));

        // 判定技能3：攻击时，15%概率对范围内的敌人造成攻击力1000%的伤害
        if (RandomUtil.getRandom(1, 100) <= (15 + heroLogicBase.getAddSkillRate()))
        {
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    if (EnemyManager.s_instance.list_enemy[i].damage(atk * 10, false))
                    {
                        --i;
                    }
                }
            }
        }

        if (!enemyLogic)
        {
            return;
        }

        // 先判定技能2：攻击时，10%概率击杀血量不满的敌人（对精英和BOSS无效）
        if (enemyLogic.enemyWaveData.enemyType == 1 && enemyLogic.curHP < enemyLogic.fullHP && RandomUtil.getRandom(1, 100) <= (10 + heroLogicBase.getAddSkillRate()))
        {
            enemyLogic.damage(enemyLogic.curHP, false);
        }
        else
        {
            enemyLogic.damage(atk, isCrit);
        }
    }

    private void OnDestroy()
    {
        // 删除全局敌人减速buff
        GameFightData.s_instance.removeGlobalEnemyBuff(Consts.BuffType.MoveSpeed, "117");
    }
}
