using DG.Tweening;
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

    static List<GameObject> list_summonHeroEffect = new List<GameObject>();
    static Vector3 summonEffectStartPos;
    public static void summonHero(Vector3 pos)
    {
        Transform trans = null;
        for (int i = 0; i < list_summonHeroEffect.Count; i++)
        {
            if (!list_summonHeroEffect[i].activeInHierarchy)
            {
                trans = list_summonHeroEffect[i].transform;
                trans.gameObject.SetActive(true);
                break; ;
            }
        }

        if(list_summonHeroEffect.Count == 0)
        {
            summonEffectStartPos = GameUILayer.s_instance.btn_summon_gold.transform.parent.position;
            summonEffectStartPos.z = 0;
        }

        if (trans == null)
        {
            GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/SummonStar"), GameLayer.s_instance.effectPoint);
            trans = obj.transform;
            list_summonHeroEffect.Add(obj);
        }

        trans.transform.position = summonEffectStartPos;
        trans.transform.DOMove(pos, 1).OnComplete(()=>
        {
            trans.gameObject.SetActive(false);
        });

        // 飞行路径动画
        //Vector3 targetPos = new Vector3(0, 0.1f * (toBlockBaseScript.childBlockPointTrans.childCount - 1), 0);
        //Vector3[] vec = new Vector3[2];
        //vec[0] = new Vector3(blockTrans.localPosition.x, targetPos.y + 1f, blockTrans.localPosition.z);       // +1是为了抬高一点，防止穿模
        //vec[1] = targetPos;
        //vec[0] = (vec[0] + vec[1]) / 2f;
        //blockTrans.DOLocalPath(vec, aniTime, PathType.CatmullRom).SetEase(Ease.OutSine).SetDelay(mergeCount * jiangeTime);
    }
}
