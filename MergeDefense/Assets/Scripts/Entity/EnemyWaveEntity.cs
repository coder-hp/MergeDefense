using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class EnemyWaveData
{
    public int wave;
    public string prefab;
    public int enemyType;       // 1小怪  2精英  3Boss
    public int count;
    public int hp;
    public float speed;
    public int time;
}

public class EnemyWaveEntity
{
    static EnemyWaveEntity s_instance = null;
    public List<EnemyWaveData> list;

    static public EnemyWaveEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new EnemyWaveEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<EnemyWaveData>("enemyWave");
    }

    public EnemyWaveData getData(int wave)
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
