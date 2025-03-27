using DG.Tweening;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LuckySummonLayer : MonoBehaviour
{
    public Text text_gailv;
    public SkeletonGraphic skeletonGraphic;

    private void Awake()
    {
        transform.Find("bg").localScale = new Vector3(1,0,1);
        transform.Find("bg").DOScaleY(1,0.2f).OnComplete(()=>
        {
            skeletonGraphic.freeze = false;
        });

        transform.Find("bg").DOScaleY(0, 0.2f).SetDelay(2).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    public void init(int gailv)
    {
        text_gailv.text = gailv / 10000f + "%";
    }
}
