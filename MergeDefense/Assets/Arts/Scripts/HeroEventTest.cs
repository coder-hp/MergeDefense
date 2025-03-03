using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEventTest : MonoBehaviour
{
    public void onAttack()
    {
        Debug.Log("攻击");
    }

    public void onAttackEnd()
    {
        Debug.Log("攻击结束");
    }
}
