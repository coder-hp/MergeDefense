using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class KillRewardData
{
    public int wave;
    public int diamond;
    public int heroStar;
    public int weaponLevel;
}

public class KillRewardEntity
{
    static KillRewardEntity s_instance = null;
    public List<KillRewardData> list;

    static public KillRewardEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new KillRewardEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<KillRewardData>("killReward");
    }

    public KillRewardData getData(int wave)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].wave == wave)
            {
                return list[i];
            }
        }

        return null;
    }
}
