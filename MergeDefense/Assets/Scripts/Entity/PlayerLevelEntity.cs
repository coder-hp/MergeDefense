using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class PlayerLevelData
{
    public int level;
    public int exp;
}

public class PlayerLevelEntity
{
    static PlayerLevelEntity s_instance = null;
    public List<PlayerLevelData> list;

    static public PlayerLevelEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new PlayerLevelEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<PlayerLevelData>("playerLevel");
    }

    public PlayerLevelData getData(int level)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].level == level)
            {
                return list[i];
            }
        }

        return null;
    }
}
