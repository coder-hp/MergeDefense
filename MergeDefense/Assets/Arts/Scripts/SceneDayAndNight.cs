using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneDayAndNight : MonoBehaviour
{
    public float duration;
    Material sceneMat,rayLightMat,nightStarMat;
    float dayAndNight = 0f;
    GameObject fireObj;

    bool isDay = true;
    bool isFire = false;
    int RefreshRate = 0;


    void Start()
    {
        sceneMat = transform.Find("ground").GetComponent<MeshRenderer>().material;
        rayLightMat = transform.Find("effect/eff_rayLight").GetComponent<ParticleSystemRenderer>().material;
        nightStarMat = transform.Find("effect/eff_nightStar").GetComponent<ParticleSystemRenderer>().material;

        fireObj = transform.Find("effect/fire").gameObject;
        fireObj.SetActive(false);
    }

    
    void Update()
    {
        if (isDay)
        {
            dayAndNight += Time.deltaTime / duration;
            if (dayAndNight >= 1)
            {
                isDay = false;
            }
        }
        else
        {
            dayAndNight -= Time.deltaTime / duration;
            if (dayAndNight <= 0f)
            {
                isDay = true;
            }
        }

        // 开关灯
        if (!isFire)
        {
            if (dayAndNight >= 0.7f)
            {
                isFire = true;
                fireObj.SetActive(true);
            }
        }
        else
        {
            if (dayAndNight < 0.7f)
            {
                isFire = false;
                fireObj.SetActive(false);
            }
        }

        // 刷新频率
        RefreshRate++;
        if (RefreshRate > 20)
        {
            sceneMat.SetFloat("_DayAndNight", dayAndNight);
            rayLightMat.SetFloat("_Alpha", dayAndNight * dayAndNight);
            nightStarMat.SetFloat("_Alpha", dayAndNight * dayAndNight);
            
            RefreshRate = 0;
        }
    }



}
