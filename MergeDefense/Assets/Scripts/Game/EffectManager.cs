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
    static List<TrailRenderer> list_summonHeroTrail = new List<TrailRenderer>();
    static Vector3 summonEffectStartPos;
    static Vector3 summonEffectOffset1 = new Vector3(1, 2, 0);
    static Vector3 summonEffectOffset2 = new Vector3(0.5f, 2f, 0);
    public static void summonHero(Vector3 targetPos)
    {
        Transform trans = null;
        for (int i = 0; i < list_summonHeroEffect.Count; i++)
        {
            if (!list_summonHeroEffect[i].activeInHierarchy)
            {
                trans = list_summonHeroEffect[i].transform;
                trans.transform.position = summonEffectStartPos;
                list_summonHeroTrail[i].Clear();
                trans.gameObject.SetActive(true);
                break; ;
            }
        }

        if (list_summonHeroEffect.Count == 0)
        {
            summonEffectStartPos = GameUILayer.s_instance.btn_summon_gold.transform.parent.position;
            summonEffectStartPos.y -= 1.3f;
            summonEffectStartPos.z = 0;
        }

        if (trans == null)
        {
            GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/SummonStar"), GameLayer.s_instance.effectPoint);
            trans = obj.transform;
            trans.transform.position = summonEffectStartPos;
            list_summonHeroEffect.Add(obj);
            list_summonHeroTrail.Add(obj.transform.GetChild(1).GetComponent<TrailRenderer>());
        }

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
        trans.DOMove(targetPos, 0.5f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            trans.gameObject.SetActive(false);
        });

        // 飞行路径动画
        //{
        //    Vector3[] vec = new Vector3[2];
        //    if (targetPos.x >= summonEffectStartPos.x)
        //    {
        //        vec[0] = (targetPos + summonEffectStartPos) / 2 - summonEffectOffset1;
        //    }
        //    else
        //    {
        //        vec[0] = (targetPos + summonEffectStartPos) / 2 + summonEffectOffset2;
        //    }
        //    vec[1] = targetPos;
        //    trans.DOLocalPath(vec, 0.5f, PathType.CatmullRom).SetEase(Ease.OutQuad).OnComplete(() =>
        //    {
        //        trans.gameObject.SetActive(false);
        //    });
        //}
    }

    static List<GameObject> list_sellHeroEffect = new List<GameObject>();
    public static void sellHero(Vector3 pos)
    {
        for (int i = 0; i < list_sellHeroEffect.Count; i++)
        {
            if (!list_sellHeroEffect[i].activeInHierarchy)
            {
                list_sellHeroEffect[i].transform.position = pos;
                list_sellHeroEffect[i].SetActive(true);
                return;
            }
        }

        GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/SellOut"), GameLayer.s_instance.effectPoint);
        obj.transform.position = pos;
        list_sellHeroEffect.Add(obj);
    }

    static Dictionary<string, List<GameObject>> dic_enemyDamage = new Dictionary<string, List<GameObject>>();
    public static void enemyDamage(Vector3 pos,int heroId)
    {
        string effectName = "eff_hit_hero" + heroId;

        if (!dic_enemyDamage.ContainsKey(effectName))
        {
            dic_enemyDamage[effectName] = new List<GameObject>();
        }

        for (int i = 0; i < dic_enemyDamage[effectName].Count; i++)
        {
            if (!dic_enemyDamage[effectName][i].activeInHierarchy)
            {
                dic_enemyDamage[effectName][i].transform.position = pos;
                dic_enemyDamage[effectName][i].SetActive(true);
                return;
            }
        }

        GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/" + effectName), GameLayer.s_instance.effectPoint);
        obj.transform.position = pos;
        dic_enemyDamage[effectName].Add(obj);
    }

    static Dictionary<string, List<GameObject>> dic_heroSkill = new Dictionary<string, List<GameObject>>();
    public static void heroSkill(Vector3 pos, int heroId)
    {
        return;
        string effectName = "eff_skill_hero" + heroId;

        if (!dic_heroSkill.ContainsKey(effectName))
        {
            dic_heroSkill[effectName] = new List<GameObject>();
        }

        for (int i = 0; i < dic_heroSkill[effectName].Count; i++)
        {
            if (!dic_heroSkill[effectName][i].activeInHierarchy)
            {
                dic_heroSkill[effectName][i].transform.position = pos;
                dic_heroSkill[effectName][i].SetActive(true);
                return;
            }
        }

        GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/" + effectName), GameLayer.s_instance.effectPoint);
        obj.transform.position = pos;
        dic_heroSkill[effectName].Add(obj);
    }

    static Dictionary<string, List<GameObject>> dic_heroAttack = new Dictionary<string, List<GameObject>>();
    public static void heroAttack(HeroLogicBase heroLogicBase)
    {
        string effectName = "eff_attack_hero" + heroLogicBase.id;

        if (!dic_heroAttack.ContainsKey(effectName))
        {
            dic_heroAttack[effectName] = new List<GameObject>();
        }

        for (int i = 0; i < dic_heroAttack[effectName].Count; i++)
        {
            if (!dic_heroAttack[effectName][i].activeInHierarchy)
            {
                dic_heroAttack[effectName][i].transform.position = heroLogicBase.transform.position;
                dic_heroAttack[effectName][i].transform.rotation = heroLogicBase.transform.rotation;
                dic_heroAttack[effectName][i].SetActive(true);
                return;
            }
        }

        GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Effects/" + effectName), GameLayer.s_instance.effectPoint);
        obj.transform.position = heroLogicBase.transform.position;
        obj.transform.rotation = heroLogicBase.transform.rotation;
        dic_heroAttack[effectName].Add(obj);
    }
}
