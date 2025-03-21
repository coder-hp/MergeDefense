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

    public static int startHaveGold = 100;      // 进入游戏初始金币
    public static int startHaveDiamond = 10;    // 进入游戏初始钻石
    public static int startSummonGold = 20;     // 初始召唤金额
    public static int summonAddGold = 2;        // 每召唤一次增加的金额
    public static int startForgeGold = 20;      // 初始锻造金额
    public static int forgeAddGold = 2;         // 每锻造一次增加的金额


    public static int maxEnemyCount = 60;      // 最大存在怪物数量

    // 103、110、115、116
    public static float megaSkillRange = 1.9f; // 所有法师的技能范围

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
        HeroInfoPanel,
        WeaponInfoPanel,
        WeaponShopPanel,
        KillEnemyRewardPanel,
        BossComingLayer,
        BossNoticeLayer,
        BossRewardPanel,
        MythicHeroLayer,
        GetMythicHeroLayer,
        ClawLayer,
        RankLayer,
        HeroLayer,
        BattleLayer,
    }

    public enum WeaponType
    {
        Sword = 1,          // 刀剑
        Arrow,              // 弓箭
        Axe,                // 斧头
        Knuckle,            // 拳套
        Staff,              // 法杖
        End
    }

    public enum BuffType
    {
        Atk,                // 0攻击力
        AtkSpeed,           // 1攻速
        CritRate,           // 2暴击率
        CritDamage,         // 3暴击伤害
        MoveSpeed,          // 4移速
        SkillRate,          // 5技能概率
        Stun,               // 6眩晕
        AtkBaiFenBi,        // 7攻击力百分比
        YiShang,            // 8易伤，受到的伤害+20%，不能叠加
    }

    public class BuffData
    {
        public BuffType buffType;
        public float value;
        public float time;
        public string from;
        public bool isForever = false;
        public bool isCanRepeatFrom = false;

        // 全局buff使用
        public int addedCount = 1;

        public BuffData(BuffType _buffType, float _value, float _time,string _from , bool _isForever, bool _isCanRepeatFrom)
        {
            buffType = _buffType;
            value = _value;
            time = _time;
            from = _from;
            isForever = _isForever;
            isCanRepeatFrom = _isCanRepeatFrom;
        }
    }

    // 角色不同品质的颜色
    public static List<Color> list_heroQualityColor = new List<Color>()
    {
        CommonUtil.stringToColor("#FFFFFF"),    // 默认色
        CommonUtil.stringToColor("#6E8999"),    // 白
        CommonUtil.stringToColor("#457dd8"),    // 蓝
        CommonUtil.stringToColor("#9146da"),    // 紫
        CommonUtil.stringToColor("#eb9b10"),    // 橙
    };

    public static string HeroAniNameIdle = "idle";
    public static string HeroAniNameAttack = "attack";
    public static string HeroAniNameRun = "run";

    public static Vector3 vec_flipX = new Vector3(-1,1,1);
    public static Vector2 weaponItemDragOffset = new Vector2(0, 100);
    public static Vector3 mouseRayOffset = new Vector3(0, 1,0);
    public static Vector3 heroQualityOffset = new Vector3(0, 0f, -0.01f);
    public static Vector3 heroSellBtnOffset = new Vector3(0, -0.9f, 0);
    public static Vector3 summonHeroBigScale = new Vector3(1.3f, 1.3f, 1.3f);
    public static Vector3 weapinUIIconStartScale = new Vector3(1.6f, 1.6f, 1.6f);
    public static Vector3 heroIdleAngle = new Vector3(0, 180, 0);

    public static Color color_weaponCantEquip = new Color(1,0,0, 0.5f);
    public static Color color_critText = new Color(1, 0.45f, 0.2f, 1);

    public static string getServerUrl()
    {
        //return "http://127.0.0.1:8001/";

        if (GMLayer.s_instance.isDebug)
        {
            return "http://10.26.53.6:8001/";
        }

        return "http://10.26.53.6:8001/";
    }
}
