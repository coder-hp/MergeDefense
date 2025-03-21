using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

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

    HeroSkillData[] skillsArray;
    HeroData heroData;
    bool isFullLevel = false;

    public void init(HeroData _heroData)
    {
        heroData = _heroData;

        text_name.text = heroData.name;
        text_level.text = "Lv."+GameData.getHeroLevel(heroData.id);
        text_atk.text = heroData.atk.ToString();
        text_atk_speed.text = heroData.atkSpeed.ToString();
        text_crit_rate.text = heroData.critRate + "%";
        text_crit_damage.text = Mathf.RoundToInt(heroData.critDamage * 100) + "%";
        img_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("head_" + heroData.id);
        img_weapon_icon.sprite = AtlasUtil.getAtlas_icon().GetSprite("weapon_" + heroData.goodWeapon);

        // 品质标签
        {
            tabsPoint.Find("quality").GetComponent<Image>().sprite = AtlasUtil.getAtlas_hero().GetSprite("biaoqian_quality_" + heroData.quality);
            tabsPoint.Find("quality/Text").GetComponent<Text>().text = Consts.list_heroQualityLabel[heroData.quality];
        }

        // 职业定位
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
            }
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

            ToastScript.show("Get New Hero!");
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
