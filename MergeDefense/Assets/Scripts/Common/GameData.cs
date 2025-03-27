using UnityEngine;

public class GameData
{
    public static int getMyGold()
    {
        return PlayerPrefs.GetInt("gold", 0);
    }

    public static void changeMyGold(int value, string reason)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getMyGold() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("gold", curCount);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
        if(HeroUpgradeLayer.s_instance)
        {
            HeroUpgradeLayer.s_instance.refreshUI();
        }
    }

    public static int getMyDiamond()
    {
        return PlayerPrefs.GetInt("Diamond", 0);
    }

    public static void changeMyDiamond(int value)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getMyDiamond() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("Diamond", curCount);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
        if (HeroUpgradeLayer.s_instance)
        {
            HeroUpgradeLayer.s_instance.refreshUI();
        }
    }

    public static int getClawTicket()
    {
        return PlayerPrefs.GetInt("ClawTicket", 0);
    }

    public static void changeClawTicket(int value)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getClawTicket() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("ClawTicket", curCount);
    }

    public static int getMyTiLi()
    {
        return PlayerPrefs.GetInt("TiLi", 99);
    }

    public static void changeMyTiLi(int value)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getMyTiLi() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("TiLi", curCount);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
    }

    public static int getPlayerLevel()
    {
        return PlayerPrefs.GetInt("PlayerLevel", 1);
    }

    public static void addPlayerLevel(int value)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getPlayerLevel() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("PlayerLevel", curCount);

        //if (MainLayer.s_instance)
        //{
        //    MainLayer.s_instance.refreshUI();
        //}
    }

    public static int getPlayerExp()
    {
        return PlayerPrefs.GetInt("PlayerExp", 0);
    }

    public static void setPlayerExp(int exp)
    {
        PlayerPrefs.SetInt("PlayerExp", exp);
    }

    public static void changePlayerExp(int value)
    {
        if (value == 0)
        {
            return;
        }

        int curCount = getPlayerExp() + value;
        curCount = curCount < 0 ? 0 : curCount;

        int curLevel = getPlayerLevel();
        PlayerLevelData nextPlayerLevelData = PlayerLevelEntity.getInstance().getData(curLevel + 1);
        if (nextPlayerLevelData != null)
        {
            if(curCount >= nextPlayerLevelData.exp)
            {
                addPlayerLevel(1);
                curCount -= nextPlayerLevelData.exp;
            }
        }

        PlayerPrefs.SetInt("PlayerExp", curCount);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
    }

    public static int getIsOpenVibrate()
    {
        return PlayerPrefs.GetInt("IsOpenVibrate", 1);
    }

    public static void setIsOpenVibrate(int value)
    {
        PlayerPrefs.SetInt("IsOpenVibrate", value);
    }

    public static int getOpenCount()
    {
        return PlayerPrefs.GetInt("OpenCount", 0);
    }

    public static int addOpenCount()
    {
        int count = getOpenCount() + 1;
        PlayerPrefs.SetInt("OpenCount", count);
        return count;
    }

    static string curLanguage = "";
    public static string getLanguage()
    {
        if (curLanguage == "")
        {
            curLanguage = PlayerPrefs.GetString("language", "");
            if (curLanguage == "")
            {
                //curLanguage = PlatformUtil.getInstance().getCountryZipCode();
                //curLanguage = LanguageManager.getDefaultLanguage(curLanguage);
                //setLanguage(curLanguage);
            }
        }

        return curLanguage;
    }

    public static void setLanguage(string language)
    {
        curLanguage = language;
        PlayerPrefs.SetString("language", language);
    }

    public static string getFirstOpenTime()
    {
        if (PlayerPrefs.GetString("FirstOpenTime", "") == "")
        {
            setFirstOpenTime();
        }

        return PlayerPrefs.GetString("FirstOpenTime", "");
    }

    public static void setFirstOpenTime()
    {
        if (PlayerPrefs.GetString("FirstOpenTime", "") == "")
        {
            PlayerPrefs.SetString("FirstOpenTime", CommonUtil.getCurYearMonthDay());
        }
    }

    public static bool isUnlockHero(int heroId)
    {
        return PlayerPrefs.GetInt("isUnlockHero" + heroId, 0) == 0 ? false : true;
    }

    public static void unlockHero(int heroId)
    {
        PlayerPrefs.SetInt("isUnlockHero" + heroId, 1);
    }

    public static string getUID()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }

    public static string getName()
    {
        return PlayerPrefs.GetString("name", SystemInfo.deviceUniqueIdentifier.Substring(0, 8));
    }

    public static void setName(string name)
    {
        PlayerPrefs.SetString("name", name);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
    }

    public static int getHead()
    {
        return PlayerPrefs.GetInt("head",101);
    }

    public static void setHead(int head)
    {
        PlayerPrefs.SetInt("head", head);

        if (MainLayer.s_instance)
        {
            MainLayer.s_instance.refreshUI();
        }
    }

    public static int getLevel()
    {
        return PlayerPrefs.GetInt("level", 1);
    }

    public static void setLevel(int level)
    {
        PlayerPrefs.SetInt("level", level);
    }

    public static string getMaxWaveDamage()
    {
        return PlayerPrefs.GetString("MaxWaveDamage", "0_0");
    }

    public static void setMaxWaveDamage(int wave, long damage)
    {
        PlayerPrefs.SetString("MaxWaveDamage", wave + "_" + damage);
    }

    public static int getHeroLevel(int id)
    {
        return PlayerPrefs.GetInt("HeroLevel" + id, 1);
    }

    public static void setHeroLevel(int id, int level)
    {
        PlayerPrefs.SetInt("HeroLevel" + id, level);
    }

    public static int getHeroExp(int id)
    {
        return PlayerPrefs.GetInt("HeroExp" + id, 0);
    }

    public static void changeHeroExp(int id, int exp)
    {
        int value = getHeroExp(id) + exp;
        if (value < 0)
        {
            value = 0;
        }
        PlayerPrefs.SetInt("HeroExp" + id, value);
    }

    public static float getMusicVolume()
    {
        return PlayerPrefs.GetFloat("MusicVolume", 1);
    }

    public static void setMusicVolume(float volume)
    {
        PlayerPrefs.SetFloat("MusicVolume", volume);
        AudioScript.s_instance.setMusicVolume(volume);
    }

    public static float getSoundVolume()
    {
        return PlayerPrefs.GetFloat("SoundVolume", 1);
    }

    public static void setSoundVolume(float volume)
    {
        PlayerPrefs.SetFloat("SoundVolume", volume);
        AudioScript.s_instance.setSoundVolume(volume);
    }
}
