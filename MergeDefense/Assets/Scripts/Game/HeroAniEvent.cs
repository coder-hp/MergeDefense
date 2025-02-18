using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAniEvent : MonoBehaviour
{
    HeroLogicBase heroLogicBase = null;

    [HideInInspector]
    public EnemyLogic enemyLogic = null;

    private void Awake()
    {
        heroLogicBase = transform.parent.GetComponent<HeroLogicBase>();
    }

    public void onAttack()
    {
        if (enemyLogic)
        {
            switch (heroLogicBase.id)
            {
                // 剑士
                case 101:
                    {
                        enemyLogic.damage(heroLogicBase.getAtk());
                        break;
                    }

                // 射手
                case 102:
                    {
                        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/arrow"), GameLayer.s_instance.flyPoint).transform;
                        arrow.position = heroLogicBase.centerPoint.position;
                        arrow.GetComponent<ArrowLogic>().init(heroLogicBase.getAtk(),enemyLogic);
                        break;
                    }

                // 法师
                case 103:
                    {
                        Transform arrow = Instantiate(ObjectPool.getPrefab("Prefabs/Games/arrow"), GameLayer.s_instance.flyPoint).transform;
                        arrow.position = heroLogicBase.centerPoint.position;
                        arrow.GetComponent<ArrowLogic>().init(heroLogicBase.getAtk(), enemyLogic);
                        break;
                    }
            }
        }

        //// 单体攻击
        //if (heroLogicBase.heroData.isAtkSingle == 1)
        //{
        //    enemyLogic.damage(heroLogicBase.getAtk());
        //}
        //// 群体攻击
        //else
        //{

        //}
    }

    public void onAttackEnd()
    {
        heroLogicBase.isAttacking = false;

        if(!heroLogicBase.checkAttack())
        {
            heroLogicBase.playAni("idle",0.3f);
            if (transform.localEulerAngles.y != 180)
            {
                heroLogicBase.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
