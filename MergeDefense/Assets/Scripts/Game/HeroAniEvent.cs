using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAniEvent : MonoBehaviour
{
    HeroLogicBase heroLogicBase = null;
    HeroBase heroBase = null;

    [HideInInspector]
    public EnemyLogic enemyLogic = null;

    private void Awake()
    {
        heroLogicBase = transform.parent.parent.GetComponent<HeroLogicBase>();
        heroBase = transform.parent.parent.GetComponent<HeroBase>();
    }

    public void onAttack()
    {
        if (heroBase)
        {
            heroBase.AttackLogic(enemyLogic);
        }
    }

    public void onAttackEnd()
    {
        heroLogicBase.isAttacking = false;

        if(!heroLogicBase.checkAttack())
        {
            heroLogicBase.playAni(Consts.HeroAniNameIdle, 0.1f);
        }
    }
}
