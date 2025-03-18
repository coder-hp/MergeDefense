using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 深渊巨龙
// 远程单体攻击
// 技能1：成功召唤时，对所有敌人造成攻击力3000%的伤害
// 技能2：场上敌人数量大于30时，攻击提升20%
// 技能3：攻击时，10%概率对范围内的敌人造成攻击力1500%的伤害，并追加其最大生命值10%的伤害
// 技能4：每攻击20次，喷射三个火球，每个火球对范围内的敌人造成攻击力2500%的伤害
public class HeroLogic118 : HeroBase
{
    private void Start()
    {
        // 技能1：成功召唤时，对所有敌人造成攻击力3000%的伤害
        int atk = heroLogicBase.getAtk() * 30;
        for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        {
            if (EnemyManager.s_instance.list_enemy[i].damage(atk, false))
            {
                --i;
            }
        }

        InvokeRepeating("onInvokeSkill2",1,1);
    }

    // 技能2：场上敌人数量大于30时，攻击提升20%
    void onInvokeSkill2()
    {
        if (EnemyManager.s_instance.list_enemy.Count >= 30)
        {
            heroLogicBase.addBuff(new Consts.BuffData(Consts.BuffType.AtkBaiFenBi, 0.2f, 99999, "118", false, false));
        }
        else
        {
            heroLogicBase.removeBuff(Consts.BuffType.AtkBaiFenBi, "118");
        }
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("118_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon118"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon118>().init(heroLogicBase, enemyLogic);
    }
}
