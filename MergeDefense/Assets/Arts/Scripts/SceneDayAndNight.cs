using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDayAndNight : MonoBehaviour
{
    public float Duration;
    Material SceneMat,RayLightMat,NightStarMat;
    float DayAndNight = 0f;
    

    bool IsDay = true;
    int RefreshRate = 0;

    void Start()
    {
        SceneMat = transform.Find("ground").GetComponent<MeshRenderer>().material;
        RayLightMat = transform.Find("effect/eff_rayLight").GetComponent<ParticleSystemRenderer>().material;
        NightStarMat = transform.Find("effect/eff_nightStar").GetComponent<ParticleSystemRenderer>().material;
    }

    
    void Update()
    {
        if (IsDay)
        {
            DayAndNight += Time.deltaTime / Duration;
            if (DayAndNight >= 1)
            {
                IsDay = false;
            }
        }
        else
        {
            DayAndNight -= Time.deltaTime / Duration;
            if (DayAndNight <= 0f)
            {
                IsDay = true;
            }
        }

        RefreshRate++;
        if (RefreshRate > 20)
        {
            SceneMat.SetFloat("_DayAndNight", DayAndNight);
            RayLightMat.SetFloat("_Alpha", DayAndNight * DayAndNight);
            NightStarMat.SetFloat("_Alpha", DayAndNight * DayAndNight);
            RefreshRate = 0;
            //Debug.Log(DayAndNight);
        }
    }



}
