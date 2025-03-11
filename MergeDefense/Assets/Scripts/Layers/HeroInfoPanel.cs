using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroInfoPanel : MonoBehaviour
{
    public static HeroInfoPanel s_instance = null;

    public GameObject bgTrans;
    public Image img_head;
    public Image img_head_bg;
    public Image img_head_kuang;
    public Image img_footShadow;
    public Image img_weaponIcon1;
    public Image img_weaponIcon2;
    public Image img_choicedWeapon1;
    public Image img_choicedWeapon2;
    public GameObject obj_weapon1;
    public GameObject obj_weapon2;
    public Text text_heroName;
    public Text text_weaponName;
    public Text text_career;
    public Text text_skillDesc;
    public Text text_atk;
    public Text text_atk_speed;
    public Text text_crit_rate;
    public Transform weaponBuffsTrans;
    public Transform skillTrans;
    public Transform tab_weaponTrans;
    public Transform tab_skillTrans;
    public Transform btn_sellHeroTrans;
    public Text text_sellHeroPrice;

    [HideInInspector]
    public bool isCanClose = true;      // 为了连续点击角色时，角色信息面板不用关闭再显示
    [HideInInspector]
    public HeroLogicBase heroLogicBase;

    HeroSkillData[] skillsArray;
    int sellPrice = 0;

    List<int> list_weaponType = new List<int>();

    private void Awake()
    {
        s_instance = this;
        gameObject.SetActive(false);
    }

    public void show(HeroLogicBase _heroLogicBase)
    {
        GameLayer.s_instance.attackRangeTrans.localScale = new Vector3(_heroLogicBase.heroData.atkRange, _heroLogicBase.heroData.atkRange, _heroLogicBase.heroData.atkRange);
        GameLayer.s_instance.matrial_attackRange.SetFloat("_OutLineRange", 1.0f / _heroLogicBase.heroData.atkRange);

        gameObject.SetActive(true);

        heroLogicBase = _heroLogicBase;
        list_weaponType.Clear();

        btn_sellHeroTrans.localScale = Vector3.one;
        text_heroName.text = heroLogicBase.heroData.name;
        text_career.text = heroLogicBase.heroData.career;
        text_atk.text = heroLogicBase.getAtk().ToString();
        text_atk_speed.text = heroLogicBase.getAtkSpeed().ToString();
        text_crit_rate.text = heroLogicBase.getCritRate() + "%";
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);
        img_head_kuang.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_1");
        img_head_bg.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_2");
        img_footShadow.color = Consts.list_heroQualityColor[heroLogicBase.heroData.quality];

        text_weaponName.text = "No Weapons";
        obj_weapon1.gameObject.SetActive(false);
        obj_weapon2.gameObject.SetActive(false);
        img_choicedWeapon1.transform.localScale = Vector3.zero;
        img_choicedWeapon2.transform.localScale = Vector3.zero;

        for(int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
        {
            if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null && GameUILayer.s_instance.list_weaponBar[i].weaponData.type == heroLogicBase.heroData.goodWeapon)
            {
                list_weaponType.Add(GameUILayer.s_instance.list_weaponBar[i].weaponData.type);
                break;
            }
        }

        if (list_weaponType.Count >= 1)
        {
            obj_weapon1.gameObject.SetActive(true);
            img_weaponIcon1.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + list_weaponType[0]);
        }

        if (list_weaponType.Count >= 2)
        {
            obj_weapon2.gameObject.SetActive(true);
            img_weaponIcon2.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + list_weaponType[1]);
        }

        // 技能
        {
            string[] array = heroLogicBase.heroData.skills.Split("_");
            skillsArray = new HeroSkillData[array.Length];
            for (int i = 0; i < skillTrans.childCount; i++)
            {
                if((i + 1) <= array.Length)
                {
                    skillsArray[i] = HeroSkillEntity.getInstance().getData(int.Parse(array[i]));
                    skillTrans.Find(i.ToString()).gameObject.SetActive(true);
                    skillTrans.Find(i + "/mask/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("skill_" + array[i]);
                }
                else
                {
                    skillTrans.Find(i.ToString()).gameObject.SetActive(false);
                }
            }
        }

        if (list_weaponType.Count > 0)
        {
            onClickWeaponIcon(0);
        }
        else if (skillsArray.Length > 0)
        {
            onClickSkillIcon(0);
        }

        // 卖出按钮
        {
            sellPrice = Mathf.RoundToInt(heroLogicBase.heroStarData.sellPrice * GameUILayer.s_instance.curSummonGold * 0.2f);
            btn_sellHeroTrans.position = heroLogicBase.heroUITrans.position + Consts.heroSellBtnOffset;
            text_sellHeroPrice.text = sellPrice.ToString();
        }
    }

    public void onClickWeaponIcon(int index)
    {
        AudioScript.s_instance.playSound_btn();

        if (index == 0 && list_weaponType.Count == 0)
        {
            return;
        }
        else if (index == 1 && list_weaponType.Count < 2)
        {
            return;
        }

        tab_weaponTrans.localScale = Vector3.one;
        tab_skillTrans.localScale = Vector3.zero;

        weaponBuffsTrans.localScale = Vector3.zero;
        text_skillDesc.transform.localScale = Vector3.zero;

        for (int i = 0; i < skillTrans.childCount; i++)
        {
            skillTrans.Find(i + "/choiced").localScale = Vector3.zero;
        }

        if (index == 0)
        {
            if (list_weaponType.Count >= 1)
            {
                showWeaponBuffInfo(list_weaponType[0]);

                img_choicedWeapon1.transform.localScale = Vector3.one;
                img_choicedWeapon2.transform.localScale = Vector3.zero;

                text_weaponName.text = WeaponEntity.getInstance().getData(list_weaponType[0],1).name;
            }
            else
            {
                return;
            }
        }
        else if (index == 1)
        {
            if (list_weaponType.Count >= 2)
            {
                showWeaponBuffInfo(list_weaponType[1]);

                img_choicedWeapon1.transform.localScale = Vector3.zero;
                img_choicedWeapon2.transform.localScale = Vector3.one;

                text_weaponName.text = WeaponEntity.getInstance().getData(list_weaponType[1], 1).name;
            }
        }
    }

    void showWeaponBuffInfo(int type)
    {
        weaponBuffsTrans.localScale = Vector3.one;

        // 攻击力
        {
            int atk = 0;
            for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
            {
                if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null && GameUILayer.s_instance.list_weaponBar[i].weaponData.type == type)
                {
                    atk += GameUILayer.s_instance.list_weaponBar[i].weaponData.buff1;
                }
            }
            weaponBuffsTrans.Find("buff1/value").GetComponent<Text>().text = atk.ToString();
        }

        // 攻击力百分比加成
        {
            int atkBaiFenBi = 0;
            for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
            {
                if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null && GameUILayer.s_instance.list_weaponBar[i].weaponData.type == type)
                {
                    atkBaiFenBi += Mathf.RoundToInt(GameUILayer.s_instance.list_weaponBar[i].weaponData.buff2 * 100f);
                }
            }
            weaponBuffsTrans.Find("buff2/value").GetComponent<Text>().text = atkBaiFenBi + "%";
        }

        // 第三个任意属性Buff
        {
            Consts.BuffType buff3Type = Consts.BuffType.Atk;
            float buff3Value = 0;
            for (int i = 0; i < GameUILayer.s_instance.list_weaponBar.Count; i++)
            {
                if (GameUILayer.s_instance.list_weaponBar[i].weaponData != null && GameUILayer.s_instance.list_weaponBar[i].weaponData.type == type)
                {
                    buff3Type = GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Type;
                    buff3Value += GameUILayer.s_instance.list_weaponBar[i].weaponData.buff3Value;
                }
            }
            weaponBuffsTrans.Find("buff3/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("buff_" + (int)buff3Type);

            switch (buff3Type)
            {
                case Consts.BuffType.CritRate:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Rate";

                        int value = Mathf.RoundToInt(buff3Value);
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }

                case Consts.BuffType.CritDamage:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Damage";

                        int value = Mathf.RoundToInt(buff3Value * 100);
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }

                case Consts.BuffType.AtkSpeed:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Attack Speed";
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = buff3Value.ToString();
                        break;
                    }

                case Consts.BuffType.SkillRate:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Proc Chance";

                        int value = Mathf.RoundToInt(buff3Value);
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }
            }
        }
    }

    public void onClickSkillIcon(int index)
    {
        AudioScript.s_instance.playSound_btn();

        img_choicedWeapon1.transform.localScale = Vector3.zero;
        img_choicedWeapon2.transform.localScale = Vector3.zero;

        tab_weaponTrans.localScale = Vector3.zero;
        tab_skillTrans.localScale = Vector3.one;

        weaponBuffsTrans.localScale = Vector3.zero;
        text_skillDesc.transform.localScale = Vector3.one;

        for (int i = 0; i < skillTrans.childCount; i++)
        {
            if (i == index)
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.one;
                text_weaponName.text = skillsArray[index].name;
                text_skillDesc.text = skillsArray[index].desc;
            }
            else
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickSellHero()
    {
        //AudioScript.s_instance.playSound_btn();
        AudioScript.s_instance.playSound("sellHero");

        GameUILayer.s_instance.changeGold(sellPrice);

        EffectManager.sellHero(heroLogicBase.transform.position);
        Destroy(heroLogicBase.gameObject);
        onClickClose();
    }

    public void onClickClose()
    {
        if (!isCanClose)
        {
            isCanClose = true;
            return;
        }

        AudioScript.s_instance.playSound_btn();

        heroLogicBase = null;
        gameObject.SetActive(false);
        GameLayer.s_instance.attackRangeTrans.localScale = Vector3.zero;
    }
}
