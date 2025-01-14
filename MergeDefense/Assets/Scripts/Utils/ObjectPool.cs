using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    static Dictionary<string, GameObject> dic_prefab = new Dictionary<string, GameObject>();

    public static GameObject getPrefab(string path)
    {
        if(dic_prefab.ContainsKey(path))
        {
            return dic_prefab[path];
        }
        else
        {
            GameObject prefab = Resources.Load(path) as GameObject;
            dic_prefab[path] = prefab;
            return prefab;
        }
    }
}
