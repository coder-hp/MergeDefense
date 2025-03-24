using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class BattleMissionData
{
    public int id;
    public string desc;
    public int time;
    public int value;
    public string reward;
    public int weight;
}

public class BattleMissionEntity
{
    static BattleMissionEntity s_instance = null;
    public List<BattleMissionData> list;
    public List<int> list_weight = new List<int>();

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

        int allWeight = 0;
        for(int i = 0; i < list.Count; i++)
        {
            list_weight.Add(list[i].weight);
            allWeight += list[i].weight;
        }
        Debug.Log("battleMission AllWeight=" + allWeight);
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
