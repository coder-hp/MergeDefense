using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroLogicSword : MonoBehaviour
{
    HeroLogicBase heroLogicBase;

    private void Start()
    {
        heroLogicBase = GetComponent<HeroLogicBase>();
    }
}
