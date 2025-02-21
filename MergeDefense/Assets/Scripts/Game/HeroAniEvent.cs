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
        heroLogicBase = transform.parent.GetComponent<HeroLogicBase>();
        heroBase = transform.parent.GetComponent<HeroBase>();
    }

    public void onAttack()
    {
        if (enemyLogic)
        {
            if(heroBase)
            {
                heroBase.AttackLogic(enemyLogic);
            }
        }
    }

    public void onAttackEnd()
    {
        heroLogicBase.isAttacking = false;

        if(!heroLogicBase.checkAttack())
        {
            heroLogicBase.playAni(Consts.HeroAniNameIdle, 0.3f);
            if (transform.localEulerAngles.y != 180)
            {
                heroLogicBase.transform.localRotation = Quaternion.Euler(0, 180, 0);
            }
        }
    }
}
