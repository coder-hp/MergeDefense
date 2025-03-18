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

        // 技能2：攻击时，15%概率生成一道剑气，对剑气上的敌人造成攻击力1500%的伤害，并附加30%的减速效果，持续5s
        if (RandomUtil.getRandom(1, 100) <= (15 + heroLogicBase.getAddSkillRate()))
        {
            bool roadUpHaveEnemy = false;
            bool roadDownHaveEnemy = false;

            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (EnemyManager.s_instance.list_enemy[i].curTargetPosIndex == 2)
                {
                    roadUpHaveEnemy = true;
                }
                else if (EnemyManager.s_instance.list_enemy[i].curTargetPosIndex == 0)
                {
                    roadDownHaveEnemy = true;
                }
            }

            if (roadUpHaveEnemy || roadDownHaveEnemy)
            {
                Transform skillEffect = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroSkill114_2"), GameLayer.s_instance.flyPoint).transform;

                int atkTargetPosIndex = 0;

                float posY = 0;
                if (roadUpHaveEnemy && roadDownHaveEnemy)
                {
                    if (RandomUtil.getRandom(1, 100) <= 50)
                    {
                        posY = GameLayer.s_instance.list_enemyMoveFourPos[1].y;
                        atkTargetPosIndex = 2;
                    }
                    else
                    {
                        posY = GameLayer.s_instance.list_enemyMoveFourPos[0].y;
                        atkTargetPosIndex = 0;
                    }
                }
                else if (roadUpHaveEnemy)
                {
                    posY = GameLayer.s_instance.list_enemyMoveFourPos[1].y;
                    atkTargetPosIndex = 2;
                }
                else if (roadDownHaveEnemy)
                {
                    posY = GameLayer.s_instance.list_enemyMoveFourPos[0].y;
                    atkTargetPosIndex = 0;
                }
                skillEffect.position = new Vector3(0, posY, 0);

                atk = heroLogicBase.getAtk() * 15;
                for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                {
                    if (EnemyManager.s_instance.list_enemy[i].curTargetPosIndex == atkTargetPosIndex)
                    {
                        if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
                        {
                            --i;
                        }
                        else
                        {
                            // 附加30%的减速效果，持续5s
                            EnemyManager.s_instance.list_enemy[i].addBuff(new Consts.BuffData(Consts.BuffType.MoveSpeed, -0.3f, 5, "114", false, false));
                        }
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        // 删除全局buff
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.AtkBaiFenBi,"114");
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.CritRate,"114");
    }
}
