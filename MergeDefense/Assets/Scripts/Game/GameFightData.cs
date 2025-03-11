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

    //[HideInInspector]
    public List<int> list_canSummonHero = new List<int>() {101,102,103,104,105 };                    // 初始可以召唤的角色
    //[HideInInspector]
    public List<int> list_heroWeight = new List<int>() { 100, 0, 0, 0, 0, 0, 0, 0, 00, 0 };          // 角色1-10星的召唤权重
    //[HideInInspector]
    public List<int> list_weaponWeight = new List<int>() { 100, 0, 0, 0, 0, 0, 0, 0, 0, 0 };         // 武器1-10级的锻造权重

    [HideInInspector]
    public bool isGameOver = false;
    [HideInInspector]
    public bool isCanOnInvokeBoCiSecond = true;

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

    int heroHighStarRate = 0;
    int weaponHighLevelRate = 0;

    public void changeHeroHighStarRate(int rate)
    {
        heroHighStarRate += rate;

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

        if(restRate > 0)
        {
            changeHeroHighStarRate(restRate);
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
}
