using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFightData : MonoBehaviour
{
    public static GameFightData s_instance = null;

    private void Awake()
    {
        s_instance = this;
    }

    public long allDamage = 0;

    //[HideInInspector]
    public List<int> list_canSummonHero = new List<int>() {101,102,103,104,105 };                    // 初始可以召唤的角色
    //[HideInInspector]
    public List<int> list_heroWeight = new List<int>() { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };           // 角色1-10星的召唤权重
    //[HideInInspector]
    public List<int> list_weaponWeight = new List<int>() { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };         // 武器1-10级的锻造权重

    [HideInInspector]
    public List<Consts.BuffData> list_globalHeroBuff = new List<Consts.BuffData>();                  // 对所有角色生效的buff,from不可重复
    [HideInInspector]
    public List<Consts.BuffData> list_globalEnemyBuff = new List<Consts.BuffData>();                 // 对所有敌人生效的buff,from不可重复

    [HideInInspector]
    public bool isGameOver = false;
    [HideInInspector]
    public bool isCanOnInvokeBoCiSecond = true;
    [HideInInspector]
    public bool isAddEnemyEnd = false;
    [HideInInspector]
    public bool isUsedHeroSkill_116_2 = false;

    [HideInInspector]
    public int addedEnemyCount = 0;
    [HideInInspector]
    public int curBoCi = 0;
    [HideInInspector]
    public int curBoCiRestTime = 20;
    [HideInInspector]
    public int curGold = Consts.startHaveGold;
    [HideInInspector]
    public int curDiamond = Consts.startHaveDiamond;
    [HideInInspector]
    public int curSummonGold = Consts.startSummonGold;
    [HideInInspector]
    public int curForgeGold = Consts.startForgeGold;

    [HideInInspector]
    public float gameTimeScale = 1;

    int heroHighStarRate = 0;
    int weaponHighLevelRate = 0;

    public void changeHeroHighStarRate(int rate,bool isRepeatCall = false)
    {
        if(!isRepeatCall)
        {
            heroHighStarRate += rate;
        }

        // 溢出的概率
        int restRate = 0;

        for (int i = 0; i < list_heroWeight.Count; i++)
        {
            if (list_heroWeight[i] > 0 && (i < list_heroWeight.Count - 1))
            {
                if(list_heroWeight[i] < rate)
                {
                    restRate = rate - list_heroWeight[i];
                    rate = list_heroWeight[i];
                }
                list_heroWeight[i] -= rate;
                list_heroWeight[i + 1] += rate;
                break;
            }
        }

        // 第10级橙色1星不能有概率
        if (list_heroWeight[list_heroWeight.Count - 1] > 0)
        {
            list_heroWeight[0] = 0;
            list_heroWeight[1] = 0;
            list_heroWeight[2] = 0;
            list_heroWeight[3] = 0;
            list_heroWeight[4] = 0;
            list_heroWeight[5] = 0;
            list_heroWeight[6] = 0;
            list_heroWeight[7] = 0;
            list_heroWeight[8] = 100;
            list_heroWeight[9] = 0;

            restRate = 0;

            Debug.Log("角色召唤概率已达上限");
        }

        if (restRate > 0)
        {
            changeHeroHighStarRate(restRate, true);
        }
    }

    public void changeWeaponHighLevelRate(int rate)
    {
        weaponHighLevelRate += rate;

        // 溢出的概率
        int restRate = 0;

        for (int i = 0; i < list_weaponWeight.Count; i++)
        {
            if (list_weaponWeight[i] > 0 && (i < list_weaponWeight.Count - 1))
            {
                if (list_weaponWeight[i] < rate)
                {
                    restRate = rate - list_weaponWeight[i];
                    rate = list_weaponWeight[i];
                }
                list_weaponWeight[i] -= rate;
                list_weaponWeight[i + 1] += rate;
                break;
            }
        }

        if (restRate > 0)
        {
            changeWeaponHighLevelRate(restRate);
        }
    }

    public void changeHeroWeight(List<int> list)
    {
        list_heroWeight = list;

        int temp = heroHighStarRate;
        heroHighStarRate = 0;
        changeHeroHighStarRate(temp);
    }

    public void changeWeaponWeight(List<int> list)
    {
        list_weaponWeight = list;

        int temp = weaponHighLevelRate;
        weaponHighLevelRate = 0;
        changeWeaponHighLevelRate(temp);
    }

    public int randomSummonHero()
    {
        return list_canSummonHero[RandomUtil.getRandom(0, list_canSummonHero.Count - 1)];
    }

    public void addAllDamage(int dmg)
    {
        allDamage += dmg;
    }

    public void addGlobalHeroBuff(Consts.BuffData buffData)
    {
        for(int i = 0; i < list_globalHeroBuff.Count; i++)
        {
            if (list_globalHeroBuff[i].buffType == buffData.buffType && list_globalHeroBuff[i].from == buffData.from)
            {
                ++list_globalHeroBuff[i].addedCount;
                return;
            }
        }

        list_globalHeroBuff.Add(buffData);
    }

    public void removeGlobalHeroBuff(Consts.BuffType buffType, string from)
    {
        for (int i = 0; i < list_globalHeroBuff.Count; i++)
        {
            if (list_globalHeroBuff[i].buffType == buffType && list_globalHeroBuff[i].from == from)
            {
                --list_globalHeroBuff[i].addedCount;
                if (list_globalHeroBuff[i].addedCount <= 0)
                {
                    list_globalHeroBuff.RemoveAt(i);
                }
                break;
            }
        }
    }

    public void addGlobalEnemyBuff(Consts.BuffData buffData)
    {
        for (int i = 0; i < list_globalEnemyBuff.Count; i++)
        {
            if (list_globalEnemyBuff[i].buffType == buffData.buffType && list_globalEnemyBuff[i].from == buffData.from)
            {
                ++list_globalEnemyBuff[i].addedCount;
                return;
            }
        }

        list_globalEnemyBuff.Add(buffData);
    }

    public void removeGlobalEnemyBuff(Consts.BuffType buffType, string from)
    {
        for (int i = 0; i < list_globalEnemyBuff.Count; i++)
        {
            if (list_globalEnemyBuff[i].buffType == buffType && list_globalEnemyBuff[i].from == from)
            {
                --list_globalEnemyBuff[i].addedCount;
                if (list_globalEnemyBuff[i].addedCount <= 0)
                {
                    list_globalEnemyBuff.RemoveAt(i);
                }
                break;
            }
        }
    }
}
