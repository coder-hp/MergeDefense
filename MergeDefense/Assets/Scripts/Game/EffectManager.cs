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
    public static void summonHero(Vector3 targetPos)
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

        // 飞行路径动画
        //{
        //    Vector3[] vec = new Vector3[2];
        //    vec[0] = targetPos + new Vector3(0.5f, 0.5f);
        //    vec[1] = targetPos;
        //    trans.DOLocalPath(vec, 0.6f, PathType.CatmullRom).SetEase(Ease.OutCubic).OnComplete(() =>
        //    {
        //        trans.gameObject.SetActive(false);
        //    });
        //}

        // 直线
        //trans.DOMove(targetPos,0.4f).SetEase(Ease.OutCubic).OnComplete(() =>
        //{
        //    trans.gameObject.SetActive(false);
        //});

        // 飞行路径动画
        {
            Vector3[] vec = new Vector3[2];
            vec[0] = (targetPos + summonEffectStartPos) / 2f + new Vector3(1,0,0);
            vec[1] = targetPos;
            trans.DOLocalPath(vec, 0.6f, PathType.CatmullRom).SetEase(Ease.OutCubic).OnComplete(() =>
            {
                trans.gameObject.SetActive(false);
            });
        }
    }
}
