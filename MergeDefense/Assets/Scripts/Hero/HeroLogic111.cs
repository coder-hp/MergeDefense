using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 精灵王
// 远程单体
// 技能1：我方单位攻速增加20%
// 技能2：每2s释放三支箭对范围内的敌人造成攻击力500%的伤害
// 技能3：攻击时，15%概率对范围内的敌人造成攻击力1000%的伤害
public class HeroLogic111 : HeroBase
{
    private void Start()
    {
        // 技能1：我方单位攻速增加20%
        GameFightData.s_instance.addGlobalHeroBuff(new Consts.BuffData(Consts.BuffType.AtkSpeed, 0.2f, 9999,"111", true, true));

        InvokeRepeating("onInvokeSkill", 2, 2);
    }

    // 技能2：每2s释放三支箭对范围内的敌人造成攻击力500%的伤害
    void onInvokeSkill()
    {
        if (heroLogicBase.isCanUpdate)
        {
            int atk = heroLogicBase.getAtk() * 5;
            int count = 0;
            for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
            {
                if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
                {
                    Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon111"), GameLayer.s_instance.flyPoint).transform;
                    arrow.GetComponent<HeroFlyWeaponBase>().init(heroLogicBase, EnemyManager.s_instance.list_enemy[i], atk);

                    if (++count >= 3)
                    {
                        return;
                    }
                }
            }
        }
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("111_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon111"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<HeroFlyWeaponBase>().init(heroLogicBase, enemyLogic);
    }

    private void OnDestroy()
    {
        // 删除全局角色加攻速buff
        GameFightData.s_instance.removeGlobalHeroBuff(Consts.BuffType.AtkSpeed,"111");
    }
}
