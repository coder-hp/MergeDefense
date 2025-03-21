using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroUpgradeLayer : MonoBehaviour
{
    public Text text_name;
    public Text text_level;
    public Text text_skillDesc;
    public Text text_atk;
    public Text text_atk_speed;
    public Text text_crit_rate;
    public Text text_crit_damage;
    public Image img_icon;
    public Image img_weapon_icon;
    public Transform skillTrans;
    public Transform tabsPoint;
    public Transform btn_upgrade;
    public Transform btn_buy;
    public Transform btn_loginGet;
    public Transform mythicHeroMergeWay;
    public Transform yaoqiu_hero;
    public Transform yaoqiu_weapon;

    HeroSkillData[] skillsArray;
    HeroData heroData;
    bool isFullLevel = false;
    bool isCanUpgrade = false;
    int upgradeNeedExp = 0;
    int upgradeNeedGold = 0;

    public void init(HeroData _heroData,bool isUpgrade = false)
    {
        heroData = _heroData;

        text_name.text = heroData.name;
        if (!isUpgrade)
        {
            text_level.text = "Lv." + GameData.getHeroLevel(heroData.id);
        }
        text_atk.text = heroData.atk.ToString();
        text_atk_speed.text = heroData.atkSpeed.ToString();
        text_crit_rate.text = heroData.critRate + "%";
        text_crit_damage.text = Mathf.RoundToInt(heroData.critDamage * 100) + "%";
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);
        img_weapon_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroData.goodWeapon);

        // 品质标签
        if(!isUpgrade)
        {
            tabsPoint.Find("quality").GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("biaoqian_quality_" + heroData.quality);
            tabsPoint.Find("quality/Text").GetComponent<Text>().text = Consts.list_heroQualityLabel[heroData.quality];
        }

        // 职业定位
        if (!isUpgrade)
        {
            string[] strArray = heroData.career.Split('/');
            if(strArray.Length == 1)
            {
                tabsPoint.Find("desc1/Text").GetComponent<Text>().text = heroData.career;
                tabsPoint.Find("desc2").gameObject.SetActive(false);
            }
            else if (strArray.Length == 2)
            {
                tabsPoint.Find("desc1/Text").GetComponent<Text>().text = strArray[0];
                tabsPoint.Find("desc2/Text").GetComponent<Text>().text = strArray[1];
                tabsPoint.Find("desc2").gameObject.SetActive(true);
            }
        }

        // 经验条
        {
            isCanUpgrade = false;
            int curLevel = GameData.getHeroLevel(heroData.id);
            int curHeroExp = GameData.getHeroExp(heroData.id);
            HeroLevelData nextHeroLevelData = HeroLevelEntity.getInstance().getData(heroData.id, curLevel + 1);

            if (nextHeroLevelData != null)
            {
                isFullLevel = false;
                transform.Find("exp_bg/progress").GetComponent<Image>().fillAmount = (float)curHeroExp / (float)nextHeroLevelData.exp;
                transform.Find("exp_bg/Text").GetComponent<Text>().text = curHeroExp + "/" + nextHeroLevelData.exp;

                if (curHeroExp >= nextHeroLevelData.exp)
                {
                    isCanUpgrade = true;
                    upgradeNeedExp = nextHeroLevelData.exp;
                    transform.Find("exp_bg/jiantou").localScale = Vector3.one;
                }
                else
                {
                    transform.Find("exp_bg/jiantou").localScale = Vector3.zero;
                }
            }
            else
            {
                isFullLevel = true;
                transform.Find("exp_bg").localScale = Vector3.zero;
                transform.Find("maxLevel").localScale = Vector3.one;
            }
        }

        // 技能
        if (!isUpgrade)
        {
            string[] array = heroData.skills.Split("_");
            skillsArray = new HeroSkillData[array.Length];
            for (int i = 0; i < skillTrans.childCount; i++)
            {
                if ((i + 1) <= array.Length)
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
            onClickSkillIcon(0);
        }

        if(GameData.isUnlockHero(heroData.id))
        {
            btn_buy.localScale = Vector3.zero;
            btn_loginGet.localScale = Vector3.zero;

            if (isFullLevel)
            {
                btn_upgrade.localScale = Vector3.zero;
            }
            else
            {
                btn_upgrade.localScale = Vector3.one;
                upgradeNeedGold = HeroLevelEntity.getInstance().getData(heroData.id, GameData.getHeroLevel(heroData.id) + 1).gold;
                btn_upgrade.Find("price").GetComponent<Text>().text = upgradeNeedGold.ToString();

                if (isCanUpgrade)
                {
                    btn_upgrade.GetComponent<Button>().enabled = true;
                    btn_upgrade.Find("blackMask").localScale = Vector3.zero;
                }
                else
                {
                    btn_upgrade.GetComponent<Button>().enabled = false;
                    btn_upgrade.Find("blackMask").localScale = Vector3.one;
                }
            }
        }
        else
        {
            // 签到送的角色
            if (heroData.id == 118 || heroData.id == 119)
            {
                btn_upgrade.localScale = Vector3.zero;
                btn_loginGet.localScale = Vector3.one;
                btn_buy.localScale = Vector3.zero;
            }
            else
            {
                btn_upgrade.localScale = Vector3.zero;
                btn_loginGet.localScale = Vector3.zero;
                btn_buy.localScale = Vector3.one;
                btn_buy.Find("price").GetComponent<Text>().text = heroData.price.ToString();
            }
        }

        // 神话角色合成方式
        if(heroData.quality == 4)
        {
            mythicHeroMergeWay.localScale = Vector3.one;

            // 合成方式
            {
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
            }
        }
        else
        {
            mythicHeroMergeWay.localScale = Vector3.zero;
        }
    }

    public void onClickSkillIcon(int index)
    {
        AudioScript.s_instance.playSound_btn();

        for (int i = 0; i < skillTrans.childCount; i++)
        {
            if (i == index)
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.one;
                text_skillDesc.text = skillsArray[index].desc;
            }
            else
            {
                skillTrans.Find(i + "/choiced").localScale = Vector3.zero;
            }
        }
    }

    public void onClickLeft()
    {
        AudioScript.s_instance.playSound_btn();

        for(int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            if (HeroEntity.getInstance().list[i].id == heroData.id)
            {
                if(i > 0)
                {
                    init(HeroEntity.getInstance().list[i - 1]);
                }
                else if (i == 0)
                {
                    init(HeroEntity.getInstance().list[HeroEntity.getInstance().list.Count - 1]);
                }
                return;
            }
        }
    }

    public void onClickRight()
    {
        AudioScript.s_instance.playSound_btn();

        for (int i = 0; i < HeroEntity.getInstance().list.Count; i++)
        {
            if (HeroEntity.getInstance().list[i].id == heroData.id)
            {
                if (i ==  (HeroEntity.getInstance().list.Count - 1))
                {
                    init(HeroEntity.getInstance().list[0]);
                }
                else
                {
                    init(HeroEntity.getInstance().list[i + 1]);
                }
                return;
            }
        }
    }

    public void onClickUpgrade()
    {
        AudioScript.s_instance.playSound_btn();

        if(GameData.getMyGold() >= upgradeNeedGold)
        {
            if (GameData.getHeroExp(heroData.id) >= upgradeNeedExp)
            {
                GameData.changeMyGold(-upgradeNeedGold,"heroUpgrade");
                GameData.changeHeroExp(heroData.id, -upgradeNeedExp);
                GameData.setHeroLevel(heroData.id, GameData.getHeroLevel(heroData.id) + 1);
                init(heroData,true);
            }
        }
        else
        {
            ToastScript.show("Coins Not Enough!");
        }
    }

    public void onClickLoginGet()
    {
        AudioScript.s_instance.playSound_btn();

        ToastScript.show("暂未开放");
    }

    public void onClickBuy()
    {
        AudioScript.s_instance.playSound_btn();

        if(GameData.getMyDiamond() >= heroData.price)
        {
            GameData.changeMyDiamond(-heroData.price);
            GameData.unlockHero(heroData.id);

            btn_upgrade.localScale = Vector3.one;
            btn_loginGet.localScale = Vector3.zero;
            btn_buy.localScale = Vector3.zero;

            ToastScript.show("Mythic Hero Obtained！");
        }
        else
        {
            ToastScript.show("Gem Not Enough!");
        }
    }

    public void onClickClose()
    {
        AudioScript.s_instance.playSound_btn();

        Destroy(gameObject);
    }
}
