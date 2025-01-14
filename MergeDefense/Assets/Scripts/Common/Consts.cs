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
}
