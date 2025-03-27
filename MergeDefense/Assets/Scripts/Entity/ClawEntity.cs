using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class ClawData
{
    public int id;
    public int eggstyle;
    public int rewardtype;
    public int count;
    public int spawnrate;
    public int droprate;
    public string bonus;
}

public class ClawEntity
{
    static ClawEntity s_instance = null;
    public List<ClawData> list;

    List<int> list_rate = new List<int>();

    static public ClawEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new ClawEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<ClawData>("claw");

        for(int i = 0; i < list.Count; i++)
        {
            list_rate.Add(list[i].spawnrate);
        }
    }

    public ClawData getData(int id)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if(id == list[i].id)
            {
                return list[i];
            }
        }

        return null;
    }

    public ClawData getRandomEgg()
    {
        return list[RandomUtil.SelectProbability(list_rate)];
    }
}
