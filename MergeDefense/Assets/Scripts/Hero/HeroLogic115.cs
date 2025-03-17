using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 蜡烛人
// 远程群攻
// 技能1：杀死敌人后，攻击力+10
// 技能2：攻击时，10%概率对范围内的敌人造成攻击力2000%的伤害，并禁锢3s
// 技能3：每10s召唤6个扭曲物质落于随机位置持续5s，对接触到的敌人造成600%的伤害，并附加20%的减速效果
public class HeroLogic115 : HeroBase
{
    private void Start()
    {
        //InvokeRepeating("onInvokeSkill",10,10);
    }

    void onInvokeSkill()
    {
        //// 技能1：每1s对范围内的敌人造成攻击力150% 的伤害
        //if (heroLogicBase.isCanUpdate)
        //{
        //    int atk = heroLogicBase.getAtk();
        //    for (int i = 0; i < EnemyManager.s_instance.list_enemy.Count; i++)
        //    {
        //        if (Vector3.Distance(heroLogicBase.curStandGrid.position, EnemyManager.s_instance.list_enemy[i].transform.position) <= heroLogicBase.heroData.atkRange)
        //        {
        //            if (EnemyManager.s_instance.list_enemy[i].damage(Mathf.RoundToInt(atk * 1.5f), false))
        //            {
        //                --i;
        //            }
        //        }
        //    }
        //}
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("115_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon115"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon115>().init(heroLogicBase, enemyLogic);
    }
}
