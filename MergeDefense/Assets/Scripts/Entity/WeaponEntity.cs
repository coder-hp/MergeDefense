using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class WeaponData
{
    public int id;
    public int type;
    public string name;
    public int level;
}

public class WeaponEntity
{
    static WeaponEntity s_instance = null;
    public List<WeaponData> list;

    static public WeaponEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new WeaponEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<WeaponData>("weapon");
    }

    public WeaponData getData(int type,int level)
    {
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].type == type && list[i].level == level)
            {
                return list[i];
            }
        }

        return null;
    }
}
