using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchScript : MonoBehaviour
{
    public static LaunchScript s_instance = null;

    public Camera uiCamera;
    public Canvas canvas;
    public Transform canvas_top;

    private void Awake()
    {
        s_instance = this;

        // 禁用多点触摸
        Input.multiTouchEnabled = false;

        // 设备禁止休眠
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // 游戏主线程强制转化为英语环境
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("en-US");

        // 设置帧率
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        // 新玩家进来需要的处理
        if(GameData.addOpenCount() == 1)
        {
            // 解锁所有白蓝紫角色
            for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
            {
                if (HeroEntity.getInstance().list[i].price == 0)
                {
                    GameData.unlockHero(HeroEntity.getInstance().list[i].id);
                }
            }
        }
    }
}
