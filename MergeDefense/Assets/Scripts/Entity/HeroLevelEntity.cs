using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HeroLevelData
{
    public int id;
    public int level;
    public int exp;
}

public class HeroLevelEntity
{
    static HeroLevelEntity s_instance = null;
    public List<HeroLevelData> list;

    static public HeroLevelEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new HeroLevelEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<HeroLevelData>("heroLevel");
    }

    public HeroLevelData getData(int id,int level)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].id == id && list[i].level == level)
            {
                return list[i];
            }
        }

        return null;
    }
}
