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

    public List<int> list_bonus = new List<int>();
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

            string[] bonusArray = list[i].bonus.Split('_');
            for(int j = 0; j < bonusArray.Length; j++)
            {
                list[i].list_bonus.Add(int.Parse(bonusArray[j]));
            }
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
