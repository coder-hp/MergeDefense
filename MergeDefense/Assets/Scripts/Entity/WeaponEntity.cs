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
    public int buff1;      // 攻击力
    public float buff2;    // 攻击力百分比
    public string buff3;   // 不固定：暴击率、爆伤、攻速、技能概率

    public Consts.BuffType buff3Type;
    public float buff3Value;
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

        for (int i = 0; i < list.Count; i++)
        {
            string[] strArray = list[i].buff3.Split('_');
            list[i].buff3Type = (Consts.BuffType)int.Parse(strArray[0]);
            list[i].buff3Value = float.Parse(strArray[1]);
        }
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
