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
    public string career;
    public string skills;
    public int maxStar;
    public int atk;
    public float atkSpeed;
    public float atkRange;
    public int critRate;
    public float critDamage;
    public int goodWeapon;      // 0:没有擅长武器    -1:擅长所有武器
    public int badWeapon;       // 0:没有不擅长武器  -1:不擅长所有武器
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
