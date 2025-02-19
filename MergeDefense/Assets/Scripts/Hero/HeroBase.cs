using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroBase : MonoBehaviour
{
    [HideInInspector]
    public HeroLogicBase heroLogicBase = null;

    private void Awake()
    {
        heroLogicBase = transform.GetComponent<HeroLogicBase>();
    }

    public virtual void AttackLogic(EnemyLogic enemyLogic)
    {

    }
}
