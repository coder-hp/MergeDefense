using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour
{
    public GameObject bgTrans;
    public Image img_head;
    public Image img_weapon1;
    public Image img_weapon2;
    public GameObject obj_choicedWeapon1;
    public GameObject obj_choicedWeapon2;
    public Text text_heroName;
    public Text text_weaponName;

    [HideInInspector]
    public bool isCanClose = true;      // 为了连续点击角色时，角色信息面板不用关闭再显示
    [HideInInspector]
    public HeroLogicBase heroLogicBase;

    public void show(HeroLogicBase _heroLogicBase)
    {
        GameLayer.s_instance.attackRangeTrans.localScale = new Vector3(_heroLogicBase.atkRange, _heroLogicBase.atkRange, _heroLogicBase.atkRange);

        gameObject.SetActive(true);

        heroLogicBase = _heroLogicBase;

        text_heroName.text = heroLogicBase.heroData.name;
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);

        text_weaponName.text = "No Weapons";
        img_weapon1.gameObject.SetActive(false);
        img_weapon2.gameObject.SetActive(false);
        obj_choicedWeapon1.SetActive(false);
        obj_choicedWeapon2.SetActive(false);

        if (heroLogicBase.list_weapon.Count >= 1)
        {
            img_weapon1.gameObject.SetActive(true);
            img_weapon1.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[0].type);
            onClickWeaponIcon(0);
        }

        if (heroLogicBase.list_weapon.Count >= 2)
        {
            img_weapon2.gameObject.SetActive(true);
            img_weapon2.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[1].type);
        }
    }

    public void onClickWeaponIcon(int index)
    {
        if(index == 0)
        {
            if (heroLogicBase.list_weapon.Count >= 1)
            {
                obj_choicedWeapon1.SetActive(true);
                obj_choicedWeapon2.SetActive(false);

                text_weaponName.text = heroLogicBase.list_weapon[0].name;
            }
            else
            {
                return;
            }
        }
        else if (index == 1)
        {
            if (heroLogicBase.list_weapon.Count >= 2)
            {
                obj_choicedWeapon1.SetActive(false);
                obj_choicedWeapon2.SetActive(true);

                text_weaponName.text = heroLogicBase.list_weapon[1].name;
            }
        }
    }

    public void onClickClose()
    {
        if (!isCanClose)
        {
            isCanClose = true;
            return;
        }

        heroLogicBase = null;
        gameObject.SetActive(false);
        GameLayer.s_instance.attackRangeTrans.localScale = Vector3.zero;
    }
}
