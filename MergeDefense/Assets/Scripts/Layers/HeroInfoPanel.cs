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
    public Image img_weaponFrame1;
    public Image img_weaponFrame2;
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
        
        btn_sellHeroTrans.localScale = Vector3.one;
        text_heroName.text = heroLogicBase.heroData.name;
        text_career.text = heroLogicBase.heroData.career;
        text_atk.text = heroLogicBase.getAtk().ToString();
        text_atk_speed.text = heroLogicBase.getAtkSpeed().ToString();
        text_crit_rate.text = heroLogicBase.getCritRate() + "%";
        img_head.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroLogicBase.id);
        img_head_kuang.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_1");
        img_head_bg.sprite = AtlasUtil.getAtlas_game().GetSprite("kuang_hero_" + heroLogicBase.heroData.quality + "_2");

        if (heroLogicBase.heroData.quality == 2)
        {
            img_footShadow.color = CommonUtil.stringToColor("#457dd8");
        }
        else if (heroLogicBase.heroData.quality == 3)
        {
            img_footShadow.color = CommonUtil.stringToColor("#9146da");
        }
        else if (heroLogicBase.heroData.quality == 4)
        {
            img_footShadow.color = CommonUtil.stringToColor("#eb9b10");
        }

        text_weaponName.text = "No Weapons";
        obj_weapon1.gameObject.SetActive(false);
        obj_weapon2.gameObject.SetActive(false);
        img_choicedWeapon1.transform.localScale = Vector3.zero;
        img_choicedWeapon2.transform.localScale = Vector3.zero;

        if (heroLogicBase.list_weapon.Count >= 1)
        {
            obj_weapon1.gameObject.SetActive(true);
            img_weaponIcon1.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[0].type);
            img_weaponFrame1.color = Consts.list_weaponColor[heroLogicBase.list_weapon[0].type];
            obj_weapon1.transform.Find("level_bg").GetComponent<Image>().color = Consts.list_weaponColor[heroLogicBase.list_weapon[0].type];
            obj_weapon1.transform.Find("level_bg/level").GetComponent<Text>().text = heroLogicBase.list_weapon[0].level.ToString();

            onClickWeaponIcon(0);
        }

        if (heroLogicBase.list_weapon.Count >= 2)
        {
            obj_weapon2.gameObject.SetActive(true);
            img_weaponIcon2.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroLogicBase.list_weapon[1].type);
            img_weaponFrame2.color = Consts.list_weaponColor[heroLogicBase.list_weapon[1].type];
            obj_weapon2.transform.Find("level_bg").GetComponent<Image>().color = Consts.list_weaponColor[heroLogicBase.list_weapon[1].type];
            obj_weapon2.transform.Find("level_bg/level").GetComponent<Text>().text = heroLogicBase.list_weapon[1].level.ToString();
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

        if(heroLogicBase.list_weapon.Count > 0)
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
            btn_sellHeroTrans.position = heroLogicBase.heroUITrans.position + new Vector3(0,-0.9f,0);
            text_sellHeroPrice.text = sellPrice.ToString();
        }
    }

    public void onClickWeaponIcon(int index)
    {
        if (index == 0 && heroLogicBase.list_weapon.Count == 0)
        {
            return;
        }
        else if (index == 1 && heroLogicBase.list_weapon.Count < 2)
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
            if (heroLogicBase.list_weapon.Count >= 1)
            {
                showWeaponBuffInfo(heroLogicBase.list_weapon[0]);

                img_choicedWeapon1.transform.localScale = Vector3.one;
                img_choicedWeapon2.transform.localScale = Vector3.zero;

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
                showWeaponBuffInfo(heroLogicBase.list_weapon[1]);

                img_choicedWeapon1.transform.localScale = Vector3.zero;
                img_choicedWeapon2.transform.localScale = Vector3.one;

                text_weaponName.text = heroLogicBase.list_weapon[1].name;
            }
        }
    }

    void showWeaponBuffInfo(WeaponData weaponData)
    {
        weaponBuffsTrans.localScale = Vector3.one;

        // 攻击力
        {
            int atk = weaponData.buff1;
            string buffValue = atk.ToString();

            // 擅长
            if ((heroLogicBase.heroData.goodWeapon == -1) || (heroLogicBase.heroData.goodWeapon == weaponData.type))
            {
                atk = Mathf.RoundToInt(atk * 1.2f);
                buffValue = "<color=\"#60D262\">(+" + (atk - weaponData.buff1) + ") </color>" + atk;
            }
            // 不擅长
            else if ((heroLogicBase.heroData.badWeapon == -1) || (heroLogicBase.heroData.badWeapon == weaponData.type))
            {
                atk = Mathf.RoundToInt(atk * 0.8f);
                buffValue = "<color=\"#FB6061\">(" + (atk - weaponData.buff1) + ") </color>" + atk;
            }
            
            weaponBuffsTrans.Find("buff1/value").GetComponent<Text>().text = buffValue;
        }

        // 攻击力百分比加成
        {
            int atk = Mathf.RoundToInt(weaponData.buff2 * 100f);
            string buffValue = atk + "%";

            // 擅长
            if ((heroLogicBase.heroData.goodWeapon == -1) || (heroLogicBase.heroData.goodWeapon == weaponData.type))
            {
                int newAtk = Mathf.RoundToInt(weaponData.buff2 * 1.2f * 100f);
                buffValue = "<color=\"#60D262\">(+" + (newAtk - atk) + "%) </color>" + newAtk + "%";
            }
            // 不擅长
            else if ((heroLogicBase.heroData.badWeapon == -1) || (heroLogicBase.heroData.badWeapon == weaponData.type))
            {
                int newAtk = Mathf.RoundToInt(weaponData.buff2 * 0.8f * 100f);
                buffValue = "<color=\"#FB6061\">(" + (newAtk - atk) + "%) </color>" + newAtk + "%";
            }

            weaponBuffsTrans.Find("buff2/value").GetComponent<Text>().text = buffValue;
        }


        // 第三个任意属性Buff
        {
            weaponBuffsTrans.Find("buff3/icon").GetComponent<Image>().sprite = AtlasUtil.getAtlas_icon().GetSprite("buff_" + (int)weaponData.buff3Type);
            switch (weaponData.buff3Type)
            {
                case Consts.BuffType.CritRate:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Rate";

                        int value = Mathf.RoundToInt(float.Parse(weaponData.buff3ValueStr));
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }

                case Consts.BuffType.CritDamage:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Crit Damage";

                        int value = Mathf.RoundToInt(float.Parse(weaponData.buff3ValueStr) * 100);
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }

                case Consts.BuffType.AtkSpeed:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Attack Speed";
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = float.Parse(weaponData.buff3ValueStr).ToString();
                        break;
                    }

                case Consts.BuffType.SkillRate:
                    {
                        weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().text = "Proc Chance";

                        int value = Mathf.RoundToInt(float.Parse(weaponData.buff3ValueStr));
                        weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().text = value + "%";
                        break;
                    }
            }

            // 未激活
            if((heroLogicBase.heroData.badWeapon == -1) || (heroLogicBase.heroData.badWeapon == weaponData.type))
            {
                Color color = new Color(0.28f,0.3f,0.42f);
                weaponBuffsTrans.Find("buff3/icon").GetComponent<Image>().color = color;
                weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().color = color;
                weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().color = color;
            }
            // 激活
            else
            {
                Color color = weaponBuffsTrans.Find("buff1/icon").GetComponent<Image>().color;
                weaponBuffsTrans.Find("buff3/icon").GetComponent<Image>().color = color;
                weaponBuffsTrans.Find("buff3/name").GetComponent<Text>().color = color;
                weaponBuffsTrans.Find("buff3/value").GetComponent<Text>().color = color;
            }
        }
    }

    public void onClickSkillIcon(int index)
    {
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

        heroLogicBase = null;
        gameObject.SetActive(false);
        GameLayer.s_instance.attackRangeTrans.localScale = Vector3.zero;
    }
}
