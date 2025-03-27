using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class BattleMissionData
{
    public int id;
    public string desc;
    public string shortDesc;
    public int time;
    public int value;
    public string reward;
    public int wave;
}

public class BattleMissionEntity
{
    static BattleMissionEntity s_instance = null;
    public List<BattleMissionData> list;

    static public BattleMissionEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new BattleMissionEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<BattleMissionData>("battleMission");
    }

    public BattleMissionData getData(int id)
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
