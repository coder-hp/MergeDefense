using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 灭霸猫咪
// 远程单体攻击
// 技能1：每波WAVE开始时5%的概率消灭一半敌人
// 技能2：每波WAVE时长增加3s
// 技能3：每8s触发一次，普攻将替换为攻击力1000%的范围伤害，直到有敌人阵亡
public class HeroLogic119 : HeroBase
{
    [HideInInspector]
    public bool isSkill3Trigger = false;

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

        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Effects/eff_attack_hero119"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<HeroFlyWeaponBase>().init(heroLogicBase, enemyLogic);
        arrow.GetComponent<heroFlyWeapon119>().heroLogic119 = this;
    }

    public void skill3End()
    {
        isSkill3Trigger = false;
        Invoke("onInvokeSkill3", 8);
    }
}
