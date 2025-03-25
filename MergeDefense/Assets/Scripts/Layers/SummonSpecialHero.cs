using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SummonSpecialHero : MonoBehaviour
{
    public Transform list_content_hero;
    public Transform list_content_weapon;
    public GameObject item_hero;
    public GameObject item_weapon;

    Transform curClickHeroItem = null;
    Transform curClickWeaponItem = null;

    HeroData heroData;

    public void init(HeroData _heroData)
    {
        heroData = _heroData;

        for (int i = 0; i < heroData.list_summonWay.Count; i++)
        {
            int summonType = heroData.list_summonWay[i][0];

            // 角色要求
            if (summonType == 1)
            {
                int id = heroData.list_summonWay[i][1];
                int star = heroData.list_summonWay[i][2];

                // 遍历已上场角色，检查条件是否满足
                for (int j = 0; j < HeroManager.s_instance.list_hero.Count; j++)
                {
                    HeroLogicBase heroLogicBase = HeroManager.s_instance.list_hero[j];
                    bool isAdd = false;
                    if (heroLogicBase.curStar >= star)
                    {
                        if (id == 999)
                        {
                            isAdd = true;
                        }
                        else if(heroLogicBase.heroData.id == id)
                        {
                            isAdd = true;
                        }
                    }
                        
                    if (isAdd)
                    {
                        Transform itemTrans = Instantiate(item_hero, list_content_hero).transform;
                        itemTrans.name = heroLogicBase.id + "_" + heroLogicBase.curStar;
                        itemTrans.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.heroData.id);
                        itemTrans.GetComponent<Button>().onClick.AddListener(()=>
                        {
                            onClickItemHero(itemTrans);
                        });

                        if(j == 0)
                        {
                            curClickHeroItem = itemTrans;
                            itemTrans.Find("choiced").localScale = Vector3.one;
                        }

                        // 星级
                        {
                            int showCount = heroLogicBase.curStar % 3;
                            if (showCount == 0)
                            {
                                showCount = 3;
                            }
                            for (int k = 1; k <= 3; k++)
                            {
                                if (k <= showCount)
                                {
                                    itemTrans.Find("stars").Find(k.ToString()).gameObject.SetActive(true);
                                }
                                else
                                {
                                    itemTrans.Find("stars").Find(k.ToString()).gameObject.SetActive(false);
                                }
                            }
                        }
                    }
                }
            }
            // 武器要求
            else if (summonType == 2)
            {
                int weaponType = heroData.list_summonWay[i][1];
                int level = heroData.list_summonWay[i][2];

                // 遍历武器栏的武器
                for (int j = 0; j < GameUILayer.s_instance.list_weaponBar.Count; j++)
                {
                    WeaponBar weaponBar = GameUILayer.s_instance.list_weaponBar[j];
                    bool isAdd = false;

                    if(weaponType == 999)
                    {
                        if (GameUILayer.s_instance.list_weaponBar[j].weaponData != null && GameUILayer.s_instance.list_weaponBar[j].weaponData.level >= level)
                        {
                            isAdd = true;
                        }
                    }
                    else
                    {
                        if (GameUILayer.s_instance.list_weaponBar[j].weaponData != null && GameUILayer.s_instance.list_weaponBar[j].weaponData.level >= level && GameUILayer.s_instance.list_weaponBar[j].weaponData.type == weaponType)
                        {
                            isAdd = true;
                        }
                    }

                    if (isAdd)
                    {
                        Transform itemTrans = Instantiate(item_weapon, list_content_weapon).transform;
                        itemTrans.name = weaponBar.weaponData.type + "_" + weaponBar.weaponData.level;
                        itemTrans.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponBar.weaponData.type);
                        itemTrans.Find("level").GetComponent<Text>().text = weaponBar.weaponData.level.ToString();

                        itemTrans.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            onClickItemWeapon(itemTrans);
                        });

                        if (curClickWeaponItem == null)
                        {
                            curClickWeaponItem = itemTrans;
                            itemTrans.Find("choiced").localScale = Vector3.one;
                        }
                    }
                }

                // 遍历武器格子
                for (int j = 0; j < GameUILayer.s_instance.weaponGridTrans.childCount; j++)
                {
                    if (GameUILayer.s_instance.weaponGridTrans.GetChild(j).childCount == 1)
                    {
                        UIItemWeapon uiItemWeapon = GameUILayer.s_instance.weaponGridTrans.GetChild(j).GetChild(0).GetComponent<UIItemWeapon>();
                        bool isAdd = false;
                        if (weaponType == 999)
                        {
                            if (uiItemWeapon.weaponData.level >= level)
                            {
                                isAdd = true;
                            }
                        }
                        else
                        {
                            if (uiItemWeapon.weaponData.level >= level && uiItemWeapon.weaponData.type == weaponType)
                            {
                                isAdd = true;
                            }
                        }

                        if (isAdd)
                        {
                            Transform itemTrans = Instantiate(item_weapon, list_content_weapon).transform;
                            itemTrans.name = uiItemWeapon.weaponData.type + "_" + uiItemWeapon.weaponData.level;
                            itemTrans.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + uiItemWeapon.weaponData.type);
                            itemTrans.Find("level").GetComponent<Text>().text = uiItemWeapon.weaponData.level.ToString();

                            itemTrans.GetComponent<Button>().onClick.AddListener(() =>
                            {
                                onClickItemWeapon(itemTrans);
                            });

                            if (curClickWeaponItem == null)
                            {
                                curClickWeaponItem = itemTrans;
                                itemTrans.Find("choiced").localScale = Vector3.one;
                            }
                        }
                    }
                }
            }
        }
    }

    void onClickItemHero(Transform trans)
    {
        AudioScript.s_instance.playSound_btn();

        curClickHeroItem = trans;

        for (int i = 0; i < list_content_hero.childCount; i++)
        {
            Transform itemTrans = list_content_hero.GetChild(i);
            if (itemTrans == trans)
            {
                itemTrans.Find("choiced").localScale = Vector3.one;
            }
            else
            {
                itemTrans.Find("choiced").localScale = Vector3.zero;
            }
        }
    }

    void onClickItemWeapon(Transform trans)
    {
        AudioScript.s_instance.playSound_btn();

        curClickWeaponItem = trans;

        for (int i = 0; i < list_content_weapon.childCount; i++)
        {
            Transform itemTrans = list_content_weapon.GetChild(i);
            if (itemTrans == trans)
            {
                itemTrans.Find("choiced").localScale = Vector3.one;
            }
            else
            {
                itemTrans.Find("choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickSummon()
    {
        GameLayer.s_instance.summonMythicHero(heroData, curClickHeroItem.name, curClickWeaponItem.name);
        Destroy(gameObject);
    }
}
