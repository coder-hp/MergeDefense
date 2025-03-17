using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroManager : MonoBehaviour
{
    public static HeroManager s_instance = null;

    //[HideInInspector]
    public List<HeroLogicBase> list_hero = new List<HeroLogicBase>();

    private void Awake()
    {
        s_instance = this;
    }

    public int getHeroCount()
    {
        return list_hero.Count;
    }

    public void addHero(HeroLogicBase heroLogicBase)
    {
        list_hero.Add(heroLogicBase);
    }

    public void removeHero(HeroLogicBase heroLogicBase)
    {
        list_hero.Remove(heroLogicBase);
    }
}
