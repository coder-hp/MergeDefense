using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroLayer : MonoBehaviour
{
    public GameObject item_hero;
    public Transform list_content;
    public Transform btn_allHero;
    public Transform btn_mythicHero;
    public Text btn_allHeroText;
    public Text btn_mythicHeroText;

    void Start()
    {
        for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            HeroData heroData = HeroEntity.getInstance().list[i];
            Transform item = Instantiate(item_hero, list_content).transform;
            item.name = heroData.quality.ToString();
            item.Find("icon_mask/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);
            item.Find("name_bg/name").GetComponent<Text>().text = heroData.name;
            item.Find("qualityCard").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_2");
            item.Find("qualityKuang").GetComponent<Image>().sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroData.quality + "_1");
            item.Find("shadow").GetComponent<Image>().color = Consts.list_heroQualityColor[heroData.quality];

            if(GameData.isUnlockHero(heroData.id))
            {
                item.Find("level_bg").GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("kuang_hero_" + heroData.quality + "_3");
                item.Find("level_bg/level").GetComponent<Text>().text = "Lv." + GameData.getHeroLevel(heroData.id);
                item.Find("exp_bg/Text").GetComponent<Text>().text = GameData.getHeroExp(heroData.id) + "/50";
            }
            else
            {
                item.Find("level_bg").localScale = Vector3.zero;
                item.Find("exp_bg").localScale = Vector3.zero;
                item.Find("lock").gameObject.SetActive(true);

                // 签到送的角色
                if(heroData.id == 118 || heroData.id == 119)
                {
                    item.Find("lock/unlockType").GetComponent<Text>().text = "7-Day Login";
                }
                else
                {
                    item.Find("price").gameObject.SetActive(true);
                    item.Find("price").GetComponent<Text>().text = heroData.price.ToString();
                }
            }
        }
    }

    public void onClickBtnAllHero()
    {
        AudioScript.s_instance.playSound_btn();

        btn_allHero.GetComponent<Image>().color = Color.white;
        btn_mythicHero.GetComponent<Image>().color = Color.clear;
        btn_allHeroText.color = Color.white;
        btn_mythicHeroText.color = CommonUtil.stringToColor("#7B94DB");
        btn_allHeroText.GetComponent<Outline>().enabled = true;
        btn_mythicHeroText.GetComponent<Outline>().enabled = false;

        for(int i = 0; i < list_content.childCount; i++)
        {
            list_content.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void onClickBtnMythicHero()
    {
        AudioScript.s_instance.playSound_btn();

        btn_allHero.GetComponent<Image>().color = Color.clear;
        btn_mythicHero.GetComponent<Image>().color = Color.white;
        btn_allHeroText.color = CommonUtil.stringToColor("#7B94DB");
        btn_mythicHeroText.color = Color.white;
        btn_allHeroText.GetComponent<Outline>().enabled = false;
        btn_mythicHeroText.GetComponent<Outline>().enabled = true;

        for (int i = 0; i < list_content.childCount; i++)
        {
            if (list_content.GetChild(i).name != "4")
            {
                list_content.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}
