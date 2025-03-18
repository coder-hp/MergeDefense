using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 丘比特
// 远程单体攻击
// 技能1：我方单位攻击增加5%，攻速增加5%，暴伤增加10%
// 技能2：场上敌人数量大于50时，立即减少至40（生效一次）
// 技能3：攻击时，15%概率随机强化一名我方单位。攻击+10%，暴击+5%，持续5s
public class HeroLogic116 : HeroBase
{
    private void Start()
    {
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi, 0.05f, 9999, "116", true, true));
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.AtkSpeed, 0.05f, 9999, "116", true, true));
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.CritDamage, 0.1f, 9999, "116", true, true));
    }

    private void Update()
    {
        // 技能2：场上敌人数量大于50时，立即减少至40（生效一次）
        if (!GameFightData.s_instance.isGameOver && !GameFightData.s_instance.isUsedHeroSkill_116_2)
        {
            if(EnemyManager.s_instance.list_enemy.Count > 50)
            {
                GameFightData.s_instance.isUsedHeroSkill_116_2 = true;
                for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
                {
                    if (EnemyManager.s_instance.list_enemy[i].enemyWaveData.enemyType == 1)
                    {
                        if(EnemyManager.s_instance.list_enemy[i].damage(EnemyManager.s_instance.list_enemy[i].curHP, false))
                        {
                            --i;

                            if(EnemyManager.s_instance.list_enemy.Count <= 40)
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        // 技能3：攻击时，15%概率随机强化一名我方单位。攻击+10%，暴击+5%，持续5s
        if (RandomUtil.getRandom(1, 100) <= (15 + heroLogicBase.getAddSkillRate()))
        {
            HeroLogicBase hero = HeroManager.s_instance.list_hero[RandomUtil.getRandom(0, HeroManager.s_instance.list_hero.Count - 1)];
            if(hero)
            {
                hero.addBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi,0.1f,5,"116",false,false));
                hero.addBuff(new Consts.BuffData(Consts.BuffType.CritRate, 5, 5, "116", false, false));
            }
        }

        AudioScript.s_instance.playSound("116_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon116"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon116>().init(heroLogicBase, enemyLogic);
    }

    private void OnDestroy()
    {
        // 删除全局buff
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.AtkBaiFenBi, "114");
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.AtkSpeed, "114");
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.CritDamage, "114");
    }
}
