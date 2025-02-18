using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    public static Dictionary<string, GameObject> dic_saveObj = new Dictionary<string, GameObject>();

    public static void clear()
    {
        dic_saveObj.Clear();
    }

    public static void heroMerge(Vector3 pos)
    {
        if (dic_saveObj.ContainsKey("HeroMerge"))
        {
            dic_saveObj["HeroMerge"].SetActive(false);
            dic_saveObj["HeroMerge"].SetActive(true);
            dic_saveObj["HeroMerge"].transform.position = pos;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/HeroMerge"), GameLayer.s_instance.effectPoint);
            obj.transform.position = pos;
            dic_saveObj["HeroMerge"] = obj;
        }
    }
}
