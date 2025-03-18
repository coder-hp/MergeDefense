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
        InvokeRepeating("onInvokeSkill",10,10);
    }

    void onInvokeSkill()
    {
        // 技能3：每10s召唤6个扭曲物质落于随机位置持续5s，对接触到的敌人造成600 % 的伤害，并附加20 % 的减速效果
        // if (heroLogicBase.isCanUpdate)
        {
            int atk = heroLogicBase.getAtk();

            // 上边的路
            {
                Transform obj = Instantiate(ObjectPool.getPrefab("Prefabs/Games/HeroSkill115_3"), GameLayer.s_instance.flyPoint).transform;
                obj.GetComponent<HeroSkill115_3>().init(atk);
                float posX = RandomUtil.getRandom((int)(GameLayer.s_instance.list_enemyMoveFourPos[1].x * 60), (int)(GameLayer.s_instance.list_enemyMoveFourPos[2].x * 60)) / 100f;
                obj.position = new Vector3(posX, GameLayer.s_instance.list_enemyMoveFourPos[1].y,0);
            }

            // 下边的路
            {
                Transform obj = Instantiate(ObjectPool.getPrefab("Prefabs/Games/HeroSkill115_3"), GameLayer.s_instance.flyPoint).transform;
                obj.GetComponent<HeroSkill115_3>().init(atk);
                float posX = RandomUtil.getRandom((int)(GameLayer.s_instance.list_enemyMoveFourPos[1].x * 60), (int)(GameLayer.s_instance.list_enemyMoveFourPos[2].x * 60)) / 100f;
                obj.position = new Vector3(posX, GameLayer.s_instance.list_enemyMoveFourPos[0].y, 0);
            }

            // 左边的路
            {
                Transform obj = Instantiate(ObjectPool.getPrefab("Prefabs/Games/HeroSkill115_3"), GameLayer.s_instance.flyPoint).transform;
                obj.GetComponent<HeroSkill115_3>().init(atk);
                float posY = RandomUtil.getRandom((int)(GameLayer.s_instance.list_enemyMoveFourPos[0].y * 60), (int)(GameLayer.s_instance.list_enemyMoveFourPos[1].y * 60)) / 100f;
                obj.position = new Vector3(GameLayer.s_instance.list_enemyMoveFourPos[0].x, posY, 0);
            }

            // 右边的路
            {
                Transform obj = Instantiate(ObjectPool.getPrefab("Prefabs/Games/HeroSkill115_3"), GameLayer.s_instance.flyPoint).transform;
                obj.GetComponent<HeroSkill115_3>().init(atk);
                float posY = RandomUtil.getRandom((int)(GameLayer.s_instance.list_enemyMoveFourPos[0].y * 60), (int)(GameLayer.s_instance.list_enemyMoveFourPos[1].y * 60)) / 100f;
                obj.position = new Vector3(GameLayer.s_instance.list_enemyMoveFourPos[3].x, posY, 0);
            }
        }
    }

    public override void AttackLogic(EnemyLogic enemyLogic)
    {
        AudioScript.s_instance.playSound("115_attack");
        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/heroFlyWeapon115"), GameLayer.s_instance.flyPoint).transform;
        arrow.GetComponent<heroFlyWeapon115>().init(this,heroLogicBase, enemyLogic);
    }
}
