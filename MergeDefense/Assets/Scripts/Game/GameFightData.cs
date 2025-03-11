using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFightData : MonoBehaviour
{
    public static GameFightData s_instance = null;

    [HideInInspector]
    public List<int> list_heroWeight = new List<int>() { 100, 0, 0, 0, 0, 0, 0, 0, 00, 0 };          // 角色1-10星的召唤权重
    [HideInInspector]
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

    private void Awake()
    {
        s_instance = this;
    }
}
