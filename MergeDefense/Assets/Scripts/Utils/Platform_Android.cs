using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_Android : PlatformUtil
{
    AndroidJavaClass androidUtils = null;
    AndroidJavaClass jcUnityPlayer = null;
    AndroidJavaObject u3dActivity = null;

    public override void init()
    {
        try
        {
            Debug.Log("Platform_Android.init");

            androidUtils = new AndroidJavaClass("beatmaker.edm.musicgames.mylibrary.AndroidTools");
            jcUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            if (jcUnityPlayer != null)
            {
                u3dActivity = jcUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                if (u3dActivity == null)
                {
                    Debug.Log("u3dActivity == null");
                }
            }
            else
            {
                Debug.Log("jcUnityPlayer == null");
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.init:" + ex.ToString());
        }
    }

    public override void vibrate()
    {
        try
        {
            if (GameData.getIsOpenVibrate() == 1)
            {
                if (u3dActivity != null)
                {
                    androidUtils.CallStatic("vibrate", u3dActivity, 30);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.vibrate异常----" + ex);
        }
    }

    public override long getTotalRam()
    {
        if(s_totalRam != 0)
        {
            return s_totalRam;
        }

        try
        {
            if (u3dActivity != null)
            {
                s_totalRam = androidUtils.CallStatic<long>("getTotalRam", u3dActivity);
                Debug.Log("TotalRam = " + s_totalRam);

                return s_totalRam;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getTotalRam----" + ex);
            return 2000000000;
        }
        return 2000000000;
    }

    public override string getCountryZipCode()
    {
        try
        {
            if (u3dActivity != null)
            {
                string code = androidUtils.CallStatic<string>("getCountryZipCode", u3dActivity);
                Debug.Log("countryZipCode = " + code);

                return code;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getCountryZipCode----" + ex);
            return "US";
        }

        return "US";
    }

    public override string getCPUInfo()
    {
        try
        {
            if (u3dActivity != null)
            {
                string code = androidUtils.CallStatic<string>("getCPUInfo");
                Debug.Log("getCPUInfo = " + code);

                return code;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getCPUInfo----" + ex);
            return "0-0";
        }

        return "0-0";
    }

    public override string getDeviceModel()
    {
        try
        {
            if (u3dActivity != null)
            {
                string model = androidUtils.CallStatic<string>("getDeviceModel");
                Debug.Log("getDeviceModel = " + model);

                return model;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getDeviceModel----" + ex);
            return "unknown-android";
        }

        return "unknown-android";
    }

    public override int getApiLevel()
    {
        try
        {
            if (u3dActivity != null)
            {
                int level = androidUtils.CallStatic<int>("getApiLevel");
                Debug.Log("getApiLevel = " + level);

                return level;
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getApiLevel----" + ex);
            return 0;
        }

        return 0;
    }

    public override void getREADPHONESTATE()
    {
        try
        {
            if (u3dActivity != null)
            {
                androidUtils.CallStatic("getREADPHONESTATE", u3dActivity);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.getREADPHONESTATE异常----" + ex);
        }
    }

    public override bool checkREADPHONESTATE()
    {
        try
        {
            if (u3dActivity != null)
            {
                return androidUtils.CallStatic<bool>("checkREADPHONESTATE", u3dActivity);
            }
        }
        catch (Exception ex)
        {
            Debug.Log("Platform_Android.checkREADPHONESTATE----" + ex);

            return false;
        }

        return false;
    }
}
