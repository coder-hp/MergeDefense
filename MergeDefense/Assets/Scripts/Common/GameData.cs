using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    public static int getMyGold()
    {
        return PlayerPrefs.GetInt("gold", 0);
    }

    public static void changeMyGold(int value,string reason)
    {
        if(value == 0)
        {
            return;
        }

        int curCount = getMyGold() + value;
        curCount = curCount < 0 ? 0 : curCount;
        PlayerPrefs.SetInt("gold", curCount);

        //if (MainLayer.s_instance)
        //{
        //    MainLayer.s_instance.refreshUI();
        //}
    }

    public static int getIsOpenVibrate()
    {
        return PlayerPrefs.GetInt("IsOpenVibrate", 1);
    }

    public static void setIsOpenVibrate(int value)
    {
        PlayerPrefs.SetInt("IsOpenVibrate", value);
    }

    public static int getIsOpenMusic()
    {
        return PlayerPrefs.GetInt("IsOpenMusic", 1);
    }

    public static void setIsOpenMusic(int value)
    {
        PlayerPrefs.SetInt("IsOpenMusic", value);
    }

    public static int getIsOpenSound()
    {
        return PlayerPrefs.GetInt("IsOpenSound", 1);
    }

    public static void setIsOpenSound(int value)
    {
        PlayerPrefs.SetInt("IsOpenSound", value);
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
}
