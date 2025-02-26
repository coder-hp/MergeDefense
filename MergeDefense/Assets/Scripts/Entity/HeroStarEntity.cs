using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HeroStarData
{
    public int star;
    public float baseAtkXiShu;
    public float sellPrice;
}

public class HeroStarEntity
{
    static HeroStarEntity s_instance = null;
    public List<HeroStarData> list;

    static public HeroStarEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new HeroStarEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<HeroStarData>("heroStar");
    }

    public HeroStarData getData(int star)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].star == star)
            {
                return list[i];
            }
        }

        return null;
    }
}
