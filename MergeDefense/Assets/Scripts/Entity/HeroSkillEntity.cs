using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class HeroSkillData
{
    public int id;
    public string name;
    public string desc;
}

public class HeroSkillEntity
{
    static HeroSkillEntity s_instance = null;
    public List<HeroSkillData> list;

    static public HeroSkillEntity getInstance()
    {
        if (s_instance == null)
        {
            s_instance = new HeroSkillEntity();
            s_instance.init();
        }

        return s_instance;
    }

    public void init()
    {
        list = JsonUtils.loadJsonToList<HeroSkillData>("heroSkill");
    }

    public HeroSkillData getData(int id)
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
