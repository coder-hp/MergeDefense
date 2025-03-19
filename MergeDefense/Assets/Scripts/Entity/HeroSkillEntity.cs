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

        for (int i = 0; i < list.Count; i++)
        {
            list[i].desc = HighlightNumbersInString(list[i].desc, "FFAB41");
        }
    }

    // 高亮字符串中的数字和%
    string HighlightNumbersInString(string input, string colorHex)
    {
        // 遍历字符串，找到数字和%并添加<color>标签
        System.Text.StringBuilder result = new System.Text.StringBuilder();
        int i = 0;
        while (i < input.Length)
        {
            if (char.IsDigit(input[i]) || input[i] == '%')
            {
                // 找到数字或%的开头
                int start = i;

                // 继续查找连续的数字或%
                while (i < input.Length && (char.IsDigit(input[i]) || input[i] == '%'))
                {
                    i++;
                }

                // 提取数字或%部分
                string number = input.Substring(start, i - start);

                // 添加<color>标签
                result.Append($"<color=#{colorHex}>{number}</color>");
            }
            else
            {
                // 非数字或%部分直接添加到结果
                result.Append(input[i]);
                i++;
            }
        }

        return result.ToString();
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
