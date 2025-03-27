using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckySummonLayer : MonoBehaviour
{
    public Text text_gailv;
    public Text text_title;
    public Text text_desc;
    public SkeletonGraphic skeletonGraphic;

    private void Awake()
    {
        transform.Find("bg").localScale = new Vector3(1,0,1);
        transform.Find("bg").DOScaleY(1,0.3f).OnComplete(()=>
        {
            skeletonGraphic.freeze = false;
        });

        transform.Find("bg").DOScaleY(0, 0.2f).SetDelay(2).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void init(int gailv,bool isSuper,bool isForge)
    {
        if(isSuper)
        {
            text_gailv.text = gailv / 1000f + "% Chance!";
        }
        else
        {
            text_gailv.text = gailv / 100f + "% Chance!";
        }

        if (isSuper)
        {
            text_title.text = "Super Lucky !";
        }

        if(isForge)
        {
            text_desc.text = "✦  High-Level Weapon Forged! ✦";
        }
    }
}
