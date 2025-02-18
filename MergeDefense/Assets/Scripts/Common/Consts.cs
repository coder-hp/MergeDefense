using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consts
{
    public static long closeLightShdow = 4200000000;                 // 关闭灯光阴影
    public static long closeAntiAliasing = 4200000000;               // 关闭抗锯齿
    public static long openVSync= 5200000000;                        // 开启垂直同步
    public static long lowBuildEffect = 3200000000;                  // 降低建造特效
    public static long closeBlockRemoveScale = 3200000000;           // 关闭六角片消除缩放动画


    public enum Layer
    {
        MainLayer,
        ShopLayer,
        GameUILayer,
        LanguageLayer,
        SetLayer,
        GameLoadLayer,
        GameOverLayer,
        GamePauseLayer,
        GameLayer,
        ShowRewardLayer,
    }

    public static List<Color> list_weaponColor = new List<Color>()
    {
        CommonUtil.stringToColor("#FFD618"),    // 剑
        CommonUtil.stringToColor("#64D967"),    // 弓
        CommonUtil.stringToColor("#3BD3FF"),    // 斧
        CommonUtil.stringToColor("#FF6F6F"),    // 拳套
        CommonUtil.stringToColor("#E28BFF"),    // 魔杖
    };
}
