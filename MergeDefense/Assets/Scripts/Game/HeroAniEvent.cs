using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroAniEvent : MonoBehaviour
{
    HeroLogicBase heroLogicBase = null;

    private void Awake()
    {
        heroLogicBase = transform.parent.GetComponent<HeroLogicBase>();
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
                heroLogicBase.heroQualityTrans.rotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }
}
