using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform_PC : PlatformUtil
{
    public override void init()
    {
        Debug.Log("Platform_PC.init");
    }

    public override void vibrate()
    {
        //CommonUtil.Log("Platform_PC.vibrate");
    }

    public override long getTotalRam()
    {
        s_totalRam = 8000000000;
        return s_totalRam;
    }

    public override string getCountryZipCode()
    {
        return "US";
    }

    public override string getCPUInfo()
    {
        return "4-1300000";
    }

    public override string getDeviceModel()
    {
        return "PC";
    }

    public override int getApiLevel()
    {
        return 30;
    }

    public override void getREADPHONESTATE()
    {
    }

    public override bool checkREADPHONESTATE()
    {
        return false;
    }
}
