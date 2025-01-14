using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerUtil : MonoBehaviour
{
    static TimerUtil s_instance = null;

    List<TimerData> timerDataList = new List<TimerData>();

    class TimerData
    {
        public float curTime = 0;

        public float endTime;
        public Action callback = null;

        public TimerData(float _endTime, Action _callback)
        {
            endTime = _endTime;
            callback = _callback;
        }
    }

    public static TimerUtil getInstance()
    {
        if (s_instance == null)
        {
            GameObject obj = new GameObject();
            s_instance = obj.AddComponent<TimerUtil>();
            obj.name = "TimerUtil";
            DontDestroyOnLoad(obj);
        }

        return s_instance;
    }

    public void delayTime(float timeSeconds, Action _callback)
    {
        timerDataList.Add(new TimerData(timeSeconds, _callback));
    }

    void Update()
    {
        for (int i = 0; i < timerDataList.Count; i++)
        {
            timerDataList[i].curTime += Time.deltaTime;
        }

        for (int i = timerDataList.Count - 1; i >= 0; i--)
        {
            if (timerDataList[i].curTime >= timerDataList[i].endTime)
            {
                if (timerDataList[i].callback != null)
                {
                    timerDataList[i].callback();
                }
                timerDataList.RemoveAt(i);
            }
        }
    }
}
