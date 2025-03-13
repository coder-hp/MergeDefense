using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetMythicHeroLayer : MonoBehaviour
{
    public Image img_icon;
    public Transform guangTrans;
    public Text txt_name;

    public void init(int id)
    {
        HeroData heroData = HeroEntity.getInstance().getData(id);
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + id);
        txt_name.text = heroData.name;

        guangTrans.DORotate(new Vector3(0f, 0f, -360), 5, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);

        LayerManager.LayerShowAni(transform.Find("bg"));

        Invoke("close",2);
    }

    void close()
    {
        LayerManager.LayerCloseAni(transform.Find("bg"),()=>
        {
            Destroy(gameObject);
        });
    }
}
