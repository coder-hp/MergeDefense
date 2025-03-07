using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class KillRewardData
{
    public int wave;
    public int diamond;
    public int heroQuality;
    public int heroStar;
    public int weaponLevel;
    public string heroWeight;
    public string weaponWeight;

    public List<int> list_heroWeight = new List<int>();
    public List<int> list_weaponWeight = new List<int>();
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

        for (int i = 0; i < list.Count; i++)
        {
            int heroAllWeight = 0;
            int weaponAllWeight = 0;

            {
                string[] array = list[i].heroWeight.Split("_");
                for (int j = 0; j < array.Length; j++)
                {
                    list[i].list_heroWeight.Add(int.Parse(array[j]));
                    heroAllWeight += int.Parse(array[j]);
                }
            }

            {
                string[] array = list[i].weaponWeight.Split("_");
                for (int j = 0; j < array.Length; j++)
                {
                    list[i].list_weaponWeight.Add(int.Parse(array[j]));
                    weaponAllWeight += int.Parse(array[j]);
                }
            }

            if(heroAllWeight != 100)
            {
                Debug.LogError("killReward配置表异常");
            }

            if (weaponAllWeight != 100)
            {
                Debug.LogError("killReward配置表异常");
            }

            if (list[i].list_heroWeight.Count != 10)
            {
                Debug.LogError("killReward配置表异常");
            }
            if (list[i].list_weaponWeight.Count != 10)
            {
                Debug.LogError("killReward配置表异常");
            }
        }
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
