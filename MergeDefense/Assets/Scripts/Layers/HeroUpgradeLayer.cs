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

    HeroSkillData[] skillsArray;
    HeroData heroData;

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
        }

        onClickSkillIcon(0);
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

    public void onClickUpgrade()
    {
        AudioScript.s_instance.playSound_btn();
    }

    public void onClickClose()
    {
        AudioScript.s_instance.playSound_btn();

        Destroy(gameObject);
    }
}
