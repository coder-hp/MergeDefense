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
        heroLogicBase.playAni("idle");
    }
}
