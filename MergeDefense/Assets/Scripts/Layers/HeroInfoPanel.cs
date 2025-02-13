using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour
{
    public GameObject bgTrans;
    public Image img_head;
    public Text text_heroName;
    public Text text_weaponName;

    public void show(HeroLogicBase heroLogicBase)
    {
        gameObject.SetActive(true);

        text_heroName.text = heroLogicBase.heroData.name;
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);
    }

    public void onClickWeaponIcon(int index)
    {

    }

    public void onClickClose()
    {
        gameObject.SetActive(false);
    }
}
