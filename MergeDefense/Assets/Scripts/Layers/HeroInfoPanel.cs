using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour
{
    public GameObject bgTrans;
    public Image img_head;
    public Image img_head_bg;
    public Image img_head_kuang;
    public Image img_weaponIcon1;
    public Image img_weaponIcon2;
    public Image img_weaponFrame1;
    public Image img_weaponFrame2;
    public Image img_choicedWeapon1;
    public Image img_choicedWeapon2;
    public GameObject obj_weapon1;
    public GameObject obj_weapon2;
    public Text text_heroName;
    public Text text_weaponName;
    public Text text_career;

    [HideInInspector]
    public bool isCanClose = true;      // 为了连续点击角色时，角色信息面板不用关闭再显示
    [HideInInspector]
    public HeroLogicBase heroLogicBase;

    public void show(HeroLogicBase _heroLogicBase)
    {
        GameLayer.s_instance.attackRangeTrans.localScale = new Vector3(_heroLogicBase.heroData.atkRange, _heroLogicBase.heroData.atkRange, _heroLogicBase.heroData.atkRange);

        gameObject.SetActive(true);

        heroLogicBase = _heroLogicBase;

        text_heroName.text = heroLogicBase.heroData.name;
        text_career.text = heroLogicBase.heroData.career;
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);
        img_head_kuang.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_1");
        img_head_bg.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_2");

        text_weaponName.text = "No Weapons";
        obj_weapon1.gameObject.SetActive(false);
        obj_weapon2.gameObject.SetActive(false);
        img_choicedWeapon1.gameObject.SetActive(false);
        img_choicedWeapon2.gameObject.SetActive(false);

        if (heroLogicBase.list_weapon.Count >= 1)
        {
            obj_weapon1.gameObject.SetActive(true);
            img_weaponIcon1.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[0].type);
            img_weaponFrame1.color = Consts.list_weaponColor[heroLogicBase.list_weapon[0].type - 1];
            obj_weapon1.transform.Find("level_bg").GetComponent<Image>().color = Consts.list_weaponColor[heroLogicBase.list_weapon[0].type - 1];
            obj_weapon1.transform.Find("level_bg/level").GetComponent<Text>().text = heroLogicBase.list_weapon[0].level.ToString();

            onClickWeaponIcon(0);
        }

        if (heroLogicBase.list_weapon.Count >= 2)
        {
            obj_weapon2.gameObject.SetActive(true);
            img_weaponIcon2.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[1].type);
            img_weaponFrame2.color = Consts.list_weaponColor[heroLogicBase.list_weapon[1].type - 1];
            obj_weapon2.transform.Find("level_bg").GetComponent<Image>().color = Consts.list_weaponColor[heroLogicBase.list_weapon[1].type - 1];
            obj_weapon2.transform.Find("level_bg/level").GetComponent<Text>().text = heroLogicBase.list_weapon[1].level.ToString();
        }
    }

    public void onClickWeaponIcon(int index)
    {
        if(index == 0)
        {
            if (heroLogicBase.list_weapon.Count >= 1)
            {
                img_choicedWeapon1.gameObject.SetActive(true);
                img_choicedWeapon2.gameObject.SetActive(false);
                img_choicedWeapon1.color = Consts.list_weaponColor[heroLogicBase.list_weapon[0].type - 1];

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
                img_choicedWeapon1.gameObject.SetActive(false);
                img_choicedWeapon2.gameObject.SetActive(true);
                img_choicedWeapon2.color = Consts.list_weaponColor[heroLogicBase.list_weapon[1].type - 1];

                text_weaponName.text = heroLogicBase.list_weapon[1].name;
            }
        }
    }

    public void onClickSkillIcon(int index)
    {
        if (index == 0)
        {
            
        }
        else if (index == 1)
        {
            
        }
        else if (index == 2)
        {

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
