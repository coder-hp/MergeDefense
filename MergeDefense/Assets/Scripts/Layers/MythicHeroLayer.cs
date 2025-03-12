using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MythicHeroLayer : MonoBehaviour
{
    public GameObject prefab_item_hero;
    public GameObject btn_summon;
    public Transform list_content;
    public Image img_smallIcon;
    public Image img_bigIcon;
    public Text txt_name;

    List<HeroData> list_mythicHero = new List<HeroData>();

    void Start()
    {
        LayerManager.LayerShowAni(transform.Find("bg"));

        for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            if (HeroEntity.getInstance().list[i].quality == 4 && GameData.isUnlockHero(HeroEntity.getInstance().list[i].id))
            {
                list_mythicHero.Add(HeroEntity.getInstance().list[i]);
            }
        }

        for(int i = 0; i < list_mythicHero.Count; i++)
        {
            HeroData heroData = list_mythicHero[i];
            Transform item = Instantiate(prefab_item_hero, list_content).transform;
            item.name = heroData.id.ToString();
            item.Find("bg/head_mask/head").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);

            item.GetComponent<Button>().onClick.AddListener(()=>
            {
                onClickHero(heroData);
            });
        }

        if(list_mythicHero.Count > 0)
        {
            onClickHero(list_mythicHero[0]);
        }
    }

    void onClickHero(HeroData heroData)
    {
        for(int i = 0; i < list_content.childCount; i++)
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
    }

    public void onClickSummon()
    {
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
