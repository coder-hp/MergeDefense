using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformUtil
{
    static PlatformUtil s_instance = null;

    public long s_totalRam = 0;

    public static PlatformUtil getInstance()
    {
        if (s_instance == null)
        {
#if UNITY_EDITOR
            s_instance = new Platform_PC();
#else
            s_instance = new Platform_Android();
#endif
            s_instance.init();
        }

        return s_instance;
    }

    public virtual void init()
    {
    }

    public virtual void vibrate()
    {
    }

    public virtual long getTotalRam()
    {
        return 8000000000;
    }

    public virtual string getCountryZipCode()
    {
        return "US";
    }

    public virtual string getCPUInfo()
    {
        return "0_0";
    }

    public virtual string getDeviceModel()
    {
        return "unknown";
    }

    public virtual int getApiLevel()
    {
        return 0;
    }

    public virtual void getREADPHONESTATE()
    {

    }

    public virtual bool checkREADPHONESTATE()
    {
        return true;
    }
}
