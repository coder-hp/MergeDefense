using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager
{
    static Dictionary<string, GameObject> dic_saveObj = new Dictionary<string, GameObject>();

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

    static List<GameObject> list_enemyDieEffect = new List<GameObject>();
    public static void enemyDie(Vector3 pos)
    {
        for(int i = 0; i < list_enemyDieEffect.Count; i++)
        {
            if(!list_enemyDieEffect[i].activeInHierarchy)
            {
                list_enemyDieEffect[i].transform.position = pos;
                list_enemyDieEffect[i].SetActive(true);
                return;
            }
        }

        GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/EnemyDead"), GameLayer.s_instance.effectPoint);
        obj.transform.position = pos;
        list_enemyDieEffect.Add(obj);
    }
}
