using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MythicHeroLayer : MonoBehaviour
{
    public GameObject prefab_item_hero;
    public GameObject btn_summon;
    public ScrollRect scrollRect_hero;
    public Transform list_content;
    public Transform yaoqiu_hero;
    public Transform yaoqiu_weapon;
    public Image img_smallIcon;
    public Image img_bigIcon;
    public Text txt_name;

    List<HeroData> list_mythicHero = new List<HeroData>();
    List<int[]> list_mythicHeroProgfress = new List<int[]>();

    HeroData curChoiceHero = null;

    void Start()
    {
        LayerManager.LayerShowAni(transform.Find("bg"), () =>
        {
            scrollRect_hero.enabled = true;
        });

        for (int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            if (HeroEntity.getInstance().list[i].quality == 4 && GameData.isUnlockHero(HeroEntity.getInstance().list[i].id))
            {
                list_mythicHero.Add(HeroEntity.getInstance().list[i]);
            }
        }

        for (int i = 0; i < list_mythicHero.Count; i++)
        {
            HeroData heroData = list_mythicHero[i];
            Transform item = Instantiate(prefab_item_hero, list_content).transform;
            item.name = heroData.id.ToString();
            item.Find("bg/head_mask/head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);

            // 召唤条件进度
            int[] progress = GameLayer.s_instance.getMythicHeroProgress(heroData);
            int[] progress_new = new int[progress.Length];
            {
                int okCount = 0;
                for (int j = 0; j < progress.Length; j++)
                {
                    progress_new[j] = progress[j];
                    if (progress_new[j] == 1)
                    {
                        ++okCount;
                    }
                }
                list_mythicHeroProgfress.Add(progress_new);

                item.Find("bg/progress_bg/progress").GetComponent<Image>().fillAmount = ((float)okCount / (float)heroData.list_summonWay.Count);
                item.Find("bg/progress_bg/text").GetComponent<Text>().text = Mathf.RoundToInt(((float)okCount / (float)heroData.list_summonWay.Count) * 100) + "%";
            }

            item.GetComponent<Button>().onClick.AddListener(() =>
            {
                onClickHero(heroData, progress_new);
            });
        }

        if (list_mythicHero.Count > 0)
        {
            onClickHero(list_mythicHero[0], list_mythicHeroProgfress[0]);
        }
    }

    void onClickHero(HeroData heroData,int[] progress)
    {
        curChoiceHero = heroData;

        for (int i = 0; i < list_content.childCount; i++)
        {
            if (list_content.GetChild(i).name.CompareTo(heroData.id.ToString()) == 0)
            {
                list_content.GetChild(i).Find("bg/choiced").localScale = Vector3.one;
            }
            else
            {
                list_content.GetChild(i).Find("bg/choiced").localScale = Vector3.zero;
            }
        }

        txt_name.text = heroData.name;
        img_smallIcon.sprite = AtlasUtil.getAtlas_icon().GetSprite("hero_avatar_" + heroData.id);
        img_bigIcon.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);

        // 合成方式
        {
            bool tiaojian_hero = progress[0] == 1;
            bool tiaojian_weapon = progress[1] == 1;
            for (int i = 0; i < heroData.list_summonWay.Count; i++)
            {
                int summonType = heroData.list_summonWay[i][0];

                // 角色要求
                if (summonType == 1)
                {
                    int id = heroData.list_summonWay[i][1];
                    int star = heroData.list_summonWay[i][2];

                    yaoqiu_hero.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + id);

                    // 星级
                    {
                        int showCount = star % 3;
                        if (showCount == 0)
                        {
                            showCount = 3;
                        }
                        for (int j = 1; j <= 3; j++)
                        {
                            if (j <= showCount)
                            {
                                yaoqiu_hero.Find("stars").Find(j.ToString()).gameObject.SetActive(true);
                            }
                            else
                            {
                                yaoqiu_hero.Find("stars").Find(j.ToString()).gameObject.SetActive(false);
                            }
                        }
                    }
                }
                // 武器要求
                else if (summonType == 2)
                {
                    int weaponType = heroData.list_summonWay[i][1];
                    int level = heroData.list_summonWay[i][2];

                    yaoqiu_weapon.Find("icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + weaponType);
                    yaoqiu_weapon.Find("level").GetComponent<Text>().text = level.ToString();
                }
            }

            if(tiaojian_hero)
            {
                yaoqiu_hero.Find("gou").localScale = Vector3.one;
            }
            else
            {
                yaoqiu_hero.Find("gou").localScale = Vector3.zero;
            }

            if (tiaojian_weapon)
            {
                yaoqiu_weapon.Find("gou").localScale = Vector3.one;
            }
            else
            {
                yaoqiu_weapon.Find("gou").localScale = Vector3.zero;
            }

            if(tiaojian_hero && tiaojian_weapon)
            {
                btn_summon.SetActive(true);
            }
            else
            {
                btn_summon.SetActive(false);
            }
        }
    }

    public void onClickSummon()
    {
        if (curChoiceHero != null)
        {
            GameLayer.s_instance.summonMythicHero(curChoiceHero);
        }

        Destroy(gameObject);
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

        LayerManager.LayerCloseAni(transform.Find("bg"),()=>
        {
            Destroy(gameObject);
        });
    }
}
