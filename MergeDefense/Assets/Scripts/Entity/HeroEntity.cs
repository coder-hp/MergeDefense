using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HeroData
{
    public int id;
    public string name;
    public int atk;
    public float atkSpeed;
    public int isJinZhan;
    public int isAtkSingle;
    public int critRate;
    public int critDamage;
    public string quality;
    public string goodWeapon;
    public string badWeapon;
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
