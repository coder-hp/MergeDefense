using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HeroData
{
    public int id;
    public string name;
    public int quality;
    public int price;
    public string career;
    public string skills;
    public int maxStar;
    public int atk;
    public float atkSpeed;
    public float atkRange;
    public int critRate;
    public float critDamage;
    public int goodWeapon;      // 0:没有擅长武器
    public string summonWay;

    public List<int[]> list_summonWay = new List<int[]>();
}

public class HeroEntity
{
    static HeroEntity s_instance = null;
    public List<HeroData> list;

    static public HeroEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new HeroEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<HeroData>("hero");

        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].quality == 4)
            {
                string[] strArray1 = list[i].summonWay.Split(':');
                for(int j = 0; j < strArray1.Length; j++)
                {
                    string[] strArray2 = strArray1[j].Split('_');
                    int[] intArray = new int[strArray2.Length];
                    for(int m = 0; m < strArray2.Length; m++)
                    {
                        intArray[m] = int.Parse(strArray2[m]);
                    }
                    list[i].list_summonWay.Add(intArray);
                }
            }
        }
    }

    public HeroData getData(int id)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].id == id)
            {
                return list[i];
            }
        }

        return null;
    }
}
