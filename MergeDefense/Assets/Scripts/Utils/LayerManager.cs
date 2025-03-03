using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LayerManager
{
    public static GameObject ShowLayer(Consts.Layer layer,bool isHighLayer = false)
    {
        //Debug.Log("----LayerManager.showLayer------" + layer);

        if (layer == Consts.Layer.GameLayer)
        {
            GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Layers/" + layer.ToString()));
            obj.transform.name = layer.ToString();
            return obj;
        }
        else
        {
            GameObject obj = GameObject.Instantiate(ObjectPool.getPrefab("Prefabs/Layers/" + layer.ToString()), isHighLayer ? LaunchScript.s_instance.canvas_top.transform : LaunchScript.s_instance.canvas.transform);
            obj.transform.name = layer.ToString();
            return obj;
        }
    }

    // 出生动画
    public static void LayerShowAni(Transform aniTrans)
    {
        aniTrans.localScale = Vector3.zero;
        Sequence seq = DOTween.Sequence();
        seq.Append(aniTrans.DOScale(1.1f, 0.3f))
            .Append(aniTrans.DOScale(1, 0.1f));
        seq.Play();
    }

    // 关闭动画
    public static void LayerCloseAni(Transform aniTrans, Action callback)
    {
        aniTrans.DOScale(0.5f, 0.15f).SetEase(Ease.InCubic).OnComplete(()=>
        {
            callback();
        });
    }
}
