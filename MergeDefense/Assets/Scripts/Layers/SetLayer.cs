using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetLayer : MonoBehaviour
{
    public Image img_head;
    public Text text_name;

    void Start()
    {
        text_name.text = GameData.getName();
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + GameData.getHead());

        LayerManager.LayerShowAni(transform.Find("bg"));
    }

    public void onClickHead()
    {
        AudioScript.s_instance.playSound_btn();
    }

    public void onClickName()
    {
        AudioScript.s_instance.playSound_btn();
    }

    bool isClosed = false;
    public void onClickClose()
    {
        if (isClosed)
        {
            return;
        }
        isClosed = true;

        AudioScript.s_instance.playSound_btn();

        LayerManager.LayerCloseAni(transform.Find("bg"), () =>
        {
            Destroy(gameObject);
        });
    }
}
